# TASK-25I

---
id: TASK-25I
title: Phase 25I - Research script 409 runtime shape audit
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25I-25L - Research QA script open-order runtime handling fix"
priority: high
---

## Goal

Audit why `scripts/dev-qa-create-research-order.ps1` still throws on the known open-order `409` response.

## Implementation requirements

1. Inspect `scripts/dev-qa-create-research-order.ps1`.
2. Inspect `scripts/dev-qa-common.ps1`.
3. Inspect how the PowerShell HTTP exception path currently exposes:
   - `StatusCode`
   - response body text
   - parsed JSON errors
4. Compare the implemented helper check against the real runtime body:
   `{"succeeded":false,"orderId":null,"startsAtUtc":null,"endsAtUtc":null,"errors":["Civilization already has an open research order."]}`
5. Capture the exact root cause and reflect it in the persisted-flow doc if the runtime contract note needs clarification.

## Files to read first

- ai/architecture-index.md
- scripts/dev-qa-create-research-order.ps1
- scripts/dev-qa-common.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- scripts/dev-qa-create-research-order.ps1
- scripts/dev-qa-common.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- The runtime failure path is understood precisely.
- The audit identifies why the known `409` is not treated as a controlled no-op.
- Validation commands pass.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```
