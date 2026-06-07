# TASK-29J

---
id: TASK-29J-construction-ui-state-tests-or-frontend-guard
title: Add regression guard(s) for construction UI state
status: done
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Create minimal practical regression safety for the new construction enqueue UX path.

## Context
Avoid introducing a heavy test framework if none exists.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Expected files to modify

- ai/tasks/pending/TASK-29J-construction-ui-state-tests-or-frontend-guard.md

## Tooling audit

- `src/VoidEmpires.Frontend/package.json` does not define a frontend test runner.
- No `vitest`, `jest`, `testing-library`, `playwright`, or `cypress` dependency is configured for the frontend app.
- This task therefore uses documented guard checks instead of introducing a heavy new test stack mid-queue.

## Implementation steps

1. Detect existing frontend test tooling and add focused tests when available:
   - enqueue API function available
   - confirmation copy present
   - no mutation without confirm
2. If no tests exist, add documented guard checks in task or checklist docs.
3. Keep checks minimal and maintainable.

## Acceptance criteria

- At least one practical regression guard exists.
- Build remains green.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
