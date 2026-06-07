# TASK-30B

---
id: TASK-30B-research-frontend-enqueue-api-client
title: Add typed Research enqueue API client
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Create a strictly typed frontend API client method for research enqueue, including success and known validation-failure shapes.

## Context
Frontend must call the existing backend endpoint only through typed interfaces. This enables safe parsing and prevents mutation from bypassing backend truth.

## Implementation steps

1. Inspect `src/VoidEmpires.Frontend/src/api/researchTypes.ts` and extend it with:
   - enqueue request
   - enqueue success response
   - enqueue error response
   - orderId
   - startsAtUtc
   - endsAtUtc
   - error codes/messages
   - open-order no-op response shape if backend returns it
2. In `src/VoidEmpires.Frontend/src/api/researchApi.ts`, add `enqueueResearchOrder(...)`.
3. Reuse existing endpoint base settings and avoid custom wrappers.
4. Ensure non-2xx responses are not swallowed and map known 400/409 payloads to typed discriminated cases.
5. Keep raw backend payload fields separate from primary UI copy paths.

## Files to read first

- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/api/shipyardApi.ts`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`

## Acceptance criteria

- Typed enqueue API function exists and calls existing backend endpoint.
- Non-2xx responses are handled as typed failures.
- 409/400 validation paths are represented without stringly-typed parsing.
- No unrelated gameplay behavior change.

## Constraints

- Do not mutate UI in this task.
- Do not display raw IDs in primary UX in this layer.
- Keep behavior aligned with current API client style.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `feat(frontend): add typed research enqueue API client`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
