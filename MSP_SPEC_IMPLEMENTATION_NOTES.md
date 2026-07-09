# MSP SPEC IMPLEMENTATION NOTES - v1.5.2

## 1. Nguyên tắc áp dụng

Bộ điện mẫu chỉ dùng để tham khảo format và luồng nghiệp vụ. Khi build điện cho ACBS, header và routing phải theo profile ACBS/VSDC hiện tại.

## 2. ACBS MSP header gửi đi

Theo yêu cầu VSDC mới, điện gửi đi từ ACBS dùng:

```text
{1:F01VSDC002XAXXX0000000000}
{2:I<MT>VSDC404XXXXXN}
```

Trong đó `002` là mã TVLK của ACBS. `VSDC404X` là ngân hàng lưu ký nhận mặc định Citibank. Tester có thể nhập `VSDC403X` hoặc mã ngắn `403` nếu muốn gửi sang Deutsche Bank.

## 3. Normalize ngân hàng nhận

Các màn hình tạo điện MSP có ô **Ngân hàng nhận / Block 2**. Hệ thống normalize như sau:

| Giá trị nhập | Giá trị FIN |
|---|---|
| `404` | `VSDC404X` |
| `403` | `VSDC403X` |
| `VSDC404X` | `VSDC404X` |
| `VSDC403X` | `VSDC403X` |

## 4. ACK/NAK kỹ thuật

ACK từ VSDC có field `451=0`:

```text
{1:F21VSDC002XAXXX0000000000}{4:{177:yyyyMMdd HH:mm:ss}{451:0}}
```

NAK từ VSDC có field `451=1` và field `405` chứa lỗi:

```text
{1:F21VSDC002XAXXX0000000000}{4:{177:yyyyMMdd HH:mm:ss}{451:1}{405:[T44] VSDCODE not found or deactived[T83] User: not registered}}
```

Phần quote lại điện gửi đi bên sau ACK/NAK phải dùng VSDC gateway BIC ở Block 2:

```text
{1:F01VSDC002XAXXX0000000000}{2:I524VSDCSVN6XXXXN}{4:
...
-}{5:{CHK:123456789ABC}}
```

## 5. MT524 phong tỏa / giải tỏa chứng khoán

### 5.1 Phong tỏa chứng khoán mới

Theo yêu cầu mới của VSDC, điện phong tỏa chứng khoán mới **không sinh LINK/PREV**.

```text
:16R:GENL
:20C::SEME//<reference>
:23G:NEWM
:98A::PREP//<yyyyMMdd>
:16S:GENL
:16R:INPOSDET
:97A::SAFE//<account>
:35B:ISIN <ISIN>
:36B::SETT//UNIT/<quantity>
:98A::SETT//<yyyyMMdd>
:93A::FROM//AVAI
:93A::TOBA//BLOK
:16S:INPOSDET
```

### 5.2 Hủy yêu cầu phong tỏa chứng khoán

Điện hủy phong tỏa chứng khoán dùng `:23G:CANC` và có LINK/PREV tham chiếu đến mã điện phong tỏa gốc.

```text
:16R:GENL
:20C::SEME//<reference hủy>
:23G:CANC
:98A::PREP//<yyyyMMdd>
:16R:LINK
:20C::PREV//<reference điện phong tỏa gốc>
:16S:LINK
:16S:GENL
:16R:INPOSDET
:93A::FROM//AVAI
:93A::TOBA//BLOK
:16S:INPOSDET
```

### 5.3 Giải tỏa chứng khoán mới

Điện giải tỏa chứng khoán dùng `:23G:NEWM` và bắt buộc có LINK/PREV tham chiếu đến mã điện phong tỏa trước đó.

```text
:16R:GENL
:20C::SEME//<reference giải tỏa>
:23G:NEWM
:98A::PREP//<yyyyMMdd>
:16R:LINK
:20C::PREV//<reference điện phong tỏa trước đó>
:16S:LINK
:16S:GENL
:16R:INPOSDET
:93A::FROM//BLOK
:93A::TOBA//AVAI
:16S:INPOSDET
```

### 5.4 Hủy yêu cầu giải tỏa chứng khoán

Điện hủy giải tỏa chứng khoán dùng `:23G:CANC` và có LINK/PREV tham chiếu đến mã điện giải tỏa vừa gửi.

```text
:16R:GENL
:20C::SEME//<reference hủy giải tỏa>
:23G:CANC
:98A::PREP//<yyyyMMdd>
:16R:LINK
:20C::PREV//<reference điện giải tỏa vừa gửi>
:16S:LINK
:16S:GENL
:16R:INPOSDET
:93A::FROM//BLOK
:93A::TOBA//AVAI
:16S:INPOSDET
```

## 6. MT199 tiền, MT541/MT543 thanh toán, MT599 tra soát/đối chiếu

Các nghiệp vụ này giữ nguyên mapping business tag của v1.4.2, chỉ đổi header/routing theo v1.5.2:

- Block 1 sender: `VSDC002XAXXX0000000000`.
- Block 2 receiver: ngân hàng nhận do tester nhập, mặc định `VSDC404X`.
- ACK/NAK quote receiver: `VSDCSVN6XXXX`.
