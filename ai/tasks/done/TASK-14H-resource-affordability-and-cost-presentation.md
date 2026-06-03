# TASK-14H

---
id: TASK-14H
title: Phase 14H - Resource affordability and cost presentation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Improve how resource costs and affordability are presented in Planet and Construction.

## Context
The UI already knows whether actions are affordable, but the player needs a compact readable presentation of current resources, missing resources, and construction costs. This task should keep backend validation authoritative and make the UI easier to understand.

## Implementation steps

1. Review the current resource display and affordability state in Planet.
2. Show available local resources clearly and present costs compactly.
3. Compute missing amounts in the frontend view model if the backend does not provide them directly.
4. Normalize resource names into Spanish gameplay labels.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Application/Economy/`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`

## Acceptance criteria

- Local resources are readable.
- Construction costs are compact and readable.
- Missing resources are shown in Spanish when insufficient.
- Raw resource enum names do not dominate the primary UI.

## Constraints

- Do not mutate resources from read-only views.
- Keep backend validation as the source of truth.
- Do not invent missing production values.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- if backend affordability DTOs change: `dotnet build --no-restore` and `dotnet test --no-build`

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
