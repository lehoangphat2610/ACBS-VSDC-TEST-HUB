# ACBS VSDC Test Hub - MSP Full v1.5.2

ASP.NET Core MVC .NET 10 tool phục vụ Core Test / Simulator nghiệp vụ VSDC.MSP.

## Nội dung chính

- Mode Test MSP: tạo điện MT524, MT199, MT541, MT543, MT599 gửi vào folder Send.
- Mode Simulator: AutoMode/ManualMode tạo ACK/NAK/PACK/REJT và CSV/PAR vào Receive.
- Quản lý điện gửi/nhận, đọc FIN/PAR/CSV, bóc tách vào SQL Server.
- Xử lý điện thủ công theo folder Send/Receive/Archive/Error.
- Logs hệ thống có lọc ngày.
- User management: Admin/Tester/Viewer, thêm/sửa/khóa/xóa user. User `admin` không được xóa.

## Điểm mới v1.5.2

Bản này tập trung sửa riêng nghiệp vụ **MT524 phong tỏa / giải tỏa chứng khoán** theo yêu cầu mới của VSDC:

- Điện **phong tỏa chứng khoán mới** không còn sinh 3 dòng `LINK/PREV`.
- Thêm module **MT524 - Hủy yêu cầu phong tỏa chứng khoán**: `:23G:CANC`, `:20C::PREV//<mã điện phong tỏa gốc>`.
- Điện **giải tỏa chứng khoán mới** vẫn dùng `:23G:NEWM` và có `LINK/PREV` tham chiếu đến mã điện phong tỏa trước đó.
- Thêm module **MT524 - Hủy yêu cầu giải tỏa chứng khoán**: `:23G:CANC`, `:20C::PREV//<mã điện giải tỏa vừa gửi>`.
- Preview Raw FIN trên UI và builder server-side đã đồng bộ cùng logic.

Các thay đổi header/routing từ v1.5.0/v1.5.1 vẫn được giữ nguyên:

```text
Block 1 gửi đi: {1:F01VSDC002XAXXX0000000000}
Block 2 gửi đi: {2:I524VSDC404XXXXXN}
ACK Block 1  : {1:F21VSDC002XAXXX0000000000}{4:{177:...}{451:0}}
NAK Block 1  : {1:F21VSDC002XAXXX0000000000}{4:{177:...}{451:1}{405:...}}
ACK/NAK quote Block 2: {2:I524VSDCSVN6XXXXN}
```

## Fix quan trọng v1.5.2

- ManualMode ACK/NAK sẽ tìm và quote nguyên văn điện gốc theo Reference, không còn tạo ACK chỉ có tag `:20:`.
- Điện nhận về nghiệp vụ dùng đúng output header: Block 1 là ACBS `VSDC002X`, Block 2 chứa MIR ngân hàng lưu ký như `VSDC404X`.
- AutoMode phản hồi nghiệp vụ dùng ngân hàng nhận từ Block 2 của điện gửi đi.

## Chạy local

```powershell
dotnet run --project src/Acbs.Vsdc.TestHub/Acbs.Vsdc.TestHub.csproj --environment UAT
```

URL:

```text
http://localhost:9898
```

Login mặc định:

```text
admin / ACBS@01
```

## Publish DCRSM UAT

Dùng script:

```powershell
.\scripts\publish-dcrsm-uat-win-x64-self-contained.ps1
```

Copy nguyên thư mục publish sang server, không copy riêng file `.exe` để tránh thiếu `wwwroot` làm bể giao diện.

Xem chi tiết:

```text
RUNBOOK_DEPLOY_DCRSM_UAT.md
CONFIG_EFFECTIVE_VALUES.md
CHANGELOG.md
MSP_SPEC_IMPLEMENTATION_NOTES.md
MSP_SPEC_COMPARE_MATRIX.md
```
