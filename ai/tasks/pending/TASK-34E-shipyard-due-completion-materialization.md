# TASK-34E

---
id: TASK-34E
title: Shipyard due completion materialization
status: pending
type: backend
team: gameplay
supporting_teams: [platform]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Implement backend-authoritative completion of due shipyard/orbital production orders.

## Context
Shipyard production can already be enqueued. Due orders must now materialize into backend-backed stock or inventory exactly once.

## Implementation steps

1. Read shipyard/orbital production enqueue, persistence, stock, and Fleet/readiness read-model behavior.
2. Process only shipyard/orbital production orders whose end/due time is `<= nowUtc`.
3. Increase backend-backed ship/unit stock or inventory according to the existing model.
4. Mark the order completed/closed.
5. Ensure repeated materialization does not add stock twice.
6. Ensure planet/civilization-scoped processing does not affect unrelated planets or civilizations.
7. Add focused backend tests for due, not-due, repeated, unrelated planet, and Fleet/readiness observability if supported.

## Files to read first

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- tests/VoidEmpires.Tests/
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- tests/VoidEmpires.Tests/

## Acceptance criteria

- Due shipyard orders become real stock/inventory state.
- Not-due production remains open.
- Repeated materialization does not double-add stock.
- Unrelated planets/civilizations are unaffected.
- Tests pass.

## Constraints

- Do not add fleet missions, movement, combat, attacks, or auto-complete.
- Do not fake inventory.
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
4. Commit with a clear TASK-34E message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
