using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Dto;
using Xiangjiandao.Web.Application.Req;
using Xiangjiandao.Web.Application.Vo;
using Xiangjiandao.Web.Endpoints.User;
using Xiangjiandao.Web.Endpoints.UserManage;

namespace Xiangjiandao.Web.Application.Queries;

public class UserQuery(
    ApplicationDbContext applicationDbContext,
    IMemoryCache memoryCache,
    ILogger<UserQuery> logger)
{
    /// <summary>
    /// 普通用户列表
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PagedData<UserPageVo>> UserPage(UserPageReq req, CancellationToken cancellationToken)
    {
        logger.LogInformation("普通用户列表");
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => !x.NodeUser)
            .WhereIf(!string.IsNullOrEmpty(req.NickName), x => x.NickName.Contains(req.NickName))
            .WhereIf(!string.IsNullOrEmpty(req.Did), x => x.Did.Contains(req.Did))
            .WhereIf(!string.IsNullOrEmpty(req.DomainName), x => x.DomainName.Contains(req.DomainName))
            .WhereIf(!string.IsNullOrEmpty(req.Email), x => x.Email.Contains(req.Email))
            .WhereIf(!string.IsNullOrEmpty(req.Phone), x => x.Phone.Contains(req.Phone))
            .OrderByDescending(x => x.CreatedAt)
            .Select(user => new UserPageVo
            {
                Id = user.Id,
                NickName = user.NickName,
                Did = user.Did,
                DomainName = user.DomainName,
                Phone = user.Phone,
                Email = user.Email,
                Score = user.Score,
                Disable = user.Disable,
                CreatedAt = user.CreatedAt,
            })
            .ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 节点用户列表
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PagedData<NodeUserPageVo>> NodeUserPage(NodeUserPageReq req, CancellationToken cancellationToken)
    {
        logger.LogInformation("普通用户列表");
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => x.NodeUser)
            .WhereIf(!string.IsNullOrEmpty(req.NickName), x => x.NickName.Contains(req.NickName))
            .WhereIf(!string.IsNullOrEmpty(req.Did), x => x.Did.Contains(req.Did))
            .WhereIf(!string.IsNullOrEmpty(req.DomainName), x => x.DomainName.Contains(req.DomainName))
            .WhereIf(!string.IsNullOrEmpty(req.Email), x => x.Email.Contains(req.Email))
            .WhereIf(!string.IsNullOrEmpty(req.Phone), x => x.Phone.Contains(req.Phone))
            .OrderByDescending(x => x.CreatedAt)
            .Select(user => new NodeUserPageVo
            {
                Id = user.Id,
                NickName = user.NickName,
                Did = user.Did,
                DomainName = user.DomainName,
                Phone = user.Phone,
                Email = user.Email,
                Score = user.Score,
                Disable = user.Disable,
                CreatedAt = user.CreatedAt,
            })
            .ToPagedDataAsync(req.PageNum, req.PageSize, countTotal: true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 根据Id查询用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserDetailDto?> GetUserById(UserId id, CancellationToken cancellationToken = default)
    {
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(selector: user => new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                NickName = user.NickName,
                Score = user.Score,
                Disable = user.Disable,
                Did = user.Did,
                NodeUser = user.NodeUser,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 根据手机号或邮箱查询用户信息
    /// </summary>
    /// <param name="phoneOrEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserDetailDto?> GetUserByPhoneOrEmail(string phoneOrEmail,
        CancellationToken cancellationToken = default)
    {
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => x.Email == phoneOrEmail || x.Phone == phoneOrEmail)
            .Select(user => new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                NickName = user.NickName,
                Score = user.Score,
                Disable = user.Disable,
                Did = user.Did,
                NodeUser = user.NodeUser,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 根据did查询用户信息
    /// </summary>
    /// <param name="did"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserDetailDto?> GetUserByDid(string did, CancellationToken cancellationToken = default)
    {
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => x.Did == did)
            .Select(user => new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                NickName = user.NickName,
                Score = user.Score,
                Disable = user.Disable,
                Did = user.Did,
                NodeUser = user.NodeUser,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 用户登录信息
    /// </summary>
    public async Task<UserLoginInfo?> UserLoginInfo(
        LoginType loginType,
        string domainName,
        string email,
        string phone,
        CancellationToken cancellationToken
    )
    {
        return await applicationDbContext.Users
            .WhereIf(loginType == LoginType.Email, user => user.Email == email)
            .WhereIf(loginType == LoginType.Phone, user => user.Phone == phone)
            .WhereIf(loginType == LoginType.DomainName, user => user.DomainName == domainName)
            .Select(user => new UserLoginInfo
            {
                Phone = user.Phone,
                Email = user.Email,
                DomainName = user.DomainName,
                PhoneRegion = user.PhoneRegion,
                UserId = user.Id,
                Disable = user.Disable,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 获取重置密码信息
    /// </summary>
    public async Task<ResetPasswordInfo?> GetResetPasswordInfo(string phone, string email, CancellationToken ct)
    {
        return await applicationDbContext.Users
            .WhereIf(!string.IsNullOrEmpty(email), user => user.Email == email)
            .WhereIf(!string.IsNullOrEmpty(phone), user => user.Phone == phone)
            .Where(user => !user.Disable)
            .Select(user => new ResetPasswordInfo
            {
                Did = user.Did,
            })
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// 根据Id查询用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserDetailVo?> GetLoginUserById(UserId id, CancellationToken cancellationToken = default)
    {
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => x.Id == id && !x.Disable)
            .Select(user => new UserDetailVo
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                NickName = user.NickName,
                DomainName = user.DomainName,
                Did = user.Did,
                Score = user.Score,
                NodeUser = user.NodeUser,
                Disable = user.Disable
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 模糊查找普通用户
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<UserSearchVo>> UserSearch(UserSearchReq req, CancellationToken cancellationToken)
    {
        logger.LogInformation("模糊查找普通用户");
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => !x.NodeUser && !x.Disable)
            .WhereIf(!string.IsNullOrEmpty(req.PhoneOrEmail),
                user => user.Phone.Contains(req.PhoneOrEmail) || user.Email.Contains(req.PhoneOrEmail))
            .Select(user => new UserSearchVo
            {
                UserId = user.Id,
                Phone = user.Phone,
                Email = user.Email
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 模糊查找未绑定节点的节点用户
    /// </summary>
    public async Task<List<UnboundNodeUserSearchVo>> UnboundNodeUserSearch(
        string phoneOrEmail,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("模糊查找未绑定节点的节点用户");
        var nodeUserVos = await applicationDbContext.Users.AsNoTracking()
            .Where(user => user.NodeUser && !user.Disable)
            .WhereIf(!string.IsNullOrEmpty(phoneOrEmail), user => user.Phone.Contains(phoneOrEmail) ||
                                                                  user.Email.Contains(phoneOrEmail))
            .Select(user => new UnboundNodeUserSearchVo
            {
                UserId = user.Id,
                UserDid = user.Did,
                Phone = user.Phone,
                Email = user.Email
            })
            .ToListAsync(cancellationToken);

        var boundNodeUser = await applicationDbContext.Nodes
            .Select(node => node.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var unboundNodeUserVos = nodeUserVos.Where(node => !boundNodeUser.Contains(node.UserId)).ToList();
        return unboundNodeUserVos;
    }

    /// <summary>
    /// 删除用户信息
    /// </summary>
    public async Task<UserDeleteInfo?> GetUserDeleteInfo(string phone, string email, CancellationToken ct)
    {
        return await applicationDbContext.Users
            .WhereIf(!string.IsNullOrEmpty(email), user => user.Email == email)
            .WhereIf(!string.IsNullOrEmpty(phone), user => user.Phone == phone)
            .Where(user => !user.Disable)
            .Select(user => new UserDeleteInfo
            {
                Did = user.Did,
            })
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// 根据昵称模糊查找普通用户
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<UserSearchByNameVo>> UserSearchByName(UserSearchByNameReq req,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("根据昵称模糊查找普通用户");
        return await applicationDbContext.Users.AsNoTracking()
            .WhereIf(!string.IsNullOrEmpty(req.NickName),
                user => user.NickName.Contains(req.NickName.Trim()) || user.DomainName.Contains(req.NickName.Trim()))
            .Select(user => new UserSearchByNameVo
            {
                Avatar = user.Avatar,
                NickName = user.NickName,
                DomainName = user.DomainName,
                Did = user.Did,
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 根据domainName查询用户信息
    /// </summary>
    /// <param name="domainName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserDetailDto?> GetUserByDomainName(string domainName,
        CancellationToken cancellationToken = default)
    {
        return await applicationDbContext.Users.AsNoTracking()
            .Where(x => x.DomainName == domainName)
            .Select(user => new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                NickName = user.NickName,
                Score = user.Score,
                Disable = user.Disable,
                Did = user.Did,
                NodeUser = user.NodeUser,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 根据 domain 或者 did 查询用户信息
    /// </summary>
    private async Task<UserDetailDto?> GetUserByDomainOrDid(string domainOrDid,
        CancellationToken cancellationToken = default)
    {
        return await applicationDbContext.Users.AsNoTracking()
            .Where(user => user.DomainName == domainOrDid || user.Did == domainOrDid)
            .Select(selector: user => new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                PhoneRegion = user.PhoneRegion,
                NickName = user.NickName,
                Score = user.Score,
                Did = user.Did,
                Disable = user.Disable,
                NodeUser = user.NodeUser,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 用户信息查询加上缓存
    /// </summary>
    public async Task<UserDetailDto?> GetUserFromCache(string domainOrDid,
        CancellationToken cancellationToken = default)
    {
        // 定义缓存键
        var cacheKey = $"userData:{domainOrDid}";
        if (!memoryCache.TryGetValue(cacheKey, out UserDetailDto? cachedData) ||
            cachedData is null)
        {
            logger.LogInformation("缓存未命中，执行查询 UserQuery.GetUserByDomainOrDid");
            var data = await GetUserByDomainOrDid(domainOrDid, cancellationToken);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

            memoryCache.Set(cacheKey, data, cacheEntryOptions);

            return data;
        }

        // 如果缓存命中，直接返回缓存的数据
        logger.LogInformation("缓存命中,直接返回");
        return cachedData;
    }

    /// <summary>
    /// 校验用户
    /// </summary>
    public async Task<bool> UserDisabled(string did, CancellationToken cancellationToken = default)
    {
        var user = await GetUserFromCache(did, cancellationToken);
        if (user is null)
        {
            throw new KnownException("用户不存在");
        }

        return user.Disable;
    }

    /// <summary>
    /// 判断用户是否已经存在
    /// </summary>
    public async Task<bool> UserExists(string email, string phone, CancellationToken cancellationToken)
    {
        User? result = null;
        if (!string.IsNullOrEmpty(phone))
        {
            result = await applicationDbContext.Users.FirstOrDefaultAsync(
                predicate: user => user.Phone == phone,
                cancellationToken: cancellationToken
            );
        }

        if (!string.IsNullOrEmpty(email))
        {
            result ??= await applicationDbContext.Users.FirstOrDefaultAsync(
                predicate: user => user.Email == email,
                cancellationToken: cancellationToken
            );
        }

        return result is not null;
    }

    /// <summary>
    /// 是否是节点用户
    /// </summary>
    public async Task<bool> IsNodeUser(string userDid)
    {
        var user = await GetUserFromCache(userDid);
        return user?.NodeUser ?? false;
    }

    /// <summary>
    /// 前台节点用户列表
    /// </summary>
    public async Task<List<NodeUserVo>> NodeUserList(CancellationToken cancellationToken)
    {
        return await applicationDbContext.Users
            .Where(user => user.NodeUser)
            .Select(user => new NodeUserVo
            {
                Id = user.Id,
                Did = user.Did
            })
            .ToListAsync(cancellationToken);
    }
}