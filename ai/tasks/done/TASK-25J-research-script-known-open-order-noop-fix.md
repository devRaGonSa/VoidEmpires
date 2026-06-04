# TASK-25J

---
id: TASK-25J
title: Phase 25J - Research script known open-order no-op fix
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 25I-25L - Research QA script open-order runtime handling fix"
priority: high
---

## Goal

Make `scripts/dev-qa-create-research-order.ps1` treat the known open-order `409` as an expected Development no-op state.

## Implementation requirements

1. Update `scripts/dev-qa-create-research-order.ps1`.
2. When enqueue returns `HTTP 409` and the response body contains `Civilization already has an open research order`:
   - do not throw a fatal exception
   - print `Ya existe una investigacion abierta para esta civilizacion. No se crea otra orden.`
   - fetch Research UI state again
   - print queue count and current order summary when available
   - exit cleanly with code `0`
3. Keep fatal behavior for unknown `409` responses or unexpected failures.
4. Do not cancel, complete, or delete existing research.
5. Do not create another order.

## Files to read first

- scripts/dev-qa-create-research-order.ps1
- scripts/dev-qa-common.ps1
- scripts/check-dev-qa-scripts.ps1
- src/VoidEmpires.Web/DevEndpointMappings.cs

## Expected files to modify

- scripts/dev-qa-create-research-order.ps1
- scripts/dev-qa-common.ps1
- scripts/check-dev-qa-scripts.ps1

## Acceptance criteria

- The known open-order `409` exits as a controlled no-op.
- Unknown failures still surface clearly.
- Validation commands pass.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```
