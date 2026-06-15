# TASK-35E

---
id: TASK-35E
title: Dev playable session diagnostics endpoint
status: pending
type: backend
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Add a Development-only endpoint for a consolidated playable session diagnostics summary.

## Context
A single read-only diagnostics endpoint should help QA inspect the playable loop without mutating resources, queues, buildings, research, or stock.

## Implementation steps

1. Review Development endpoint conventions.
2. Add an endpoint such as `GET /api/dev/playable-session/diagnostics?civilizationId=...&planetId=...`.
3. Ensure the endpoint is Development-only.
4. Return:
   - `succeeded`;
   - `civilizationId`;
   - `planetId`;
   - planet/resource summary if available;
   - construction queue/open order summary;
   - research queue/open order summary;
   - shipyard queue/open order summary;
   - stock/readiness summary if available;
   - notes/warnings.
5. Ensure it does not mutate state.
6. Fail safely for invalid ids.
7. Add tests for valid seeded ids, invalid ids, no mutation, and unrelated civilization leakage.

## Files to read first

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria

- Consolidated diagnostics endpoint exists.
- Endpoint is Development-only and read-only.
- Invalid ids fail safely.
- Unrelated civilization data is not leaked.
- Tests pass.

## Constraints

- Do not mutate gameplay state.
- Do not add production auth/admin behavior.
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
4. Commit with a clear TASK-35E message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
