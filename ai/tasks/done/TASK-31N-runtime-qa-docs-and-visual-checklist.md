# TASK-31N

---
id: TASK-31N
title: Runtime QA docs and visual checklist
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Document the runtime manual QA sequence for the full orbital production or military preparation phase without overclaiming visual validation.

## Context
This block spans multiple cockpits and adds a new Development-only QA preparation flow. The repository needs one clear manual QA checklist that explains the runtime order, reused-database warnings, cockpit sequence, and which outcomes remain user-driven visual confirmation rather than already performed acceptance.

## Implementation steps

1. Update `docs/dev/frontend-foundation-smoke-checklist.md` with the exact runtime sequence for this block.
2. Include backend start, seed apply, orbital production QA preparation, frontend start, and the cockpit-by-cockpit verification path through Shipyard, Defenses, Fleets, and Planet.
3. Document reused Development database warnings and the role of the preparation helper script.
4. Keep visual QA clearly marked as user-driven rather than already completed.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md
- docs/dev/fleets-cockpit-checklist.md
- docs/dev/planet-cockpit-checklist.md

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md
- docs/dev/fleets-cockpit-checklist.md
- docs/dev/planet-cockpit-checklist.md

## Acceptance criteria

- The runtime QA checklist is actionable and ordered.
- The checklist includes the new orbital production QA preparation command and reused-database warnings.
- The docs do not claim that visual QA has already been performed.
- `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend` pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not overclaim manual or visual validation

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- no new warnings or obvious regressions are introduced

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
