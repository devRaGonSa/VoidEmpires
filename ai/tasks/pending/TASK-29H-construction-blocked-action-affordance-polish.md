# TASK-29H

---
id: TASK-29H-construction-blocked-action-affordance-polish
title: Polish blocked action affordances in Construction cockpit
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Clarify blocked actions are non-executable and provide reason cues.

## Context
This is a UX safety task without gameplay logic changes.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Expected files to modify

- ai/tasks/pending/TASK-29H-construction-blocked-action-affordance-polish.md
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Implementation steps

1. Ensure blocked cards do not expose primary confirm action.
2. Add visible blocked reason labels:
   - faltan recursos
   - requisito pendiente
   - cola no disponible
   - fuera de alcance
3. Keep available actions visually distinct.

## Acceptance criteria

- Blocked items cannot be mistaken for ready actions.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
