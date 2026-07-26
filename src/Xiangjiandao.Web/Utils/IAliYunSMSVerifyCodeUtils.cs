using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Primitives;
using StackExchange.Redis;
using Xiangjiandao.Web.Clients;
using Xiangjiandao.Web.Options;

namespace Xiangjiandao.Web.Utils;

/// <summary>
/// 短信发送验证码工具
/// </summary>
#pragma warning disable S101
public interface IAliYunSMSVerifyCodeUtils
#pragma warning restore S101
{
    /// <summary>
    /// 发送验证码, 默认过期时间 30分钟
    /// </summary>
    Task<string> SendAsync(string phoneRegion, string phone, string scene, int expireTime = 1800, CancellationToken cancellationToken = default!);

    /// <summary>
    /// 验证短信验证码
    /// </summary>
    Task<bool> VerifyAsync(string phoneRegion, string phone, string code, string scene, CancellationToken cancellationToken = default!);
}

/// <summary>
/// 短信发送验证码工具
/// </summary>
#pragma warning disable S101
public class AliYunSmsVerifyCodeUtils(
#pragma warning restore S101
    IConnectionMultiplexer redisClient,
    IAliYunSmsClient aliYunSmsClient,
    ILogger<AliYunSmsVerifyCodeUtils> logger,
    IOptions<AliYunSmsOptions> aliYunSmsOptions
) : IAliYunSMSVerifyCodeUtils
{
    /// <summary>
    /// 短信验证码 Redis Key 前缀
    /// </summary>
    private const string SmsVerifyCodePrefix = "_xiangjiandao_sms_verify_code_";

    /// <summary>
    /// 发送短信验证码
    /// </summary>
    /// <param name="phoneRegion"></param>
    /// <param name="phone"></param>
    /// <param name="scene"></param>
    /// <param name="expireTime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> SendAsync(string phoneRegion, string phone,string scene, int expireTime = 1800, CancellationToken cancellationToken = default!)
    {
        var code = Random.Shared.Next(100000, 999999).ToString();
        logger.LogInformation("SendCode: phoneRegion {PhoneRegion}, phone {Phone}, code {Code}", phoneRegion, phone, code);
        var database = redisClient.GetDatabase();
        var key = scene + SmsVerifyCodePrefix + "_" + phoneRegion + "_" + phone;
        
        await aliYunSmsClient.SendSmsAsync(phoneRegion + phone, code);
        database.StringSet(key, code, TimeSpan.FromSeconds(expireTime));
        database.StringSet(key+":times", aliYunSmsOptions.Value.Times, TimeSpan.FromSeconds(expireTime));
        return code;
    }
    
    /// <summary>
    /// 验证短信验证码
    /// </summary>
    /// <param name="phoneRegion"></param>
    /// <param name="phone"></param>
    /// <param name="code"></param>
    /// <param name="scene"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="KnownException"></exception>
    public async Task<bool> VerifyAsync(string phoneRegion, string phone, string code, string scene, CancellationToken cancellationToken = default!)
    {
        var database = redisClient.GetDatabase();
        var key = scene + SmsVerifyCodePrefix + "_" + phoneRegion + "_" + phone;
        var storedCode = await database.StringGetAsync(key);
        if (string.IsNullOrEmpty(storedCode))
        {
            throw new KnownException("验证码已过期");
        }
        var result = storedCode.ToString() == code;
        if (result)
        {
            await database.KeyDeleteAsync(key);
            await database.KeyDeleteAsync(key+":times");
        }
        else
        {
            var times = await database.StringDecrementAsync(key+":times");
            if (times <= 0)
            {
                await database.KeyDeleteAsync(key);
                await database.KeyDeleteAsync(key+":times");
                throw new KnownException("验证码已过期");
            }
            else
            {
                throw new KnownException("验证码错误");
            }
        }
        
        return result;
    }
}