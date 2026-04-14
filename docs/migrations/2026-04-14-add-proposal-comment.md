# 提案评论功能后端实现方案

## 背景

为提案详情页增加简单的文字回复功能。仅支持一级评论（不支持回复的回复），显示回复者、时间及回复内容。

## 数据库变更

### 1. 新增 `t_proposal_comment` 表

```sql
CREATE TABLE `t_proposal_comment` (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '评论主键Id',
  `proposal_id` char(36) NOT NULL COMMENT '提案Id',
  `user_id` char(36) NOT NULL COMMENT '评论用户Id',
  `user_name` varchar(64) NOT NULL DEFAULT '' COMMENT '评论用户名称',
  `content` varchar(512) NOT NULL DEFAULT '' COMMENT '评论内容',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `deleted` tinyint NOT NULL DEFAULT '0' COMMENT '是否删除',
  PRIMARY KEY (`id`),
  KEY `idx_proposal_id` (`proposal_id`),
  KEY `idx_created_at` (`created_at`)
) CHARACTER SET=utf8mb4;
```

## 后端代码变更

### 2. Domain 层 - 新增 `ProposalComment` 实体

文件：`src/Xiangjiandao.Domain/AggregatesModel/ProposalAggregate/ProposalComment.cs`

- `ProposalCommentId` (long, IInt64StronglyTypedId)
- `ProposalId` (ProposalId)
- `UserId` (UserId)
- `UserName` (string)
- `Content` (string)
- `CreatedAt` (DateTimeOffset)
- `Deleted` (Deleted)
- 工厂方法 `Create(ProposalId proposalId, UserId userId, string userName, string content)`

### 3. Infrastructure 层

#### 3.1 EF Configuration

文件：`src/Xiangjiandao.Infrastructure/EntityConfigurations/ProposalCommentTypeConfiguration.cs`

配置 `t_proposal_comment` 表映射。

#### 3.2 DbContext

文件：`src/Xiangjiandao.Infrastructure/ApplicationDbContext.cs`

新增 `DbSet<ProposalComment> ProposalComments => Set<ProposalComment>();`

#### 3.3 EF Migration

生成 Migration：`Add-Migration AddProposalComment`

### 4. Application 层 - 新增评论命令

文件：`src/Xiangjiandao.Web/Application/Commands/CreateProposalCommentCommand.cs`

```csharp
public record CreateProposalCommentCommand(
    ProposalId ProposalId,
    UserId UserId,
    string UserName,
    string Content
) : ICommand<bool>;
```

Handler 逻辑：
- 校验提案存在且未删除
- `ProposalComment.Create(...)`
- `dbContext.ProposalComments.Add`
- `await dbContext.SaveChangesAsync`

### 5. Web 层 - 新增接口与修改现有接口

#### 5.1 发表评论接口

文件：`src/Xiangjiandao.Web/Endpoints/Proposal/CreateProposalCommentEndpoint.cs`

```csharp
[Tags("Proposal")]
[HttpPost("/api/v1/proposal/comment")]
[Authorize(PolicyNames.Client)]
```

请求体：
- `ProposalId` (ProposalId)
- `Content` (string, max 512)

#### 5.2 提案详情接口增加评论列表

文件：`src/Xiangjiandao.Web/Endpoints/Proposal/ProposalDetailEndpoint.cs`

在 `ProposalDetailVo` 中新增字段：

```csharp
public List<ProposalCommentVo> Comments { get; set; } = [];
```

`ProposalCommentVo` 字段：
- `CommentId` (long)
- `UserName` (string)
- `Content` (string)
- `CreatedAt` (DateTimeOffset)

修改 `ProposalQuery.Detail`：
- 联查 `ProposalComments`（按 `proposal_id` + `deleted = 0` + 按 `created_at` 升序）

## 接口变更汇总

| 接口 | 方法 | 变更 |
|---|---|---|
| `/api/v1/proposal/detail` | POST | 响应体新增 `comments` 数组 |
| `/api/v1/proposal/comment` | POST | 新增接口，用于发表评论 |

## 安全性

- 登录用户才能发表评论（`[Authorize(PolicyNames.Client)]`）
- 评论内容最大 512 字符
- 对不存在的提案返回 `"提案未找到"`
