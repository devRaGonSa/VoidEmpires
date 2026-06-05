# TASK-28F Ranking Dashboard Power Summary

---
id: TASK-28F
title: Build Ranking dashboard power summary and recommended focus
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Provide an immediate overview of own ranking posture so users understand their power profile at a glance.

## Current problem
Ranking should not start directly on deep score detail; it needs a top-level index summary with clear scope messaging.

## Context from current implementation
Cockpit hero and summary card patterns should be reused from accepted modules.

## Goal
Create a safe top-level dashboard with explicit read-only boundary.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/components/CockpitHero.tsx
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Implementation requirements
- Add summary cards for:
- Índice de poder
- Potencia total
- Categoría dominante
- Área a reforzar
- Comparativa demo
- next recommended focus
- Display explicit statement:
- Esta cabina no publica ranking global ni recompensas.
- Include fallback message for limited demo-state:
- La clasificación actual se calcula desde el escenario demo.
- Avoid raw ids in user-facing cards.

## UI/UX requirements
- Spanish-first, compact, read-only.
- No wording implying global ladder activation.

## Backend/API requirements
- Use ranking read model once endpoint is ready.

## Safety constraints
- No mutation.
- No public ranking or rewards behavior.

## Acceptance criteria
- Ranking dashboard renders coherent overview.
- Read-only disclaimer is visible.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Use placeholders if category dominance/weakness is unavailable in seed state.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
