# Kiến trúc

```text
                         MODE TEST VỚI VSDC
Tester -> Web MVC -> FinMessageBuilder -> \\CSD\Send -> VSD Gateway -> VSDC
                                                        |
VSDC -> VSD Gateway -> \\CSD\Receive -> ReceiveFolderWorker -> Parser -> SQL Server
                                              |                         |
                                              +-----------------> Web tra cứu

                         MODE SIMULATOR NỘI BỘ
Core/App -> \\CSD\Send -> SimulatorSendWorker -> Response FIN -> \\CSD\Receive
                                |                         |
                                +-> move source ----------+-> \\CSD\Archive
                                +-> error -------------------> \\CSD\Error
ManualMode -> FIN hoặc PAR+CSV ----------------------------> \\CSD\Receive
```

## Nguyên tắc xử lý file

1. Chỉ nhận `.fin`, `.par`, `.csv`.
2. File phải ổn định về kích thước và thời gian sửa trước khi đọc.
3. Đọc bằng event realtime và có reconciliation scan để bù event bị mất trên SMB.
4. Ghi file mới qua file tạm rồi rename/move sang tên chính thức.
5. Dùng SHA-256 để nhận biết file đã lưu.
6. Lưu raw content song song dữ liệu đã chuẩn hóa.
7. Không sửa nội dung file nguồn khi chuyển `Send -> Archive`.

## Nhóm bảng

- Raw/transport: GatewayFiles, GatewayMessages, MessageHeaders, MessageBlocks, MessageTags, MessageReferences.
- Customer/account: Customers, CustomerIdentities, CustomerAddresses, CustomerContacts, Accounts, AccountMappings, AccountChanges.
- Securities/business: Securities, Orders, Trades, CashTransactions, SecuritiesTransfers, RightsRegistrations, CorporateActions, Fees, Taxes, NavRecords.
- Reports: ReportFiles, ReportRows.
- Operations/audit: ProcessingHistories, ValidationErrors, OutboxJobs, InboxJobs, SystemLogs, MessageStatusHistories, FileCheckpoints.
- Simulator/config: SimulatorRules, SimulatorRuns, ManualTemplates, SystemSettings.
