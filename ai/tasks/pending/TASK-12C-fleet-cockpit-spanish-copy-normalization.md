# TASK-12C

---
id: TASK-12C
title: Phase 12C - Fleet cockpit Spanish copy normalization
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12C"
priority: medium
---

## Goal
Normalize the Fleet cockpit UI copy to Spanish so the screen reads like a coherent playable fleet view instead of a mixed debug prototype.

## Context
The Fleet cockpit milestone is already implemented and visually accepted in broad terms, but manual review found heavy English and Spanish mixing across labels, helper text, statuses, and action areas. This task should make the Fleet screen feel more like a cohesive game interface while preserving existing safe prototype boundaries and backend contracts.

## Implementation steps

1. Inspect the Fleet page, summary panel, selected-group panel, presentation helpers, and any related Fleet labels or types that feed visible cockpit copy.
2. Replace visible English copy with concise Spanish equivalents across headers, cards, status areas, command sections, guardrail text, and helper labels.
3. Keep technical route names or action keys unchanged when they are identifiers, but demote them from primary user-facing phrasing.
4. Favor short game-like Spanish wording over formal explanatory text so the Fleet screen reads naturally and consistently.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- The Fleet cockpit visible copy is mostly Spanish and reads coherently across headers, action areas, statuses, and helper text.
- English phrases such as command, group, selected group, current planet, quantity, command state, and transfer support are replaced with concise Spanish user-facing labels where appropriate.
- Technical identifiers remain technically correct but are no longer presented as the main user-facing copy.
- No backend contracts, new commands, or extra executable mutation controls are introduced.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused.
- Do not change backend contracts, add new commands, or enable complete, split, or merge execution.
- Keep the copy short, readable, and game-like rather than verbose or overly formal.
- Split follow-up work if copy cleanup spreads beyond the Fleet cockpit scope.

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
- Prefer a single commit for this task.
- If copy normalization grows into broader UI restructuring, stop and create a focused follow-up task.
