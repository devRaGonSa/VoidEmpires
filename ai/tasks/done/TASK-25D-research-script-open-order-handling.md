# TASK-25D

---
id: TASK-25D
title: Phase 25D - Research script open order handling
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 25A-25H - Persisted QA create scripts command payload alignment"
priority: high
---

## Goal

Treat `Civilization already has an open research order` as an expected reused-database state in Development QA.

## Purpose

The Research helper should behave calmly when a reused dev database already contains an open research order, because that is a legitimate state under the documented persisted QA flow.

## Implementation requirements

1. Update `scripts/dev-qa-create-research-order.ps1`.
2. If the POST returns `409` and the response contains `already has an open research order`:
   - do not show a raw fatal stack trace
   - print the controlled message:
     `Ya existe una investigacion abierta para esta civilizacion. No se crea otra orden.`
   - fetch Research UI state again
   - print queue count and current order summary
3. Decide whether the script should exit `0` or non-zero:
   - prefer `0` with a no-op expected-state explanation for reused dev DB safety
   - document the decision
4. Do not complete or cancel existing research automatically.
5. Do not delete existing orders.

## Acceptance criteria

- The Research script handles open-queue state gracefully.
- The expected reused-DB state is documented.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```
