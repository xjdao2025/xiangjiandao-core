using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Endpoints.AdminScore;
using Xiangjiandao.Web.Endpoints.Score;

namespace Xiangjiandao.Web.Application.Queries;

public class ScoreRecordQuery(ApplicationDbContext dbContext)
{
    /// <summary>
    /// 用户稻米记录分页
    /// </summary>
    public async Task<PagedData<UserScoreRecordPageVo>> Page(UserId userId, UserScoreRecordPageReq req,
        CancellationToken cancellationToken)
    {
        return await dbContext.ScoreRecords
            .Where(record => record.UserId == userId)
            .OrderByDescending(record => record.CreatedAt)
            .Select(record => new UserScoreRecordPageVo
            {
                Id = record.Id,
                Score = record.Score,
                Type = record.Type,
                Reason = record.Reason,
                Remark = record.Remark,
                CreatedAt = record.CreatedAt,
            }).ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken);
    }

    /// <summary>
    /// 后台用户稻米记录分页
    /// </summary>
    public async Task<PagedData<AdminUserScoreRecordPageVo>> AdminPage(
        UserId userId,
        int pageNum,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        return await dbContext.ScoreRecords
            .Where(record => record.UserId == userId)
            .OrderByDescending(record => record.CreatedAt)
            .Select(record => new AdminUserScoreRecordPageVo
            {
                Id = record.Id,
                Score = record.Score,
                Type = record.Type,
                ParticipatorDomainName = record.ParticipatorDomainName,
                Reason = record.Reason,
                Remark = record.Remark,
                CreatedAt = record.CreatedAt,
            }).ToPagedDataAsync(
                pageIndex: pageNum,
                pageSize: pageSize,
                countTotal: true,
                cancellationToken: cancellationToken
            );
    }
}