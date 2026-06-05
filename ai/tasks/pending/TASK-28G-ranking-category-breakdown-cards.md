# TASK-28G Ranking Category Breakdown Cards

---
id: TASK-28G
title: Render read-only category score cards for Ranking
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Show category-level power contribution in clear, non-technical cards.

## Current problem
Without category breakdown, Ranking risks being a single number without context.

## Context from current implementation
Other cockpits use compact grouped summaries; Ranking should follow that pattern.

## Goal
Render deterministic score cards with status and optional readiness/confidence text.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Implementation requirements
- Add cards for:
- Economía
- Colonias
- Investigación
- Astillero
- Flotas
- Defensas
- Ejército Tierra
- Inteligencia
- Diplomacia
- Each card should include score, label, short explanation, and optional confidence/readiness.
- Keep formulas and raw calculations in diagnostics if needed.
- If read model is sparse, show deterministic placeholders.

## UI/UX requirements
- Spanish-first.
- Read-only and compact.
- No fake precision beyond provided precision.

## Backend/API requirements
- No backend mutation.
- Ensure category payload is stable from endpoint.

## Safety constraints
- No ranking action or public ladder behavior.

## Acceptance criteria
- Category breakdown is understandable and deterministic.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Avoid implying hidden formulas as active game logic in primary UI.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
