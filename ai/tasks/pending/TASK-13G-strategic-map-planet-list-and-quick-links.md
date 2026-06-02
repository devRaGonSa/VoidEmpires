# TASK-13G

---
id: TASK-13G
title: Phase 13G - Strategic map planet list and quick links
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13E-13J"
priority: medium
---

## Goal
Make planets within the selected system readable and provide clear navigation intent toward Planet and Fleet screens.

## Context
The selected system should expose a useful planet list that helps the user understand what exists in that system and where to go next. This task remains read-only and should not implement the Planet page itself.

## Implementation steps

1. Review the existing strategic map data used for planets in the selected system.
2. Add or improve a readable planet list with Spanish, game-like labels.
3. Add quick links or navigation intent toward `/planet` or `/fleets` where routing already supports it.
4. Keep any unavailable destinations clearly labeled as preparation or placeholder views.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers
- existing routing helpers for the frontend

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers, if needed
- frontend routing helpers, if needed

## Acceptance criteria

- The selected system has a readable planet list.
- Primary labels are Spanish and game-like.
- Navigation intent toward Planet and Fleet screens is clear.
- The task does not implement the Planet page.
- No backend endpoints are added.
- No mutations are added.

## Constraints

- Do not implement the Planet page.
- Do not add backend endpoints.
- Do not add 3D.
- Preserve read-only behavior.

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
