/*
 ACBS VSDC TESTHUB - FULL SQL SERVER SCHEMA
 Generated from current Acbs.Vsdc.TestHub domain model.
 Purpose: create schema vsdc and all 56 tables for MSP TestHub when EF EnsureCreated does not create everything.
 Safe mode: CREATE IF NOT EXISTS. Does not drop existing data.
*/
SET NOCOUNT ON;
GO

IF DB_ID(N'ACBS_VSDC_TESTHUB') IS NULL
BEGIN
    CREATE DATABASE [ACBS_VSDC_TESTHUB];
END
GO

USE [ACBS_VSDC_TESTHUB];
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'vsdc')
BEGIN
    EXEC(N'CREATE SCHEMA vsdc');
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[GatewayFiles]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GatewayFiles] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_GatewayFiles_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [OriginalFileName] nvarchar(260) NOT NULL CONSTRAINT [DF_GatewayFiles_OriginalFileName] DEFAULT N'',
        [SourcePath] nvarchar(1000) NOT NULL CONSTRAINT [DF_GatewayFiles_SourcePath] DEFAULT N'',
        [Extension] nvarchar(16) NOT NULL CONSTRAINT [DF_GatewayFiles_Extension] DEFAULT N'',
        [Sha256] nvarchar(64) NOT NULL CONSTRAINT [DF_GatewayFiles_Sha256] DEFAULT N'',
        [SizeBytes] bigint NOT NULL CONSTRAINT [DF_GatewayFiles_SizeBytes] DEFAULT 0,
        [Direction] int NOT NULL CONSTRAINT [DF_GatewayFiles_Direction] DEFAULT 0,
        [FolderKind] int NOT NULL CONSTRAINT [DF_GatewayFiles_FolderKind] DEFAULT 0,
        [Status] int NOT NULL CONSTRAINT [DF_GatewayFiles_Status] DEFAULT 0,
        [FileCreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_GatewayFiles_FileCreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [FileModifiedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_GatewayFiles_FileModifiedAtUtc] DEFAULT SYSUTCDATETIME(),
        [DiscoveredAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_GatewayFiles_DiscoveredAtUtc] DEFAULT SYSUTCDATETIME(),
        [ProcessedAtUtc] datetime2(7) NULL,
        [RawText] nvarchar(max) NULL,
        [ErrorMessage] nvarchar(2000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[GatewayMessages]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GatewayMessages] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_GatewayMessages_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayFileId] bigint NOT NULL CONSTRAINT [DF_GatewayMessages_GatewayFileId] DEFAULT 0,
        [Direction] int NOT NULL CONSTRAINT [DF_GatewayMessages_Direction] DEFAULT 0,
        [Standard] int NOT NULL CONSTRAINT [DF_GatewayMessages_Standard] DEFAULT 0,
        [MessageType] nvarchar(50) NULL,
        [OperationCode] nvarchar(50) NULL,
        [OperationName] nvarchar(250) NULL,
        [Reference] nvarchar(100) NULL,
        [RelatedReference] nvarchar(100) NULL,
        [AccountNo] nvarchar(100) NULL,
        [SecurityCode] nvarchar(100) NULL,
        [ProcessingStatus] nvarchar(50) NULL,
        [PreparationDate] datetime2(7) NULL,
        [RawContent] nvarchar(max) NOT NULL CONSTRAINT [DF_GatewayMessages_RawContent] DEFAULT N''
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MessageHeaders]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MessageHeaders]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MessageHeaders] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MessageHeaders_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MessageHeaders_GatewayMessageId] DEFAULT 0,
        [HeaderType] nvarchar(20) NOT NULL CONSTRAINT [DF_MessageHeaders_HeaderType] DEFAULT N'',
        [HeaderValue] nvarchar(max) NOT NULL CONSTRAINT [DF_MessageHeaders_HeaderValue] DEFAULT N''
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MessageBlocks]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MessageBlocks]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MessageBlocks] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MessageBlocks_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MessageBlocks_GatewayMessageId] DEFAULT 0,
        [BlockCode] nvarchar(20) NOT NULL CONSTRAINT [DF_MessageBlocks_BlockCode] DEFAULT N'',
        [SequenceNo] int NOT NULL CONSTRAINT [DF_MessageBlocks_SequenceNo] DEFAULT 0,
        [BlockValue] nvarchar(max) NOT NULL CONSTRAINT [DF_MessageBlocks_BlockValue] DEFAULT N''
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MessageTags]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MessageTags]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MessageTags] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MessageTags_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MessageTags_GatewayMessageId] DEFAULT 0,
        [TagCode] nvarchar(20) NOT NULL CONSTRAINT [DF_MessageTags_TagCode] DEFAULT N'',
        [Qualifier] nvarchar(50) NULL,
        [SequenceNo] int NOT NULL CONSTRAINT [DF_MessageTags_SequenceNo] DEFAULT 0,
        [TagValue] nvarchar(max) NOT NULL CONSTRAINT [DF_MessageTags_TagValue] DEFAULT N'',
        [DecodedValue] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MessageReferences]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MessageReferences]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MessageReferences] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MessageReferences_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MessageReferences_GatewayMessageId] DEFAULT 0,
        [ReferenceType] nvarchar(30) NOT NULL CONSTRAINT [DF_MessageReferences_ReferenceType] DEFAULT N'',
        [ReferenceValue] nvarchar(200) NOT NULL CONSTRAINT [DF_MessageReferences_ReferenceValue] DEFAULT N''
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MessageStatusHistories]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MessageStatusHistories]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MessageStatusHistories] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MessageStatusHistories_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MessageStatusHistories_GatewayMessageId] DEFAULT 0,
        [PreviousStatus] nvarchar(30) NULL,
        [CurrentStatus] nvarchar(30) NOT NULL CONSTRAINT [DF_MessageStatusHistories_CurrentStatus] DEFAULT N'',
        [Reason] nvarchar(500) NULL,
        [ChangedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MessageStatusHistories_ChangedAtUtc] DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID(N'[vsdc].[Customers]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[Customers]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Customers] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_Customers_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NULL,
        [CustomerCode] nvarchar(100) NULL,
        [FullName] nvarchar(300) NULL,
        [CustomerType] nvarchar(30) NULL,
        [ResidencyStatus] nvarchar(30) NULL,
        [Nationality] nvarchar(30) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[CustomerIdentities]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[CustomerIdentities]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomerIdentities] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_CustomerIdentities_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [CustomerId] bigint NOT NULL CONSTRAINT [DF_CustomerIdentities_CustomerId] DEFAULT 0,
        [IdentityType] nvarchar(30) NULL,
        [IdentityNo] nvarchar(100) NULL,
        [IssueDate] datetime2(7) NULL,
        [IssuePlace] nvarchar(200) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[CustomerAddresses]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[CustomerAddresses]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomerAddresses] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_CustomerAddresses_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [CustomerId] bigint NOT NULL CONSTRAINT [DF_CustomerAddresses_CustomerId] DEFAULT 0,
        [AddressType] nvarchar(30) NOT NULL CONSTRAINT [DF_CustomerAddresses_AddressType] DEFAULT N'',
        [AddressLine] nvarchar(500) NULL,
        [Province] nvarchar(100) NULL,
        [Country] nvarchar(100) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[CustomerContacts]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[CustomerContacts]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomerContacts] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_CustomerContacts_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [CustomerId] bigint NOT NULL CONSTRAINT [DF_CustomerContacts_CustomerId] DEFAULT 0,
        [ContactType] nvarchar(30) NOT NULL CONSTRAINT [DF_CustomerContacts_ContactType] DEFAULT N'',
        [ContactValue] nvarchar(300) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[ReportFiles]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[ReportFiles]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ReportFiles] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_ReportFiles_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayFileId] bigint NOT NULL CONSTRAINT [DF_ReportFiles_GatewayFileId] DEFAULT 0,
        [ReportCode] nvarchar(30) NULL,
        [PairKey] nvarchar(50) NULL,
        [Delimiter] nvarchar(20) NULL,
        [RowCount] int NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[ReportRows]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[ReportRows]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ReportRows] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_ReportRows_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [ReportFileId] bigint NOT NULL CONSTRAINT [DF_ReportRows_ReportFileId] DEFAULT 0,
        [RowNo] int NOT NULL CONSTRAINT [DF_ReportRows_RowNo] DEFAULT 0,
        [RawRow] nvarchar(max) NOT NULL CONSTRAINT [DF_ReportRows_RawRow] DEFAULT N'',
        [JsonData] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspOperationDefinitions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspOperationDefinitions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspOperationDefinitions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspOperationDefinitions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [Code] nvarchar(80) NOT NULL CONSTRAINT [DF_MspOperationDefinitions_Code] DEFAULT N'',
        [Name] nvarchar(250) NOT NULL CONSTRAINT [DF_MspOperationDefinitions_Name] DEFAULT N'',
        [MessageType] nvarchar(3) NOT NULL CONSTRAINT [DF_MspOperationDefinitions_MessageType] DEFAULT N'',
        [Direction] nvarchar(30) NOT NULL CONSTRAINT [DF_MspOperationDefinitions_Direction] DEFAULT N'',
        [IsEnabled] bit NOT NULL CONSTRAINT [DF_MspOperationDefinitions_IsEnabled] DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspFieldDefinitions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspFieldDefinitions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspFieldDefinitions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspFieldDefinitions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [MspOperationDefinitionId] bigint NOT NULL CONSTRAINT [DF_MspFieldDefinitions_MspOperationDefinitionId] DEFAULT 0,
        [TagCode] nvarchar(20) NOT NULL CONSTRAINT [DF_MspFieldDefinitions_TagCode] DEFAULT N'',
        [Qualifier] nvarchar(20) NULL,
        [FieldName] nvarchar(250) NOT NULL CONSTRAINT [DF_MspFieldDefinitions_FieldName] DEFAULT N'',
        [Requirement] nvarchar(20) NOT NULL CONSTRAINT [DF_MspFieldDefinitions_Requirement] DEFAULT N'',
        [Format] nvarchar(100) NULL,
        [SequenceNo] int NOT NULL CONSTRAINT [DF_MspFieldDefinitions_SequenceNo] DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'[vsdc].[Accounts]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[Accounts]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Accounts] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_Accounts_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NULL,
        [AccountNo] nvarchar(100) NOT NULL CONSTRAINT [DF_Accounts_AccountNo] DEFAULT N'',
        [AccountType] nvarchar(50) NULL,
        [DepositoryMemberCode] nvarchar(50) NULL,
        [Status] nvarchar(30) NULL,
        [OpenDate] datetime2(7) NULL,
        [CloseDate] datetime2(7) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[AccountMappings]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[AccountMappings]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AccountMappings] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_AccountMappings_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_AccountMappings_GatewayMessageId] DEFAULT 0,
        [SourceAccount] nvarchar(100) NULL,
        [TargetAccount] nvarchar(100) NULL,
        [MappingType] nvarchar(50) NULL,
        [Status] nvarchar(30) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[AccountChanges]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[AccountChanges]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AccountChanges] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_AccountChanges_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_AccountChanges_GatewayMessageId] DEFAULT 0,
        [AccountNo] nvarchar(100) NULL,
        [FieldName] nvarchar(100) NULL,
        [OldValue] nvarchar(max) NULL,
        [NewValue] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[Securities]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[Securities]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Securities] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_Securities_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NULL,
        [Symbol] nvarchar(50) NULL,
        [Isin] nvarchar(50) NULL,
        [SecurityName] nvarchar(300) NULL,
        [SecurityType] nvarchar(50) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[Orders]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[Orders]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Orders] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_Orders_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_Orders_GatewayMessageId] DEFAULT 0,
        [OrderId] nvarchar(100) NULL,
        [Side] nvarchar(20) NULL,
        [Quantity] decimal(28,8) NULL,
        [Price] decimal(28,8) NULL,
        [Amount] decimal(28,8) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[Trades]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[Trades]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Trades] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_Trades_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_Trades_GatewayMessageId] DEFAULT 0,
        [TradeId] nvarchar(100) NULL,
        [TradeDate] datetime2(7) NULL,
        [Quantity] decimal(28,8) NULL,
        [Price] decimal(28,8) NULL,
        [Amount] decimal(28,8) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[CashTransactions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[CashTransactions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CashTransactions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_CashTransactions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_CashTransactions_GatewayMessageId] DEFAULT 0,
        [AccountNo] nvarchar(100) NULL,
        [Currency] nvarchar(10) NULL,
        [TransactionType] nvarchar(30) NULL,
        [Amount] decimal(28,8) NULL,
        [ValueDate] datetime2(7) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[SecuritiesTransfers]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[SecuritiesTransfers]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SecuritiesTransfers] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_SecuritiesTransfers_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_SecuritiesTransfers_GatewayMessageId] DEFAULT 0,
        [SourceAccount] nvarchar(100) NULL,
        [TargetAccount] nvarchar(100) NULL,
        [SecurityCode] nvarchar(50) NULL,
        [Quantity] decimal(28,8) NULL,
        [TransferType] nvarchar(30) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[RightsRegistrations]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[RightsRegistrations]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_RightsRegistrations] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_RightsRegistrations_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_RightsRegistrations_GatewayMessageId] DEFAULT 0,
        [AccountNo] nvarchar(100) NULL,
        [RightCode] nvarchar(50) NULL,
        [SecurityCode] nvarchar(50) NULL,
        [Quantity] decimal(28,8) NULL,
        [Status] nvarchar(30) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[CorporateActions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[CorporateActions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CorporateActions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_CorporateActions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_CorporateActions_GatewayMessageId] DEFAULT 0,
        [EventId] nvarchar(100) NULL,
        [EventType] nvarchar(50) NULL,
        [SecurityCode] nvarchar(50) NULL,
        [RecordDate] datetime2(7) NULL,
        [PaymentDate] datetime2(7) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[Fees]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[Fees]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Fees] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_Fees_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_Fees_GatewayMessageId] DEFAULT 0,
        [FeeType] nvarchar(50) NULL,
        [Currency] nvarchar(10) NULL,
        [Amount] decimal(28,8) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[Taxes]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[Taxes]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Taxes] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_Taxes_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_Taxes_GatewayMessageId] DEFAULT 0,
        [TaxType] nvarchar(50) NULL,
        [Currency] nvarchar(10) NULL,
        [Amount] decimal(28,8) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[NavRecords]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[NavRecords]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_NavRecords] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_NavRecords_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_NavRecords_GatewayMessageId] DEFAULT 0,
        [FundCode] nvarchar(50) NULL,
        [TradingDate] datetime2(7) NULL,
        [Nav] decimal(28,8) NULL,
        [TotalNav] decimal(28,8) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[ProcessingHistories]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[ProcessingHistories]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProcessingHistories] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_ProcessingHistories_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayFileId] bigint NOT NULL CONSTRAINT [DF_ProcessingHistories_GatewayFileId] DEFAULT 0,
        [Stage] nvarchar(50) NOT NULL CONSTRAINT [DF_ProcessingHistories_Stage] DEFAULT N'',
        [Status] nvarchar(30) NOT NULL CONSTRAINT [DF_ProcessingHistories_Status] DEFAULT N'',
        [Detail] nvarchar(max) NULL,
        [OccurredAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_ProcessingHistories_OccurredAtUtc] DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID(N'[vsdc].[ValidationErrors]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[ValidationErrors]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ValidationErrors] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_ValidationErrors_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayFileId] bigint NOT NULL CONSTRAINT [DF_ValidationErrors_GatewayFileId] DEFAULT 0,
        [Severity] nvarchar(30) NOT NULL CONSTRAINT [DF_ValidationErrors_Severity] DEFAULT N'',
        [FieldOrTag] nvarchar(100) NULL,
        [ErrorCode] nvarchar(100) NULL,
        [ErrorMessage] nvarchar(2000) NOT NULL CONSTRAINT [DF_ValidationErrors_ErrorMessage] DEFAULT N''
    );
