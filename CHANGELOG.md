# CHANGELOG

## v1.5.2 - 2026-06-26

### Điều chỉnh MT524 phong tỏa / giải tỏa chứng khoán theo yêu cầu mới của VSDC

#### 1. Phong tỏa chứng khoán mới

Điện MT524 phong tỏa chứng khoán mới vẫn dùng `:23G:NEWM`, nhưng **không còn sinh 3 dòng LINK/PREV**:

```text
:16R:LINK
:20C::PREV//ACBSXXXXXX
:16S:LINK
```

Cấu trúc GENL hiện tại:

```text
:16R:GENL
:20C::SEME//<reference>
:23G:NEWM
:98A::PREP//<yyyyMMdd>
:16S:GENL
```

#### 2. Hủy yêu cầu phong tỏa chứng khoán

Đã bổ sung module `MT524 - Hủy yêu cầu phong tỏa chứng khoán`. Điện hủy gần giống điện phong tỏa gốc nhưng:

- `:23G:CANC`
- Có block LINK/PREV tham chiếu đến mã điện phong tỏa gốc đã gửi trước đó.

```text
:16R:LINK
:20C::PREV//<mã điện phong tỏa gốc>
:16S:LINK
```

#### 3. Giải tỏa chứng khoán mới

Điện MT524 giải tỏa chứng khoán mới dùng `:23G:NEWM` và **bắt buộc có LINK/PREV** tham chiếu đến mã điện phong tỏa trước đó:

```text
:16R:LINK
:20C::PREV//<mã điện phong tỏa trước đó>
:16S:LINK
```

Phần số dư giữ đúng logic giải tỏa:

```text
:93A::FROM//BLOK
:93A::TOBA//AVAI
```

#### 4. Hủy yêu cầu giải tỏa chứng khoán

Đã bổ sung module `MT524 - Hủy yêu cầu giải tỏa chứng khoán`. Điện hủy gần giống điện giải tỏa vừa gửi nhưng:

- `:23G:CANC`
- `:20C::PREV//...` tham chiếu đến mã điện giải tỏa vừa gửi.

#### 5. UI và server-side builder đồng bộ

- Danh mục MSP hiện có 4 module MT524 chứng khoán: phong tỏa, hủy phong tỏa, giải tỏa, hủy giải tỏa.
- Form MT524 tự đổi label `Reference liên quan` theo từng nghiệp vụ.
- Phong tỏa mới disable trường tham chiếu liên quan vì không dùng LINK/PREV.
- Preview Raw FIN trong browser đã sửa cùng logic với file thực tế sinh bởi server-side builder.


## v1.5.1 - 2026-06-26

### Sửa lỗi ManualMode ACK/NAK và điện nhận về MSP

#### 1. ManualMode ACK/NAK không còn quote sai điện gốc

Trước đây ManualMode tạo ACK/NAK bằng một điện giả chỉ có tag `:20:`, nên file ACK tạo ra thiếu toàn bộ nội dung điện gửi đi.

Bản này đã sửa theo đúng pattern trong bộ điện mẫu MSP:

```text
{1:F21VSDC002XAXXX0000000000}{4:{177:...}{451:0}}{1:F01VSDC002XAXXX0000000000}{2:I524VSDCSVN6XXXXN}{4:
... nguyên nội dung điện gửi đi ...
-}{5:{CHK:123456789ABC}}
```

Khi nhấn ACK/NAK trong ManualMode, hệ thống sẽ tìm điện gốc theo `Reference` trong database `GatewayMessages` và trong các folder `Send`, `Archive`, `Error`. Nếu tìm thấy, ACK/NAK sẽ trích dẫn nguyên điện gốc, chỉ normalize Block 2 quote về `VSDCSVN6` theo yêu cầu VSDC.

#### 2. Sửa header điện nhận về nghiệp vụ

Đã sửa lại `BuildOutput()` cho các điện VSDC.MSP/NHLK trả về ACBS. Block 1 của output FIN giờ là bên nhận ACBS, còn Block 2 chứa MIR của ngân hàng lưu ký gửi điện.

Ví dụ điện nhận về ACBS từ Citibank:

```text
{1:F01VSDC002XAXXX0000000000}{2:O5241511010606VSDC404XAXXX03250130850105151149N}
```

#### 3. Sửa AutoMode business response

AutoMode khi nhận điện ACBS gửi đi sẽ lấy ngân hàng nhận từ Block 2 input, ví dụ `VSDC404X`, rồi dùng lại làm MIR sender trong điện response output.

#### 4. Sửa ManualMode business response

