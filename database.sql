CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `CAPLock` (
    `Key` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Instance` varchar(256) CHARACTER SET utf8mb4 NULL,
    `LastLockTime` DATETIME(6) NULL,
    CONSTRAINT `PK_CAPLock` PRIMARY KEY (`Key`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `CAPPublishedMessage` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Version` varchar(20) CHARACTER SET utf8mb4 NULL,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Content` longtext CHARACTER SET utf8mb4 NULL,
    `Retries` int NULL,
    `Added` DATETIME(6) NOT NULL,
    `ExpiresAt` DATETIME(6) NULL,
    `StatusName` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_CAPPublishedMessage` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `CAPReceivedMessage` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Version` varchar(20) CHARACTER SET utf8mb4 NULL,
    `Name` varchar(400) CHARACTER SET utf8mb4 NOT NULL,
    `Group` varchar(200) CHARACTER SET utf8mb4 NULL,
    `Content` longtext CHARACTER SET utf8mb4 NULL,
    `Retries` int NULL,
    `Added` DATETIME(6) NOT NULL,
    `ExpiresAt` DATETIME(6) NULL,
    `StatusName` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_CAPReceivedMessage` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `DataProtectionKeys` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FriendlyName` longtext CHARACTER SET utf8mb4 NULL,
    `Xml` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_DataProtectionKeys` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_admin_user` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `email` varchar(128) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '注册邮箱',
    `phone` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '手机号',
    `phone_region` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '手机区号',
    `avatar` varchar(128) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户头像',
    `role` int NOT NULL DEFAULT 0 COMMENT '用户角色',
    `special` tinyint NOT NULL DEFAULT 0 COMMENT '是否特殊账号超级管理员',
    `secret_data` json NOT NULL COMMENT '密码摘要',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_admin_user` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_app` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `name` varchar(256) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '应用名称',
    `desc` varchar(256) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '应用描述',
    `logo` varchar(256) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '应用图标',
    `sort` int NOT NULL COMMENT '应用排序',
    `link` varchar(256) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '应用链接',
    `row_version` int NOT NULL DEFAULT 0 COMMENT '并发乐观锁',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_app` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_banner` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `banner_file_id` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT 'Banner 文件 Id',
    `link_address` varchar(512) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '链接地址',
    `sort` int NOT NULL COMMENT '排序',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_banner` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_global_config` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `fund_scale` bigint NOT NULL COMMENT '基金规模',
    `issue_points_scale` bigint NOT NULL COMMENT '发行积分规模',
    `foundation_public_document` json NOT NULL COMMENT '基金会公开信息文件',
    `proposal_approval_votes` int NOT NULL COMMENT '提案通过票数',
    `row_version` int NOT NULL DEFAULT 0 COMMENT '行版本，处理并发问题',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_global_config` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_information` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `name` varchar(128) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '公告名称',
    `attach_id` varchar(128) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '附件 Id',
    `sort` int NOT NULL DEFAULT 0 COMMENT '公告排序',
    `row_version` int NOT NULL DEFAULT 0 COMMENT '并发乐观锁',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_information` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_medal` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `attach_id` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '附件 Id',
    `name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '勋章名称',
    `file_id` varchar(200) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '获得用户文件 Id',
    `quantity` bigint NOT NULL COMMENT '发放用户数量',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_medal` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_node` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `user_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '节点用户 Id',
    `user_did` varchar(128) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '节点用户 Did',
    `logo` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '节点 Logo',
    `name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '节点名称',
    `description` longtext CHARACTER SET utf8mb4 NOT NULL COMMENT '节点描述',
    `sort` int NOT NULL COMMENT '排序',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_node` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_point_distribute_record` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `user_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '用户id',
    `nick_name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '昵称',
    `phone` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '手机号',
    `phone_region` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户手机区号',
    `email` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '邮箱',
    `score` bigint NOT NULL COMMENT '发放积分',
    `get_time` datetime NOT NULL DEFAULT current_timestamp COMMENT '获取时间',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_point_distribute_record` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_point_record` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `user_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '所属用户',
    `participator_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '积分记录的另一个参与用户 Id',
    `participator_domain_name` varchar(256) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '参与方域名',
    `participator_nick_name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '参与方昵称',
    `type` int NOT NULL DEFAULT 0 COMMENT '积分来源类型',
    `reason` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '获得原因',
    `remark` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '附言',
    `score` bigint NOT NULL COMMENT '积分数量',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_point_record` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_proposal` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '提案名称',
    `initiator_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '发起方名称',
    `initiator_did` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '发起方 Did',
    `initiator_domain_name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '发起方 DomainName',
    `initiator_name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '发起方名称',
    `initiator_email` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '发起方邮箱',
    `initiator_avatar` varchar(512) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '发起方头像',
    `end_at` datetime NOT NULL DEFAULT '0001-01-01 00:00:00' COMMENT '投票截至时间',
    `attach_id` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '附件 Id',
    `total_votes` bigint NOT NULL COMMENT '总投票数',
    `oppose_votes` bigint NOT NULL COMMENT '反对票数',
    `agree_votes` bigint NOT NULL COMMENT '同意票数',
    `status` int NOT NULL DEFAULT 0 COMMENT '提案状态',
    `on_shelf` tinyint NOT NULL DEFAULT 0 COMMENT '是否上架',
    `row_version` int NOT NULL DEFAULT 0 COMMENT '行版本，处理并发问题',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_proposal` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_user` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `email` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '邮箱',
    `phone` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '手机号',
    `phone_region` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '手机区号',
    `avatar` varchar(500) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户头像',
    `nick_name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户昵称',
    `introduction` varchar(512) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '简介',
    `domain_name` varchar(256) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '完整用户名，域名',
    `did` varchar(128) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT 'DID',
    `score` bigint NOT NULL COMMENT '积分',
    `node_user` tinyint NOT NULL DEFAULT 0 COMMENT '是否是节点用户',
    `disable` tinyint NOT NULL DEFAULT 0 COMMENT '是否禁用',
    `row_version` int NOT NULL DEFAULT 0 COMMENT '行版本，处理并发问题',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_user` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_user_medal` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `user_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '用户 Id',
    `medal_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '勋章 Id',
    `nick_name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户昵称',
    `avatar` varchar(500) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户头像',
    `phone` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户手机号',
    `phone_region` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '用户手机区号',
    `email` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '邮箱',
    `attach_id` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '封面 Id',
    `name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '勋章名称',
    `get_time` datetime NOT NULL DEFAULT '0001-01-01 00:00:00' COMMENT '获得时间',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_user_medal` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `t_vote_record` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `proposal_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '提案 Id',
    `user_id` char(36) COLLATE ascii_general_ci NOT NULL COMMENT '投票用户 Id',
    `choice` int NOT NULL DEFAULT 0 COMMENT '投票选项',
    `created_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '创建时间',
    `created_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '创建者',
    `updated_at` datetime NOT NULL DEFAULT current_timestamp COMMENT '更新时间',
    `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' COMMENT '更新者',
    `deleted` tinyint NOT NULL DEFAULT 0 COMMENT '是否删除',
    CONSTRAINT `PK_t_vote_record` PRIMARY KEY (`id`),
    CONSTRAINT `FK_t_vote_record_t_proposal_proposal_id` FOREIGN KEY (`proposal_id`) REFERENCES `t_proposal` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_ExpiresAt_StatusName` ON `CAPPublishedMessage` (`ExpiresAt`, `StatusName`);

CREATE INDEX `IX_Version_ExpiresAt_StatusName` ON `CAPPublishedMessage` (`Version`, `ExpiresAt`, `StatusName`);

CREATE INDEX `IX_ExpiresAt_StatusName1` ON `CAPReceivedMessage` (`ExpiresAt`, `StatusName`);

CREATE INDEX `IX_Version_ExpiresAt_StatusName1` ON `CAPReceivedMessage` (`Version`, `ExpiresAt`, `StatusName`);

CREATE INDEX `IX_t_vote_record_proposal_id` ON `t_vote_record` (`proposal_id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260413074810_InitialCreate1', '9.0.0');

COMMIT;

