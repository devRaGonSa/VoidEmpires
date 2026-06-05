# TASK-28D Ranking Frontend Api Types And View Models

---
id: TASK-28D
title: Add typed Ranking API contract and frontend view models
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: high
---

## Purpose
Introduce typed API models and normalization helpers so Ranking renders structured, non-raw read data.

## Current problem
Ranking needs to render power, category, and comparison data with stable UI semantics and clear disabled action handling.

## Context from current implementation
Other modules use explicit API-to-view-model transformation with Spanish-ready output.

## Goal
Create `Ranking` frontend types and helper functions for deterministic rendering.

## Files to inspect first
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify
- src/VoidEmpires.Frontend/src/api/rankingApi.ts
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx

## Implementation requirements
- Add types:
- RankingUiState
- RankingPowerSummary
- RankingCategoryScore
- RankingComparisonRow
- RankingFutureAction
- RankingDiagnostics
- Add function:
- fetchRankingUiState(...)
- Add mapping helpers:
- mapRankingUiStateToViewModel(...)
- groupRankingCategories(...)
- selectRecommendedRankingFocus(...)
- getRankingPrimaryAction(...)
- Normalize labels, action states, confidence/readiness, and placeholder reasons.
- Keep technical detail hidden in diagnostics by default.

## UI/UX requirements
- Must support dashboard, category cards, comparison table and future action cards.
- Spanish-first display.

## Backend/API requirements
- No backend change required unless DTO mismatch appears.

## Safety constraints
- No mutation endpoints are called.
- No optimistic ranking updates.
- No public ladder execution.

## Acceptance criteria
- Types and mappings are used by Ranking page.
- Frontend compiles cleanly.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Keep mapping layer robust to partial payloads because read endpoints may evolve.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
