# 新增 `remark` 附言字段

## 背景

为积分（稻米）记录表 `t_point_record` 增加 `remark` 附言字段，用于存储用户在赠送稻米时输入的附言。

## 执行 SQL

在目标数据库直接执行以下语句：

```sql
ALTER TABLE `t_point_record`
ADD COLUMN `remark` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '附言';
```

## 说明

- 已有数据会自动填充为空字符串 `''`，不会导致 `NOT NULL` 报错。
- 新产生的记录：用户填写附言时入库；未填写时默认为空字符串。
- 查询接口已同步返回 `remark` 字段，前端如需兜底展示可结合 `reason` 字段处理。

## 影响范围

- `SendScoreEndpoint` (`/api/v1/score/send`)：请求体增加 `remark` 字段，最大长度 20 个字符。
- `UserScoreRecordPageEndpoint` (`/api/v1/score/user-sore-record-page`)：响应体增加 `remark` 字段。
- `AdminUserScoreRecordPageEndpoint` (`/api/v1/admin/score/user-sore-record-page`)：响应体增加 `remark` 字段。
