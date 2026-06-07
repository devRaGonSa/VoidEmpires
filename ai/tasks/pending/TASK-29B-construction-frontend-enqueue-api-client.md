# TASK-29B

---
id: TASK-29B-construction-frontend-enqueue-api-client
title: Add typed frontend enqueue API client
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Add explicit frontend types and a typed function for calling the existing persisted construction enqueue endpoint.

## Context
No UI mutation occurs in this task. API shape and error parsing must be defined for later tasks.

## Files to read first

- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/api/

## Expected files to modify

- ai/tasks/pending/TASK-29B-construction-frontend-enqueue-api-client.md
- src/VoidEmpires.Frontend/src/api/planetTypes.ts

## Implementation steps

1. Add request type, success response type, failure response type, orderId, startsAtUtc, endsAtUtc, and error entry types.
2. Implement `enqueueConstructionOrder(...)` client function.
3. Call the existing backend endpoint identified in TASK-29A.
4. Preserve raw error details in typed structure while keeping primary UI unaffected.
5. Ensure non-2xx responses are not swallowed.
6. Parse 400/409 style responses into typed failure shape.

## Acceptance criteria

- Typed API model and function exist.
- No UI behavior changes.
- npm build passes.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
