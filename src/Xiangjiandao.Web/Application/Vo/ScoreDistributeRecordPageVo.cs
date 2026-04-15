using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Web.Application.Vo;

public class ScoreDistributeRecordPageVo
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public UserId UserId { get; set; } = null!;

    /// <summary>
    /// 完整用户名，域名
    /// </summary>
    public string DomainName { get; set; } = string.Empty;

    /// <summary>
    /// 发放稻米
    /// </summary>
    public long Score { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.MinValue;
}
