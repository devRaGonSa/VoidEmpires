# TASK-28H Ranking Demo Comparison And Future Leaderboard

---
id: TASK-28H
title: Add demo comparison and disabled future leaderboard placeholders
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Add non-persistent comparison context to Ranking while clearly preventing public leaderboard behavior.

## Current problem
Ranking requires comparative framing, but real public ladders are out of scope.

## Context from current implementation
Accepted cockpits emphasize deterministic, placeholder-safe UX when future mechanics are present.

## Goal
Render a comparison section with explicit read-only and demo markers.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- docs/dev/development-seed-profiles.md
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- docs/dev/ranking-cockpit-checklist.md

## Implementation requirements
- Add section:
- Comparativa demo
- Include own row plus demo/reference rows where data exists.
- Add explicit states:
- Ranking global no publicado
- Solo lectura
- Referencia de escenario demo
- Show future placeholders:
- Clasificación global
- Clasificación de alianzas
- Temporadas
- Recompensas
- Keep those clearly disabled/non-executable.

## UI/UX requirements
- Spanish-first.
- Comparison should be realistic and non-misleading.
- Keep future actions visually secondary.

## Backend/API requirements
- If backend provides comparison rows, ensure deterministic output and test coverage.
- No persistence table for ranking rows.

## Safety constraints
- No public ladder publication.
- No rewards.
- No public profile exposure.

## Acceptance criteria
- Demo comparison is visible and explicit.
- Future leaderboard items remain disabled.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend
- dotnet build --no-restore
- dotnet test --no-build if backend model changed

## Notes / residual risks
- If backend lacks comparison data, clearly document fallback that only own row is shown in v1.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
