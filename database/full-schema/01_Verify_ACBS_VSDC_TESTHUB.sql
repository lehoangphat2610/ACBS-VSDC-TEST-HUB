USE [ACBS_VSDC_TESTHUB];
GO
SELECT DB_NAME() AS CurrentDatabase;
SELECT s.name AS SchemaName, COUNT(*) AS TableCount
FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id
GROUP BY s.name
ORDER BY s.name;
SELECT s.name AS SchemaName, t.name AS TableName
FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = N'vsdc'
ORDER BY t.name;
SELECT TOP 100 * FROM [vsdc].[GatewayMessages] ORDER BY Id DESC;
SELECT TOP 100 * FROM [vsdc].[MspBusinessMessages] ORDER BY Id DESC;
SELECT TOP 100 * FROM [vsdc].[SystemLogs] ORDER BY Id DESC;

SELECT TOP 100 * FROM [vsdc].[UserAccounts] ORDER BY Id DESC;
SELECT TOP 100 * FROM [vsdc].[UserRoles] ORDER BY Level DESC;
SELECT TOP 100 * FROM [vsdc].[UserAuditLogs] ORDER BY Id DESC;
