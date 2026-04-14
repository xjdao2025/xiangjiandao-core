using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 创建提案评论
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/comment")]
[Authorize(PolicyNames.Client)]
public class CreateProposalCommentEndpoint(
    IMediator mediator,
    ILoginUser loginUser
) : Endpoint<CreateProposalCommentReq, ResponseData<bool>>
{
    public override async Task HandleAsync(CreateProposalCommentReq req, CancellationToken ct)
    {
        var userId = new UserId(loginUser.Id);
        var command = req.ToCommand(userId);
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 创建提案评论请求
/// </summary>
public class CreateProposalCommentReq
{
    /// <summary>
    /// 提案 Id
    /// </summary>
    public required ProposalId ProposalId { get; set; }

    /// <summary>
    /// 评论内容
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public CreateProposalCommentCommand ToCommand(UserId userId)
    {
        return new CreateProposalCommentCommand(
            ProposalId: ProposalId,
            UserId: userId,
            Content: Content
        );
    }
}
