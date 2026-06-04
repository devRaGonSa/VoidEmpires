# TASK-25K

---
id: TASK-25K
title: Phase 25K - Research script runtime doc update
status: done
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 25I-25L - Research QA script open-order runtime handling fix"
priority: medium
---

## Goal

Update the persisted-flow runbook with the actual expected runtime result for a reused Development database with an open research order.

## Implementation requirements

1. Update `docs/dev/persisted-gameplay-flow-checklist.md`.
2. Explain that:
   - if the research queue is already open, the helper reports it and exits cleanly
   - this is expected on a reused Development database
   - the script does not delete, complete, or cancel existing research orders
   - reapplying `cockpit-validation` does not remove manual or open orders
3. Keep commands copy-pasteable.

## Files to read first

- docs/dev/persisted-gameplay-flow-checklist.md
- scripts/dev-qa-create-research-order.ps1
- ai/current-state.md

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- ai/current-state.md

## Acceptance criteria

- The runbook matches the real supported runtime behavior.
- No destructive or misleading guidance remains.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```
