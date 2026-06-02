# TASK-10K

---
id: TASK-10K
title: Phase 10K - Cancel and complete orbital transfer lifecycle hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10K"
priority: medium
---

## Goal
Harden cancel and complete-due transfer flows so transfer lifecycle state remains consistent and testable.

## Context
Target the cancel and complete-due flows, including endpoints or services, group status handling, current planet updates, and the tests that exercise transfer lifecycle transitions.

## Implementation steps

1. Inspect the cancel transfer service or endpoint, the complete-due service or endpoint, group updates, and the current tests for cancellation and completion.
2. Add or tighten tests for cancellation safety, status transitions, and rejection cases.
3. Add or tighten tests for due completion, idempotency, and cases that should remain incomplete.

## Files to read first

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

## Expected files to modify

- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Web/*`
- `src/VoidEmpires.Infrastructure/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- Active transfers can be cancelled and move to the expected cancelled state.
- Due transfers complete and update the group current planet and status correctly.
- Rejected or premature flows do not mutate state.
- Repeated complete-due calls remain idempotent.
- No combat, interception, ETA worker, frontend changes, or EF migrations.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end: run `git status`, stage the intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
