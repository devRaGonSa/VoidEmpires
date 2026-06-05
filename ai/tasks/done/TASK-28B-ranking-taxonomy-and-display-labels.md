# TASK-28B Ranking Taxonomy And Display Labels

---
id: TASK-28B
title: Define Spanish ranking terminology and player-facing labels
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: high
---

## Purpose
Create consistent Spanish terms for ranking categories, score states, comparison labels, and disabled future features.

## Current problem
Without clear vocabulary, Ranking UI risks exposing raw technical fields and implying real public ranking behavior.

## Context from current implementation
Cockpit copy conventions and status helpers already standardize Spanish-first output and diagnostics handling.

## Goal
Add a shared presentation helper for Ranking labels and fallback text.

## Files to inspect first
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx
- docs/dev/cockpit-copy-guidelines.md

## Expected files to modify
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- docs/dev/ranking-cockpit-checklist.md

## Implementation requirements
- Add a centralized label map for:
- Índice de poder
- Potencia económica
- Desarrollo colonial
- Progreso tecnológico
- Capacidad orbital
- Preparación defensiva
- Guarnición terrestre
- Inteligencia estratégica
- Diplomacia
- Comparativa demo
- Clasificación no publicada
- Temporada futura
- Recompensas no disponibles
- Métrica pendiente de clasificar
- Unknown/unknown states should fall back to Spanish text, never technical enum names.
- Keep raw values only for diagnostics.

## UI/UX requirements
- Spanish-first labels.
- No hint that full public ranking is active.

## Backend/API requirements
- No backend changes required in this task.
- Use backend fields only through mapping.

## Safety constraints
- No copy implying mutable ranking actions.

## Acceptance criteria
- Labels are centralized and reusable.
- Raw technical names stay in diagnostic areas.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Keep labels version-safe to avoid future expansion noise.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
