# TASK-17B-research-regression-check-planet-construction-galaxy-fleets

---
id: TASK-17B-research-regression-check-planet-construction-galaxy-fleets
title: Research regression check for Planet Construction Galaxy Fleets
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Run a narrow regression pass over the neighboring accepted cockpits after Research changes.

## Purpose
Research changes can touch shared route helpers, resource display, module layout, and seed data. We need to make sure the accepted surfaces around it remain stable.

## Current Problem
The current block is focused on Research, but the shared helpers and seed adjustments may have side effects on Planet, Construction, Galaxy, or Fleets. Those surfaces should remain in their accepted state.

## Context
Accepted neighboring surfaces are:

- `/galaxy` read-only;
- `/fleets` playable dev cockpit foundation;
- `/planet` dashboard/resumen;
- `/construction` scoped to general construction, infrastructure, economy, and civil work.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/strategic-map-cockpit-checklist.md`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`

## Implementation Requirements
1. Update or verify smoke checklist coverage for:
   - `/planet` seeded URL loads;
   - `/construction` seeded URL loads;
   - `/research` seeded URL loads;
   - `/galaxy` seeded URL loads and remains read-only;
   - `/fleets` seeded URL loads and still shows the accepted fleet cockpit.
2. Ensure route helpers still preserve `civilizationId` and `planetId`.
3. Do not redesign these screens.
4. If a regression is found, fix it narrowly.
5. Keep the regression pass focused on route helpers, seed context, and the shared layout language.

## UI/UX Requirements
- Keep changes minimal.
- No new visual redesign.
- Do not let a Research fix accidentally make another cockpit visually inconsistent.

## Backend/API Requirements
- No backend change is expected unless the seed regression is found there.

## Safety Constraints
- No new mutations outside Research enqueue.
- Galaxy remains read-only.
- Do not loosen the accepted boundaries of Planet, Construction, or Fleets.

## Expected Files to Modify
- `docs/dev/frontend-foundation-smoke-checklist.md`
- One or more cockpit checklist docs if a small wording update is needed
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts` only if a regression is found there

## Acceptance Criteria
- Accepted neighboring surfaces still work.
- Validation passes.
- Documentation reflects the regression check.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Keep the scope narrow so the regression check does not become a second product block.
- If one neighboring surface is broken, document exactly which one and avoid broad rewrite pressure.
- The goal is to protect the accepted surfaces while Research is being closed.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer checklist updates over code churn unless a real regression is found.
