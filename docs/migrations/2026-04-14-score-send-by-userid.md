# 稻米明细页面快捷发送稻米方案

## 背景

用户希望在稻米明细页面可以直接继续发送稻米。为此需要：
1. 稻米明细接口返回对方的 `participator_id`，供前端直接调用发送接口。
2. 发送稻米接口支持通过 `userid`（`participator_id`）定位接收人，不再强制依赖手机号/邮箱。

## 变更内容

### 1. 查询接口：`/api/v1/score/user-sore-record-page`

**变更：** `UserScoreRecordPageVo` 新增 `ParticipatorId` 字段。

前端可在明细列表中拿到对方的 `UserId`，直接作为发送接口的 `ToUserId` 入参。

### 2. 发送接口：`/api/v1/score/send`

**变更：** `SendScoreReq` 支持 `ToUserId` 与 `UserPhoneOrEmail` 二选一。

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `ToUserId` | `UserId?` | 否 | 接收用户的 UserId，优先使用 |
| `UserPhoneOrEmail` | `string` | 否 | 接收用户的手机号或邮箱，未传 `ToUserId` 时必填 |
| `Score` | `long` | 是 | 稻米数量，必须大于 0 |
| `Remark` | `string` | 否 | 附言，最大 20 个字符，默认空字符串 |

**接口处理逻辑：**
1. 若 `ToUserId` 不为空且有效，通过 `UserQuery.GetUserById` 查询接收用户。
2. 否则若 `UserPhoneOrEmail` 不为空，通过 `UserQuery.GetUserByPhoneOrEmail` 查询接收用户。
3. 两者都未命中时返回 `"接收用户不存在"`。
4. 其余业务逻辑（余额检查、不能给自己发送、分布式锁、创建记录等）保持不变。

## 涉及文件

- `src/Xiangjiandao.Web/Endpoints/Score/UserScoreRecordPageEndpoint.cs`
  - `UserScoreRecordPageVo` 增加 `ParticipatorId`
- `src/Xiangjiandao.Web/Endpoints/Score/SendScoreEndpoint.cs`
  - `SendScoreReq` 增加 `ToUserId`，`UserPhoneOrEmail` 改为可选
  - `HandleAsync` 支持按 `ToUserId` 查询接收用户
  - `SendScoreReqValidator` 校验规则更新为二选一
- `src/Xiangjiandao.Web/Application/Queries/ScoreRecordQuery.cs`
  - `Page` 查询映射 `ParticipatorId`

## 数据兼容性

- **数据库表结构：** 无变更。`t_point_record` 表已有 `participator_id` 列；发送接口仅变更入参校验与查询方式。
- **已有前端：** 若继续只传 `UserPhoneOrEmail`，行为与改造前完全一致。
