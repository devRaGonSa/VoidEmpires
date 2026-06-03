# TASK-19Q-cockpit-validation-defenses-seed-readiness

---
id: TASK-19Q-cockpit-validation-defenses-seed-readiness
title: Cockpit validation defenses seed readiness
status: completed
type: platform
team: platform
supporting_teams:
  - backend
  - qa
  - docs
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Ensure `cockpit-validation` supports meaningful Defenses QA, or add a narrowly scoped `defenses-validation` profile only if the existing profile contract cannot safely carry the new readiness needs.

## Purpose
Make Defenses visually and functionally testable through the same deterministic seed workflow already used by the accepted cockpit suite, without falling back to manual SQL.

## Current Problem
The Defenses cockpit cannot be accepted if seeded QA data does not include useful defensive state. Placeholder routes can survive on navigation alone, but a cockpit foundation needs deterministic structures, resources, blockers, and optional queue context.

## Context
- `cockpit-validation` is the preferred richer QA baseline.
- The current seed service is Development-only, deterministic, idempotent, and additive.
- Defenses should join the existing accepted cockpit suite without breaking reapply behavior or queue uniqueness guarantees.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/Infrastructure/Development/DevelopmentSeedServiceTests.cs`
- Defenses read-model tests
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Implementation Requirements
1. Extend `cockpit-validation` when possible; add `defenses-validation` only if the seed profile system or data shape makes that clearly safer.
2. Ensure seeded Defenses context includes:
   - controlled `Aurelia`
   - useful resource balances
   - at least one visible defensive structure or readiness item
   - at least one available or ready defense option if safe
   - at least one blocked defense option
   - queue state if the cockpit supports it
   - clear complete-due limitation if complete-due stays disabled
3. Keep the seed idempotent and additive.
4. Do not duplicate queue or order rows when reapplying.
5. Add tests that cover:
   - Defenses UI state loads after seeding
   - defensive structures or readiness count meets expectations
   - available and blocked distributions if supported
   - profile reapply does not duplicate data
6. Update seed docs with the new expectations and QA route.

## UI/UX Requirements
- Seed output should support meaningful screenshot-style cockpit QA, not just empty cards.

## Backend/API Requirements
- Seed behavior must remain Development-only.
- Avoid migrations.
- Preserve the existing seed profile discovery contract.

## Safety Constraints
- No active attack or combat data.
- No manual SQL fallback for standard QA.
- No destructive reset behavior.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- related seed tests
- `docs/dev/development-seed-profiles.md`
- optional Defenses checklist docs

## Acceptance Criteria
- `cockpit-validation` or an explicitly justified Defenses-focused profile supports Defenses QA.
- Seed behavior remains deterministic and idempotent.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend docs or types are touched

## Notes / Residual Risks
- If backend action support remains limited, the seed should still produce a strong readiness and blocked-state screenshot path rather than forcing fake available actions.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the seed adjustments targeted to Defenses readiness only.
