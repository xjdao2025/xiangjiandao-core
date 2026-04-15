using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;

namespace Xiangjiandao.Web.Application.Queries;

public class ScoreDistributeRecordQuery(ApplicationDbContext dbContext)
{
    public async Task<PagedData<AdminScoreDistributeRecordPageVo>> Page(AdminScoreDistributeRecordPageReq req,
        CancellationToken cancellationToken)
    {
        return await dbContext.ScoreDistributeRecords
            .WhereIf(!string.IsNullOrEmpty(req.NickName), record => record.NickName.Contains(req.NickName))
            .WhereIf(!string.IsNullOrEmpty(req.PhoneOrEmail), record => record.Phone.Contains(req.PhoneOrEmail) || record.Email.Contains(req.PhoneOrEmail))
            .WhereIf(req.StartTime != null, record => DateOnly.FromDateTime(record.GetTime.DateTime) >= req.StartTime)
            .WhereIf(req.EndTime != null, record => DateOnly.FromDateTime(record.GetTime.DateTime) <= req.EndTime)
            .OrderByDescending(medal => medal.GetTime)
            .Select(record => new AdminScoreDistributeRecordPageVo
            {
                NickName = record.NickName,
                Phone = record.Phone,
                PhoneRegion = record.PhoneRegion,
                Email = record.Email,
                GetTime = record.GetTime,
                Score = record.Score,
                CreatedBy = record.CreatedBy
            }).ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }

    public async Task<PagedData<ScoreDistributeRecordPageVo>> Page(ScoreDistributeRecordPageReq req,
        CancellationToken cancellationToken)
    {
        return await (from record in dbContext.ScoreDistributeRecords
                join user in dbContext.Users on record.UserId equals user.Id
                orderby record.GetTime descending
                select new ScoreDistributeRecordPageVo
                {
                    UserId = record.UserId,
                    DomainName = user.DomainName,
                    Score = record.Score,
                    GetTime = record.GetTime
                })
            .ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }
}