END
GO

IF OBJECT_ID(N'[vsdc].[OutboxJobs]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[OutboxJobs]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OutboxJobs] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_OutboxJobs_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NULL,
        [Reference] nvarchar(100) NULL,
        [TargetPath] nvarchar(1000) NULL,
        [Status] nvarchar(30) NOT NULL CONSTRAINT [DF_OutboxJobs_Status] DEFAULT N'',
        [AttemptCount] int NOT NULL CONSTRAINT [DF_OutboxJobs_AttemptCount] DEFAULT 0,
        [SentAtUtc] datetime2(7) NULL,
        [ErrorMessage] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[InboxJobs]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[InboxJobs]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InboxJobs] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_InboxJobs_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayFileId] bigint NOT NULL CONSTRAINT [DF_InboxJobs_GatewayFileId] DEFAULT 0,
        [Status] nvarchar(30) NOT NULL CONSTRAINT [DF_InboxJobs_Status] DEFAULT N'',
        [AttemptCount] int NOT NULL CONSTRAINT [DF_InboxJobs_AttemptCount] DEFAULT 0,
        [CompletedAtUtc] datetime2(7) NULL,
        [ErrorMessage] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[SystemLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[SystemLogs]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SystemLogs] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_SystemLogs_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [LoggedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_SystemLogs_LoggedAtUtc] DEFAULT SYSUTCDATETIME(),
        [Level] nvarchar(20) NOT NULL CONSTRAINT [DF_SystemLogs_Level] DEFAULT N'',
        [Category] nvarchar(100) NOT NULL CONSTRAINT [DF_SystemLogs_Category] DEFAULT N'',
        [EventCode] nvarchar(100) NULL,
        [Message] nvarchar(max) NOT NULL CONSTRAINT [DF_SystemLogs_Message] DEFAULT N'',
        [Exception] nvarchar(max) NULL,
        [CorrelationId] nvarchar(100) NULL,
        [UserName] nvarchar(100) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[SimulatorRules]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[SimulatorRules]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SimulatorRules] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_SimulatorRules_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [OperationCode] nvarchar(100) NOT NULL CONSTRAINT [DF_SimulatorRules_OperationCode] DEFAULT N'',
        [IncomingMessageType] nvarchar(100) NULL,
        [ResponseMessageType] nvarchar(100) NULL,
        [Result] nvarchar(30) NOT NULL CONSTRAINT [DF_SimulatorRules_Result] DEFAULT N'',
        [DelayMilliseconds] int NOT NULL CONSTRAINT [DF_SimulatorRules_DelayMilliseconds] DEFAULT 0,
        [IsEnabled] bit NOT NULL CONSTRAINT [DF_SimulatorRules_IsEnabled] DEFAULT 0,
        [ResponseTemplate] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[SimulatorRuns]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[SimulatorRuns]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SimulatorRuns] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_SimulatorRuns_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [SourceFileName] nvarchar(100) NULL,
        [ResponseFileName] nvarchar(100) NULL,
        [Reference] nvarchar(100) NULL,
        [Status] int NOT NULL CONSTRAINT [DF_SimulatorRuns_Status] DEFAULT 0,
        [StartedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_SimulatorRuns_StartedAtUtc] DEFAULT SYSUTCDATETIME(),
        [CompletedAtUtc] datetime2(7) NULL,
        [ErrorMessage] nvarchar(max) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[ManualTemplates]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[ManualTemplates]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ManualTemplates] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_ManualTemplates_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [Code] nvarchar(100) NOT NULL CONSTRAINT [DF_ManualTemplates_Code] DEFAULT N'',
        [Name] nvarchar(250) NOT NULL CONSTRAINT [DF_ManualTemplates_Name] DEFAULT N'',
        [FileType] nvarchar(20) NOT NULL CONSTRAINT [DF_ManualTemplates_FileType] DEFAULT N'',
        [Module] nvarchar(100) NOT NULL CONSTRAINT [DF_ManualTemplates_Module] DEFAULT N'',
        [TemplateBody] nvarchar(max) NOT NULL CONSTRAINT [DF_ManualTemplates_TemplateBody] DEFAULT N'',
        [IsEnabled] bit NOT NULL CONSTRAINT [DF_ManualTemplates_IsEnabled] DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'[vsdc].[SystemSettings]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[SystemSettings]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SystemSettings] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_SystemSettings_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [SettingKey] nvarchar(200) NOT NULL CONSTRAINT [DF_SystemSettings_SettingKey] DEFAULT N'',
        [SettingValue] nvarchar(max) NULL,
        [GroupName] nvarchar(100) NULL,
        [IsSecret] bit NOT NULL CONSTRAINT [DF_SystemSettings_IsSecret] DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'[vsdc].[FileCheckpoints]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[FileCheckpoints]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FileCheckpoints] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_FileCheckpoints_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [FolderPath] nvarchar(1000) NOT NULL CONSTRAINT [DF_FileCheckpoints_FolderPath] DEFAULT N'',
        [LastFileName] nvarchar(260) NULL,
        [LastScanAtUtc] datetime2(7) NULL,
        [LastFileModifiedAtUtc] datetime2(7) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspBusinessMessages]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspBusinessMessages]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspBusinessMessages] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspBusinessMessages_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspBusinessMessages_GatewayMessageId] DEFAULT 0,
        [MessageType] nvarchar(3) NOT NULL CONSTRAINT [DF_MspBusinessMessages_MessageType] DEFAULT N'',
        [OperationCode] nvarchar(80) NOT NULL CONSTRAINT [DF_MspBusinessMessages_OperationCode] DEFAULT N'',
        [FunctionCode] nvarchar(30) NULL,
        [SenderBic] nvarchar(20) NULL,
        [ReceiverBic] nvarchar(20) NULL,
        [SenderReference] nvarchar(100) NULL,
        [RelatedReference] nvarchar(100) NULL,
        [BusinessStatus] nvarchar(20) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspAckNaks]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspAckNaks]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspAckNaks] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspAckNaks_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspAckNaks_GatewayMessageId] DEFAULT 0,
        [AckAt] datetime2(7) NULL,
        [IsAccepted] bit NOT NULL CONSTRAINT [DF_MspAckNaks_IsAccepted] DEFAULT 0,
        [RejectionCode] nvarchar(20) NULL,
        [RejectionReason] nvarchar(1000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspNarrativeItems]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspNarrativeItems]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspNarrativeItems] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspNarrativeItems_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspNarrativeItems_GatewayMessageId] DEFAULT 0,
        [SequenceNo] int NOT NULL CONSTRAINT [DF_MspNarrativeItems_SequenceNo] DEFAULT 0,
        [Key] nvarchar(80) NOT NULL CONSTRAINT [DF_MspNarrativeItems_Key] DEFAULT N'',
        [Value] nvarchar(2000) NULL,
        [DecodedValue] nvarchar(2000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspSecuritiesPositionInstructions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspSecuritiesPositionInstructions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspSecuritiesPositionInstructions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspSecuritiesPositionInstructions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspSecuritiesPositionInstructions_GatewayMessageId] DEFAULT 0,
        [InstructionKind] nvarchar(20) NOT NULL CONSTRAINT [DF_MspSecuritiesPositionInstructions_InstructionKind] DEFAULT N'',
        [SafeAccount] nvarchar(100) NOT NULL CONSTRAINT [DF_MspSecuritiesPositionInstructions_SafeAccount] DEFAULT N'',
        [Isin] nvarchar(20) NULL,
        [Quantity] decimal(28,8) NULL,
        [EffectiveDate] datetime2(7) NULL,
        [FromBalance] nvarchar(20) NULL,
        [ToBalance] nvarchar(20) NULL,
        [PreviousReference] nvarchar(100) NULL,
        [Narrative] nvarchar(1000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspSecuritiesPositionResponses]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspSecuritiesPositionResponses]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspSecuritiesPositionResponses] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspSecuritiesPositionResponses_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspSecuritiesPositionResponses_GatewayMessageId] DEFAULT 0,
        [RelatedReference] nvarchar(100) NULL,
        [StatusCode] nvarchar(10) NOT NULL CONSTRAINT [DF_MspSecuritiesPositionResponses_StatusCode] DEFAULT N'',
        [ReasonCode] nvarchar(20) NULL,
        [ReasonText] nvarchar(1000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspCashInstructions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspCashInstructions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspCashInstructions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspCashInstructions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspCashInstructions_GatewayMessageId] DEFAULT 0,
        [InstructionKind] nvarchar(20) NOT NULL CONSTRAINT [DF_MspCashInstructions_InstructionKind] DEFAULT N'',
        [AccountNo] nvarchar(100) NOT NULL CONSTRAINT [DF_MspCashInstructions_AccountNo] DEFAULT N'',
        [Amount] decimal(28,8) NULL,
        [Currency] nvarchar(3) NOT NULL CONSTRAINT [DF_MspCashInstructions_Currency] DEFAULT N'',
        [Reason] nvarchar(1000) NULL,
        [PreviousReference] nvarchar(100) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspCashResponses]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspCashResponses]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspCashResponses] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspCashResponses_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspCashResponses_GatewayMessageId] DEFAULT 0,
        [RelatedReference] nvarchar(100) NULL,
        [StatusCode] nvarchar(10) NOT NULL CONSTRAINT [DF_MspCashResponses_StatusCode] DEFAULT N'',
        [ReasonText] nvarchar(1000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspSettlementInstructions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspSettlementInstructions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspSettlementInstructions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspSettlementInstructions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspSettlementInstructions_GatewayMessageId] DEFAULT 0,
        [Side] nvarchar(10) NOT NULL CONSTRAINT [DF_MspSettlementInstructions_Side] DEFAULT N'',
        [SafeAccount] nvarchar(100) NOT NULL CONSTRAINT [DF_MspSettlementInstructions_SafeAccount] DEFAULT N'',
        [Isin] nvarchar(20) NULL,
        [TradeDate] datetime2(7) NULL,
        [Price] decimal(28,8) NULL,
        [Quantity] decimal(28,8) NULL,
        [SettlementAmount] decimal(28,8) NULL,
        [FeeAmount] decimal(28,8) NULL,
        [TaxAmount] decimal(28,8) NULL,
        [AgentQualifier] nvarchar(20) NULL,
        [AgentBic] nvarchar(20) NULL,
        [Narrative] nvarchar(1000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspStatusInquiries]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspStatusInquiries]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspStatusInquiries] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspStatusInquiries_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspStatusInquiries_GatewayMessageId] DEFAULT 0,
        [OriginalMessageType] nvarchar(3) NOT NULL CONSTRAINT [DF_MspStatusInquiries_OriginalMessageType] DEFAULT N'',
        [OriginalReference] nvarchar(100) NOT NULL CONSTRAINT [DF_MspStatusInquiries_OriginalReference] DEFAULT N'',
        [AccountNo] nvarchar(100) NULL,
        [Isin] nvarchar(20) NULL,
        [TradeDate] datetime2(7) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspStatusResponses]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspStatusResponses]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspStatusResponses] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspStatusResponses_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspStatusResponses_GatewayMessageId] DEFAULT 0,
        [OriginalReference] nvarchar(100) NOT NULL CONSTRAINT [DF_MspStatusResponses_OriginalReference] DEFAULT N'',
        [StatusCode] nvarchar(10) NOT NULL CONSTRAINT [DF_MspStatusResponses_StatusCode] DEFAULT N'',
        [ReasonCode] nvarchar(20) NULL,
        [ReasonText] nvarchar(1000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspReconcileInquiries]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspReconcileInquiries]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspReconcileInquiries] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspReconcileInquiries_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspReconcileInquiries_GatewayMessageId] DEFAULT 0,
        [TradeDate] datetime2(7) NOT NULL CONSTRAINT [DF_MspReconcileInquiries_TradeDate] DEFAULT '0001-01-01T00:00:00'
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspReconcileResponses]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspReconcileResponses]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspReconcileResponses] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspReconcileResponses_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspReconcileResponses_GatewayMessageId] DEFAULT 0,
        [StatusCode] nvarchar(10) NOT NULL CONSTRAINT [DF_MspReconcileResponses_StatusCode] DEFAULT N'',
        [ReasonCode] nvarchar(20) NULL,
        [ReasonText] nvarchar(1000) NULL,
        [CsvLogicalName] nvarchar(260) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspParties]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspParties]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspParties] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspParties_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspParties_GatewayMessageId] DEFAULT 0,
        [Qualifier] nvarchar(20) NOT NULL CONSTRAINT [DF_MspParties_Qualifier] DEFAULT N'',
        [BicCode] nvarchar(20) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspAmounts]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspAmounts]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspAmounts] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspAmounts_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspAmounts_GatewayMessageId] DEFAULT 0,
        [Qualifier] nvarchar(20) NOT NULL CONSTRAINT [DF_MspAmounts_Qualifier] DEFAULT N'',
        [Currency] nvarchar(3) NOT NULL CONSTRAINT [DF_MspAmounts_Currency] DEFAULT N'',
        [Amount] decimal(28,8) NOT NULL CONSTRAINT [DF_MspAmounts_Amount] DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspBalanceMovements]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspBalanceMovements]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspBalanceMovements] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspBalanceMovements_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayMessageId] bigint NOT NULL CONSTRAINT [DF_MspBalanceMovements_GatewayMessageId] DEFAULT 0,
        [FromBalance] nvarchar(20) NULL,
        [ToBalance] nvarchar(20) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspReportStatisticRows]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspReportStatisticRows]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspReportStatisticRows] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspReportStatisticRows_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayFileId] bigint NOT NULL CONSTRAINT [DF_MspReportStatisticRows_GatewayFileId] DEFAULT 0,
        [SendTime] datetime2(7) NULL,
        [ReceiveTime] datetime2(7) NULL,
        [MessageType] nvarchar(100) NULL,
        [SenderBic] nvarchar(20) NULL,
        [ReceiverBic] nvarchar(20) NULL,
        [AckStatus] nvarchar(10) NULL,
        [MessageReference] nvarchar(100) NULL,
        [RelatedReference] nvarchar(100) NULL,
        [Summary] nvarchar(1000) NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspParMetadata]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspParMetadata]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspParMetadata] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspParMetadata_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [GatewayFileId] bigint NOT NULL CONSTRAINT [DF_MspParMetadata_GatewayFileId] DEFAULT 0,
        [SwiftTime] datetime2(7) NULL,
        [NonRep] bit NULL,
        [DeliveryTime] datetime2(7) NULL,
        [MessageId] nvarchar(100) NULL,
        [CreationTime] datetime2(7) NULL,
        [PdIndication] bit NULL,
        [Requestor] nvarchar(150) NULL,
        [Responder] nvarchar(150) NULL,
        [Service] nvarchar(50) NULL,
        [RequestType] nvarchar(50) NULL,
        [Priority] nvarchar(20) NULL,
        [RequestReference] nvarchar(100) NULL,
        [TransferReference] nvarchar(100) NULL,
        [TransferDescription] nvarchar(500) NULL,
        [TransferInfo] nvarchar(1000) NULL,
        [PossibleDuplicate] bit NULL,
        [OriginalTransferReference] nvarchar(100) NULL,
        [AckIndicator] bit NULL,
        [LogicalName] nvarchar(260) NULL,
        [FileDescription] nvarchar(500) NULL,
        [FileInfo] nvarchar(500) NULL,
        [Size] bigint NULL
    );
