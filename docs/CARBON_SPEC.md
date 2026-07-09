# Carbon Module Specification

## Purpose

Carbon is a new independent module in ACBS VSDC Test Hub.

Carbon must not reuse MSP business logic.

## Shared with Test Hub

Carbon may reuse:
- Web layout
- Menu shell
- Logging
- Database connection
- SMB/file watcher infrastructure
- Common UI components

## Not shared with MSP

Carbon must have its own:
- Parser
- Builder
- Validator
- Templates
- Models
- Field mapping
- Tag mapping
- Message input forms

## Carbon message types

TODO: Add Carbon message list here.

## Carbon sample messages

TODO: Add Carbon sample FIN/XML/PAR/CSV here.

## Carbon folder structure

Suggested:

- Modules/Carbon
- Pages/Carbon
- samples/carbon