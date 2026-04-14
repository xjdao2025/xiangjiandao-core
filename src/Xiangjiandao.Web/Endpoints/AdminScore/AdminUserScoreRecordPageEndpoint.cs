using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminScore;

/// <summary>
/// 用户稻米明细
/// </summary>
[Tags("AdminScores")]
[HttpPost("/api/v1/admin/score/user-sore-record-page")]
[Authorize(PolicyNames.Admin)]
public class AdminUserScoreRecordPageEndpoint(ScoreRecordQuery query)
    : Endpoint<AdminUserScoreRecordPageReq, ResponseData<PagedData<AdminUserScoreRecordPageVo>>>
{
    public override async Task HandleAsync(AdminUserScoreRecordPageReq req, CancellationToken ct)
    {
        var userId = req.UserId;
        var result = await query.AdminPage(userId, req.PageNum, req.PageSize, ct);
        await SendAsync(result.AsSuccessResponseData(), cancellation: ct);
    }
}

/// <summary>
/// 后台用户稻米明细翻页查询请求
/// </summary>
public record AdminUserScoreRecordPageReq
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; } = 1;

    /// <summary>
    /// 分页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// 用户稻米明细
/// </summary>
public class AdminUserScoreRecordPageVo
{
    /// <summary>
    /// 稻米明细主键Id
    /// </summary>
    public ScoreRecordId Id { get; set; } = null!;

    /// <summary>
    /// 稻米来源类型
    /// </summary> 
    public ScoreSourceType Type { get; set; } = ScoreSourceType.Unknown;

    /// <summary>
    /// 参与方域名
    /// </summary>
    public string ParticipatorDomainName { get; set; } = string.Empty;

    /// <summary>
    /// 获得原因
    /// </summary> 
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 附言
    /// </summary> 
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// 稻米数量
    /// </summary> 
    public long Score { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary> 
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.MinValue;
}