# TASK-31M

---
id: TASK-31M
title: Cross-cockpit safety and guardrails
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Ensure the orbital or military preparation changes stay contained and do not leak unintended mutation, movement, mission, or eager-loading regressions across cockpits.

## Context
This block touches Shipyard, Defenses, Fleets, and Planet together, which increases the risk of query-param drift, unsupported links, accidental action copy, or route-loading regressions. The repository already has lightweight static guards for lazy routes and copy drift; this task expands those guardrails for the new safety boundary.

## Implementation steps

1. Review cross-cockpit links among Planet, Shipyard, Defenses, and Fleets and ensure they preserve `civilizationId` and `planetId`.
2. Audit copy and action affordances to ensure normal UI does not imply auto-complete, combat, movement, or mission creation.
3. Add or update lightweight static checks where feasible, keeping existing lazy-import and copy-regression guards green.
4. Update the frontend smoke checklist with the intended cross-cockpit safety expectations for this block.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Cross-cockpit links preserve the needed query parameters.
- No accidental combat, movement, mission creation, or auto-complete is introduced.
- Guard scripts and the frontend build stay green.
- The smoke checklist documents the intended safety boundary.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not weaken the lazy-loading or copy guards

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
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
