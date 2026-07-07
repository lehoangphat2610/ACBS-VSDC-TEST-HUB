-- Chạy bởi DBA và thay DOMAIN\ServiceAccount theo tài khoản dịch vụ thực tế.
USE [ACBS_VSDC_TESTHUB];
GO
-- CREATE USER [DOMAIN\ServiceAccount] FOR LOGIN [DOMAIN\ServiceAccount];
-- ALTER ROLE db_datareader ADD MEMBER [DOMAIN\ServiceAccount];
-- ALTER ROLE db_datawriter ADD MEMBER [DOMAIN\ServiceAccount];
-- ALTER ROLE db_ddladmin ADD MEMBER [DOMAIN\ServiceAccount]; -- chỉ dùng khi app tự EnsureCreated/Migrate
