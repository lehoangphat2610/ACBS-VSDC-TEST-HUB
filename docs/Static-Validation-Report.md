# Báo cáo kiểm tra tĩnh khi đóng gói

Ngày kiểm tra: 2026-06-17

- JSON config: hợp lệ.
- Project XML (`.csproj`): hợp lệ.
- `Program.cs`: 10 dòng, chỉ bootstrap.
- C# source: 157 file.
- Folder trong project ASP.NET: 66 folder.
- Không tìm thấy đường dẫn `\\CSD\...` hard-code trong file `.cs`.
- Kiểm tra lexer cơ bản đối với chuỗi, comment và cặp ngoặc: không phát hiện lỗi.
- Không phát hiện trùng tên top-level type.
- Bộ mẫu MSP: 50 file, gồm 48 FIN, 1 CSV, 1 PAR.

## Giới hạn

Môi trường đóng gói không có .NET 10 SDK nên chưa chạy được `dotnet restore` và `dotnet build`. Trên máy phát triển/UAT phải chạy compiler validation:

```powershell
dotnet restore
dotnet build Acbs.Vsdc.TestHub.sln -c Release
```
