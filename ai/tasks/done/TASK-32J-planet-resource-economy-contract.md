# TASK-32J

---
id: TASK-32J
title: Define planet resource economy accrual contract v1
status: pending
type: feature
team: gameplay
supporting_teams: [backend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Specify the first authoritative backend-backed resource accrual model for planets before any resource-economy implementation is added.

## Context
Construction, Research, and Shipyard already spend resources, but Planet is still effectively read-only. This task defines how resources should increase over time, how backend authority is preserved, and whether accrual is computed on read or materialized through an explicit service or endpoint.

## Implementation steps

1. Audit the current resource-storage model, production configuration, and deduction behavior.
2. Determine the safest authoritative accrual strategy consistent with current domain patterns.
3. Define deterministic, testable accrual rules and any starting or base production rates needed for v1.
4. Document intentional limitations such as no frontend fake timer, no premium acceleration, and no visual QA claims.
5. Update the planet and persisted gameplay checklists with the chosen model and constraints.

## Files to read first

- `ai/orchestrator/component-discovery.md`
- `ai/orchestrator/di-analysis.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Infrastructure`

## Expected files to modify

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`

## Acceptance criteria

- The v1 resource accrual rules are documented clearly.
- The chosen backend-authoritative accrual strategy is explicit.
- The limitations and non-goals are written down.
- No runtime behavior changes are introduced yet.

## Constraints

- Keep the backend authoritative.
- Do not add frontend-only increasing timers.
- Do not add uncontrolled mutations on page load unless an existing safe materialization pattern already exists and is documented.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs(tasks): define resource economy contract v1`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Leave implementation details to TASK-32K and TASK-32L.
