# TASK-17V-development-seed-profile-contract-and-naming

---
id: TASK-17V-development-seed-profile-contract-and-naming
title: Development seed profile contract and naming
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: high
---

## Goal
Define the supported development seed profile contract, naming strategy, and intended QA use before implementing richer profiles.

## Purpose
Prevent ad hoc profile sprawl and ensure future seed work is explicit, discoverable, Development-only, deterministic, and safe to reapply.

## Current Problem
The current seed apply flow already accepts at least `minimal-validation`, but richer seed profiles will become confusing if their names, behavior, and intended scope are not formalized first. Without a contract, users will guess names, expected outcomes, and reset semantics incorrectly.

## Context
- The product goal is richer simulated QA data without manual SQL.
- New seed profiles should be versioned or at least explicitly named, documented, and idempotent.
- The repo already has a dev-only seed apply mechanism that can likely host or expose this contract.

## Files to Inspect First
- `src/VoidEmpires.Application/Development/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Implementation Requirements
1. Define the supported profile names and whether each is:
   - already implemented;
   - introduced in this block;
   - or documented as planned-but-not-yet-supported.
2. Evaluate and formalize profile names such as:
   - `minimal-validation`
   - `cockpit-validation`
   - `shipyard-validation`
   - `fleet-validation`
   - `research-validation`
   - `planet-full-validation`
3. Define the expected contract for every supported profile, including:
   - Development-only scope;
   - deterministic ids;
   - idempotent reapply behavior;
   - inserts missing baseline rows;
   - no full-database purge;
   - optional seeded queue rows only when explicitly documented.
4. Add profile metadata either in code, docs, or both, so the contract is machine-readable or at least centrally documented.
5. Consider enhancing the seed apply response to list:
   - profile applied;
   - key ids;
   - intended cockpits;
   - recommended QA URLs.
6. Add or strengthen tests for unsupported profile handling if needed.
7. Do not implement all richer profile data in this task unless a very small change is required to make the contract executable.

## UI/UX Requirements
- Seed apply responses should be understandable from PowerShell or JSON output.
- Spanish is not required for internal seed result messages, but the wording should be unambiguous.

## Backend/API Requirements
- The apply endpoint must remain Development-only.
- Unsupported profile requests should fail clearly and safely.
- Do not couple profile naming to frontend-only concepts if backend services need broader reuse.

## Safety Constraints
- No production data seeding.
- No destructive reset behavior.
- No secrets or connection strings.
- Do not quietly alias multiple names unless that behavior is documented and tested.

## Expected Files to Modify
- `src/VoidEmpires.Application/Development/` seed profile contract files if introduced
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs` only if response metadata or validation changes
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- Seed profile naming and contract are explicit.
- Unsupported profile behavior is safe and tested.
- Documentation tells users which profile to use for which QA purpose.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- A future destructive reset profile might be useful, but it is out of scope for this block.
- Keep the contract small and actionable rather than inventing a large scenario framework too early.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Prefer small contract and doc changes over broad service rewrites.
