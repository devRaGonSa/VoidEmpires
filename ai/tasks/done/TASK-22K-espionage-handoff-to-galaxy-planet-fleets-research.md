# TASK-22K

---
id: TASK-22K
title: Phase 22K - Espionage handoff to Galaxy Planet Fleets Research
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: medium
---

## Goal

Clarify and implement safe handoffs from `Espionaje` to `Galaxia`, `Planeta`, `Flotas`, and `Investigacion`.

## Purpose

Espionage should analyze intelligence, not replace the map, the planet dashboard, fleet command, or future tech planning. The cockpit needs clear neighboring-module boundaries and reliable context-preserving links.

## Current problem

Without explicit handoffs, Espionage can feel isolated or duplicative. Without route helpers, it can also drift into manual query-string assembly and inconsistent context preservation.

## Context

Cross-cockpit polish already standardized navigation, context helpers, and module boundaries. Espionage should fit that model from the start.

## Files to read first

- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- Espionage page component
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`

## Component discovery

Inspect how current cockpits build contextual links and handoff cards. Reuse the same route-builder conventions and secondary-card hierarchy.

## Implementation requirements

1. Add or confirm route-helper support for Espionage, such as `buildEspionageUrl(...)`.
2. Add a handoff panel that explains:
   - `Galaxia` for strategic map and system context
   - `Planeta` for owned planet detail
   - `Flotas` for orbital groups and transfers
   - `Investigacion` for future intelligence technology context
3. Preserve `civilizationId` and relevant `systemId` or `planetId` when available and safe.
4. Keep handoff messaging aligned with module boundaries:
   - Espionage analyzes intelligence
   - Galaxy remains read-only strategic overview
   - Fleets owns movement and transfer command
   - Research owns future technology progress
5. Do not add new fleet behavior or imply that Galaxy or Fleets can launch spy missions.

## UI/UX requirements

- Spanish-first
- Handoff cards or links should feel secondary to the intelligence content
- Boundaries between modules must be explicit and easy to understand

## Backend/API requirements

- None expected
- No endpoint changes required for route handoffs

## Expected files to modify

- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- Espionage page component
- supporting navigation or sidebar file only if a small route-state update is needed

## Safety constraints

- No mutations
- No fleet movement
- No spy mission launch
- No hidden new module coupling

## Acceptance criteria

- Espionage clearly explains how it relates to neighboring modules.
- Links preserve context when that context is available.
- Route helpers stay centralized instead of rebuilding URLs inline.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- `Investigacion` handoff may remain explanatory if there is not yet a concrete intelligence-tech target in the seeded data.
- Avoid over-linking every target card if that would make the catalog noisy; one clear handoff area is enough.

## Commit and push

1. Run `git status`.
2. Verify only intended route and page files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep route-helper changes minimal and centralized.
