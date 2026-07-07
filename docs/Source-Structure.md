# Cấu trúc source

```text
src/Acbs.Vsdc.TestHub/
├─ Areas/MSP/                 Form và controller riêng từng nghiệp vụ MSP
├─ Bootstrap/                 Configuration, DI, pipeline, initialization
├─ Config/                    IP/port, SQL Server, gateway folder, MSP settings
├─ Controllers/               Dashboard, message, log, simulator, API
├─ Data/                      DbContext, model configuration, seed catalog
├─ Domain/                    Mỗi entity một file, chia theo domain
├─ Modules/MSP/
│  ├─ Builders/               Builder theo MT524/548/199/541/543/599
│  ├─ Catalog/                Message type, tag, qualifier, status, reason, NAK
│  ├─ Encoding/               Chuyển đổi tiếng Việt ↔ SWIFT Latin
│  ├─ Models/                 Request model theo từng điện
│  ├─ Parsing/                Classifier và narrative parser
│  ├─ Persistence/            Bóc tách vào bảng chuyên biệt MSP
│  ├─ Reports/                CSV/PAR reconciliation
│  ├─ Simulator/              ACK/NAK và auto response coordinator
│  └─ Validation/             Kiểm tra cấu trúc và tag bắt buộc
├─ Options/                   Strongly typed config + validators
├─ Services/
│  ├─ Files/                  Atomic write, ingestion, outgoing
│  ├─ Fin/                    Block scanner, parser, tag parser
│  ├─ Simulator/              Auto/Manual simulator orchestration
│  └─ Workers/                Receive watcher và Send simulator worker
├─ Views/                     Quản lý điện, logs, simulator
└─ wwwroot/                   CSS/JS
```

## Quy tắc bắt buộc

- `Program.cs` chỉ gọi bootstrap extension methods.
- Không để UNC path, IP, port, BIC hoặc connection string trong `.cs`.
- Một entity một file.
- Một loại điện/request/builder một file hoặc folder riêng.
- Parser không chứa nghiệp vụ ghi file; worker không tự dựng FIN.
- Luôn giữ raw content để audit và regression.
