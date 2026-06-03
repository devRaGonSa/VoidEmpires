# TASK-14S

---
id: TASK-14S
title: Phase 14S - Planet construction smoke and regression checklist
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Add or update smoke QA documentation for Planet and Construction.

## Context
The new Planet and Construction gameplay readability work needs a deterministic manual QA checklist so later tasks can validate the exact seeded scenario and avoid regressing the primary flow.

## Implementation steps

1. Create or update the Planet cockpit checklist.
2. Create or update a Construction cockpit checklist.
3. Update the frontend smoke checklist if needed.
4. Include the exact seeded civilization and planet ids used for QA.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`, if present
- `docs/dev/strategic-map-cockpit-checklist.md`
- `README.md`

## Expected files to modify

- `docs/dev/planet-cockpit-checklist.md`, create or update
- `docs/dev/construction-cockpit-checklist.md`, create or update
- `docs/dev/frontend-foundation-smoke-checklist.md`, if needed

## Acceptance criteria

- QA docs cover `/planet` and `/construction`.
- The seeded civilization and Aurelia planet ids are documented.
- The checklist includes names, categories, availability, confirmation, queue refresh, and collapsed diagnostics.
- `/galaxy` and `/fleets` are only mentioned where necessary.

## Constraints

- Keep the docs deterministic and concise.
- Do not add screenshots or secrets.
- Keep the manual QA focused on the current block.

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
