# TASK-12Z

---
id: TASK-12Z
title: Phase 12Z - Fleet primary label ID cleanup
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12Y-13D"
priority: medium
---

## Goal
Remove technical IDs from primary gameplay labels while preserving them as secondary development metadata.

## Context
Several Fleet labels still expose raw IDs too prominently. The screen should read like a game UI first, while still keeping identifiers available in technical or development details when needed.

## Implementation steps

1. Inspect the Fleet UI places where IDs are shown in primary labels or selectors.
2. Move IDs out of the main label and into secondary metadata or collapsed details.
3. Keep select options readable and game-like while preserving enough development disambiguation.
4. Avoid changing backend DTOs unless a tiny frontend mapping helper is clearly the better option.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet label and selector helpers

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet label mapping helpers, if needed

## Acceptance criteria

- Primary labels prefer clean planet and squad names.
- IDs appear only as secondary or development metadata.
- Select labels remain readable and useful for development.
- Full technical details remain available in collapsed/debug areas.
- Backend DTOs are unchanged unless absolutely necessary.
- Split and merge remain disabled or prototype-only.

## Constraints

- Do not remove IDs from development or debug detail views.
- Do not add backend endpoints.
- Do not enable split or merge execution.
- Keep the change minimal.

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
- Split the work into additional tasks if limits are exceeded.
