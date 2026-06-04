# TASK-19U-ground-army-backend-contract-discovery-and-scope-audit

---
id: TASK-19U-ground-army-backend-contract-discovery-and-scope-audit
title: Ground Army backend contract discovery and scope audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Audit the current backend, domain, application, infrastructure, web, and test surface for Ground Army related functionality and document the safe Ground Army v1 scope before any cockpit implementation starts.

## Purpose
Prevent the Ground Army cockpit from accidentally growing into invasions, assaults, raids, occupation, troop movement, or combat resolution while still identifying the safe read-state and guarded preparation surfaces that can power a useful development cockpit.

## Current Problem
`/ground-army` still behaves as a separated module placeholder/readiness cabin. Before later tasks upgrade it, the implementation path must verify what already exists for troops, garrison, barracks, academy, manpower or population, terrestrial logistics, readiness summaries, training queues, and future invasion hooks. Without this audit, later work could duplicate contracts, misclassify Construction responsibilities, or activate unsafe mutations.

## Context
- Construction already owns general civil, economic, and infrastructure building work.
- Defenses now exposes protection and readiness without combat.
- Shipyard owns orbital asset production and Fleets owns orbital movement.
- Ground Army must stay clearly planet-side: terrestrial force readiness and preparation only.
- The audit should prefer documentation and tests first, not speculative implementation.

## Files to Inspect First
- `src/VoidEmpires.Domain/`
- `src/VoidEmpires.Domain/Buildings/`
- `src/VoidEmpires.Domain/Assets/`
- `src/VoidEmpires.Domain/Fleets/`
- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/defenses-cockpit-checklist.md`

## Implementation Requirements
1. Inventory all currently implemented Ground Army adjacent domain concepts, including:
   - troop types
   - ground units
   - garrison concepts
   - barracks or academy style structures
   - training or recruitment concepts
   - population or manpower rules
   - terrestrial logistics or support
   - planetary military readiness summaries
   - any queue or order concept that could represent training
2. Identify whether Ground Army actions already surface indirectly through Construction catalogs, requirements, queues, or readiness calculations.
3. Identify whether the backend already exposes any safe Ground Army read state through existing development endpoints, aggregate queries, or shared cockpit models.
4. Search for references to invasion, raids, assault, occupation, bombardment, damage, battle resolution, or similar hooks that Ground Army must not activate in v1.
5. Produce a clear Ground Army v1 scope statement that distinguishes:
   - safe read-only state
   - safe guarded training or enqueue paths if they already exist or can be added as Development-only
   - intentionally unsupported actions that must remain disabled or documented as unavailable
6. Document the findings in a durable repo doc, preferably `docs/dev/ground-army-cockpit-checklist.md` or a closely related backend contract note.
7. If code changes are required to support the audit output, keep them minimal and add focused tests.

## UI/UX Requirements
- This task does not build UI directly, but it must define what later UI tasks are allowed to show.
- The findings must help later tasks avoid raw technical names in the main cockpit view.
- The documented scope must explicitly say that Ground Army prepares and reads terrestrial forces only in this build.

## Backend/API Requirements
- Prefer documentation and tests over production code changes.
- If an endpoint or service contract gap must be documented, describe the desired future DTO shape without implementing combat, invasion, or occupation logic.
- Any added route or contract notes must stay Development-only unless a safe existing production pattern already exists, which is not expected here.

## Safety Constraints
- No ground combat execution.
- No invasion execution.
- No assault, raid, or occupation resolution.
- No fleet mutation.
- No orbital movement.
- No Galaxy mutation.
- No production-auth surface changes.

## Expected Files to Modify
- `docs/dev/ground-army-cockpit-checklist.md` or another Ground Army specific doc if it does not already exist
- focused backend or test files only if needed to document or lock in discovered behavior

## Acceptance Criteria
- Ground Army backend and readiness scope is clearly documented.
- Safe Ground Army v1 boundaries are explicit and conservative.
- Later tasks can tell whether the cockpit should be read-only, hand off to Construction, or use a guarded Development-only training path.
- Validation passes for any touched code or docs.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched

## Notes / Residual Risks
- If no dedicated Ground Army backend exists, the audit should still define a useful cockpit over military construction metadata and terrestrial readiness summaries.
- The most valuable output is clarity about what must stay disabled.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when code is needed at all.
- Keep the task mainly documentation, discovery, and test hardening.
