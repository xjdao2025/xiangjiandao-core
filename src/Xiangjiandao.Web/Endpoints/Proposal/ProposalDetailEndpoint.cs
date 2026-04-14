using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 提案详情
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/detail")]
[AllowAnonymous]
public class ProposalDetailEndpoint(ProposalQuery query) : Endpoint<ProposalDetailReq, ResponseData<ProposalDetailVo>>
{
    public override async Task HandleAsync(ProposalDetailReq req, CancellationToken ct)
    {
        var detail = await query.Detail(
            proposalId: req.ProposalId,
            cancellationToken: ct
        );
        if (detail is null)
        {
            throw new KnownException("提案未找到");
        }

        await SendAsync(
            response: detail.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 提案详情响应
/// </summary>
public class ProposalDetailVo
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 提案名称
    /// </summary> 
    public required string Name { get; set; }

    /// <summary>
    /// 发起方名称
    /// </summary> 
    public required UserId InitiatorId { get; set; }

    /// <summary>
    /// 发起方 Did
    /// </summary>
    public required string InitiatorDid { get; set; }

    /// <summary>
    /// 发起方域名
    /// </summary>
    public required string InitiatorDomainName { get; set; }

    /// <summary>
    /// 发起方名称
    /// </summary> 
    public required string InitiatorName { get; set; }

    /// <summary>
    /// 发起方邮箱
    /// </summary> 
    public required string InitiatorEmail { get; set; }

    /// <summary>
    /// 发起方头像
    /// </summary> 
    public required string InitiatorAvatar { get; set; }

    /// <summary>
    /// 投票截至时间
    /// </summary> 
    public required DateTimeOffset EndAt { get; set; }

    /// <summary>
    /// 附件 Id
    /// </summary> 
    public required string AttachId { get; set; }

    /// <summary>
    /// 总投票数
    /// </summary> 
    public required long TotalVotes { get; set; }

    /// <summary>
    /// 反对票数
    /// </summary> 
    public required long OpposeVotes { get; set; }

    /// <summary>
    /// 同意票数
    /// </summary> 
    public required long AgreeVotes { get; set; }

    /// <summary>
    /// 提案状态
    /// </summary> 
    public required ProposalStatus Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary> 
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 评论列表
    /// </summary>
    public List<ProposalCommentVo> Comments { get; set; } = [];
}

/// <summary>
/// 提案评论 Vo
/// </summary>
public class ProposalCommentVo
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required string CommentId { get; set; }

    /// <summary>
    /// 评论用户 Id
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// 评论用户名称
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// 评论内容
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// 提案详情请求
/// </summary>
public class ProposalDetailReq
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }
}