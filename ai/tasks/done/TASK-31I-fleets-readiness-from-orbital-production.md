# TASK-31I

---
id: TASK-31I
title: Fleets readiness from orbital production
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Improve the Fleets cockpit so it reflects backend-backed orbital production readiness and stock context while staying strictly read-only.

## Context
The Fleets cockpit already has an accepted read-only role. This block should deepen its usefulness as a readiness surface after Shipyard activity, but without introducing movement, missions, split or merge mutation, combat, or deployment behavior.

## Implementation steps

1. Inspect the current Fleets read model to determine what ship stock, orbital group, or readiness data is already exposed from the backend.
2. Add or refine UI summaries that explain produced ship readiness, local stock visibility, or current limitations honestly in Spanish-first copy.
3. Add a handoff from Shipyard success or readiness toward Fleets while preserving query parameters.
4. Update the Fleets checklist with the new readiness-only scope and explicit exclusions.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Web/
- docs/dev/fleets-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/
- docs/dev/fleets-cockpit-checklist.md

## Acceptance criteria

- Fleets better explains orbital production readiness or stock context using backend-backed data only.
- The page remains read-only and introduces no movement, missions, split, merge, deploy, recall, or attack actions.
- Query-parameter-preserving handoff from Shipyard to Fleets exists where useful.
- `npm run build --prefix src/VoidEmpires.Frontend` and `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not add fleet mutation

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
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
