using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Web.Application.Queries;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Score;

/// <summary>
/// 稻米发放记录翻页查询
/// </summary>
[Tags("Scores")]
[HttpPost("/api/v1/score-distribute-record/page")]
[Authorize(PolicyNames.Client)]
public class ScoreDistributeRecordPageEndpoint(
    ScoreDistributeRecordQuery query)
    : Endpoint<ScoreDistributeRecordPageReq, ResponseData<PagedData<ScoreDistributeRecordPageVo>>>
{
    public override async Task HandleAsync(ScoreDistributeRecordPageReq req, CancellationToken ct)
    {
        var result = await query.Page(req, ct);
        await SendAsync(result.AsSuccessResponseData(), cancellation: ct);
    }
}
