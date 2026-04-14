using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Endpoints.Proposal;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminProposal;

/// <summary>
/// 后台提案详情
/// </summary>
[Tags("AdminProposal")]
[HttpPost("/api/v1/admin/proposal/detail")]
[Authorize(PolicyNames.Admin)]
public class AdminProposalDetailEndpoint(ProposalQuery query)
    : Endpoint<AdminProposalDetailReq, ResponseData<AdminProposalDetailVo>>
{
    public override async Task HandleAsync(AdminProposalDetailReq req, CancellationToken ct)
    {
        var detail = await query.AdminDetail(
            proposalId: req.ProposalId,
            cancellationToken: ct
        );
        if (detail is null)
        {
            throw new KnownException("提案不存在");
        }

        await SendAsync(
            response: detail.AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 后台提案详情响应
/// </summary>
public class AdminProposalDetailVo
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
    /// 是否上架
    /// </summary>
    public bool OnShelf { get; set; }

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
/// 后台提案详情请求
/// </summary>
public class AdminProposalDetailReq
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }
}