using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Web.Application.Vo;

/// <summary>
/// 节点用户分页
/// </summary>
public class NodeUserPageVo
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public UserId Id { get; set; } = default!;
    
    /// <summary>
    /// 用户昵称
    /// </summary> 
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// DID
    /// </summary> 
    public string Did { get; set; } = string.Empty;
    
    /// <summary>
    /// 完整用户名，域名
    /// </summary> 
    public string DomainName { get; set; } = string.Empty;
    
    /// <summary>
    /// 邮箱
    /// </summary> 
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary> 
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// 稻米
    /// </summary> 
    public long Score { get; set; }
    
    /// <summary>
    /// 是否禁用
    /// </summary> 
    public bool Disable { get; set; } = false;

    /// <summary>
    /// 创建时间
    /// </summary> 
    public DateTime CreatedAt { get; set; }
}