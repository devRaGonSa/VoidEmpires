# TASK-29C

---
id: TASK-29C-construction-action-selection-state
title: Add selected action state in ConstructionPage (review only)
status: obsolete
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Allow choosing one available action on ConstructionPage and show a review summary without posting anything.

## Context
Selection is required, but this task must remain read-only.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- ai/tasks/pending/TASK-29C-construction-action-selection-state.md
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Implementation steps

1. Add selected action state variable(s).
2. Make available action cards selectable.
3. Keep blocked cards unselectable or show explicit block reason.
4. Render summary with building name, action type, target level, duration, cost, and expected scope.
5. Keep Spanish copy and no mutation.

## Acceptance criteria

- A user can select available action and review details.
- No API call or mutation exists.

## Resolution notes

- The current `/construction` route is `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx` -> `PlanetPage variant="construction"`.
- `PlanetPage` already includes available-action selection state, blocked-card handling, and a review summary for the prepared construction action.
- The branch has already advanced beyond this read-only milestone because the same route also includes the later enqueue flow.
- No additional code change was applied here to avoid duplicating state or regressing the newer construction flow.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
