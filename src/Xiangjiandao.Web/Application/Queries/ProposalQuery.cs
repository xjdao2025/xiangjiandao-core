using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Endpoints.AdminProposal;
using Xiangjiandao.Web.Endpoints.Proposal;

namespace Xiangjiandao.Web.Application.Queries;

/// <summary>
/// 提案查询
/// </summary>
public class ProposalQuery(ApplicationDbContext dbContext)
{
    /// <summary>
    /// 后台分页查询
    /// </summary>
    public async Task<PagedData<AdminProposalPageVo>> AdminPage(AdminProposalPageReq req,
        CancellationToken cancellationToken)
    {
        return await dbContext.Proposals
            .WhereIf(!string.IsNullOrEmpty(req.Name), proposal => proposal.Name.Contains(req.Name))
            .WhereIf(!string.IsNullOrEmpty(req.InitiatorName),
                proposal => proposal.InitiatorName.Contains(req.InitiatorName))
            .WhereIf(req.Status != ProposalStatus.Unknown, proposal => proposal.Status == req.Status)
            .WhereIf(req.StartTime != null,
                proposal => DateOnly.FromDateTime(proposal.CreatedAt.DateTime) >= req.StartTime)
            .WhereIf(req.EndTime != null, proposal => DateOnly.FromDateTime(proposal.CreatedAt.DateTime) <= req.EndTime)
            .OrderByDescending(p => p.CreatedAt)
            .Select(proposal => new AdminProposalPageVo
            {
                Id = proposal.Id,
                Name = proposal.Name,
                InitiatorName = proposal.InitiatorName,
                InitiatorAvatar = proposal.InitiatorAvatar,
                EndAt = proposal.EndAt,
                Status = proposal.Status,
                OnShelf = proposal.OnShelf,
                CreatedAt = proposal.CreatedAt,
            })
            .ToPagedDataAsync(
                pageIndex: req.PageNum,
                pageSize: req.PageSize,
                countTotal: true,
                cancellationToken: cancellationToken
            );
    }

    /// <summary>
    /// 前台分页查询
    /// </summary>
    public async Task<PagedData<ProposalPageVo>> Page(
        int pageNum,
        int pageSize,
        UserId userId,
        ProposalStatus status,
        CancellationToken cancellationToken)
    {
        var pageData = await dbContext.Proposals
            .WhereIf(status != ProposalStatus.Unknown, proposal => proposal.Status == status)
            .Where(proposal => proposal.OnShelf)
            .OrderByDescending(proposal => proposal.CreatedAt)
            .ToPagedDataAsync(
                pageIndex: pageNum,
                pageSize: pageSize,
                countTotal: true,
                cancellationToken: cancellationToken
            );

        var proposalPageVos = pageData.Items
            .Select(proposal => new ProposalPageVo
            {
                ProposalId = proposal.Id,
                Name = proposal.Name,
                InitiatorId = proposal.InitiatorId,
                InitiatorName = proposal.InitiatorName,
                InitiatorEmail = proposal.InitiatorEmail,
                InitiatorAvatar = proposal.InitiatorAvatar,
                Status = proposal.Status,
                CreatedAt = proposal.CreatedAt,
                OpposeVotes = proposal.OpposeVotes,
                AgreeVotes = proposal.AgreeVotes,
                InitiatorDid = proposal.InitiatorDid,
                InitiatorDomainName = proposal.InitiatorDomainName,
                Choice = proposal.Votes.FirstOrDefault(record => record.UserId == userId)?.Choice ?? VoteType.Unknown,
            })
            .ToList();

        return new PagedData<ProposalPageVo>(
            items: proposalPageVos,
            total: pageData.Total,
            pageIndex: pageData.PageIndex,
            pageSize: pageData.PageSize
        );
    }

