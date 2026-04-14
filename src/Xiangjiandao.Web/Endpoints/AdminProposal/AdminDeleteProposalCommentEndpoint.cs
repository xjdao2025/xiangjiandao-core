using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminProposal;

/// <summary>
/// 后台删除提案评论
/// </summary>
[Tags("AdminProposal")]
[HttpPost("/api/v1/admin/proposal/delete-comment")]
[Authorize(PolicyNames.Admin)]
public class AdminDeleteProposalCommentEndpoint(IMediator mediator)
    : Endpoint<AdminDeleteProposalCommentReq, ResponseData<bool>>
{
    public override async Task HandleAsync(AdminDeleteProposalCommentReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 后台删除提案评论请求
/// </summary>
public class AdminDeleteProposalCommentReq
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public AdminDeleteProposalCommentCommand ToCommand()
    {
        return new AdminDeleteProposalCommentCommand
        {
            CommentId = CommentId,
        };
    }
}
