# TASK-34H

---
id: TASK-34H
title: Frontend materialization API contract
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: medium
---

## Goal
Add a typed frontend API contract for explicit Development materialization where appropriate.

## Context
Planet may expose a Development-scoped manual action in a later task. The frontend needs typed request/response handling before UI wiring.

## Implementation steps

1. Read existing frontend API client conventions.
2. Add a typed API function for the Development materialization endpoint if UI will expose the manual action.
3. Define request and response types including:
   - processed counts;
   - skipped counts;
   - notes;
   - errors or failure details.
4. Ensure non-2xx responses are not swallowed.
5. Keep raw backend payload available only for diagnostics or secondary technical display.
6. Avoid page UI changes unless required for compilation.

## Files to read first

- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/package.json

## Expected files to modify

- src/VoidEmpires.Frontend/src/api/
- Optional: src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Frontend can call the materialization endpoint in a typed way.
- Non-2xx responses surface as errors.
- No page UI changes are introduced unless necessary.
- Frontend build passes.

## Constraints

- Do not fake completion in frontend.
- Do not optimistic-update queues, stock, research, buildings, or resources.
- Do not add production auth.
- Do not claim visual QA.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34H message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
