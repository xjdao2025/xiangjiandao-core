# 删除提案评论功能实现方案

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 增加删除提案评论功能：个人可删除自己的评论（`/api/v1/proposal/delete-my-comment`），管理员可删除任意评论（`/api/v1/admin/proposal/delete-comment`）。

**Architecture:** 采用与现有提案删除（`delete-my-proposal` / `take-off`）一致的 MediatR + FastEndpoints 架构。每个接口对应一个 Endpoint、一个 Request DTO、一个 Command 和一个 CommandHandler。评论软删除复用 `ProposalComment.Delete()` 方法。查询详情时已过滤 `!c.Deleted`，无需额外修改查询逻辑。

**Tech Stack:** .NET 9, ASP.NET Core, FastEndpoints, MediatR, EF Core, MySQL

---

## 前置知识（必读）

### 现有评论相关代码

- **Domain 实体:** `src/Xiangjiandao.Domain/AggregatesModel/ProposalAggregate/ProposalComment.cs`
  - 已有 `Delete()` 方法：`Deleted = true; return true;`
- **EF 配置:** `src/Xiangjiandao.Infrastructure/EntityConfigurations/ProposalCommentTypeConfiguration.cs`
- **DbContext:** `src/Xiangjiandao.Infrastructure/ApplicationDbContext.cs` 已有 `DbSet<ProposalComment> ProposalComments`
- **创建评论命令:** `src/Xiangjiandao.Web/Application/Commands/CreateProposalCommentCommand.cs`
  - Handler 直接使用 `ApplicationDbContext` 查询和保存
- **创建评论接口:** `src/Xiangjiandao.Web/Endpoints/Proposal/CreateProposalCommentEndpoint.cs`
- **现有删除提案模式:**
  - 个人: `src/Xiangjiandao.Web/Endpoints/Proposal/DeleteMyProposalEndpoint.cs` + `DeleteMyProposalCommand.cs`
  - 管理员: `src/Xiangjiandao.Web/Endpoints/AdminProposal/AdminTakeOffProposalEndpoint.cs` + `TakeOffProposalCommand.cs`

### 权限策略

- 客户端用户: `[Authorize(PolicyNames.Client)]`
- 管理员: `[Authorize(PolicyNames.Admin)]`

---

### Task 1: 创建个人删除评论命令

**Files:**
- Create: `src/Xiangjiandao.Web/Application/Commands/DeleteMyProposalCommentCommand.cs`

**Step 1: Write the command and handler**

```csharp
using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Infrastructure;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 删除我的提案评论命令
/// </summary>
public class DeleteMyProposalCommentCommand : ICommand<bool>
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    public required UserId UserId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class DeleteMyProposalCommentCommandHandler(
    ApplicationDbContext dbContext,
    ILogger<DeleteMyProposalCommentCommandHandler> logger
) : ICommandHandler<DeleteMyProposalCommentCommand, bool>
{
    public async Task<bool> Handle(DeleteMyProposalCommentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteMyProposalCommentCommand Handling");

        var comment = await dbContext.ProposalComments
            .FirstOrDefaultAsync(c => c.Id == command.CommentId && !c.Deleted, cancellationToken);

        if (comment is null)
        {
            throw new KnownException("评论未找到");
        }

        if (comment.UserId != command.UserId)
        {
            throw new KnownException("无权删除该评论");
        }

        comment.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
```

**Step 2: Build verification**

Run: `dotnet build src/Xiangjiandao.Web/Xiangjiandao.Web.csproj`
Expected: PASS

**Step 3: Commit**

```bash
git add src/Xiangjiandao.Web/Application/Commands/DeleteMyProposalCommentCommand.cs
git commit -m "feat: add delete my proposal comment command"
```

---

### Task 2: 创建管理员删除评论命令

**Files:**
- Create: `src/Xiangjiandao.Web/Application/Commands/AdminDeleteProposalCommentCommand.cs`

**Step 1: Write the command and handler**

```csharp
using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Primitives;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Infrastructure;

namespace Xiangjiandao.Web.Application.Commands;

/// <summary>
/// 管理员删除提案评论命令
/// </summary>
public class AdminDeleteProposalCommentCommand : ICommand<bool>
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }
}

/// <summary>
/// 命令处理器
/// </summary>
public class AdminDeleteProposalCommentCommandHandler(
    ApplicationDbContext dbContext,
    ILogger<AdminDeleteProposalCommentCommandHandler> logger
) : ICommandHandler<AdminDeleteProposalCommentCommand, bool>
{
    public async Task<bool> Handle(AdminDeleteProposalCommentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("AdminDeleteProposalCommentCommand Handling");

        var comment = await dbContext.ProposalComments
            .FirstOrDefaultAsync(c => c.Id == command.CommentId && !c.Deleted, cancellationToken);

        if (comment is null)
        {
            throw new KnownException("评论未找到");
        }

        comment.Delete();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
```

**Step 2: Build verification**

Run: `dotnet build src/Xiangjiandao.Web/Xiangjiandao.Web.csproj`
Expected: PASS

**Step 3: Commit**

