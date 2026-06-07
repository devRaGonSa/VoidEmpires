# TASK-30A

---
id: TASK-30A-research-real-enqueue-contract-audit
title: Research real enqueue contract and safe-boundary audit
status: pending
type: platform
team: platform
supporting_teams: [backend]
roadmap_item: ""
priority: medium
---

## Goal
Document the exact research enqueue API contract and current behavior before any UI mutation is introduced.

## Context
This block starts with a read-only Research cockpit and a known persisted gameplay pattern already implemented in Construction. Before building Research mutation UX, we need a confirmed contract map and backend safety boundary.

## Implementation steps

1. Verify the concrete endpoint in `src/VoidEmpires.Web/DevEndpointMappings.cs`.
2. Map request and response DTOs in the closest application/API layer for research enqueue.
3. Verify command/validation handling in `src/VoidEmpires.Application` with tests in `tests/VoidEmpires.Tests`.
4. Confirm Development-only route visibility and any environment/proxy guard.
5. Inventory current rejection cases:
   - Open order already exists
   - Insufficient resources
   - Unavailable technology / prerequisite missing
   - Invalid civilization
   - Invalid source planet
   - Invalid research type
   - Already researched or level constraints
6. Confirm resource subtraction behavior and whether DB mutation is truly persisted (not optimistic cache-only).
7. Document a safe UI boundary for Research in:
   - `docs/dev/persisted-gameplay-flow-checklist.md`
   - `docs/dev/research-cockpit-checklist.md`

## Files to read first

- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application/**/Research*`
- `tests/VoidEmpires.Tests/**/Research*`
- `scripts/dev-qa-create-research-order.ps1`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Expected files to modify

- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Acceptance criteria

- Endpoint path, payload shape, and response behavior are explicitly documented.
- Safe UI boundary and rejection behavior are documented.
- No gameplay behavior changes occur in this task.
- No user-facing validation bypass is introduced.

## Constraints

- No gameplay code changes.
- No frontend mutation wiring in this task.
- Keep the backend as source of truth.
- Respect Spanish-first copy for all newly documented user-facing guidance.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `docs: audit research enqueue contract before UI mutation`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
