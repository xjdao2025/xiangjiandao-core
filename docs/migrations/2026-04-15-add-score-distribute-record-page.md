# 新增客户端稻米发放记录分页查询接口

## 背景

现有后台接口 `/api/v1/admin/score-distribute-record/page` 仅供管理员使用，且返回字段包含 `nick_name`、`phone`、`email`、`created_by` 等敏感/后台信息。

需要新增一个普通客户端接口 `/api/v1/score-distribute-record/page`，用于公开查询每个节点获取稻米的数量记录，返回字段精简为 `user_id`、`domain_name`、`score`、`get_time`。

## 变更内容

### 1. 新增接口：`/api/v1/score-distribute-record/page`

**方法：** `POST`

**权限：** `[Authorize(PolicyNames.Client)]`

**Tag：** `Scores`

**请求参数：**

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `PageNum` | `int` | 否 | 页码，默认 1 |
| `PageSize` | `int` | 否 | 分页大小，默认 10 |

**返回字段（`ScoreDistributeRecordPageVo`）：**

| 字段 | 类型 | 说明 |
|------|------|------|
| `user_id` | `UserId` | 用户Id |
| `domain_name` | `string` | 完整用户名，域名 |
| `score` | `long` | 发放稻米数量 |
| `get_time` | `DateTimeOffset` | 获取时间 |

### 2. 实现说明

- **Endpoint 位置：** `src/Xiangjiandao.Web/Endpoints/Score/ScoreDistributeRecordPageEndpoint.cs`
- **数据来源：** `t_point_distribute_record` 表（`ScoreDistributeRecord` 实体）
- **`domain_name` 获取方式：** 由于 `t_point_distribute_record` 表本身不存储 `domain_name`，查询时通过 `UserId` **join `Users` 表**获取 `DomainName`。
- **排序：** 按 `get_time` 降序排列。
- **分页：** 复用 `ToPagedDataAsync` 扩展方法，返回总条数。

## 涉及文件

| 操作 | 文件路径 |
|------|----------|
| 新增 | `src/Xiangjiandao.Web/Endpoints/Score/ScoreDistributeRecordPageEndpoint.cs` |
| 新增 | `src/Xiangjiandao.Web/Application/Req/ScoreDistributeRecordPageReq.cs` |
| 新增 | `src/Xiangjiandao.Web/Application/Vo/ScoreDistributeRecordPageVo.cs` |
| 修改 | `src/Xiangjiandao.Web/Application/Queries/ScoreDistributeRecordQuery.cs` |

## 数据兼容性

- **数据库表结构：** 无变更。`t_point_distribute_record` 表结构保持不变。
- **已有接口：** 后台接口 `/api/v1/admin/score-distribute-record/page` 不受影响，继续使用。
