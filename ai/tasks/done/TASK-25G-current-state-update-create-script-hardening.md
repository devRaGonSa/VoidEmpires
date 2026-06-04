# TASK-25G

---
id: TASK-25G
title: Phase 25G - Current state update create script hardening
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 25A-25H - Persisted QA create scripts command payload alignment"
priority: medium
---

## Goal

Update `ai/current-state.md` after fixing the runtime create scripts.

## Implementation requirements

1. Record that the baseline script was already fixed and the create scripts are now aligned with backend command contracts.
2. Record the current test count.
3. Record that Research open-order `409` is handled as expected reused dev state.
4. Preserve the existing Phase 24P state and no-production or no-destructive-reset warnings.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```
