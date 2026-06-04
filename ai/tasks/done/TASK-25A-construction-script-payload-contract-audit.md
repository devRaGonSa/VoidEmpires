# TASK-25A

---
id: TASK-25A
title: Phase 25A - Construction script payload contract audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25A-25H - Persisted QA create scripts command payload alignment"
priority: high
---

## Goal

Audit the exact payload currently sent by `scripts/dev-qa-create-construction-order.ps1` and compare it to `EnqueueConstructionOrderApiRequest`.

## Purpose

The Construction QA helper currently fails with a JSON conversion error at `$.action`. The repo needs a precise contract audit before changing the script so the fix is grounded in the real backend request shape.

## Current problem

The script is sending an invalid `action` value. It may be using a display label, translated label, normalized view-model string, numeric value, or some other non-backend field instead of the enum value expected by the enqueue endpoint.

## Files to read first

- `scripts/dev-qa-create-construction-order.ps1`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application/Buildings/EnqueueConstructionOrderRequest.cs`
- Construction UI/read model fields consumed by the script
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Implementation requirements

1. Inspect the script’s current enqueue body.
2. Inspect the backend request type `EnqueueConstructionOrderApiRequest`.
3. Inspect the endpoint `/api/dev/buildings/construction-orders/enqueue`.
4. Inspect the Construction UI or read-model fields the script uses to choose the action.
5. Identify whether the script is using:
   - a display label
   - a translated label
   - a normalized view-model action
   - a backend enum name
   - a numeric enum value
   - command metadata
6. Document the real expected request body in `docs/dev/persisted-gameplay-flow-checklist.md`.
7. Do not change behavior yet unless a safe one-line correction is obvious as part of the audit.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```
