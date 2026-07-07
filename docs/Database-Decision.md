# Quyết định Database

## Khuyến nghị mặc định: SQL Server Express 2025

Chọn **SQL Server Express 2025** cho DEV/UAT của Test Hub:

- phù hợp mô hình web/service tập trung trên Windows;
- nhiều tester và hai background worker có thể truy cập cùng một database;
- dùng trực tiếp EF Core SQL Server;
- dễ xem dữ liệu, backup/restore bằng SSMS;
- có đường nâng cấp lên SQL Server Standard mà không phải đổi provider trong code;
- giới hạn database 50 GB của SQL Server 2025 Express đủ rộng cho giai đoạn test, nhưng phải thiết lập retention cho raw điện và log.

## Không chọn SQLite làm database chính

SQLite rất gọn cho demo portable một máy, nhưng không phải lựa chọn chính cho workload có Receive worker, Simulator worker, nhiều người dùng và lượng log ghi liên tục. Có thể bổ sung SQLite sau như một profile `PortableDemo`, không dùng cho UAT chung.

## Cấu hình

Chỉ sửa:

- `src/Acbs.Vsdc.TestHub/Config/appsettings.database.json`
- hoặc file override `appsettings.Development.json`, `appsettings.UAT.json`.

Không sửa source code khi đổi SQL Server/IP/database.
