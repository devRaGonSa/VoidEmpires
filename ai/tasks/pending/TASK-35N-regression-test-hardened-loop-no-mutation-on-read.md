# TASK-35N

---
id: TASK-35N
title: Regression test hardened loop no mutation on read
status: pending
type: backend
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Strengthen tests that diagnostics and read-only flows do not mutate gameplay state.

## Context
The hardened loop adds diagnostics and clearer read states. Tests should protect the line between read-only diagnostics/readiness and explicit materialization mutations.

## Implementation steps

1. Review existing backend tests around materialization, diagnostics, Defenses, and Fleets.
2. Add tests where appropriate:
   - diagnostics endpoint does not mutate resources;
   - diagnostics endpoint does not complete queues;
   - read-only Defenses/Fleets remain non-mutating;
   - materialization endpoint only mutates when explicitly called;
   - no due orders yields safe no-op.
3. Prefer deterministic `nowUtc` and controlled test data.
4. Do not add brittle UI tests unless the project already has a suitable framework.

## Files to read first

- tests/VoidEmpires.Tests/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- tests/VoidEmpires.Tests/
- Optional: src/VoidEmpires.Application/
- Optional: src/VoidEmpires.Infrastructure/

## Acceptance criteria

- Read-only vs mutation boundaries are tested.
- Diagnostics do not mutate resources or queues.
- Materialization mutation remains explicit.
- Tests pass.

## Constraints

- Do not add gameplay scope.
- Do not add frontend test framework unless already present and appropriate.
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
4. Commit with a clear TASK-35N message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
