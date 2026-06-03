# TASK-17Q-minimal-validation-seed-shipyard-usability

---
id: TASK-17Q-minimal-validation-seed-shipyard-usability
title: Minimal validation seed Shipyard usability
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Extend the deterministic minimal-validation seed so Shipyard manual QA has meaningful available, blocked, queue, resource, and stock context.

## Purpose
Guarantee that Shipyard v1 can be validated manually and in tests without relying on ad hoc local data or broken empty states.

## Current Problem
A Shipyard cockpit cannot be evaluated well if the seed has no usable asset option, no blocked example, no local resources, no queue context, or no stock visibility. The page would technically load but still fail as a cockpit.

## Context
- The minimal-validation seed already supports Galaxy, Fleets, Planet, Construction, and Research.
- Shipyard should extend that seed carefully without breaking current seeded expectations or overfunding the test scenario.
- Determinism matters because later docs and smoke checks depend on stable ids and visible states.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Asset production tests
- Shipyard UI-state tests
- Fleet seed expectations

## Implementation Requirements
1. Ensure the minimal-validation seed supports:
   - the owned planet Aurelia;
   - a relevant local resource stockpile;
   - any required shipyard-ready building or capacity state if the domain requires it;
   - at least one available asset production option if enqueue is supported;
   - at least one blocked asset production option;
   - a readable empty queue or one deterministic queue item if that better exercises the cockpit;
   - stock context if the backend can surface it.
2. Keep the seed deterministic and idempotent.
3. Do not overfund or bypass normal validation logic.
4. Do not break accepted Fleet seed behavior.
5. Add tests asserting the seeded Shipyard UI state, such as:
   - available options count;
   - blocked options count;
   - resources visible;
   - queue readable;
   - stock visible when applicable.

## UI/UX Requirements
- The resulting seed should support screenshot-friendly manual QA for available and blocked cards.
- The seed should support honest disabled states if enqueue cannot safely be enabled.

## Backend/API Requirements
- Development seed only.
- Add or update tests alongside seed changes.

## Safety Constraints
- No production data behavior.
- No automatic real database migrations.
- No unrealistic seed shortcuts that invalidate cockpit logic.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/` relevant Shipyard endpoint or UI-state tests

## Acceptance Criteria
- The minimal-validation seed supports meaningful Shipyard QA.
- Seed behavior remains deterministic and compatible with existing module expectations.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched.

## Notes / Residual Risks
- If enqueue remains disabled for safety, the seed should still demonstrate readiness and blocked states clearly.
- Avoid adding extra seed complexity that only serves hypothetical future features.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- If the seed needs broader restructuring, split follow-up work instead of overloading this task.
