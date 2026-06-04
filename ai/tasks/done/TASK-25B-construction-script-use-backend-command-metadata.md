# TASK-25B

---
id: TASK-25B
title: Phase 25B - Construction script use backend command metadata
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

Fix `dev-qa-create-construction-order.ps1` so it builds the enqueue request using backend-compatible fields.

## Purpose

The Construction helper must stop inferring payload values from display-oriented fields and instead use backend-compatible command data or exact enum names.

## Current problem

The script currently sends an invalid `$.action`, causing the endpoint to reject the JSON before application validation even runs.

## Implementation requirements

1. Prefer backend-provided command metadata if present in the Construction or Planet UI state.
2. If command metadata is absent, map from the real backend enum value, not from Spanish display text.
3. Ensure the payload contains the exact shape required by `EnqueueConstructionOrderApiRequest`.
4. Print the selected action and payload summary before the POST.
5. Do not print secrets.
6. Do not create multiple orders.
7. If no safe available action exists, exit with a controlled message.

## Acceptance criteria

- The script no longer sends invalid `$.action`.
- `scripts/check-dev-qa-scripts.ps1` passes.
- The script is suitable for runtime testing against the backend.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```
