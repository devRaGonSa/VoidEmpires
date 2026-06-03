# TASK-17U-development-seed-current-state-audit

---
id: TASK-17U-development-seed-current-state-audit
title: Development seed current state audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: high
---

## Goal
Audit the current development seed implementation and document exactly what `minimal-validation` currently creates and supports.

## Purpose
Create a precise baseline for future seed profile work so richer simulation data can be added safely without breaking the current cockpit QA flows.

## Current Problem
The repository now depends heavily on `minimal-validation` for Galaxy, Fleets, Planet, Construction, Research, and Shipyard QA. That seed grew incrementally as cockpit features landed, and it is no longer obvious which entities, queues, and relationships it creates. Before adding new profiles, the current seed graph and its idempotency characteristics must be understood and documented.

## Context
- `DevelopmentSeedService` already uses deterministic ids such as the seed civilization and Aurelia planet ids.
- Research and Shipyard cockpit QA now depend on seeded read-model state.
- The product decision is explicit: do not rely on manual SQL for standard QA; use reproducible Development-only seed profiles instead.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Application/Development/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Audit and document the current `minimal-validation` seed graph, including:
   - player profile;
   - civilization;
   - galaxy, system, and star;
   - planets;
   - ownership and visibility;
   - resources and stockpiles;
   - building state;
   - construction-related seed state;
   - research-related seed state;
   - shipyard-related seed state;
   - asset stock;
   - orbital groups;
   - active transfers.
2. List all deterministic ids currently relied on by the seed and associated cockpit docs or tests.
3. Identify which cockpit screens depend on which seeded entities or queues.
4. Document where the seed is idempotent and where it may preserve mutated state from prior local runs.
5. Clarify whether reapplying `minimal-validation`:
   - resets queues;
   - preserves queues;
   - only inserts missing rows;
   - or behaves differently for specific subsystems.
6. Add or update documentation in `docs/dev/development-seed-profiles.md` or an equivalent targeted doc section if that is a better fit.
7. Do not change seed behavior unless a small correctness fix is required so the documentation is truthful.

## UI/UX Requirements
- Documentation should be practical for manual QA, not just backend-oriented.
- Include exact URLs for already supported cockpit screens where known.
- Keep technical ids visible in docs where useful, but not framed as player-facing UI content.

## Backend/API Requirements
- Prefer documentation and tests only in this task.
- If seed code must change for accuracy, update tests alongside it.

## Safety Constraints
- Do not delete, reset, or purge user data.
- Do not add SQL scripts.
- Do not change production behavior.
- Do not introduce destructive reset logic disguised as documentation work.

## Expected Files to Modify
- `docs/dev/development-seed-profiles.md` or related development QA docs
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs` only if a small correction or documentation-alignment fix is needed
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs` only if a narrow correctness fix is unavoidable

## Acceptance Criteria
- Current `minimal-validation` behavior is documented clearly and concretely.
- Existing tests still pass.
- Later seed-profile tasks have an accurate baseline to extend.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend docs or code are touched.

## Notes / Residual Risks
- This is an audit task, not a redesign task.
- If the current seed preserves some mutated local state by design, document that explicitly rather than hiding it.
- Keep the output narrow enough to guide follow-up implementation without introducing a new seed architecture yet.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer documentation and pinpoint tests over structural refactors.
