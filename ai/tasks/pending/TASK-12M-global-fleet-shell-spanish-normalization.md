# TASK-12M

---
id: TASK-12M
title: Phase 12M - Global fleet shell Spanish normalization
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12M"
priority: medium
---

## Goal
Remove remaining visible English from the Fleet shell and adjacent Fleet navigation context so the page reads coherently in Spanish.

## Context
The Fleet cockpit is now usable, but a few English labels still leak through the shell, navigation context, and action manifest areas. This task should finish the language normalization so the Fleet screen reads as one coherent Spanish game view while keeping the safe prototype boundaries intact.

## Implementation steps

1. Inspect the Fleet page, summary panel, selected-group panel, any surrounding shell or sidebar component, and the fleet command presentation helpers.
2. Translate remaining visible English labels into concise Spanish game-style copy across the shell and adjacent Fleet navigation context.
3. Keep technical action keys or route identifiers unchanged when they serve as development metadata, but stop presenting them as primary user-facing labels.
4. Preserve all current command behavior and prototype-only restrictions, including the disabled state of complete-due, split, and merge.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Remaining visible English in the Fleet shell and adjacent navigation context is translated into concise Spanish.
- Labels such as `Empire surface`, `Command map`, `Only implemented read routes are enabled.`, `Estimate orbital travel`, `Create orbital transfer`, `Cancel orbital transfer`, `Split orbital group`, `Merge orbital groups`, `Read-only metadata`, `Read-only actions`, `Method`, and `Route` are normalized into Spanish user-facing copy where appropriate.
- Technical identifiers remain available but are not presented as the primary label layer.
- No backend contract changes, migrations, or additional executable commands are introduced.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and game-like rather than bureaucratic.
- Do not enable complete-due, split, or merge execution.
- Do not change backend contracts or add new routes.
- Split follow-up work if shell normalization grows beyond the change budget.

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
- If normalization needs broader layout rewrites, stop and create a follow-up task first.