END
GO

IF OBJECT_ID(N'[vsdc].[MspTemplateVersions]', N'U') IS NULL
BEGIN
    CREATE TABLE [vsdc].[MspTemplateVersions]
    (
        [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MspTemplateVersions] PRIMARY KEY,
        [CreatedAtUtc] datetime2(7) NOT NULL CONSTRAINT [DF_MspTemplateVersions_CreatedAtUtc] DEFAULT SYSUTCDATETIME(),
        [UpdatedAtUtc] datetime2(7) NULL,
        [OperationCode] nvarchar(80) NOT NULL CONSTRAINT [DF_MspTemplateVersions_OperationCode] DEFAULT N'',
        [Version] nvarchar(30) NOT NULL CONSTRAINT [DF_MspTemplateVersions_Version] DEFAULT N'',
        [EffectiveFrom] datetime2(7) NOT NULL CONSTRAINT [DF_MspTemplateVersions_EffectiveFrom] DEFAULT '0001-01-01T00:00:00',
        [TemplateBody] nvarchar(max) NOT NULL CONSTRAINT [DF_MspTemplateVersions_TemplateBody] DEFAULT N'',
        [IsActive] bit NOT NULL CONSTRAINT [DF_MspTemplateVersions_IsActive] DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[GatewayFiles]') AND name = N'IX_GatewayFiles_OriginalFileName_Sha256_Direction')
BEGIN
    CREATE INDEX [IX_GatewayFiles_OriginalFileName_Sha256_Direction] ON [vsdc].[GatewayFiles] ([OriginalFileName], [Sha256], [Direction]);
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[GatewayFiles]') AND name = N'IX_GatewayFiles_DiscoveredAtUtc')
BEGIN
    CREATE INDEX [IX_GatewayFiles_DiscoveredAtUtc] ON [vsdc].[GatewayFiles] ([DiscoveredAtUtc]);
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[GatewayFiles]') AND name = N'IX_GatewayFiles_Status')
BEGIN
    CREATE INDEX [IX_GatewayFiles_Status] ON [vsdc].[GatewayFiles] ([Status]);
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[GatewayMessages]') AND name = N'IX_GatewayMessages_GatewayFileId')
BEGIN
    CREATE UNIQUE INDEX [IX_GatewayMessages_GatewayFileId] ON [vsdc].[GatewayMessages] ([GatewayFileId]);
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[GatewayMessages]') AND name = N'IX_GatewayMessages_Reference')
BEGIN
    CREATE INDEX [IX_GatewayMessages_Reference] ON [vsdc].[GatewayMessages] ([Reference]);
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[GatewayMessages]') AND name = N'IX_GatewayMessages_AccountNo')
BEGIN
    CREATE INDEX [IX_GatewayMessages_AccountNo] ON [vsdc].[GatewayMessages] ([AccountNo]);
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[GatewayMessages]') AND name = N'IX_GatewayMessages_Direction_MessageType_CreatedAtUtc')
BEGIN
    CREATE INDEX [IX_GatewayMessages_Direction_MessageType_CreatedAtUtc] ON [vsdc].[GatewayMessages] ([Direction], [MessageType], [CreatedAtUtc]);
END
GO

IF OBJECT_ID(N'[vsdc].[MessageTags]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MessageTags]') AND name = N'IX_MessageTags_TagCode')
BEGIN
    -- TagValue is nvarchar(max), so SQL Server cannot use it as an index key column.
    -- Index compact columns for filtering; keep long tag content in the base table.
    CREATE INDEX [IX_MessageTags_TagCode] ON [vsdc].[MessageTags] ([TagCode], [Qualifier]) INCLUDE ([GatewayMessageId], [SequenceNo]);
