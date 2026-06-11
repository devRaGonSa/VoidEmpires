# TASK-32K

---
id: TASK-32K
title: Implement backend-backed planet resource accrual v1
status: pending
type: feature
team: gameplay
supporting_teams: [backend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Implement deterministic, testable backend logic that allows planet resources to increase over elapsed time without relying on frontend simulation.

## Context
The repository already has resource deduction paths for gameplay queues. This task adds authoritative resource accrual that integrates with the current read model and keeps repeated reads or materialization from double-counting.

## Implementation steps

1. Implement the chosen accrual strategy from TASK-32J in the appropriate application, domain, and infrastructure layers.
2. Ensure resource snapshots remain non-negative and coherent when accrual and deductions interact.
3. Integrate the new logic into the existing planet resource read model or explicit refresh flow.
4. Add tests for elapsed-time gains, zero or negative elapsed time, deduction-plus-accrual coherence, unrelated planet isolation, and repeated refresh safety.
5. Keep the resulting behavior deterministic and easy to reason about for QA and future tasks.

## Files to read first

- `ai/orchestrator/component-discovery.md`
- `ai/orchestrator/di-analysis.md`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Domain`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

## Expected files to modify

- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Domain/*`
- `src/VoidEmpires.Infrastructure/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- Planet resources can genuinely increase through backend logic.
- Repeated reads or materialization do not double-count incorrectly.
- Existing construction, research, and shipyard deductions continue to work coherently.
- Automated tests cover the new accrual behavior and pass.

## Constraints

- Do not add frontend fake timers.
- Preserve backend authority and non-negative guarantees.
- Keep the implementation deterministic and testable.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(resources): add backend planet accrual v1`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If the backend surface exceeds budget, split read-model wiring or test breadth into follow-up tasks.
