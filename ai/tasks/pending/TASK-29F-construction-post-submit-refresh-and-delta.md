# TASK-29F

---
id: TASK-29F-construction-post-submit-refresh-and-delta
title: Reload state after enqueue and show authoritative delta
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
After enqueue success, refresh Construction/planet state from backend and present observable changes.

## Context
If read model lag exists, the UX must communicate delay honestly.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Expected files to modify

- ai/tasks/pending/TASK-29F-construction-post-submit-refresh-and-delta.md
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Implementation steps

1. Trigger state reload after successful enqueue.
2. Show backend-refresh results: queue count, resources, queue entry visibility when present.
3. If entry is not yet visible, show exactly:
   - La orden fue aceptada por el backend; la cola visible se actualizará con la siguiente lectura disponible.
4. Avoid inventing new queue data.
5. Keep no auto-complete behavior.

## Acceptance criteria

- Post-submit view is sourced from backend reload.
- No fabricated state changes.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
