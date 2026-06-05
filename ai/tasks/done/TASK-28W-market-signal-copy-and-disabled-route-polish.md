# TASK-28W

---
id: TASK-28W
title: Market Signal and Disabled Route Polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
title: Market Signal and Disabled Route Polish
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Goal
Polish disabled-state visual language across Market signals, route previews, and future operations.

## Context
Market is read-only, but some text/actions still appear overly actionable. This task clarifies route/action copy and visual hierarchy.

## Implementation steps

1. Review market sections for future/disabled copy opportunities.
2. Replace awkward placeholders and over-abstract phrasing.
3. Ensure disabled actions/chips are visually secondary and non-executable.
4. Re-run frontend build.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts

## Acceptance criteria

- Market remains clearly read-only in all sections.
- Disabled route/action text appears secondary.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.