END
GO

IF OBJECT_ID(N'[vsdc].[SystemLogs]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[SystemLogs]') AND name = N'IX_SystemLogs_LoggedAtUtc')
BEGIN
    CREATE INDEX [IX_SystemLogs_LoggedAtUtc] ON [vsdc].[SystemLogs] ([LoggedAtUtc]);
END
GO

IF OBJECT_ID(N'[vsdc].[SystemLogs]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[SystemLogs]') AND name = N'IX_SystemLogs_Level_Category')
BEGIN
    CREATE INDEX [IX_SystemLogs_Level_Category] ON [vsdc].[SystemLogs] ([Level], [Category]);
END
GO

IF OBJECT_ID(N'[vsdc].[SystemSettings]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[SystemSettings]') AND name = N'IX_SystemSettings_SettingKey')
BEGIN
    CREATE UNIQUE INDEX [IX_SystemSettings_SettingKey] ON [vsdc].[SystemSettings] ([SettingKey]);
END
GO

IF OBJECT_ID(N'[vsdc].[ManualTemplates]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[ManualTemplates]') AND name = N'IX_ManualTemplates_Code')
BEGIN
    CREATE UNIQUE INDEX [IX_ManualTemplates_Code] ON [vsdc].[ManualTemplates] ([Code]);
