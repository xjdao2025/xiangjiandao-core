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
/// 删除我的提案评论
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/delete-my-comment")]
[Authorize(PolicyNames.Client)]
public class DeleteMyProposalCommentEndpoint(
    IMediator mediator,
    ILoginUser loginUser
) : Endpoint<DeleteMyProposalCommentReq, ResponseData<bool>>
{
    public override async Task HandleAsync(DeleteMyProposalCommentReq req, CancellationToken ct)
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
/// 删除我的提案评论请求
/// </summary>
public class DeleteMyProposalCommentReq
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public DeleteMyProposalCommentCommand ToCommand(UserId userId)
    {
        return new DeleteMyProposalCommentCommand
        {
            CommentId = CommentId,
            UserId = userId
        };
    }
}
