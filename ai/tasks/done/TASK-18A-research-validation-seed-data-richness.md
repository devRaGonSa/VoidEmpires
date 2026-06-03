# TASK-18A-research-validation-seed-data-richness

---
id: TASK-18A-research-validation-seed-data-richness
title: Research validation seed data richness
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: medium
---

## Goal
Enrich development seed data for Research QA while preserving the accepted available-to-confirm-to-enqueue flow.

## Purpose
Protect the recently stabilized Research cockpit and make its QA state richer without regressing the first usable enqueue path.

## Current Problem
Research is accepted after several correction blocks, but richer simulation data is still needed. The challenge is to add more varied seeded state without accidentally breaking the known working `available -> confirm -> enqueue -> queue update` path.

## Context
- Research now supports catalog display, available and blocked states, explicit confirmation, successful enqueue, queue update, and truthful disabled complete-due behavior.
- Seed data must preserve that behavior while adding more variety.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- Research domain, application, and infrastructure files
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Implementation Requirements
1. Add or extend a `research-validation` profile, or document that richer Research state belongs to `cockpit-validation`.
2. Ensure the seeded Research context includes:
   - at least one available research item;
   - at least two blocked items with distinct reasons;
   - optionally one active queue item if that does not break the primary enqueue QA path;
   - optionally one completed research record if the backend supports it safely;
   - sufficient resources for one available research.
3. Preserve the known first usable Research flow.
4. Add tests for:
   - available item count;
   - blocked item count;
   - enqueue smoke path still working;
   - queue changing after enqueue;
   - idempotent reapply without unexpected duplicate research orders.
5. Update Research docs with the expected seeded profile behavior.

## UI/UX Requirements
- Research QA should show enough catalog variety to be visually meaningful.
- Primary gameplay labels remain Spanish.

## Backend/API Requirements
- Reuse existing research services and invariants.
- Do not introduce real technology effects or hidden unlock mechanics.

## Safety Constraints
- No real tech effects.
- No production endpoint changes.
- No hidden unlocks.
- Do not seed a state that makes the accepted enqueue smoke path unusable unless that tradeoff is explicitly documented and covered by another path.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Possibly `tests/VoidEmpires.Tests/` enqueue smoke tests if the richer profile becomes their new baseline
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- Research QA seed data is richer and still stable.
- Previously accepted enqueue flow still works.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- Be careful with active queue seeding; it can accidentally hide or block the main available-item path.
- Keep the seeded state richer, not more confusing.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Split extra scenario variety into later work if it threatens the stable enqueue path.
