using System.Text.RegularExpressions;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Dto;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;
using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Web.Endpoints.Score;

/// <summary>
/// 发送稻米
/// </summary>
/// <param name="mediator"></param>
[Tags("Scores")]
[HttpPost("/api/v1/score/send")]
[Authorize(PolicyNames.Client)]
public class SendScoreEndpoint(
    IMediator mediator,
    ILoginUser loginUser,
    IXiangjiandaoDistributedDisLock distributedLock,
    UserQuery query): Endpoint<SendScoreReq, ResponseData<bool>>
{
    public override async Task HandleAsync(SendScoreReq req, CancellationToken ct)
    {
        if (!loginUser.IsAuthenticated)
        {
            throw new KnownException("当前用户未登录");
        }
        // 查当前用户信息
        var fromUserId = new UserId(loginUser.Id);
        
        var fromUser = await query.GetUserById(fromUserId, ct);
        
        if (fromUser is null)
        {
            throw new KnownException("当前用户不存在");
        }
        
        if (fromUser.Disable)
        {
            throw new KnownException("当前用户已禁用", 401);
        }

        if (fromUser.Score < req.Score)
        {
            throw new KnownException("当前用户稻米不足");
        }

        UserDetailDto? toUser = null;
        if (req.ToUserId != null && req.ToUserId.Id != Guid.Empty)
        {
            toUser = await query.GetUserById(req.ToUserId, ct);
        }
        else if (!string.IsNullOrEmpty(req.UserPhoneOrEmail))
        {
            toUser = await query.GetUserByPhoneOrEmail(req.UserPhoneOrEmail, ct);
        }

        if (toUser is null)
        {
            throw new KnownException("接收用户不存在");
        }

        if (fromUser.Id == toUser.Id)
        {
            throw new KnownException("不可以给自己赠送稻米");
        }
        var fromUserKey = $"score-distribution-lock:{fromUser.Id.Id.ToString()}";
        await using var fromUserSynchronizationHandle = await distributedLock.AcquireAsync(fromUserKey,  TimeSpan.FromSeconds(5), ct);
        var toUserKey = $"score-distribution-lock:{toUser.Id.Id.ToString()}";
        await using var toUserSynchronizationHandle = await distributedLock.AcquireAsync(toUserKey,  TimeSpan.FromSeconds(5), ct);

        await SendAsync(await mediator.Send(new CreateSendRewardScoreRecordCommand()
        {
            FromUserId = fromUser.Id,
            Score = req.Score,
            ToUserId = toUser.Id,
            Reason = ScoreSourceType.Send.GetDesc(),
            Type = ScoreSourceType.Send,
            Remark = req.Remark
        }).AsSuccessResponseData(),  cancellation: ct);
        
    }
    
}

/// <summary>
/// 发送稻米
/// </summary>
public record SendScoreReq
{
    /// <summary>
    /// 接收用户的手机号或邮箱
    /// </summary>
    public string UserPhoneOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// 接收用户的UserId（与UserPhoneOrEmail二选一）
    /// </summary>
    public UserId? ToUserId { get; set; }

    /// <summary>
    /// 稻米
    /// </summary>
    public required long Score { get; set; }

    /// <summary>
    /// 附言
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 入参校验
/// </summary>
public class SendScoreReqValidator : AbstractValidator<SendScoreReq>
{
    public SendScoreReqValidator()
    {
        var phonePattern = @"^1[3-9]\d{9}$";
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        RuleFor(x => x.UserPhoneOrEmail)
            .Must((req, phoneOrEmail) =>
            {
                if (req.ToUserId != null && req.ToUserId.Id != Guid.Empty)
                {
                    return true;
                }
                return !string.IsNullOrEmpty(phoneOrEmail);
            })
            .WithMessage("接收用户的手机号或邮箱不能为空")
            .Must(phoneOrEmail =>
            {
                if (string.IsNullOrEmpty(phoneOrEmail))
                {
                    return true;
                }
                return Regex.IsMatch(phoneOrEmail, phonePattern) || Regex.IsMatch(phoneOrEmail, emailPattern);
            })
            .WithMessage("用户手机号或邮箱格式不正确");
        RuleFor(x => x.Score).GreaterThan(0).WithMessage("稻米值必须大于0");
        RuleFor(x => x.Remark)
            .MaximumLength(20).WithMessage("附言长度不能超过20个字符");
    }
}