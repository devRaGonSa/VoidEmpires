# TASK-19E-defenses-backend-contract-discovery-and-scope-audit

---
id: TASK-19E-defenses-backend-contract-discovery-and-scope-audit
title: Defenses backend contract discovery and scope audit
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Audit the current backend, domain, application, infrastructure, web, and test surface for defense-related functionality and document the safe Defenses v1 scope before any cockpit implementation starts.

## Purpose
Prevent the Defenses cockpit from accidentally growing into combat, interception, fleet mutation, or other unsupported gameplay while still identifying the read-state and queue surfaces that can safely power a useful development cockpit.

## Current Problem
`/defenses` is still a placeholder/readiness cabin. Before later tasks upgrade it, the implementation path must verify what already exists for defensive buildings, readiness, shield concepts, construction categories, queue behavior, and any future combat hooks. Without this audit, later work could duplicate existing contracts or wire unsafe mutations.

## Context
- Construction already owns general civil, economic, and infrastructure building work.
- Research, Shipyard, Fleet, Galaxy, and Planet now use explicit cockpit read models and guarded boundaries.
- `docs/dev/planet-module-boundaries.md` intentionally keeps Defenses separate from Construction, Shipyard, and Fleets.
- The audit should prefer documentation and tests first, not speculative code.

## Files to Inspect First
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

## Implementation Requirements
1. Inventory all currently implemented defense-adjacent domain concepts, including:
   - defensive buildings
   - shield or mesh concepts
   - defense or military categories
   - fortification or planetary protection concepts
   - readiness, capacity, or protection summaries
   - any defense-like asset stock if present
2. Identify whether defense actions already surface indirectly through construction catalogs, queues, or requirements.
3. Identify whether the backend already exposes any safe defense read state through existing development endpoints, aggregate queries, or shared cockpit models.
4. Search for references to combat, interception, damage, attack resolution, bombardment, invasion, or similar hooks that Defenses must not activate in v1.
5. Produce a clear Defenses v1 scope statement that distinguishes:
   - safe read-only state
   - safe guarded enqueue paths if they already exist or can be added as Development-only
   - intentionally unsupported actions that must remain disabled or documented as unavailable
6. Write the findings into a durable repo doc, preferably `docs/dev/defenses-cockpit-checklist.md` or a closely related backend contract note.
7. If code changes are required to support the audit output, keep them minimal and add focused tests.

## UI/UX Requirements
- This task does not build UI directly, but it must define what later UI tasks are allowed to show.
- The findings must help later tasks avoid raw technical names in the main cockpit view.
- The documented scope must explicitly say that Defenses prepares readiness and protection only in this build.

## Backend/API Requirements
- Prefer documentation and tests over production code changes.
- If an endpoint or service contract gap must be documented, describe the desired future DTO shape without implementing combat logic.
- Any added route or contract notes must stay Development-only unless a safe existing production pattern already exists, which is not expected here.

## Safety Constraints
- No combat execution.
- No interception execution.
- No damage, bombardment, or invasion simulation.
- No fleet mutation.
- No Galaxy mutation.
- No production-auth surface changes.

## Expected Files to Modify
- `docs/dev/defenses-cockpit-checklist.md` or another Defenses-specific doc if it does not already exist
- focused tests only if needed to document or lock in discovered behavior

## Acceptance Criteria
- Defense-related backend/domain scope is clearly documented.
- Safe Defenses v1 boundaries are explicit and conservative.
- Later tasks can tell whether the cockpit should be read-only, hand off to Construction, or use a guarded Development-only enqueue path.
- Validation passes for any touched code or docs.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched

## Notes / Residual Risks
- If no dedicated defense backend exists, the audit should still define a useful cockpit over defensive construction metadata and readiness summaries.
- The most valuable output is clarity about what must stay disabled.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when code is needed at all.
- Keep the task mainly documentation, discovery, and test hardening.