END
GO

IF OBJECT_ID(N'[vsdc].[SimulatorRules]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[SimulatorRules]') AND name = N'IX_SimulatorRules_OperationCode')
BEGIN
    CREATE INDEX [IX_SimulatorRules_OperationCode] ON [vsdc].[SimulatorRules] ([OperationCode]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspBusinessMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspBusinessMessages]') AND name = N'IX_MspBusinessMessages_MessageType_OperationCode')
BEGIN
    CREATE INDEX [IX_MspBusinessMessages_MessageType_OperationCode] ON [vsdc].[MspBusinessMessages] ([MessageType], [OperationCode]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspBusinessMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspBusinessMessages]') AND name = N'IX_MspBusinessMessages_SenderReference')
BEGIN
    CREATE INDEX [IX_MspBusinessMessages_SenderReference] ON [vsdc].[MspBusinessMessages] ([SenderReference]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspNarrativeItems]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspNarrativeItems]') AND name = N'IX_MspNarrativeItems_GatewayMessageId_Key')
BEGIN
    CREATE INDEX [IX_MspNarrativeItems_GatewayMessageId_Key] ON [vsdc].[MspNarrativeItems] ([GatewayMessageId], [Key]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspSecuritiesPositionInstructions]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspSecuritiesPositionInstructions]') AND name = N'IX_MspSecuritiesPositionInstructions_SafeAccount_Isin')
BEGIN
    CREATE INDEX [IX_MspSecuritiesPositionInstructions_SafeAccount_Isin] ON [vsdc].[MspSecuritiesPositionInstructions] ([SafeAccount], [Isin]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspCashInstructions]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspCashInstructions]') AND name = N'IX_MspCashInstructions_AccountNo')
BEGIN
    CREATE INDEX [IX_MspCashInstructions_AccountNo] ON [vsdc].[MspCashInstructions] ([AccountNo]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspSettlementInstructions]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspSettlementInstructions]') AND name = N'IX_MspSettlementInstructions_TradeDate_SafeAccount_Isin')
BEGIN
    CREATE INDEX [IX_MspSettlementInstructions_TradeDate_SafeAccount_Isin] ON [vsdc].[MspSettlementInstructions] ([TradeDate], [SafeAccount], [Isin]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspStatusInquiries]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspStatusInquiries]') AND name = N'IX_MspStatusInquiries_OriginalReference')
BEGIN
    CREATE INDEX [IX_MspStatusInquiries_OriginalReference] ON [vsdc].[MspStatusInquiries] ([OriginalReference]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspReportStatisticRows]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspReportStatisticRows]') AND name = N'IX_MspReportStatisticRows_SendTime_MessageType')
BEGIN
    CREATE INDEX [IX_MspReportStatisticRows_SendTime_MessageType] ON [vsdc].[MspReportStatisticRows] ([SendTime], [MessageType]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspParMetadata]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspParMetadata]') AND name = N'IX_MspParMetadata_OriginalTransferReference')
BEGIN
    CREATE INDEX [IX_MspParMetadata_OriginalTransferReference] ON [vsdc].[MspParMetadata] ([OriginalTransferReference]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspOperationDefinitions]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspOperationDefinitions]') AND name = N'IX_MspOperationDefinitions_Code')
