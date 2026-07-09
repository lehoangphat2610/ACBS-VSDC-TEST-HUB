# Carbon AI Code Handoff

## Goal

Implement Carbon as a new independent module inside ACBS VSDC Test Hub.

## Existing stable module

MSP is stable and must not be modified.

## Carbon source documents

- docs/carbon/source/VSDC-TVLK-Dac ta ket noi STP_Carbon_V0.6.docx
- samples/carbon/raw

## Implementation phases

### Phase 1 - Carbon Foundation

Implement only the Carbon foundation first:

- Add Carbon menu/tab.
- Add Carbon route/page.
- Add Carbon parser skeleton.
- Add Carbon builder skeleton.
- Add Carbon validator skeleton.
- Add Carbon message catalog.
- Add generic FIN block parser.
- Add raw FIN parse/validate UI.
- Build must pass.
- MSP must still work.

Do not implement all business templates in Phase 1.

### Phase 2 - Account + Depository

Implement using samples/spec:

- 4200_ Mở tài TK
- 4204 Đóng tài khoản
- Lưu ký
- Rút ký gửi

### Phase 3 - Business Notifications

Implement using samples/spec:

- 3341_ Thông báo hạn ngạch
- 3346_ Thông báo tín chỉ
- 4292_ Điều chỉnh loại hình tài khoản
- TVLK xác nhận phân bổ tiền, hạn ngạch tín chỉ

### Phase 4 - Payment

Implement using samples/spec:

- Thanh toán
- Thanh toán giao dịch hạn ngạch, tín chỉ các-bon
- Loại bỏ thanh toán
- Rút thanh toán giao dịch hạn ngạch, tín chỉ các-bon

### Phase 5 - Regression + Hardening

- MSP smoke test.
- Carbon sample round-trip tests.
- Validation error tests.
- UI smoke test.

## Strict rules for AI-Code

- Do not edit MSP module.
- Do not rename MSP files.
- Do not refactor MSP files.
- Do not convert MSP templates into Carbon templates.
- Do not call MSP parser/builder/validator from Carbon.
- Do not guess missing Carbon tags.
- If a tag/field is not found in spec or sample, mark TODO instead of inventing it.
- If Shared/Core must be changed, explain why and test MSP again.

## Required report after each phase

AI-Code must report:

1. Files added/changed.
2. Whether any MSP file was changed.
3. Shared/Core files changed, if any.
4. Build result.
5. Test result.
6. Carbon samples covered.
7. Remaining TODO.