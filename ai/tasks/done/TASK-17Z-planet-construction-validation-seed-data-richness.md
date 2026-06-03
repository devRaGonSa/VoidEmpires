# TASK-17Z-planet-construction-validation-seed-data-richness

---
id: TASK-17Z-planet-construction-validation-seed-data-richness
title: Planet Construction validation seed data richness
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: medium
---

## Goal
Enrich development seed data for Planet and Construction QA while preserving the accepted dashboard and module boundary structure.

## Purpose
Make Planet and Construction screenshots, regression checks, and cross-cockpit handoffs more useful without collapsing specialized module boundaries back into `/planet`.

## Current Problem
Planet and Construction are accepted, but the current seed state is still relatively sparse for richer QA. More realistic buildings, resources, queue state, and blocked or available construction examples would improve coverage without changing the accepted cockpit structure.

## Context
- `/planet` is a dashboard or resumen cockpit.
- `/construction` is scoped to general, civil, economic, and infrastructure construction.
- Specialized modules remain separated and should stay that way.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`
- Construction domain, application, and infrastructure services
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/development-seed-profiles.md`

## Implementation Requirements
1. Add or extend a `planet-full-validation` profile, or document that Planet and Construction richness is provided by `cockpit-validation`.
2. Ensure the seeded context includes, if supported safely:
   - Aurelia resources;
   - several existing buildings;
   - at least one available general construction action;
   - several blocked actions;
   - one construction queue item if safe;
   - production estimates or fallback relevant data;
   - module navigation cards or state assumptions that make those cards meaningful.
3. Do not reintroduce a full construction catalog into `/planet`.
4. Keep Construction seed data focused on general construction, not specialized module catalogs such as Shipyard or Research.
5. Add tests for:
   - Planet UI-state loading;
   - current building visibility;
   - available construction actions count if supported;
   - blocked actions count;
   - readable queue if seeded;
   - idempotent reapply.
6. Update docs with expected seeded state and profile usage.

## UI/UX Requirements
- `/planet` must remain a dashboard.
- `/construction` must remain scoped to general construction.
- The seeded state should support readable module handoff cards without making the dashboard feel overloaded.

## Backend/API Requirements
- Reuse existing construction services and invariants.
- Keep any queue seeding conservative and deterministic.

## Safety Constraints
- No production auth.
- No destructive reset.
- No specialized module execution from Planet seed work.
- No collapse of module boundaries.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`
- Construction-related tests if needed
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- Richer Planet and Construction QA state exists through a documented profile.
- Tests pass.
- Planet and Construction boundaries remain intact.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend docs or code are touched.

## Notes / Residual Risks
- Queue seeding should remain conservative to avoid making Construction QA brittle.
- Prefer a richer but stable baseline over a maximal scenario with many moving parts.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Split broader construction scenario work into follow-up tasks if needed.
