# Trạng thái tích hợp MSP

## Đã tích hợp

- Đọc realtime `Receive` bằng watcher kết hợp reconciliation scan.
- Chờ file ổn định trước khi đọc, chống trùng bằng SHA-256, lưu raw content.
- Bóc tách FIN theo Block 1/2/4/5, tag, qualifier, reference, ACK/NAK F21.
- Chuyển đổi hai chiều tiếng Việt sang bộ ký tự SWIFT/ISO 15022.
- Lưu FIN, CSV, PAR vào các bảng chung và các bảng chuyên biệt MSP.
- MT524: phong tỏa và giải tỏa chứng khoán.
- MT548: xác nhận hoặc từ chối yêu cầu MT524.
- MT199: phong tỏa tiền, giải tỏa tiền, xác nhận hoặc từ chối.
- MT541: chỉ thị thanh toán giao dịch mua.
- MT543: chỉ thị thanh toán giao dịch bán.
- MT199/MT599: tra soát và phản hồi trạng thái.
- MT599: yêu cầu, phản hồi báo cáo đối chiếu; tạo cặp CSV/PAR.
- AutoMode và ManualMode.
- Bảng log, lịch sử trạng thái, validation error, simulator run và checkpoint.
- 50 file mẫu thật của bộ dự án được giữ tại `samples/MSP/Actual`.

## Ranh giới cần xác nhận trước UAT thật

- BIC của ACBS, BIC đối tác và Logical Terminal theo môi trường.
- Session number, ISN/OSN, quy tắc reset sequence và cơ chế chống trùng.
- Quy tắc tên file mà VSDC.MSP Client tại ACBS đang cấu hình.
- CHK/MAC/TNG và phạm vi VSDC.MSP Client tự ký/đóng gói.
- Quyền tài khoản dịch vụ đối với SMB share.
- Mã report chính thức và thứ tự/cột CSV cuối cùng.
- Chính sách lưu trữ, thời gian giữ raw file và log.

## Kiểm tra source

Chạy:

```powershell
./scripts/validate-structure.ps1
dotnet restore
dotnet build Acbs.Vsdc.TestHub.sln -c Release
```

Script đầu chỉ kiểm tra cấu trúc, JSON/XML và hard-code. `dotnet build` mới là bước xác nhận compiler đầy đủ.
