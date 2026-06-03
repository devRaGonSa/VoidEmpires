# TASK-14I

---
id: TASK-14I
title: Phase 14I - Construction confirmation modal gameplay polish
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Polish the construction confirmation flow so it is explicit, understandable, and safe.

## Context
Available construction actions should require a clear confirmation step that shows exactly what will happen before any mutation is sent. The confirmation must be Spanish-first and avoid exposing raw DTO or capability names in the primary text.

## Implementation steps

1. Review the current construction confirmation UI and flow.
2. Show planet, building, level, cost, and duration details in the confirmation.
3. Keep confirm and cancel actions explicit and safe.
4. Refresh Planet or Construction data after success and keep failure handling stable.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- confirmation modal or panel helpers, if needed

## Acceptance criteria

- Confirmation content is explicit and Spanish.
- Cancel does not mutate.
- Confirm calls only the safe dev construction endpoint.
- Success refreshes the relevant data.
- Failure keeps UI stable and readable.

## Constraints

- Confirmation is required before mutation.
- Do not expose raw backend names in primary text.
- Keep the flow safe and deterministic.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made
- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms one available construction can be prepared and confirmed safely

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