BEGIN
    CREATE UNIQUE INDEX [IX_MspOperationDefinitions_Code] ON [vsdc].[MspOperationDefinitions] ([Code]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspFieldDefinitions]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspFieldDefinitions]') AND name = N'IX_MspFieldDefinitions_MspOperationDefinitionId_SequenceNo')
BEGIN
    CREATE INDEX [IX_MspFieldDefinitions_MspOperationDefinitionId_SequenceNo] ON [vsdc].[MspFieldDefinitions] ([MspOperationDefinitionId], [SequenceNo]);
END
GO

IF OBJECT_ID(N'[vsdc].[MspTemplateVersions]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[vsdc].[MspTemplateVersions]') AND name = N'IX_MspTemplateVersions_OperationCode_Version')
BEGIN
    CREATE UNIQUE INDEX [IX_MspTemplateVersions_OperationCode_Version] ON [vsdc].[MspTemplateVersions] ([OperationCode], [Version]);
END
GO

IF OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_GatewayMessages_GatewayFiles_GatewayFileId')
BEGIN
    ALTER TABLE [vsdc].[GatewayMessages] WITH NOCHECK ADD CONSTRAINT [FK_GatewayMessages_GatewayFiles_GatewayFileId] FOREIGN KEY ([GatewayFileId]) REFERENCES [vsdc].[GatewayFiles] ([Id]);
    ALTER TABLE [vsdc].[GatewayMessages] NOCHECK CONSTRAINT [FK_GatewayMessages_GatewayFiles_GatewayFileId];
END
GO