ManualMode tạo MT548, MT199, MT599 phản hồi sẽ dùng ngân hàng lưu ký mặc định `VSDC404X`, hoặc giá trị nhập trong ô `TK liên kết / đối ứng` nếu tester nhập `403`, `404`, `VSDC403X`, `VSDC404X`.

## v1.5.0 - 2026-06-26

### Thay đổi theo yêu cầu VSDC MSP mới

#### 1. Header FIN gửi đi ACBS -> VSDC.MSP

Đã đổi profile ACBS từ BIC cũ `VSDACBSX` sang chuẩn VSDC member BIC:

```text
{1:F01VSDC002XAXXX0000000000}
```

Trong đó:

- `VSDC002X`: mã TVLK của ACBS theo nguyên tắc `VSDC + 002 + X`.
- `AXXX`: logical terminal của bên gửi.
- `0000`: session number theo yêu cầu UAT mới.
- `000000`: sequence number theo yêu cầu UAT mới.

#### 2. Header Block 2 gửi đi

Đã đổi mặc định Block 2 sang ngân hàng lưu ký nhận là Citibank:

```text
{2:I524VSDC404XXXXXN}
```

Trong đó:

- `524`: message type.
- `VSDC404X`: điểm đến ngân hàng lưu ký Citibank.
- `XXXX`: logical terminal của bên nhận.
- `N`: priority normal.

Đã thêm ô nhập **Ngân hàng nhận / Block 2** trên các màn hình tạo điện MSP. Tester có thể nhập:

```text
VSDC404X
VSDC403X
404
403
```

Hệ thống sẽ tự normalize `404` thành `VSDC404X` và `403` thành `VSDC403X`.

#### 3. ACK/NAK nhận về từ VSDC

Đã sửa ACK/NAK kỹ thuật F21 theo yêu cầu:

ACK:

```text
{1:F21VSDC002XAXXX0000000000}{4:{177:yyyyMMdd HH:mm:ss}{451:0}}
```

NAK:

```text
{1:F21VSDC002XAXXX0000000000}{4:{177:yyyyMMdd HH:mm:ss}{451:1}{405:[T44] VSDCODE not found or deactived[T83] User: not registered}}
```

Phần trích dẫn lại điện gửi đi trong ACK/NAK đã đổi Block 2 về VSDC gateway BIC:

```text
{2:I524VSDCSVN6XXXXN}
```

Không còn trích dẫn lại `VSDC404X` trong ACK/NAK technical quote.

#### 4. Mapping reference ACK/NAK

Khi ingest ACK/NAK, cột **TT tham chiếu** sẽ ưu tiên map về số tham chiếu của điện gửi đi, không bị lấy nhầm `:20C::PREV` của nghiệp vụ MT524.

#### 5. UI tạo điện MSP

Đã bổ sung field **Ngân hàng nhận / Block 2** cho:

- MT524 phong tỏa / giải tỏa chứng khoán.
- MT199 phong tỏa / giải tỏa tiền.
- MT541/MT543 chỉ thị thanh toán mua/bán.
- MT199/MT599 tra soát trạng thái.
- MT599 yêu cầu báo cáo đối chiếu.

Preview raw FIN trên UI cũng đã đổi theo header mới:

```text
{1:F01VSDC002XAXXX0000000000}{2:I524VSDC404XXXXXN}{4:
...
-}{5:{CHK:123456789ABC}}
```

#### 6. Simulator NAK default

ManualMode/AutoMode NAK mặc định đã chuyển sang format có thể chứa nhiều mã lỗi trong field `405`:

```text
[T44] VSDCODE not found or deactived[T83] User: not registered
```

### Không thay đổi

Giữ nguyên các phần đã ổn của v1.4.2:

```text
Port      : 9898
Database  : ACBS_VSDC_TESTHUB
Local DB  : PHATLH-LT-TD\SQLEXPRESS
DCRSM DB  : .\SQLEXPRESS hoặc DCRSM-OPS-VUAT\SQLEXPRESS
Local Send/Receive/Archive/Error : D:\VSDC\CSD\...
DCRSM Send/Receive/Archive/Error : H:\Send, H:\Receive, H:\Archive, H:\Error
User admin mặc định: admin / ACBS@01
```

## v1.4.2

- Thêm nút xóa user.
- Chỉ bảo vệ user `admin`; các user khác có thể xóa.
- Không thay đổi database schema.

## v1.4.1

- Bổ sung publish script cho DCRSM UAT.
- Sửa lỗi deploy thiếu static files khiến giao diện bị bể CSS.

## v1.4.0

- Sửa trạng thái điện gửi/nhận: SENDING/SENDED/ACK/NAK/PACK/REJT/RECEIVED.
- Thêm tab Xử lý điện.
- Logs có lọc từ ngày đến ngày.
- CSV/PAR được hiển thị trong Quản lý điện.
