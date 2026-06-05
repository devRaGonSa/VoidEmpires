# TASK-28J Ranking Handoff To Galaxy Market Espionage Alliance

---
id: TASK-28J
title: Add Ranking handoff navigation to neighboring strategic modules
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Add secondary navigation from Ranking to modules that support its score categories.

## Current problem
Ranking must not duplicate other cockpits and should clearly indicate where to inspect source signals.

## Context from current implementation
Cross-cockpit handoffs are documented and route helper based.

## Goal
Add contextual handoff cards for Galaxy, Market, Espionage, and Alliance.

## Files to inspect first
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx

## Expected files to modify
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx

## Implementation requirements
- Add handoff panel linking:
- Galaxia
- Mercado
- Espionaje
- Alianzas
- Preserve civilizationId in context-aware links.
- Ensure `buildRankingUrl` exists where appropriate.
- Avoid links to non-existent action routes.

## UI/UX requirements
- Spanish-first.
- Handoff cards secondary and non-authoritative for ranking outcomes.
- Clear module boundaries.

## Backend/API requirements
- None.

## Safety constraints
- No active ranking action handoff.
- No public ladder or matchmaking navigation claims.

## Acceptance criteria
- Handoff links preserve context.
- Existing link patterns remain stable.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Document any missing helper route limitations in the ranking checklist.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
