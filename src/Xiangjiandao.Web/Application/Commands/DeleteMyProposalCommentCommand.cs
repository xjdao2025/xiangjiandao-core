using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 删除我的提案评论命令
/// </summary>
public class DeleteMyProposalCommentCommand : ICommand<bool>
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class DeleteMyProposalCommentCommandHandler(
    ApplicationDbContext dbContext,
    ILogger<DeleteMyProposalCommentCommandHandler> logger
) : ICommandHandler<DeleteMyProposalCommentCommand, bool>
{
    public async Task<bool> Handle(DeleteMyProposalCommentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteMyProposalCommentCommand Handling");

        var comment = await dbContext.ProposalComments
            .FirstOrDefaultAsync(c => c.Id == command.CommentId && !c.Deleted, cancellationToken);

        if (comment is null)
        {
            throw new KnownException("评论未找到");
        }

        if (comment.UserId != command.UserId)
        {
            throw new KnownException("无权删除该评论");
        }

        comment.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
