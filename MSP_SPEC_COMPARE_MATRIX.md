# MSP SPEC COMPARE MATRIX - v1.5.2

## Header routing

| Nội dung | Đặc tả/mẫu chung | ACBS v1.5.2 |
|---|---|---|
| Member BIC | `VSDC[member]X` | `VSDC002X` |
| Block 1 gửi đi | `{1:F01...}` | `{1:F01VSDC002XAXXX0000000000}` |
| Block 2 gửi đi | `{2:I<MT><receiver>XXXXN}` | `{2:I524VSDC404XXXXXN}` mặc định |
| Receiver bank | Theo NHLK | `404=Citibank`, `403=Deutsche Bank` |
| VSDC gateway BIC | `VSDCSVN6` | Dùng trong ACK/NAK quoted Block 2 |
| ACK Block 1 | F21 | `{1:F21VSDC002XAXXX0000000000}` |
| ACK status | `451=0` | `ACK` |
| NAK status | `451=1` + `405` | `NAK`, giữ nguyên nội dung lỗi VSDC |

## Ví dụ ACBS MT524 phong tỏa mới gửi đi

Phong tỏa mới không có `LINK/PREV`:

```text
{1:F01VSDC002XAXXX0000000000}{2:I524VSDC404XXXXXN}{4:
:16R:GENL
:20C::SEME//ACBS2606230001
:23G:NEWM
:98A::PREP//20260623
:16S:GENL
:16R:INPOSDET
:97A::SAFE//006C123456
:35B:ISIN VN000000HPG4
:36B::SETT//UNIT/10000
:98A::SETT//20260623
:93A::FROM//AVAI
:93A::TOBA//BLOK
:16S:INPOSDET
-}{5:{CHK:123456789ABC}}
```

## Ví dụ ACBS MT524 hủy phong tỏa / giải tỏa có LINK/PREV

```text
:16R:GENL
:20C::SEME//ACBS2606230002
:23G:CANC
:98A::PREP//20260623
:16R:LINK
:20C::PREV//ACBS2606230001
:16S:LINK
:16S:GENL
...
```

## Ví dụ ACK quote

```text
{1:F21VSDC002XAXXX0000000000}{4:{177:20260623 14:00:00}{451:0}}
{1:F01VSDC002XAXXX0000000000}{2:I524VSDCSVN6XXXXN}{4:
...
-}{5:{CHK:123456789ABC}}
```

## Ví dụ NAK quote

```text
{1:F21VSDC002XAXXX0000000000}{4:{177:20260623 14:00:00}{451:1}{405:[T44] VSDCODE not found or deactived[T83] User: not registered}}
{1:F01VSDC002XAXXX0000000000}{2:I524VSDCSVN6XXXXN}{4:
...
-}{5:{CHK:123456789ABC}}
```
