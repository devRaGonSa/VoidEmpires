# TASK-17X-shipyard-validation-seed-data-richness

---
id: TASK-17X-shipyard-validation-seed-data-richness
title: Shipyard validation seed data richness
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: high
---

## Goal
Enrich the development seed data used for Shipyard QA so Astillero can be validated with more realistic available, blocked, queued, and stock states.

## Purpose
Move Shipyard QA beyond a minimal smoke baseline and into a richer deterministic state that makes the cockpit more useful for screenshots, regression checks, and handoff validation to Fleets.

## Current Problem
Shipyard currently loads correctly but still feels QA-thin:
- some labels remain generic, such as `Activo orbital pendiente de clasificar`;
- stock exists but is limited;
- queue is empty;
- there is no richer due or readiness example;
- only one production option is available while several are blocked.
The page works, but the seed state is still too sparse for realistic visual QA.

## Context
- Shipyard v1 already has a read model, frontend page, catalog, queue section, stock summary, controlled enqueue, and Fleet handoff explanation.
- Richer seed data should make those panels more meaningful without adding unsafe gameplay behavior.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs`
- Shipyard read-model or UI-state service files
- Asset production queue domain and service files
- Orbital asset stock domain files
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Implementation Requirements
1. Add or extend a `shipyard-validation` profile, or explicitly document that Shipyard richness lives inside `cockpit-validation` if the architecture centralizes it there.
2. Ensure the seeded Shipyard context includes, where the domain supports it safely:
   - Aurelia with Shipyard readiness;
   - resources sufficient for one basic asset;
   - resources insufficient for one or more advanced assets;
   - at least one available production option;
   - at least two blocked options with distinct reasons;
   - local stock containing at least two asset types if supported;
   - one active production order if the seeded queue can be represented safely;
   - optionally one due order only if it does not trigger unintended completion behavior.
3. If `Activo orbital pendiente de clasificar` is caused by presentation mapping rather than missing data, fix or reduce it through the appropriate label layer.
4. Add tests asserting:
   - available options count;
   - blocked options count;
   - stock visibility;
   - queue readability if seeded;
   - idempotent reapply without duplicate seeded orders or stock.
5. Update Shipyard checklist documentation with the expected seeded state.

## UI/UX Requirements
- Shipyard should show enough realistic state for screenshot QA.
- Known asset types should use player-facing names.
- Primary gameplay UI remains Spanish.

## Backend/API Requirements
- Reuse existing domain services and invariants.
- Do not bypass backend validation just to make the UI richer.
- Keep all seeding Development-only.

## Safety Constraints
- Seed only Development data.
- No fleet movement.
- No split or merge.
- No combat.
- No unintended due-completion side effects from seeded orders.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs` if idempotency coverage needs expansion
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`
- Frontend presentation helper files only if known label mapping must be corrected

## Acceptance Criteria
- `shipyard-validation` or `cockpit-validation` produces richer Astillero state.
- Shipyard UI-state returns meaningful available, blocked, stock, and queue data.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if label or presentation code changes.

## Notes / Residual Risks
- If active queue seeding is unsafe, prefer richer stock and option coverage and document the limitation.
- Avoid seeding a state that accidentally blocks the primary enqueue QA path unless that tradeoff is intentional and documented.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Split label-polish follow-up work if Shipyard presentation requires broader UI changes.
