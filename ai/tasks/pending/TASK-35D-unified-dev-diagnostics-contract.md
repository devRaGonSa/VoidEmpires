# TASK-35D

---
id: TASK-35D
title: Unified dev diagnostics contract
status: pending
type: backend
team: platform
supporting_teams: [gameplay, frontend]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Define a safe diagnostics contract for Development-only gameplay state summaries.

## Context
Development diagnostics should make the playable loop easier to inspect without becoming production auth, an admin panel, or a mutating endpoint.

## Implementation steps

1. Audit existing Development endpoints that return summary data.
2. Define a safe Development-only diagnostics response shape for a playable session:
   - `civilizationId`;
   - `planetId`;
   - resources;
   - open construction order summary;
   - open research order summary;
   - open shipyard order summary;
   - last materialization summary if available;
   - read-only readiness notes for defenses/fleets;
   - warnings/limitations.
3. Document that diagnostics are read-only, Development-only, and not production auth/admin behavior.
4. Document that raw ids are allowed in dev diagnostics but should remain secondary in UI.
5. Update the persisted gameplay checklist.
6. Do not implement the endpoint unless trivial and clearly within budget.

## Files to read first

- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- Optional: docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Diagnostics scope is documented.
- Response shape and safety boundaries are clear.
- No production behavior is introduced.
- Backend validation passes.

## Constraints

- Do not add production auth or admin panel behavior.
- Diagnostics must not mutate gameplay state.
- Raw ids remain secondary in UI plans.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35D message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
