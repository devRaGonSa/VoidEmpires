# TASK-28K Ranking Error Taxonomy And Diagnostics

---
id: TASK-28K
title: Normalize Ranking error messages and diagnostics
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Make Ranking failures readable in Spanish while keeping detailed technical data in collapsed diagnostics.

## Current problem
Ranking endpoint or context failures could expose technical details as primary UI and reduce reliability of manual QA.

## Context from current implementation
Existing pages map errors to friendly text with diagnostic detail collapsed.

## Goal
Create a dedicated error mapping and UI state for ranking read-state failures.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/api/rankingApi.ts
- Other cockpit diagnostic patterns

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/api/rankingApi.ts

## Implementation requirements
- Map these error classes:
- invalid civilization id
- civilization not found
- ranking read unavailable
- endpoint unavailable outside development
- unsupported future action
- unexpected error
- Add primary Spanish messages:
- No se pudo cargar la lectura de ranking.
- No hay contexto de civilización.
- La clasificación global no está disponible en esta versión.
- Keep technical details in collapsible diagnostics.
- Do not suppress backend rejection responses.

## UI/UX requirements
- Clear error copy.
- Diagnostics visible only on demand.
- No raw payload in primary text.

## Backend/API requirements
- Add result codes only if contract updates require.

## Safety constraints
- No mutation paths in error handlers.
- No routing side effects during error state.

## Acceptance criteria
- Errors are understandable and consistent with other cockpits.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend
- dotnet build --no-restore
- dotnet test --no-build if backend is modified

## Notes / residual risks
- Keep this mapping close to the page-level view-model for maintainability.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
