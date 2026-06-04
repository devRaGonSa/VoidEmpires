# TASK-25H

---
id: TASK-25H
title: Phase 25H - Final create script runtime hardening closure
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

Close the corrective block after the create-script alignment work is processed and validated.

## Implementation requirements

1. Move `TASK-25A` through `TASK-25H` to `ai/tasks/done`.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
4. Ensure the working tree is clean.
5. Push final state.

## Acceptance criteria

- `dev-qa-create-construction-order.ps1` no longer sends invalid `$.action`.
- `dev-qa-create-research-order.ps1` handles open research queue state gracefully.
- Docs are updated.
- No visual QA is required.
- No production behavior is introduced.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```
