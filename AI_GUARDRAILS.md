# AI Guardrails - ACBS VSDC Test Hub

## Stable module

The existing MSP module is stable and must be preserved.

## Domain separation

MSP and Carbon are different business domains.

They only share the Test Hub shell:
- Layout
- Menu
- Authentication
- Logging
- Database connection
- SMB/file watcher infrastructure
- Common UI components

They must not share business logic:
- Parser
- Builder
- Validator
- Templates
- Models
- Field mapping
- Tag mapping
- Message form logic

## Forbidden actions when implementing Carbon

- Do not delete MSP files.
- Do not rename MSP files.
- Do not refactor MSP business logic.
- Do not convert MSP templates into Carbon templates.
- Do not reuse MSP parser/builder/validator for Carbon.
- Do not change existing MSP message output.
- Do not remove existing MSP menus/pages.
- Do not transform MSP into Carbon.

## Required Carbon architecture

Carbon must be added as a new independent module.

Suggested structure:

- Modules/Carbon
- Pages/Carbon or Areas/Carbon
- CarbonParser
- CarbonMessageBuilder
- CarbonValidator
- CarbonTemplates
- CarbonModels

## Required report after every change

After every code change, report:

1. Files changed in Carbon.
2. Files changed in Shared/Core.
3. Whether any MSP file was changed.
4. MSP regression risk.
5. Carbon test result.
6. MSP smoke test result.