# TASK-34C

---
id: TASK-34C
title: Construction due completion materialization
status: pending
type: backend
team: gameplay
supporting_teams: [platform]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Implement backend-authoritative completion of due construction orders.

## Context
Construction orders can already be enqueued. Due orders must now materialize into real building or structure state exactly once.

## Implementation steps

1. Read construction enqueue, persistence, and read-model behavior.
2. Process only construction orders whose end/due time is `<= nowUtc`.
3. Upgrade or apply the target building/structure level according to the existing domain model.
4. Mark the order completed/closed according to the existing persistence model.
5. Ensure repeated materialization does not upgrade twice.
6. Ensure civilization/planet-scoped processing does not affect unrelated planets.
7. Handle invalid or missing targets safely with a note or skipped result rather than uncontrolled mutation.
8. Add focused backend tests for due, not-due, repeated, unrelated planet, and invalid/missing target cases.

## Files to read first

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- tests/VoidEmpires.Tests/
- docs/dev/construction-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- tests/VoidEmpires.Tests/

## Acceptance criteria

- Due construction orders become real building state.
- Not-due orders remain open.
- Repeated materialization does not double-upgrade.
- Unrelated planets are unaffected.
- Tests pass.

## Constraints

- Do not deduct resources again unless existing backend semantics require it and tests document it.
- Do not add frontend instant-complete controls.
- Do not add background workers.
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
4. Commit with a clear TASK-34C message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
