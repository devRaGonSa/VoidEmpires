# TASK-25F

---
id: TASK-25F
title: Phase 25F - Docs update for create script runtime states
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 25A-25H - Persisted QA create scripts command payload alignment"
priority: medium
---

## Goal

Update persisted QA docs with corrected Construction payload behavior and Research open-order behavior.

## Implementation requirements

1. Update `docs/dev/persisted-gameplay-flow-checklist.md`.
2. Include:
   - Construction script expected success output
   - Construction payload or action note
   - Research script expected success output
   - Research `already has open order` expected output
   - what to do if the queue is occupied
   - explicit note that scripts do not delete or complete orders automatically
3. Keep commands copy-pasteable.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```
