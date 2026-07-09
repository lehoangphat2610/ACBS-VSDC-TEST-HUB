# RUNBOOK DEPLOY DCRSM UAT - v1.5.2

## 1. Publish đúng cách

Không copy từ `bin\Release\net10.0` và không copy riêng `.exe`.

Dùng script:

```powershell
.\scripts\publish-dcrsm-uat-win-x64-self-contained.ps1
```

Sau đó copy nguyên thư mục:

```text
publish\DCRSM_UAT_win-x64_self-contained
```

sang server.

## 2. Config DCRSM

Database khi chạy ngay trên server DCRSM:

```json
"ConnectionStrings": {
  "VsdcDb": "Server=.\\SQLEXPRESS;Database=ACBS_VSDC_TESTHUB;User ID=sa;Password=ACBS@01;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

Folder:

```json
"GatewayFolders": {
  "Send": "H:\\Send",
  "Receive": "H:\\Receive",
  "Archive": "H:\\Archive",
  "Error": "H:\\Error"
}
```

Port:

```json
"Urls": "http://0.0.0.0:9898"
```

## 3. Kiểm tra CSS/JS

Mở:

```text
http://localhost:9898/css/site.css
http://localhost:9898/js/site.js
```

Nếu thấy nội dung file, giao diện sẽ không bị bể.

## 4. Kiểm tra MSP routing

Tạo thử MT524. File trong `H:\Send` phải bắt đầu bằng:

```text
{1:F01VSDC002XAXXX0000000000}{2:I524VSDC404XXXXXN}
```

Nếu nhập ngân hàng nhận `403`, file phải là:

```text
{2:I524VSDC403XXXXXN}
```

## 5. Chạy app bằng CMD để xem log

```bat
cd /d D:\VSDC\CS\Acbs.Vsdc.TestHub
Acbs.Vsdc.TestHub.exe
```

Kiểm tra port:

```bat
netstat -ano | findstr :9898
```
