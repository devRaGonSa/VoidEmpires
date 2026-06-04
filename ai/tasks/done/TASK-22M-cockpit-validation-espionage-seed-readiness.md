# TASK-22M

---
id: TASK-22M
title: Phase 22M - Cockpit validation espionage seed readiness
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - qa
  - docs
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Ensure `cockpit-validation` provides deterministic Espionage demo data that is useful for manual QA and regression coverage.

## Purpose

The new cockpit cannot be meaningfully accepted if the standard validation seed does not surface owned context, observed targets, uncertainty, and any available passive signals.

## Current problem

The repository already uses `cockpit-validation` as the preferred rich QA profile for accepted cockpits, but Espionage is not yet part of that documented and tested baseline.

## Context

Seed profiles are additive, deterministic, idempotent, and Development-only. The current accepted workflow avoids manual SQL and depends on reliable reapplication of the validation profile against a reused development database.

## Files to read first

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- espionage read-model tests if already added
- strategic map seed tests
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Component discovery

Inspect how `cockpit-validation` already prepares Galaxy, Planet, Research, Shipyard, Fleets, Defenses, and Ground Army. Extend existing seed conventions rather than introducing a separate destructive reset path.

## Dependency analysis

Document how the seed supports the Espionage read path:

- seed profile -> persisted systems, planets, ownership, fleets, transfers, knowledge
- strategic and espionage read models -> deterministic UI state
- smoke tests -> proof that reapplying the seed remains idempotent

## Implementation requirements

1. Extend `cockpit-validation` or add a clearly compatible seed contract that supports Espionage.
2. Ensure the seeded state includes:
   - Aurelia and Helios Gate owned context
   - at least one owned target
   - at least one visible observed target
   - at least one partial or unknown target if current backend support allows it
   - at least one fleet or transfer signal if supported
   - disabled future mission placeholders on the frontend side
3. Keep the seed idempotent and additive.
4. Do not duplicate systems, planets, fleets, transfers, or observations on reapply.
5. Add or update tests for:
   - Espionage UI state loads after `cockpit-validation`
   - target count is non-zero
   - observed or partial distribution if supported
   - signal visibility if supported
   - profile reapply does not duplicate data
6. Update seed docs with Espionage expectations and canonical QA context.

## UI/UX requirements

- The seed must support screenshot-friendly QA for the cockpit
- The seeded state should expose both known and uncertain intelligence if the current backend can do so truthfully

## Backend/API requirements

- Development-only seed changes only
- No migrations
- No destructive reset
- No active mission data that implies spy execution

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- espionage endpoint or read-model tests if needed
- `docs/dev/development-seed-profiles.md`

## Safety constraints

- No mission execution
- No manual SQL requirement
- No destructive seed reset
- No data duplication on repeated apply

## Acceptance criteria

- `cockpit-validation` supports useful Espionage QA.
- Reapplying the profile remains deterministic and idempotent.
- Tests cover the Espionage readiness expectations.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if seed-related docs or types touched by the task require it.

If integration validation is reviewed and no dedicated suite exists, record:

`No integration tests configured.`

## Notes / residual risks

- If the current backend cannot truly represent partial intelligence yet, seed owned and visible targets only and document the limitation explicitly.
- Do not let seed richness force unsafe gameplay features into the backend.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only seed, docs, and related tests changed.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep seed work deterministic and narrowly aligned with Espionage QA.
