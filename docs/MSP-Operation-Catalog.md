# Danh mục nghiệp vụ MSP đã tích hợp

| Nghiệp vụ | Điện | Builder | AutoMode |
|---|---:|---|---|
| Phong tỏa chứng khoán | MT524 | riêng | ACK + MT548 PACK |
| Giải tỏa chứng khoán | MT524 | riêng | ACK + MT548 PACK |
| Xác nhận/Từ chối chứng khoán | MT548 | riêng | ManualMode |
| Phong tỏa tiền | MT199 | riêng | ACK + MT199 PACK |
| Giải tỏa tiền | MT199 | riêng | ACK + MT199 PACK |
| Xác nhận/Từ chối tiền | MT199 | riêng | ManualMode |
| Chỉ thị thanh toán mua | MT541 | riêng | ACK/NAK |
| Chỉ thị thanh toán bán | MT543 | riêng | ACK/NAK |
| Tra soát trạng thái | MT199/MT599 | riêng | ACK + Response |
| Báo cáo đối chiếu | MT599 + CSV + PAR | riêng | ACK + PACK + CSV/PAR |

Các mẫu thật được chép vào `samples/MSP/Actual` để so sánh regression.
