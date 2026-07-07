USE [ACBS_VSDC_TESTHUB];
GO

/*
ACBS VSDC Test Hub v1.2.0
Purpose: add user authentication / authorization tables.
Safe mode: CREATE IF NOT EXISTS. Does not drop existing data.
*/

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'vsdc')
    EXEC(N'CREATE SCHEMA vsdc');
GO

IF OBJECT_ID(N'[vsdc].[UserAccounts]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[UserAccounts]
    (
        [Id] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserAccounts] PRIMARY KEY,
        [CreatedAtUtc] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserAccounts_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] DATETIME2(7) NULL,
        [UserName] NVARCHAR(80) NOT NULL,
        [DisplayName] NVARCHAR(160) NOT NULL,
        [Email] NVARCHAR(200) NULL,
        [PasswordHash] NVARCHAR(500) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_UserAccounts_IsActive] DEFAULT 1,
        [MustChangePassword] BIT NOT NULL CONSTRAINT [DF_UserAccounts_MustChangePassword] DEFAULT 0,
        [LastLoginAtUtc] DATETIME2(7) NULL,
        [LastPasswordChangedAtUtc] DATETIME2(7) NULL,
        [LastLoginIp] NVARCHAR(80) NULL,
        [Note] NVARCHAR(500) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_UserAccounts_UserName' AND object_id = OBJECT_ID(N'[vsdc].[UserAccounts]'))
    CREATE UNIQUE INDEX [UX_UserAccounts_UserName] ON [vsdc].[UserAccounts]([UserName]);
GO

IF OBJECT_ID(N'[vsdc].[UserRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[UserRoles]
    (
        [Id] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserRoles] PRIMARY KEY,
        [CreatedAtUtc] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserRoles_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] DATETIME2(7) NULL,
        [RoleCode] NVARCHAR(40) NOT NULL,
        [RoleName] NVARCHAR(120) NOT NULL,
        [Level] INT NOT NULL CONSTRAINT [DF_UserRoles_Level] DEFAULT 1,
        [Description] NVARCHAR(500) NULL,
        [IsSystem] BIT NOT NULL CONSTRAINT [DF_UserRoles_IsSystem] DEFAULT 1
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_UserRoles_RoleCode' AND object_id = OBJECT_ID(N'[vsdc].[UserRoles]'))
    CREATE UNIQUE INDEX [UX_UserRoles_RoleCode] ON [vsdc].[UserRoles]([RoleCode]);
GO

IF OBJECT_ID(N'[vsdc].[UserRoleAssignments]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[UserRoleAssignments]
    (
        [Id] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserRoleAssignments] PRIMARY KEY,
        [CreatedAtUtc] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserRoleAssignments_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] DATETIME2(7) NULL,
        [UserAccountId] BIGINT NOT NULL,
        [UserRoleId] BIGINT NOT NULL,
        [GrantedBy] NVARCHAR(80) NULL,
        [GrantedAtUtc] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserRoleAssignments_GrantedAtUtc] DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [FK_UserRoleAssignments_UserAccounts] FOREIGN KEY ([UserAccountId]) REFERENCES [vsdc].[UserAccounts]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoleAssignments_UserRoles] FOREIGN KEY ([UserRoleId]) REFERENCES [vsdc].[UserRoles]([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_UserRoleAssignments_User_Role' AND object_id = OBJECT_ID(N'[vsdc].[UserRoleAssignments]'))
    CREATE UNIQUE INDEX [UX_UserRoleAssignments_User_Role] ON [vsdc].[UserRoleAssignments]([UserAccountId], [UserRoleId]);
GO

IF OBJECT_ID(N'[vsdc].[UserAuditLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[UserAuditLogs]
    (
        [Id] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserAuditLogs] PRIMARY KEY,
        [CreatedAtUtc] DATETIME2(7) NOT NULL CONSTRAINT [DF_UserAuditLogs_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] DATETIME2(7) NULL,
        [UserName] NVARCHAR(80) NOT NULL,
        [EventCode] NVARCHAR(80) NOT NULL,
        [IpAddress] NVARCHAR(80) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [Detail] NVARCHAR(MAX) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserAuditLogs_CreatedAtUtc' AND object_id = OBJECT_ID(N'[vsdc].[UserAuditLogs]'))
    CREATE INDEX [IX_UserAuditLogs_CreatedAtUtc] ON [vsdc].[UserAuditLogs]([CreatedAtUtc]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserAuditLogs_User_Event' AND object_id = OBJECT_ID(N'[vsdc].[UserAuditLogs]'))
    CREATE INDEX [IX_UserAuditLogs_User_Event] ON [vsdc].[UserAuditLogs]([UserName], [EventCode]);
GO

SELECT s.name AS SchemaName, COUNT(*) AS TableCount
FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id
GROUP BY s.name
ORDER BY s.name;
GO
