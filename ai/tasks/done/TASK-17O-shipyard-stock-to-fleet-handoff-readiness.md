# TASK-17O-shipyard-stock-to-fleet-handoff-readiness

---
id: TASK-17O-shipyard-stock-to-fleet-handoff-readiness
title: Shipyard stock to Fleet handoff readiness
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: medium
---

## Goal
Show how Shipyard stock, production queue, and existing orbital groups relate to Fleets without introducing new Fleet behavior.

## Purpose
Clarify the module boundary so players understand where assets are produced and where orbital groups are inspected or moved.

## Current Problem
Shipyard and Fleets are adjacent but distinct modules. If Shipyard shows stock without explaining how it relates to active orbital groups, the cockpit becomes confusing. If it goes too far and adds group creation or movement, it breaks the accepted module boundaries.

## Context
- The repository already has foundations around stock, orbital groups, and Fleet cockpit behavior.
- Shipyard must not duplicate Fleet movement, split, or merge.
- Any stock-to-group allocation workflow is future work unless safe behavior already exists and is explicitly scoped.

## Files to Inspect First
- `docs/dev/fleet-api-contracts.md`
- `src/VoidEmpires.Domain/Assets/`
- `src/VoidEmpires.Domain/Fleets/`
- `src/VoidEmpires.Application/Fleets/`
- `src/VoidEmpires.Infrastructure/Fleets/`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`

## Implementation Requirements
1. If backend state supports it, distinguish clearly between:
   - stock or reserve assets;
   - queue items still in production;
   - orbital groups already visible in Fleets.
2. Add a handoff explanation card or section with context-aware links such as:
   - `Abrir Flotas`
   - `Ver grupos orbitales`
3. If a safe dev endpoint exists to allocate stock into orbital groups, do not wire it here unless it is explicitly in scope, tested, and still non-invasive.
4. If no safe allocation UI is wired, show a Spanish explanation such as:
   - `La asignacion a flota se mantiene fuera de esta cabina en esta build.`
5. Preserve navigation context between Shipyard and Fleets.

## UI/UX Requirements
- Spanish-first explanatory copy.
- The handoff should look informative, not like a disguised mutation action.
- The relationship between stock and Fleet groups should be clear at a glance.

## Backend/API Requirements
- No backend mutation change is expected.
- Read-model additions are acceptable only if needed to distinguish stock from active groups.

## Safety Constraints
- No split or merge.
- No transfer creation from Shipyard.
- No fleet movement.
- No implicit orbital group creation.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx` only if a narrow return link is helpful
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- Narrow read-model files only if one missing field blocks truthful stock versus group display

## Acceptance Criteria
- Shipyard explains how its stock relates to Fleets.
- Existing stock is visible if the backend exposes it.
- No new Fleet behavior is introduced.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet test --no-build` only if backend read-model changes are made.

## Notes / Residual Risks
- A future block can implement explicit stock-to-orbital-group allocation if safe.
- Keep the wording strict enough that users do not assume Shipyard can command fleets.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Keep this as a boundary-clarity task, not a hidden Fleet feature task.
