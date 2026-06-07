# TASK-29E

---
id: TASK-29E-construction-real-enqueue-submit
title: Wire Confirmar orden to real backend enqueue
status: obsolete
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Submit to real backend endpoint only when user confirms and preserve backend-as-source-of-truth behavior.

## Context
This introduces first actual mutation flow; it must avoid optimistic updates.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts

## Expected files to modify

- ai/tasks/pending/TASK-29E-construction-real-enqueue-submit.md
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

## Implementation steps

1. Bind confirm button to `enqueueConstructionOrder(...)`.
2. Add submit loading state and disable button during request.
3. Prevent double-click/duplicate enqueue.
4. On success show non-intrusive success and pass-through details only in secondary diagnostics.
5. On failure show Spanish summary and preserve action for retry.
6. Do not update queue/resources prior to success.

## Acceptance criteria

- Backend enqueue occurs only on explicit confirmation.
- No fake success.
- Double submit prevented.

## Resolution notes

- The current `/construction` route already binds the confirmation action to the real backend enqueue flow.
- The existing implementation already disables submit while the request is in flight and does not apply optimistic queue or stockpile updates before the backend confirms success.
- Re-applying this task would duplicate behavior that is already live in the current branch, so no frontend code change was applied here.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