    /// <summary>
    /// 前台提案详情请求
    /// </summary>
    public async Task<ProposalDetailVo?> Detail(
        ProposalId proposalId,
        CancellationToken cancellationToken
    )
    {
        var detail = await dbContext.Proposals
            .Where(proposal => proposal.Id == proposalId)
            .Select(proposal => new ProposalDetailVo
            {
                ProposalId = proposal.Id,
                Name = proposal.Name,
                InitiatorId = proposal.InitiatorId,
                InitiatorName = proposal.InitiatorName,
                InitiatorEmail = proposal.InitiatorEmail,
                InitiatorAvatar = proposal.InitiatorAvatar,
                Status = proposal.Status,
                CreatedAt = proposal.CreatedAt,
                TotalVotes = proposal.TotalVotes,
                OpposeVotes = proposal.OpposeVotes,
                AgreeVotes = proposal.AgreeVotes,
                EndAt = proposal.EndAt,
                AttachId = proposal.AttachId,
                InitiatorDid = proposal.InitiatorDid,
                InitiatorDomainName = proposal.InitiatorDomainName,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (detail is not null)
        {
            detail.Comments = await dbContext.ProposalComments
                .Where(c => c.ProposalId == proposalId && !c.Deleted)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new ProposalCommentVo
                {
                    CommentId = c.Id.ToString(),
                    UserId = c.UserId.ToString(),
                    UserName = c.UserName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                })
                .ToListAsync(cancellationToken);
        }

        return detail;
    }

    /// <summary>
    /// 后台详情
    /// </summary>
    public async Task<AdminProposalDetailVo?> AdminDetail(
        ProposalId proposalId,
        CancellationToken cancellationToken
    )
    {
        var detail = await dbContext.Proposals
            .Where(proposal => proposal.Id == proposalId)
            .Select(proposal => new AdminProposalDetailVo
            {
                ProposalId = proposal.Id,
                Name = proposal.Name,
                InitiatorId = proposal.InitiatorId,
                InitiatorName = proposal.InitiatorName,
                InitiatorEmail = proposal.InitiatorEmail,
                InitiatorAvatar = proposal.InitiatorAvatar,
                Status = proposal.Status,
                OnShelf = proposal.OnShelf,
                CreatedAt = proposal.CreatedAt,
                TotalVotes = proposal.TotalVotes,
                OpposeVotes = proposal.OpposeVotes,
                AgreeVotes = proposal.AgreeVotes,
                EndAt = proposal.EndAt,
                AttachId = proposal.AttachId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (detail is not null)
        {
            detail.Comments = await dbContext.ProposalComments
                .Where(c => c.ProposalId == proposalId && !c.Deleted)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new ProposalCommentVo
                {
                    CommentId = c.Id.ToString(),
                    UserId = c.UserId.ToString(),
                    UserName = c.UserName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                })
                .ToListAsync(cancellationToken);
        }

        return detail;
    }

    /// <summary>
    /// 我的提案列表
    /// </summary>
    public async Task<List<MyProposalVo>> MyProposalList(
        UserId userId,
        int type,
        CancellationToken cancellationToken
    )
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Id == userId,
                cancellationToken: cancellationToken
            );
        if (user is not null && user.Disable)
        {
            throw new KnownException("该用户已被禁用");
        }

        var createProposals = await dbContext.Proposals
            .Where(proposal => proposal.InitiatorId == userId)
            .ToListAsync(cancellationToken);

        var createdProposalVos = createProposals
            .Select(proposal => new MyProposalVo
            {
                ProposalId = proposal.Id,
                Name = proposal.Name,
                InitiatorId = proposal.InitiatorId,
                InitiatorName = proposal.InitiatorName,
                InitiatorEmail = proposal.InitiatorEmail,
                InitiatorAvatar = proposal.InitiatorAvatar,
                OpposeVotes = proposal.OpposeVotes,
                AgreeVotes = proposal.AgreeVotes,
                Status = proposal.Status,
                CreatedAt = proposal.CreatedAt,
                InitiatorDid = proposal.InitiatorDid,
                InitiatorDomainName = proposal.InitiatorDomainName,
                Choice = proposal.Votes.FirstOrDefault(record => record.UserId == userId)?.Choice ?? VoteType.Unknown,
            })
            .ToList();

        var joinedProposals = await dbContext.Proposals
            .Where(proposal => proposal.Votes.Select(record => record.UserId).Contains(userId))
            .ToListAsync(cancellationToken);
        var joinedProposalVos = joinedProposals
            .Select(proposal => new MyProposalVo
            {
                ProposalId = proposal.Id,
                Name = proposal.Name,
                InitiatorId = proposal.InitiatorId,
                InitiatorName = proposal.InitiatorName,
                InitiatorEmail = proposal.InitiatorEmail,
                InitiatorAvatar = proposal.InitiatorAvatar,
                OpposeVotes = proposal.OpposeVotes,
                AgreeVotes = proposal.AgreeVotes,
                Status = proposal.Status,
                CreatedAt = proposal.CreatedAt,
                InitiatorDid = proposal.InitiatorDid,
                InitiatorDomainName = proposal.InitiatorDomainName,
                Choice = proposal.Votes.FirstOrDefault(record => record.UserId == userId)?.Choice ?? VoteType.Unknown,
            })
            .ToList();

        return type switch
        {
            0 => createdProposalVos
                .Concat(joinedProposalVos)
                .DistinctBy(proposal => proposal.ProposalId)
                .OrderByDescending(proposal => proposal.CreatedAt)
                .ToList(),
            1 => createdProposalVos.OrderByDescending(proposal => proposal.CreatedAt).ToList(),
            2 => joinedProposalVos.OrderByDescending(proposal => proposal.CreatedAt).ToList(),
            _ => throw new KnownException("非法的类型")
        };
    }

    /// <summary>
    /// 即将结束的提案
    /// </summary>
    public async Task<List<ProposalId>> ToEndProposalIds(DateTimeOffset endAt)
    {
        return await dbContext.Proposals
            .Where(proposal => proposal.EndAt <= endAt &&
                               proposal.Status == ProposalStatus.Review)
            .Select(proposal => proposal.Id)
            .ToListAsync();
    }

    /// <summary>
    /// 我的投票选择
    /// </summary>
    public async Task<MyProposalChoiceVo> MyProposalChoice(
        UserId loginUserId,
        ProposalId proposalId,
        CancellationToken cancellationToken
    )
    {
        var voteRecord = await dbContext.VoteRecords
            .Where(vote => vote.ProposalId == proposalId && vote.UserId == loginUserId)
            .FirstOrDefaultAsync(cancellationToken);
        if (voteRecord is null)
        {
            return new MyProposalChoiceVo
            {
                Choice = VoteType.Unknown
            };
        }

        return voteRecord.Choice switch
        {
            VoteType.Agree => new MyProposalChoiceVo { Choice = VoteType.Agree },
            VoteType.Oppose => new MyProposalChoiceVo { Choice = VoteType.Oppose },
            VoteType.Unknown => new MyProposalChoiceVo { Choice = VoteType.Unknown },
            _ => throw new KnownException("非法的投票类型")
        };
    }
}