IF OBJECT_ID(N'[vsdc].[MessageHeaders]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MessageHeaders_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MessageHeaders] WITH NOCHECK ADD CONSTRAINT [FK_MessageHeaders_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MessageHeaders] NOCHECK CONSTRAINT [FK_MessageHeaders_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MessageBlocks]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MessageBlocks_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MessageBlocks] WITH NOCHECK ADD CONSTRAINT [FK_MessageBlocks_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MessageBlocks] NOCHECK CONSTRAINT [FK_MessageBlocks_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MessageTags]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MessageTags_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MessageTags] WITH NOCHECK ADD CONSTRAINT [FK_MessageTags_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MessageTags] NOCHECK CONSTRAINT [FK_MessageTags_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MessageReferences]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MessageReferences_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MessageReferences] WITH NOCHECK ADD CONSTRAINT [FK_MessageReferences_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MessageReferences] NOCHECK CONSTRAINT [FK_MessageReferences_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MessageStatusHistories]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MessageStatusHistories_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MessageStatusHistories] WITH NOCHECK ADD CONSTRAINT [FK_MessageStatusHistories_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MessageStatusHistories] NOCHECK CONSTRAINT [FK_MessageStatusHistories_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[Customers]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Customers_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[Customers] WITH NOCHECK ADD CONSTRAINT [FK_Customers_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[Customers] NOCHECK CONSTRAINT [FK_Customers_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[CustomerIdentities]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[Customers]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CustomerIdentities_Customers_CustomerId')
BEGIN
    ALTER TABLE [vsdc].[CustomerIdentities] WITH NOCHECK ADD CONSTRAINT [FK_CustomerIdentities_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [vsdc].[Customers] ([Id]);
    ALTER TABLE [vsdc].[CustomerIdentities] NOCHECK CONSTRAINT [FK_CustomerIdentities_Customers_CustomerId];
END
GO

IF OBJECT_ID(N'[vsdc].[CustomerAddresses]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[Customers]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CustomerAddresses_Customers_CustomerId')
BEGIN
    ALTER TABLE [vsdc].[CustomerAddresses] WITH NOCHECK ADD CONSTRAINT [FK_CustomerAddresses_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [vsdc].[Customers] ([Id]);
    ALTER TABLE [vsdc].[CustomerAddresses] NOCHECK CONSTRAINT [FK_CustomerAddresses_Customers_CustomerId];
END
GO

IF OBJECT_ID(N'[vsdc].[CustomerContacts]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[Customers]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CustomerContacts_Customers_CustomerId')
BEGIN
    ALTER TABLE [vsdc].[CustomerContacts] WITH NOCHECK ADD CONSTRAINT [FK_CustomerContacts_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [vsdc].[Customers] ([Id]);
    ALTER TABLE [vsdc].[CustomerContacts] NOCHECK CONSTRAINT [FK_CustomerContacts_Customers_CustomerId];
END
GO

IF OBJECT_ID(N'[vsdc].[ReportFiles]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ReportFiles_GatewayFiles_GatewayFileId')
BEGIN
    ALTER TABLE [vsdc].[ReportFiles] WITH NOCHECK ADD CONSTRAINT [FK_ReportFiles_GatewayFiles_GatewayFileId] FOREIGN KEY ([GatewayFileId]) REFERENCES [vsdc].[GatewayFiles] ([Id]);
    ALTER TABLE [vsdc].[ReportFiles] NOCHECK CONSTRAINT [FK_ReportFiles_GatewayFiles_GatewayFileId];
END
GO

IF OBJECT_ID(N'[vsdc].[ReportRows]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[ReportFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ReportRows_ReportFiles_ReportFileId')
BEGIN
    ALTER TABLE [vsdc].[ReportRows] WITH NOCHECK ADD CONSTRAINT [FK_ReportRows_ReportFiles_ReportFileId] FOREIGN KEY ([ReportFileId]) REFERENCES [vsdc].[ReportFiles] ([Id]);
    ALTER TABLE [vsdc].[ReportRows] NOCHECK CONSTRAINT [FK_ReportRows_ReportFiles_ReportFileId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspFieldDefinitions]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[MspOperationDefinitions]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspFieldDefinitions_MspOperationDefinitions_MspOperationDefinitionId')
BEGIN
    ALTER TABLE [vsdc].[MspFieldDefinitions] WITH NOCHECK ADD CONSTRAINT [FK_MspFieldDefinitions_MspOperationDefinitions_MspOperationDefinitionId] FOREIGN KEY ([MspOperationDefinitionId]) REFERENCES [vsdc].[MspOperationDefinitions] ([Id]);
    ALTER TABLE [vsdc].[MspFieldDefinitions] NOCHECK CONSTRAINT [FK_MspFieldDefinitions_MspOperationDefinitions_MspOperationDefinitionId];
END
GO

IF OBJECT_ID(N'[vsdc].[Accounts]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Accounts_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[Accounts] WITH NOCHECK ADD CONSTRAINT [FK_Accounts_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[Accounts] NOCHECK CONSTRAINT [FK_Accounts_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[AccountMappings]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_AccountMappings_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[AccountMappings] WITH NOCHECK ADD CONSTRAINT [FK_AccountMappings_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[AccountMappings] NOCHECK CONSTRAINT [FK_AccountMappings_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[AccountChanges]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_AccountChanges_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[AccountChanges] WITH NOCHECK ADD CONSTRAINT [FK_AccountChanges_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[AccountChanges] NOCHECK CONSTRAINT [FK_AccountChanges_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[Securities]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Securities_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[Securities] WITH NOCHECK ADD CONSTRAINT [FK_Securities_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[Securities] NOCHECK CONSTRAINT [FK_Securities_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[Orders]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Orders_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[Orders] WITH NOCHECK ADD CONSTRAINT [FK_Orders_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[Orders] NOCHECK CONSTRAINT [FK_Orders_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[Trades]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Trades_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[Trades] WITH NOCHECK ADD CONSTRAINT [FK_Trades_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[Trades] NOCHECK CONSTRAINT [FK_Trades_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[CashTransactions]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CashTransactions_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[CashTransactions] WITH NOCHECK ADD CONSTRAINT [FK_CashTransactions_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[CashTransactions] NOCHECK CONSTRAINT [FK_CashTransactions_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[SecuritiesTransfers]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_SecuritiesTransfers_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[SecuritiesTransfers] WITH NOCHECK ADD CONSTRAINT [FK_SecuritiesTransfers_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[SecuritiesTransfers] NOCHECK CONSTRAINT [FK_SecuritiesTransfers_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[RightsRegistrations]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_RightsRegistrations_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[RightsRegistrations] WITH NOCHECK ADD CONSTRAINT [FK_RightsRegistrations_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[RightsRegistrations] NOCHECK CONSTRAINT [FK_RightsRegistrations_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[CorporateActions]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CorporateActions_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[CorporateActions] WITH NOCHECK ADD CONSTRAINT [FK_CorporateActions_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[CorporateActions] NOCHECK CONSTRAINT [FK_CorporateActions_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[Fees]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Fees_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[Fees] WITH NOCHECK ADD CONSTRAINT [FK_Fees_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[Fees] NOCHECK CONSTRAINT [FK_Fees_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[Taxes]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Taxes_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[Taxes] WITH NOCHECK ADD CONSTRAINT [FK_Taxes_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[Taxes] NOCHECK CONSTRAINT [FK_Taxes_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[NavRecords]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NavRecords_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[NavRecords] WITH NOCHECK ADD CONSTRAINT [FK_NavRecords_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[NavRecords] NOCHECK CONSTRAINT [FK_NavRecords_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[ProcessingHistories]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ProcessingHistories_GatewayFiles_GatewayFileId')
BEGIN
    ALTER TABLE [vsdc].[ProcessingHistories] WITH NOCHECK ADD CONSTRAINT [FK_ProcessingHistories_GatewayFiles_GatewayFileId] FOREIGN KEY ([GatewayFileId]) REFERENCES [vsdc].[GatewayFiles] ([Id]);
    ALTER TABLE [vsdc].[ProcessingHistories] NOCHECK CONSTRAINT [FK_ProcessingHistories_GatewayFiles_GatewayFileId];
END
GO

IF OBJECT_ID(N'[vsdc].[ValidationErrors]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ValidationErrors_GatewayFiles_GatewayFileId')
BEGIN
    ALTER TABLE [vsdc].[ValidationErrors] WITH NOCHECK ADD CONSTRAINT [FK_ValidationErrors_GatewayFiles_GatewayFileId] FOREIGN KEY ([GatewayFileId]) REFERENCES [vsdc].[GatewayFiles] ([Id]);
    ALTER TABLE [vsdc].[ValidationErrors] NOCHECK CONSTRAINT [FK_ValidationErrors_GatewayFiles_GatewayFileId];
END
GO

IF OBJECT_ID(N'[vsdc].[OutboxJobs]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_OutboxJobs_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[OutboxJobs] WITH NOCHECK ADD CONSTRAINT [FK_OutboxJobs_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[OutboxJobs] NOCHECK CONSTRAINT [FK_OutboxJobs_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[InboxJobs]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_InboxJobs_GatewayFiles_GatewayFileId')
BEGIN
    ALTER TABLE [vsdc].[InboxJobs] WITH NOCHECK ADD CONSTRAINT [FK_InboxJobs_GatewayFiles_GatewayFileId] FOREIGN KEY ([GatewayFileId]) REFERENCES [vsdc].[GatewayFiles] ([Id]);
    ALTER TABLE [vsdc].[InboxJobs] NOCHECK CONSTRAINT [FK_InboxJobs_GatewayFiles_GatewayFileId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspBusinessMessages]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspBusinessMessages_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspBusinessMessages] WITH NOCHECK ADD CONSTRAINT [FK_MspBusinessMessages_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspBusinessMessages] NOCHECK CONSTRAINT [FK_MspBusinessMessages_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspAckNaks]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspAckNaks_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspAckNaks] WITH NOCHECK ADD CONSTRAINT [FK_MspAckNaks_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspAckNaks] NOCHECK CONSTRAINT [FK_MspAckNaks_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspNarrativeItems]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspNarrativeItems_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspNarrativeItems] WITH NOCHECK ADD CONSTRAINT [FK_MspNarrativeItems_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspNarrativeItems] NOCHECK CONSTRAINT [FK_MspNarrativeItems_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspSecuritiesPositionInstructions]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspSecuritiesPositionInstructions_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspSecuritiesPositionInstructions] WITH NOCHECK ADD CONSTRAINT [FK_MspSecuritiesPositionInstructions_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspSecuritiesPositionInstructions] NOCHECK CONSTRAINT [FK_MspSecuritiesPositionInstructions_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspSecuritiesPositionResponses]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspSecuritiesPositionResponses_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspSecuritiesPositionResponses] WITH NOCHECK ADD CONSTRAINT [FK_MspSecuritiesPositionResponses_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspSecuritiesPositionResponses] NOCHECK CONSTRAINT [FK_MspSecuritiesPositionResponses_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspCashInstructions]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspCashInstructions_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspCashInstructions] WITH NOCHECK ADD CONSTRAINT [FK_MspCashInstructions_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspCashInstructions] NOCHECK CONSTRAINT [FK_MspCashInstructions_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspCashResponses]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspCashResponses_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspCashResponses] WITH NOCHECK ADD CONSTRAINT [FK_MspCashResponses_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspCashResponses] NOCHECK CONSTRAINT [FK_MspCashResponses_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspSettlementInstructions]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspSettlementInstructions_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspSettlementInstructions] WITH NOCHECK ADD CONSTRAINT [FK_MspSettlementInstructions_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspSettlementInstructions] NOCHECK CONSTRAINT [FK_MspSettlementInstructions_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspStatusInquiries]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspStatusInquiries_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspStatusInquiries] WITH NOCHECK ADD CONSTRAINT [FK_MspStatusInquiries_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspStatusInquiries] NOCHECK CONSTRAINT [FK_MspStatusInquiries_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspStatusResponses]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspStatusResponses_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspStatusResponses] WITH NOCHECK ADD CONSTRAINT [FK_MspStatusResponses_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspStatusResponses] NOCHECK CONSTRAINT [FK_MspStatusResponses_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspReconcileInquiries]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspReconcileInquiries_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspReconcileInquiries] WITH NOCHECK ADD CONSTRAINT [FK_MspReconcileInquiries_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspReconcileInquiries] NOCHECK CONSTRAINT [FK_MspReconcileInquiries_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspReconcileResponses]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspReconcileResponses_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspReconcileResponses] WITH NOCHECK ADD CONSTRAINT [FK_MspReconcileResponses_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspReconcileResponses] NOCHECK CONSTRAINT [FK_MspReconcileResponses_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspParties]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspParties_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspParties] WITH NOCHECK ADD CONSTRAINT [FK_MspParties_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspParties] NOCHECK CONSTRAINT [FK_MspParties_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspAmounts]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspAmounts_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspAmounts] WITH NOCHECK ADD CONSTRAINT [FK_MspAmounts_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspAmounts] NOCHECK CONSTRAINT [FK_MspAmounts_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspBalanceMovements]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayMessages]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspBalanceMovements_GatewayMessages_GatewayMessageId')
BEGIN
    ALTER TABLE [vsdc].[MspBalanceMovements] WITH NOCHECK ADD CONSTRAINT [FK_MspBalanceMovements_GatewayMessages_GatewayMessageId] FOREIGN KEY ([GatewayMessageId]) REFERENCES [vsdc].[GatewayMessages] ([Id]);
    ALTER TABLE [vsdc].[MspBalanceMovements] NOCHECK CONSTRAINT [FK_MspBalanceMovements_GatewayMessages_GatewayMessageId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspReportStatisticRows]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspReportStatisticRows_GatewayFiles_GatewayFileId')
BEGIN
    ALTER TABLE [vsdc].[MspReportStatisticRows] WITH NOCHECK ADD CONSTRAINT [FK_MspReportStatisticRows_GatewayFiles_GatewayFileId] FOREIGN KEY ([GatewayFileId]) REFERENCES [vsdc].[GatewayFiles] ([Id]);
    ALTER TABLE [vsdc].[MspReportStatisticRows] NOCHECK CONSTRAINT [FK_MspReportStatisticRows_GatewayFiles_GatewayFileId];
END
GO

