# TASK-34L

---
id: TASK-34L
title: Idempotency and safety regression tests
status: pending
type: backend
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Strengthen tests around queue completion idempotency and scope safety.

## Context
Queue materialization is state-changing and must be deterministic. Consolidated regression coverage should protect the core safety guarantees after domain-specific completion is implemented.

## Implementation steps

1. Review tests added for Construction, Research, Shipyard, and Development endpoint materialization.
2. Add consolidated tests where useful for:
   - construction not double-completed;
   - research not double-completed;
   - shipyard not double-completed;
   - scoped materialization does not touch unrelated civilization;
   - materialization with no due orders is a safe no-op;
   - materialization after enqueue but before due time is a no-op.
3. Prefer backend tests over frontend tests for domain guarantees.
4. Avoid brittle timing by injecting `nowUtc` where possible.
5. Keep tests focused and aligned with existing test style.

## Files to read first

- tests/VoidEmpires.Tests/
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/

## Expected files to modify

- tests/VoidEmpires.Tests/
- Optional: src/VoidEmpires.Application/
- Optional: src/VoidEmpires.Infrastructure/

## Acceptance criteria

- Idempotency and scope safety are well covered.
- No-due and not-yet-due cases are safe no-ops.
- Tests avoid wall-clock flakiness.
- Backend tests pass.

## Constraints

- Do not add frontend UI in this task.
- Do not broaden gameplay scope.
- Do not perform or claim visual QA.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34L message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
