using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 创建提案评论命令
/// </summary>
public record CreateProposalCommentCommand(
    ProposalId ProposalId,
    UserId UserId,
    string Content
) : ICommand<bool>;

/// <summary>
/// 命令处理器
/// </summary>
public class CreateProposalCommentCommandHandler(
    ApplicationDbContext dbContext
) : ICommandHandler<CreateProposalCommentCommand, bool>
{
    public async Task<bool> Handle(CreateProposalCommentCommand command, CancellationToken cancellationToken)
    {
        var proposal = await dbContext.Proposals
            .FirstOrDefaultAsync(p => p.Id == command.ProposalId && !p.Deleted, cancellationToken);

        if (proposal is null)
        {
            throw new KnownException("提案未找到");
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            throw new KnownException("用户未找到");
        }

        var comment = ProposalComment.Create(
            proposalId: command.ProposalId,
            userId: command.UserId,
            userName: user.DomainName,
            content: command.Content
        );

        dbContext.ProposalComments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
