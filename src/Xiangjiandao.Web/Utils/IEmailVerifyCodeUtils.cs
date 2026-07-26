using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Primitives;
using StackExchange.Redis;
using Xiangjiandao.Web.Options;

namespace Xiangjiandao.Web.Utils;

/// <summary>
/// 发送邮箱验证码工具
/// </summary>
public interface IEmailVerifyCodeUtils
{
    /// <summary>
    /// 发送验证码, 默认过期时间 30 分钟
    /// </summary>
    Task<string> SendAsync(string toName, string toAddress, CodeType codeType, int expireTime = 1800);

    /// <summary>
    /// 验证邮箱验证码
    /// </summary>
    Task<bool> VerifyAsync(string emailAddress, string code, CodeType codeType);

    /// <summary>
    /// 通知管理员
    /// </summary>
    Task<bool> SendToAdminAsync(string toName, string toAddress, string content);
}

/// <summary>
/// 验证码类型 0-未知 1-登录 2-重置密码 3-注册 4-修改邮箱 5-更换手机号 6-后台用户登录 7-后台用户重置密码 8-后台稻米发放 9-注销账户
/// </summary>
public enum CodeType
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 登录
    /// </summary>
    Login = 1,

    /// <summary>
    /// 重置密码
    /// </summary>
    ResetPassword = 2,

    /// <summary>
    /// 注册
    /// </summary>
    Register = 3,

    /// <summary>
    /// 更换账号邮箱
    /// </summary>
    ChangeEmail = 4,

    /// <summary>
    /// 更换手机号
    /// </summary>
    ChangePhone = 5,

    /// <summary>
    /// 后台用户登录
    /// </summary>
    AdminUserLogin = 6,

    /// <summary>
    /// 后台用户重置密码
    /// </summary>
    AdminUserResetPassword = 7,

    /// <summary>
    /// 8-后台稻米发放
    /// </summary>
    AdminUserScoreDistribution = 8,

    /// <summary>
    /// 注销账户
    /// </summary>
    DeleteAccount = 9,
}

/// <summary>
/// CodeType 扩展
/// </summary>
public static class CodeTypeExtension
{
    /// <summary>
    /// 获取枚举描述
    /// </summary>
    public static string GetDesc(this CodeType codeType)
    {
        return codeType switch
        {
            CodeType.Unknown => "unknown",
            CodeType.Login => "login",
            CodeType.ResetPassword => "reset_password",
            CodeType.Register => "register",
            CodeType.ChangeEmail => "change_email",
            CodeType.ChangePhone => "create_phone",
            CodeType.AdminUserLogin => "admin_user_login",
            CodeType.AdminUserResetPassword => "admin_user_reset_password",
            CodeType.AdminUserScoreDistribution => "admin_user_score_distribution",
            CodeType.DeleteAccount => "delete_account",
            _ => throw new KnownException("不支持的验证码类型"),
        };
    }
}

/// <summary>
/// 发送邮箱验证码接口实现
/// </summary>
public class EmailVerifyCodeUtils(
    IConnectionMultiplexer redisClient,
    EmailSender emailSender,
    IOptions<AppOptions> appOptions,
    ILogger<EmailVerifyCodeUtils> logger
) : IEmailVerifyCodeUtils
{
    /// <summary>
    /// 邮箱验证码 Redis Key 前缀
    /// </summary>
    private const string VerifyCodePrefix = "xiangjiandao_email_verify_code";

    /// <summary>
    /// 邮箱验证码模板占位符
    /// </summary>
    private const string VerifyCodePlaceHolder = "${verify-code}";

    /// <summary>
    /// 发送验证码, 默认过期时间 30 分钟
    /// </summary>
    public async Task<string> SendAsync(string toName, string toAddress, CodeType codeType, int expireTime = 1800)
    {
        var code = Random.Shared.Next(1000, 10000).ToString();
        logger.LogInformation("SendCode: toName {ToName}, toAddress {ToAddress}, code {Code}", toName, toAddress, code);
        var database = redisClient.GetDatabase();
        var key = codeType.GetDesc() + "_" + VerifyCodePrefix + "_" + toAddress;

        var officialWebsite = appOptions.Value.OfficialWebsite;
        var officialEmail = appOptions.Value.OfficialEmail;
        var emailTemplate = "<!DOCTYPE html><html lang=\"en\"><head> <meta charset=\"UTF-8\"> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> <title>乡建DAO Verification Email</title> <style> body { font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f5f5f5; } .container { max-width: 600px; margin: 20px auto; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 10px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); } .header { background-color: #0078d4; color: #ffffff; padding: 10px 20px; border-radius: 10px 10px 0 0; text-align: left; } .header img { max-width: 100px; } .content { padding: 20px 20px 10px 20px; } .content p { margin: 0; line-height: 1.5; } .content .highlight { font-weight: bold; color: #0078d4; } .footer { font-size: 12px; color: #666666; padding: 5px 20px; } </style></head><body> <div class=\"container\"> <div class=\"header\"> <p><strong> 乡建DAO </strong></p> </div> <div class=\"content\"> <p><strong>[ 乡建DAO ] 验证码</strong></p> <br/> <p>您的验证码是 <span class=\"highlight\">${verify-code}</span>，验证有效期为30分钟。</p> <br/> </div> <div class=\"footer\"> <p>乡建DAO 系统邮件，请勿回复。</p> <p>官方网站: <a href=\""+officialWebsite+"\">"+officialWebsite+"</a> 官方邮箱: <a href=\"mailto:"+officialEmail+"\">"+officialEmail+"</a></p> </div> </div></body></html>";
        var content = emailTemplate.Replace(VerifyCodePlaceHolder, code);

        database.StringSet(key, code, TimeSpan.FromSeconds(expireTime));
        await emailSender.SendAsync(content, toName, toAddress);
        return code;
    }

    /// <summary>
    /// 验证邮箱验证码
    /// </summary>
    public async Task<bool> VerifyAsync(string emailAddress, string code, CodeType codeType)
    {
        var database = redisClient.GetDatabase();
        var key = codeType.GetDesc() + "_" + VerifyCodePrefix + "_" + emailAddress;
        if (!database.KeyExists(key))
        {
            return false;
        }

        var verifyCode = (await database.StringGetAsync(key)).ToString();
        return verifyCode == code;
    }

    /// <summary>
    /// 通知管理员
    /// </summary>
    public async Task<bool> SendToAdminAsync(string toName, string toAddress, string content)
    {
        await emailSender.SendAsync(content, toName, toAddress);
        return true;
    }
}