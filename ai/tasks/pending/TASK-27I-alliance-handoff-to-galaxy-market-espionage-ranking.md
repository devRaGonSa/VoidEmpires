# TASK-27I Alliance Handoff To Galaxy Market Espionage Ranking

---
id: TASK-27I
title: Add Alliance handoff links and context-preserving navigation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: medium
---

## Purpose
Guide users from Alliance to neighboring strategic modules while preserving context and explicitly keeping diplomacy read-only.

## Current problem
Alliance currently risks being siloed. It should provide orientation to galaxy, market, espionage and ranking without duplicating their logic.

## Context from current implementation
Cross-module handoffs exist in other cockpits and should be reused with route helpers and query preservation.

## Goal
Add secondary handoff panel and helpers for supported routes.

## Files to inspect first
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx

## Expected files to modify
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx

## Implementation requirements
- Add handoff targets:
- Galaxia
- Mercado
- Espionaje
- Ranking where route exists
- Preserve civilizationId in handoff URLs when supported.
- Add buildAllianceUrl helper if missing.
- Add buildRankingUrl only if route helper conventions and route are available.
- Avoid implying invite/request workflows through these handoffs.

## UI/UX requirements
- Spanish-first.
- Handoff cards should be secondary and clearly non-primary.
- Keep disabled nature of future modules clear.

## Backend/API requirements
- None for this task.

## Safety constraints
- No executable diplomacy actions from handoffs.
- No messaging or pact flow links.

## Acceptance criteria
- All handoff links resolve with context.
- No route regression in existing links.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- If ranking route helper is missing, document as intentionally not shown.
