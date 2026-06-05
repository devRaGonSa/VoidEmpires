# TASK-28R

---
id: TASK-28R
title: Ranking Spanish Copy and Comparison Label Polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: high
---

## Goal
Remove English leaks and test-like/technical wording from Ranking cockpit comparison views and category cards.

## Context
Ranking cockpit is technically functional but visible labels still include English and technical phrasing that degrade player UX. No scoring/backend logic changes are allowed.

## Implementation steps

1. Replace visible English strings in ranking cockpit and presentation/view-model files.
2. Align comparison/validation labels to natural Spanish player-facing copy.
3. Remove or move raw technical keys/labels from primary UI where possible.
4. Re-run frontend build.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/utils/rankingViewModel.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/rankingPresentation.ts
- src/VoidEmpires.Frontend/src/utils/rankingViewModel.ts

## Acceptance criteria

- No visible English remains in Ranking primary UI.
- Comparison area reads as Spanish cockpit copy (no test output tone).
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

