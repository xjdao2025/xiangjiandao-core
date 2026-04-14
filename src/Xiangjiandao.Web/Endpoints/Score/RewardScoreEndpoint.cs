using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Score;

/// <summary>
/// 打赏稻米
/// </summary>
/// <param name="mediator"></param>
[Tags("Scores")]
[HttpPost("/api/v1/score/reward")]
[Authorize(PolicyNames.Client)]
public class RewardScoreEndpoint(
    IMediator mediator,
    ILoginUser loginUser,
    IXiangjiandaoDistributedDisLock distributedLock,
    UserQuery query): Endpoint<RewardScoreReq, ResponseData<bool>>
{
    public override async Task HandleAsync(RewardScoreReq req, CancellationToken ct)
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
        
        var toUser = await query.GetUserByDid(req.ToUserDid, ct);
        
        if (toUser is null)
        {
            throw new KnownException("接收用户不存在");
        }
        
        if (fromUser.Id == toUser.Id)
        {
            throw new KnownException("不可以给自己打赏稻米");
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
            Reason = ScoreSourceType.Reward.GetDesc(),
            Type = ScoreSourceType.Reward,
            Remark = string.Empty
        }).AsSuccessResponseData(),  cancellation: ct);
        
    }
    
}

/// <summary>
/// 打赏稻米
/// </summary>
public record RewardScoreReq
{
    /// <summary>
    /// 接收用户的Did
    /// </summary>
    public required string ToUserDid{ get; set; }
    
    /// <summary>
    /// 稻米
    /// </summary>
    public required long Score { get; set; }
    
    /// <summary>
    /// 扩展信息（帖子的标题等相关信息）
    /// </summary>
    public string ExtendInfo { get; set; } = string.Empty;
}

/// <summary>
/// 入参校验
/// </summary>
public class RewardScoreReqValidator : AbstractValidator<RewardScoreReq>
{
    public RewardScoreReqValidator()
    {
        RuleFor(x => x.ToUserDid).NotEmpty().WithMessage("接收用户的Did不能为空");
        RuleFor(x => x.Score).GreaterThan(0).WithMessage("稻米值必须大于0");
    }
}