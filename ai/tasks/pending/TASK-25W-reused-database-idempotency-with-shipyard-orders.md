# TASK-25W

---
id: TASK-25W
title: Phase 25W - Reused database idempotency with Shipyard orders
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Prove that `cockpit-validation` remains additive and safe when manual QA has already created real Shipyard production orders.

## Current problem

The new Shipyard QA scripts will create real persisted rows. Reapplying Development seeds must preserve those manual rows and avoid destructive resets, duplicate history, or unique-key conflicts.

## Context from current implementation

- Construction and Research already have reseed-preservation coverage.
- `cockpit-validation` is documented as additive and idempotent.
- Shipyard adds another persisted queue type that could collide with seed data or sequence assumptions if not tested explicitly.

## Files to read first

- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs
- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- docs/dev/development-seed-profiles.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs
- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- docs/dev/development-seed-profiles.md
- docs/dev/persisted-gameplay-flow-checklist.md
- ai/current-state.md

## Implementation requirements

1. Add a deterministic test that:
   - applies `cockpit-validation`
   - creates one real Shipyard production order through the application path
   - reapplies `cockpit-validation`
   - asserts the manual Shipyard order still exists
   - asserts the seed did not duplicate deterministic baseline rows
   - asserts no unique-key or sequence conflict occurs
   - asserts Shipyard UI-state still loads afterward
2. Reuse the same additive-seed expectations already established for Construction and Research where appropriate.
3. If the seed docs currently omit Shipyard-specific preservation notes, update them.
4. Avoid changing seed behavior unless the new test demonstrates a real problem.

## Backend/API requirements

- Tests are required.
- Any seed change must remain Development-only and additive.
- Do not add cleanup logic that deletes manual QA rows.

## Script/QA requirements

- The persisted QA runbook should later rely on this task to justify reapplying `cockpit-validation` after Shipyard manual QA.
- If a known no-op or preserved-order summary should be surfaced in scripts, note it in docs.

## Safety constraints

- No destructive reseed behavior.
- No deletion of manual Shipyard orders.
- No manual SQL.

## Acceptance criteria

- Reseed preservation is covered for Shipyard manual QA rows.
- `cockpit-validation` remains safe on reused databases after Shipyard enqueue.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If Shipyard uses a sequence range or history pattern different from Construction and Research, the fix may need to be in seed data ordering rather than endpoint behavior.

