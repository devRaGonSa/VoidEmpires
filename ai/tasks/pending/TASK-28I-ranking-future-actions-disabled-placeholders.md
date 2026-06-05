# TASK-28I Ranking Future Actions Disabled Placeholders

---
id: TASK-28I
title: Show disabled future ranking actions for roadmap clarity
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Display intended future ranking capabilities as intentionally disabled placeholders.

## Current problem
Users need roadmap visibility without exposing executable ranking features outside v1.

## Context from current implementation
Other modules already communicate disabled future actions with clear copy and secondary visual treatment.

## Goal
Add a secondary section of non-clickable, clearly disabled entries.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- docs/dev/cockpit-copy-guidelines.md

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts

## Implementation requirements
- Add section: `Funciones futuras de clasificación`.
- Add disabled placeholders:
- Ver clasificación global
- Ver ranking de alianzas
- Abrir temporada
- Reclamar recompensa
- Publicar perfil
- Comparar jugador
- Each entry should state:
- No disponible en esta versión.
- Solo lectura en esta cabina.
- La función queda visible como referencia futura, pero no se puede ejecutar.
- No click handlers or route mutations.
- Keep disabled actions visually non-primary.

## UI/UX requirements
- Spanish-first.
- Secondary visual hierarchy.
- No action-like affordances.

## Backend/API requirements
- None.

## Safety constraints
- No ranking execution.
- No rewards flow.
- No public player profiles.

## Acceptance criteria
- Future actions are visible but unmistakably inactive.
- No mutation/navigation side effects.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Keep copy explicit to avoid misinterpretation as implemented ranking features.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
