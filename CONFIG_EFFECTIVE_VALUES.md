# CONFIG EFFECTIVE VALUES - v1.5.2

## Hosting

```text
Urls          = http://0.0.0.0:9898
PublicBaseUrl = http://localhost:9898
```

## Database local DEV/UAT

```text
Server   = PHATLH-LT-TD\SQLEXPRESS
Database = ACBS_VSDC_TESTHUB
User     = sa
Password = ACBS@01
Schema   = vsdc
```

## Database DCRSM UAT deploy

Khi chạy ngay trên server DCRSM:

```text
Server   = .\SQLEXPRESS
Database = ACBS_VSDC_TESTHUB
User     = sa
Password = ACBS@01
Schema   = vsdc
```

Hoặc khi máy khác kết nối về DCRSM:

```text
Server   = DCRSM-OPS-VUAT\SQLEXPRESS
Database = ACBS_VSDC_TESTHUB
User     = sa
Password = ACBS@01
Schema   = vsdc
```

## Gateway folders local

```text
Send    = D:\VSDC\CSD\Send
Receive = D:\VSDC\CSD\Receive
Archive = D:\VSDC\CSD\Archive
Error   = D:\VSDC\CSD\Error
```

## Gateway folders DCRSM UAT

```text
Send    = H:\Send
Receive = H:\Receive
Archive = H:\Archive
Error   = H:\Error
```

## MSP routing profile

```text
ACBS member BIC          = VSDC002X
VSDC gateway BIC         = VSDCSVN6
Default receiver bank    = VSDC404X (Citibank)
Alternative receiver     = VSDC403X (Deutsche Bank)
Session number           = 0000
Sequence number          = 000000
Input sender LT          = AXXX
Input receiver LT        = XXXX
Trailer CHK              = 123456789ABC
```

## Header gửi đi

```text
{1:F01VSDC002XAXXX0000000000}{2:I524VSDC404XXXXXN}
```

## ACK/NAK kỹ thuật

```text
ACK: {1:F21VSDC002XAXXX0000000000}{4:{177:yyyyMMdd HH:mm:ss}{451:0}}
NAK: {1:F21VSDC002XAXXX0000000000}{4:{177:yyyyMMdd HH:mm:ss}{451:1}{405:...}}
```

Phần quote lại điện gửi đi trong ACK/NAK:

```text
{1:F01VSDC002XAXXX0000000000}{2:I524VSDCSVN6XXXXN}
```
