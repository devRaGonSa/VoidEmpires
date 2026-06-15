# TASK-34F

---
id: TASK-34F
title: Development materialization endpoint
status: pending
type: backend
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Expose a Development-only endpoint for explicit due queue materialization.

## Context
Queue completion must be explicit and controlled in this phase. A Development-only endpoint lets QA materialize due orders without adding production cheating semantics.

## Implementation steps

1. Review existing Development endpoint conventions in `DevEndpointMappings.cs`.
2. Add an endpoint such as `POST /api/dev/queues/materialize-due`.
3. Support request fields:
   - `civilizationId`;
   - optional `planetId`;
   - optional `nowUtc` or safe `elapsedSeconds`;
   - `includeConstruction`;
   - `includeResearch`;
   - `includeShipyard`.
4. Ensure the endpoint is Development-only using the repository's existing convention.
5. Call the shared materialization service.
6. Return summary fields:
   - `succeeded`;
   - processed counts;
   - skipped counts;
   - notes;
   - relevant resulting ids if useful.
7. Add tests for scoped due processing, environment availability when convention supports it, idempotent repeated calls, and unrelated civilization safety.

## Files to read first

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Expected files to modify

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- tests/VoidEmpires.Tests/

## Acceptance criteria

- Explicit Development materialization path exists.
- Endpoint calls the shared backend service.
- Endpoint does not expose production cheating behavior.
- Tests pass.

## Constraints

- Do not make the endpoint available in production.
- Do not add normal UI instant-complete behavior.
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
4. Commit with a clear TASK-34F message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
