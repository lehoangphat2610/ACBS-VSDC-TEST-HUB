/*
ACBS VSDC TESTHUB - HOTFIX for MessageTags index
Reason: SQL Server cannot create an index key on nvarchar(max) column TagValue.
Safe to run multiple times.
*/
USE [ACBS_VSDC_TESTHUB];
GO

IF OBJECT_ID(N'[vsdc].[MessageTags]', N'U') IS NULL
BEGIN
    PRINT N'Table [vsdc].[MessageTags] does not exist. Please run 00_CreateFullDatabaseAndTables_ACBS_VSDC_TESTHUB.sql first.';
END
GO

IF OBJECT_ID(N'[vsdc].[MessageTags]', N'U') IS NOT NULL
   AND NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'[vsdc].[MessageTags]')
          AND name = N'IX_MessageTags_TagCode'
   )
BEGIN
    CREATE INDEX [IX_MessageTags_TagCode]
    ON [vsdc].[MessageTags] ([TagCode], [Qualifier])
    INCLUDE ([GatewayMessageId], [SequenceNo]);

    PRINT N'Created index [vsdc].[MessageTags].[IX_MessageTags_TagCode].';
END
ELSE
BEGIN
    PRINT N'Index [IX_MessageTags_TagCode] already exists or table is missing.';
END
GO

SELECT
    s.name AS SchemaName,
    t.name AS TableName,
    i.name AS IndexName
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = N'vsdc'
  AND t.name = N'MessageTags'
ORDER BY i.name;
GO
