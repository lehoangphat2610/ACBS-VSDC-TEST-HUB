# RUNBOOK LOCAL/UAT - v1.5.2

## 1. Chuẩn bị database

Database mặc định:

```text
Server   = PHATLH-LT-TD\SQLEXPRESS
Database = ACBS_VSDC_TESTHUB
User     = sa
Password = ACBS@01
```

Nếu chưa có DB, chạy:

```text
database/full-schema/00_CreateFullDatabaseAndTables_ACBS_VSDC_TESTHUB.sql
database/full-schema/03_Upgrade_AuthUsers_v1_2_0.sql
```

## 2. Chuẩn bị folder local

```powershell
New-Item -ItemType Directory -Force "D:\VSDC\CSD\Send"
New-Item -ItemType Directory -Force "D:\VSDC\CSD\Receive"
New-Item -ItemType Directory -Force "D:\VSDC\CSD\Archive"
New-Item -ItemType Directory -Force "D:\VSDC\CSD\Error"
```

## 3. Chạy app

```powershell
dotnet run --project src/Acbs.Vsdc.TestHub/Acbs.Vsdc.TestHub.csproj --environment UAT
```

Mở:

```text
http://localhost:9898
```

## 4. Kiểm tra MSP header

Vào **Mode Test MSP** → **MT524 - Phong tỏa chứng khoán**.

Mặc định preview phải có:

```text
{1:F01VSDC002XAXXX0000000000}{2:I524VSDC404XXXXXN}{4:
```

Nếu nhập ô **Ngân hàng nhận / Block 2** là `403`, preview phải đổi thành:

```text
{2:I524VSDC403XXXXXN}
```

## 5. Kiểm tra ACK/NAK simulator

Vào **Mode Simulator** → ManualMode → ACK/NAK kỹ thuật F21.

ACK phải có:

```text
{451:0}
```

NAK phải có:

```text
{451:1}{405:[T44] VSDCODE not found or deactived[T83] User: not registered}
```

Phần quote lại điện gốc phải có receiver VSDC gateway:

```text
{2:I524VSDCSVN6XXXXN}
```
