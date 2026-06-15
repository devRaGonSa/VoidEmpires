# TASK-34D

---
id: TASK-34D
title: Research due completion materialization
status: pending
type: backend
team: gameplay
supporting_teams: [platform]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Implement backend-authoritative completion of due research orders.

## Context
Research orders can already be enqueued. Due orders must now materialize into real technology level or unlock state exactly once.

## Implementation steps

1. Read research enqueue, persistence, and read-model behavior.
2. Process only research orders whose end/due time is `<= nowUtc`.
3. Apply the technology level or unlock according to the existing domain model.
4. Mark the order completed/closed.
5. Ensure repeated materialization does not apply research twice.
6. Ensure civilization-scoped processing does not affect unrelated civilizations.
7. Confirm and test whether the open-order rule is cleared after completion.
8. Add focused backend tests for due, not-due, repeated, unrelated civilization, and open-order rule behavior.

## Files to read first

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- tests/VoidEmpires.Tests/
- docs/dev/research-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- tests/VoidEmpires.Tests/

## Acceptance criteria

- Due research orders become real research state.
- Not-due research remains open.
- Repeated materialization does not double-apply.
- Unrelated civilizations are unaffected.
- Tests pass.

## Constraints

- Do not add frontend instant-complete controls.
- Do not add production auth or background workers.
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
4. Commit with a clear TASK-34D message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
