using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;

/// <summary>
/// 提案评论 Id
/// </summary>
public partial record ProposalCommentId : IInt64StronglyTypedId;

/// <summary>
/// 提案评论
/// </summary>
public class ProposalComment : Entity<ProposalCommentId>
{
    protected ProposalComment()
    {
    }

    /// <summary>
    /// 提案 Id
    /// </summary>
    public ProposalId ProposalId { get; private set; } = null!;

    /// <summary>
    /// 评论用户 Id
    /// </summary>
    public UserId UserId { get; private set; } = null!;

    /// <summary>
    /// 评论用户名称
    /// </summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>
    /// 评论内容
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.MinValue;

    /// <summary>
    /// 是否删除
    /// </summary>
    public Deleted Deleted { get; private set; } = new Deleted(false);

    /// <summary>
    /// 创建评论
    /// </summary>
    public static ProposalComment Create(
        ProposalId proposalId,
        UserId userId,
        string userName,
        string content
    )
    {
        var instance = new ProposalComment
        {
            ProposalId = proposalId,
            UserId = userId,
            UserName = userName,
            Content = content,
            CreatedAt = DateTimeOffset.Now,
        };
        return instance;
    }

    /// <summary>
    /// 软删除评论
    /// </summary>
    public bool Delete()
    {
        Deleted = true;
        return true;
    }
}
