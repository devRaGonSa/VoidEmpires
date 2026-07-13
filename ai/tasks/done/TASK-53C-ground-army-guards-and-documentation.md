# TASK-53C

---
id: TASK-53C
title: Ground Army productization guards and documentation
status: done
type: platform
team: platform
supporting_teams: [frontend, gameplay]
roadmap_item: "Block 53"
priority: high
---

## Goal
Lock in the Ground Army productization boundary with focused guards, current-state documentation, and full Block 53 validation.

## Context
Block 53 must remain free of readiness/prototype copy, defense queue leakage, and non-production action layouts while documenting the new playable module.

## Implementation steps

1. Inspect existing Block guard scripts and current-state conventions.
2. Add or update focused guards for Ground Army copy, queue separation, and production action rows.
3. Update current-state documentation and run all requested Block 53 validations.

## Files to read first

- `ai/current-state.md`
- `scripts/check-frontend-copy-regressions.ps1`
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`

## Expected files to modify

- `ai/current-state.md`
- `scripts/check-frontend-copy-regressions.ps1`

## Acceptance criteria

- Focused guard scripts fail on regressions in copy, queue isolation, or action-row use.
- Current-state documentation accurately records Block 53 behavior and validation.
- All requested .NET, frontend, and Block 53 commands pass.
- Pending tasks contain only `.gitkeep` after completion.

## Constraints

- Do not claim browser or manual QA.
- Do not modify unrelated product areas.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build`
- All four Block 53 validation scripts supplied by the task prompt

## Commit and push

Commit this task separately, then push the validated Block 53 branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
