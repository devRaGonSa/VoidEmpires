# TASK-12B

---
id: TASK-12B
title: Phase 12B - Fleet cockpit component extraction
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12B"
priority: medium
---

## Goal
Split the Fleet cockpit page into smaller focused frontend components so future cockpit work stays within the repository AI change budget.

## Context
`src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx` now owns the cockpit layout, selected-group detail, guarded command execution, and prototype manifest presentation. The layout task completed the intended UI restructuring, but the page-level line churn exceeded the preferred budget for a single task. This follow-up should reduce future risk by extracting stable cockpit sections into nearby components without changing behavior.

## Implementation steps

1. Inspect the current Fleet cockpit page, `FleetSummaryPanel`, and related shared UI patterns.
2. Extract one or two stable sections from `FleetsPage.tsx` into focused components such as selected-group detail or command/result panels.
3. Preserve all existing command guards and UI wording while reducing page-level complexity.
4. Re-run the frontend build and repository validation commands after the extraction.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetCommandExecutionPanel.tsx

## Acceptance criteria

- `FleetsPage.tsx` is reduced by extracting stable cockpit sections into focused components.
- Existing command behavior remains unchanged: estimate stays read-only, create requires explicit confirmation, cancel requires explicit confirmation, and prototype-only controls stay disabled.
- The extracted components follow the existing frontend styling and naming conventions.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-only.
- Do not add new backend routes, data contracts, or mutation capabilities.
- Do not modify unrelated files.
- Keep the extraction small enough to fit within the repository AI change budget.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work again if the extraction exceeds these limits.
