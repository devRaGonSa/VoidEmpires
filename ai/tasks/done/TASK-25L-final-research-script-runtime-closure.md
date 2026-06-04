# TASK-25L

---
id: TASK-25L
title: Phase 25L - Final research script runtime closure
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

Close the corrective block after the Research script runtime handling fix is validated.

## Implementation requirements

1. Move `TASK-25I` through `TASK-25L` to `ai/tasks/done`.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
4. Ensure the working tree is clean.
5. Push final state.

## Acceptance criteria

- The Research helper handles the known open-order runtime state as a controlled no-op.
- Docs and current-state notes are updated.
- No visual QA is required.
- No production behavior is introduced.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```