```bash
git add src/Xiangjiandao.Web/Application/Commands/AdminDeleteProposalCommentCommand.cs
git commit -m "feat: add admin delete proposal comment command"
```

---

### Task 3: 创建个人删除评论接口

**Files:**
- Create: `src/Xiangjiandao.Web/Endpoints/Proposal/DeleteMyProposalCommentEndpoint.cs`

**Step 1: Write the endpoint and request DTO**

参考 `DeleteMyProposalEndpoint.cs` 的代码风格：

```csharp
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Shared;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.Proposal;

/// <summary>
/// 删除我的提案评论
/// </summary>
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/delete-my-comment")]
[Authorize(PolicyNames.Client)]
public class DeleteMyProposalCommentEndpoint(
    IMediator mediator,
    ILoginUser loginUser
) : Endpoint<DeleteMyProposalCommentReq, ResponseData<bool>>
{
    public override async Task HandleAsync(DeleteMyProposalCommentReq req, CancellationToken ct)
    {
        var userId = new UserId(loginUser.Id);
        var command = req.ToCommand(userId);
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 删除我的提案评论请求
/// </summary>
public class DeleteMyProposalCommentReq
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public DeleteMyProposalCommentCommand ToCommand(UserId userId)
    {
        return new DeleteMyProposalCommentCommand
        {
            CommentId = CommentId,
            UserId = userId
        };
    }
}
```

**Step 2: Build verification**

Run: `dotnet build src/Xiangjiandao.Web/Xiangjiandao.Web.csproj`
Expected: PASS

**Step 3: Commit**

```bash
git add src/Xiangjiandao.Web/Endpoints/Proposal/DeleteMyProposalCommentEndpoint.cs
git commit -m "feat: add delete my proposal comment endpoint"
```

---

### Task 4: 创建管理员删除评论接口

**Files:**
- Create: `src/Xiangjiandao.Web/Endpoints/AdminProposal/AdminDeleteProposalCommentEndpoint.cs`

**Step 1: Write the endpoint and request DTO**

参考 `AdminTakeOffProposalEndpoint.cs` 的代码风格：

```csharp
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Web.Application.Commands;
using Xiangjiandao.Web.Extensions;
using Xiangjiandao.Web.Utils;

namespace Xiangjiandao.Web.Endpoints.AdminProposal;

/// <summary>
/// 后台删除提案评论
/// </summary>
[Tags("AdminProposal")]
[HttpPost("/api/v1/admin/proposal/delete-comment")]
[Authorize(PolicyNames.Admin)]
public class AdminDeleteProposalCommentEndpoint(IMediator mediator)
    : Endpoint<AdminDeleteProposalCommentReq, ResponseData<bool>>
{
    public override async Task HandleAsync(AdminDeleteProposalCommentReq req, CancellationToken ct)
    {
        var command = req.ToCommand();
        await SendAsync(
            response: await mediator.Send(command, ct).AsSuccessResponseData(),
            cancellation: ct
        );
    }
}

/// <summary>
/// 后台删除提案评论请求
/// </summary>
public class AdminDeleteProposalCommentReq
{
    /// <summary>
    /// 评论 Id
    /// </summary>
    public required ProposalCommentId CommentId { get; set; }

    /// <summary>
    /// 请求转命令
    /// </summary>
    public AdminDeleteProposalCommentCommand ToCommand()
    {
        return new AdminDeleteProposalCommentCommand
        {
            CommentId = CommentId,
        };
    }
}
```

**Step 2: Build verification**

Run: `dotnet build src/Xiangjiandao.Web/Xiangjiandao.Web.csproj`
Expected: PASS

**Step 3: Commit**

```bash
git add src/Xiangjiandao.Web/Endpoints/AdminProposal/AdminDeleteProposalCommentEndpoint.cs
git commit -m "feat: add admin delete proposal comment endpoint"
```

---

### Task 5: 运行 LSP Diagnostics / 全量编译验证

**Step 1: 编译整个解决方案**

Run: `dotnet build Xiangjiandao.sln`
Expected: 0 error(s), 0 warning(s)（或仅有与本次变更无关的已有 warning）

**Step 2: 如有测试项目，运行相关测试**

当前 test 目录下没有针对 ProposalComment 的专门测试。如果有集成测试基础设施，可以补充测试；否则以编译通过为准。

---

## 接口变更汇总

| 接口 | 方法 | 权限 | 说明 |
|---|---|---|---|
| `/api/v1/proposal/delete-my-comment` | POST | Client | 删除当前用户自己的评论 |
| `/api/v1/admin/proposal/delete-comment` | POST | Admin | 管理员删除任意评论 |

## 安全性

- 个人删除接口验证 `comment.UserId == command.UserId`，否则抛出 `"无权删除该评论"`
- 两个接口均对不存在的评论或已删除评论返回 `"评论未找到"`
- 采用软删除（`Deleted = true`），不物理删除数据
- 查询详情接口（`ProposalQuery.Detail` / `AdminDetail`）已过滤 `!c.Deleted`，被删除评论不会出现在详情中
