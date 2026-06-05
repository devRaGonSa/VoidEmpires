# TASK-28S

---
id: TASK-28S
title: Ranking Unknown Metric Fallback Polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: high
---

## Goal
Improve unknown/pending metric fallback copy in Ranking so visible text reads natural and non-technical.

## Context
The Ranking cockpit currently exposes placeholder-like labels when metric mapping is incomplete. This task normalizes user-facing fallback language without altering ranking logic.

## Implementation steps

1. Replace technical fallback phrases in ranking presentation/model.
2. Ensure fallback variants align with context (unknown vs incomplete vs no critical area visible).
3. Keep technical keys available in diagnostics only.
4. Re-run frontend build.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/utils/rankingViewModel.ts
- src/VoidEmpires.Frontend/src/api/rankingTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/utils/rankingViewModel.ts

## Acceptance criteria

- No placeholder-like metric keys in primary UI.
- Fallbacks are clear Spanish and player-facing.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

