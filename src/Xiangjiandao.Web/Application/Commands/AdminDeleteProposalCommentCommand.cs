using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Infrastructure;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 管理员删除提案评论命令
/// </summary>
public class AdminDeleteProposalCommentCommand : ICommand<bool>
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class AdminDeleteProposalCommentCommandHandler(
    ApplicationDbContext dbContext,
    ILogger<AdminDeleteProposalCommentCommandHandler> logger
) : ICommandHandler<AdminDeleteProposalCommentCommand, bool>
{
    public async Task<bool> Handle(AdminDeleteProposalCommentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("AdminDeleteProposalCommentCommand Handling");

        var comment = await dbContext.ProposalComments
            .FirstOrDefaultAsync(c => c.Id == command.CommentId && !c.Deleted, cancellationToken);

        if (comment is null)
        {
            throw new KnownException("评论未找到");
        }

        comment.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