IF OBJECT_ID(N'[vsdc].[MspParMetadata]', N'U') IS NOT NULL AND OBJECT_ID(N'[vsdc].[GatewayFiles]', N'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MspParMetadata_GatewayFiles_GatewayFileId')
BEGIN
    ALTER TABLE [vsdc].[MspParMetadata] WITH NOCHECK ADD CONSTRAINT [FK_MspParMetadata_GatewayFiles_GatewayFileId] FOREIGN KEY ([GatewayFileId]) REFERENCES [vsdc].[GatewayFiles] ([Id]);
    ALTER TABLE [vsdc].[MspParMetadata] NOCHECK CONSTRAINT [FK_MspParMetadata_GatewayFiles_GatewayFileId];
END
GO

-- Seed MSP operation catalog
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_SECURITIES_BLOCK')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SECURITIES_BLOCK', N'MT524 - Yêu cầu phong tỏa chứng khoán', N'524', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_SECURITIES_UNBLOCK')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SECURITIES_UNBLOCK', N'MT524 - Yêu cầu giải tỏa chứng khoán', N'524', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_SECURITIES_RESPONSE_ACCEPT')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SECURITIES_RESPONSE_ACCEPT', N'MT548 - Xác nhận phong tỏa/giải tỏa', N'548', N'IN', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_SECURITIES_RESPONSE_REJECT')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SECURITIES_RESPONSE_REJECT', N'MT548 - Từ chối phong tỏa/giải tỏa', N'548', N'IN', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_CASH_BLOCK')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_CASH_BLOCK', N'MT199 - Yêu cầu phong tỏa tiền', N'199', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_CASH_UNBLOCK')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_CASH_UNBLOCK', N'MT199 - Yêu cầu giải tỏa tiền', N'199', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_CASH_RESPONSE')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_CASH_RESPONSE', N'MT199 - Xác nhận/Từ chối tiền', N'199', N'IN', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_SETTLEMENT_BUY')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SETTLEMENT_BUY', N'MT541 - Chỉ thị thanh toán lệnh mua', N'541', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_SETTLEMENT_SELL')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SETTLEMENT_SELL', N'MT543 - Chỉ thị thanh toán lệnh bán', N'543', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_STATUS_INQUIRY')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_STATUS_INQUIRY', N'MT199/MT599 - Tra soát trạng thái', N'199', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_STATUS_RESPONSE')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_STATUS_RESPONSE', N'MT199/MT599 - Phản hồi tra soát', N'199', N'IN', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_RECONCILE_INQUIRY')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_RECONCILE_INQUIRY', N'MT599 - Yêu cầu báo cáo đối chiếu', N'599', N'OUT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[MspOperationDefinitions] WHERE [Code] = N'MSP_RECONCILE_RESPONSE')
    INSERT INTO [vsdc].[MspOperationDefinitions] ([CreatedAtUtc],[Code],[Name],[MessageType],[Direction],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_RECONCILE_RESPONSE', N'MT599 - Phản hồi báo cáo đối chiếu', N'599', N'IN', 1);
GO

-- Seed simulator rules
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_SECURITIES_BLOCK' AND [IncomingMessageType] = N'524')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SECURITIES_BLOCK', N'524', N'548', N'ACCEPT', 250, 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_SECURITIES_UNBLOCK' AND [IncomingMessageType] = N'524')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SECURITIES_UNBLOCK', N'524', N'548', N'ACCEPT', 250, 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_CASH_BLOCK' AND [IncomingMessageType] = N'199')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_CASH_BLOCK', N'199', N'199', N'ACCEPT', 250, 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_CASH_UNBLOCK' AND [IncomingMessageType] = N'199')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_CASH_UNBLOCK', N'199', N'199', N'ACCEPT', 250, 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_SETTLEMENT_BUY' AND [IncomingMessageType] = N'541')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SETTLEMENT_BUY', N'541', N'ACK', N'ACCEPT', 250, 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_SETTLEMENT_SELL' AND [IncomingMessageType] = N'543')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_SETTLEMENT_SELL', N'543', N'ACK', N'ACCEPT', 250, 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_STATUS_INQUIRY' AND [IncomingMessageType] = N'199')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_STATUS_INQUIRY', N'199', N'199', N'ACCEPT', 250, 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[SimulatorRules] WHERE [OperationCode] = N'MSP_RECONCILE_INQUIRY' AND [IncomingMessageType] = N'599')
    INSERT INTO [vsdc].[SimulatorRules] ([CreatedAtUtc],[OperationCode],[IncomingMessageType],[ResponseMessageType],[Result],[DelayMilliseconds],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_RECONCILE_INQUIRY', N'599', N'599', N'ACCEPT', 250, 1);
GO

-- Seed manual templates
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_MT548_ACCEPT')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_MT548_ACCEPT', N'MT548 - Xác nhận phong tỏa/giải tỏa CK', N'FIN', N'MSP - Phong tỏa', N'MSP_MT548_RESPONSE|ACCEPT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_MT548_REJECT')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_MT548_REJECT', N'MT548 - Từ chối phong tỏa/giải tỏa CK', N'FIN', N'MSP - Phong tỏa', N'MSP_MT548_RESPONSE|REJECT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_MT199_ACCEPT')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_MT199_ACCEPT', N'MT199 - Xác nhận phong tỏa/giải tỏa tiền', N'FIN', N'MSP - Tiền', N'MSP_MT199_CASH_RESPONSE|ACCEPT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_MT199_REJECT')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_MT199_REJECT', N'MT199 - Từ chối phong tỏa/giải tỏa tiền', N'FIN', N'MSP - Tiền', N'MSP_MT199_CASH_RESPONSE|REJECT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_STATUS_RESPONSE')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_STATUS_RESPONSE', N'MT199 - Phản hồi tra soát trạng thái', N'FIN', N'MSP - Tra soát', N'MSP_STATUS_RESPONSE|ACCEPT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_RECONCILE_PACK')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_RECONCILE_PACK', N'MT599 - Phản hồi báo cáo PACK + CSV/PAR', N'PAIR', N'MSP - Đối chiếu', N'MSP_RECONCILE_RESPONSE|ACCEPT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_RECONCILE_REJECT')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_RECONCILE_REJECT', N'MT599 - Từ chối báo cáo đối chiếu', N'FIN', N'MSP - Đối chiếu', N'MSP_RECONCILE_RESPONSE|REJECT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_TECH_ACK')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_TECH_ACK', N'ACK kỹ thuật F21', N'FIN', N'MSP - Kỹ thuật', N'MSP_TECH_ACK|ACCEPT', 1);
IF NOT EXISTS (SELECT 1 FROM [vsdc].[ManualTemplates] WHERE [Code] = N'MSP_TECH_NAK')
    INSERT INTO [vsdc].[ManualTemplates] ([CreatedAtUtc],[Code],[Name],[FileType],[Module],[TemplateBody],[IsEnabled]) VALUES (SYSUTCDATETIME(), N'MSP_TECH_NAK', N'NAK kỹ thuật F21', N'FIN', N'MSP - Kỹ thuật', N'MSP_TECH_ACK|REJECT', 1);
GO

IF NOT EXISTS (SELECT 1 FROM [vsdc].[SystemSettings] WHERE [SettingKey] = N'Simulator.AutoMode')
    INSERT INTO [vsdc].[SystemSettings] ([CreatedAtUtc],[SettingKey],[SettingValue],[GroupName],[IsSecret]) VALUES (SYSUTCDATETIME(), N'Simulator.AutoMode', N'False', N'Simulator', 0);
GO



-- v1.2.0 User authentication / authorization tables
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

-- Verification
SELECT s.name AS SchemaName, COUNT(*) AS TableCount FROM sys.tables t JOIN sys.schemas s ON t.schema_id=s.schema_id WHERE s.name=N'vsdc' GROUP BY s.name;
SELECT s.name AS SchemaName, t.name AS TableName FROM sys.tables t JOIN sys.schemas s ON t.schema_id=s.schema_id WHERE s.name=N'vsdc' ORDER BY t.name;
GO
