# TASK-30Q

---
id: TASK-30Q
title: Research QA state preparation contract
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 30Q-30T - Research manual QA state preparation"
priority: medium
---

## Goal
Audit the persisted research queue and define a safe Development-only contract for preparing the seeded manual QA state without changing production behavior.

## Context
Manual `/research` QA is currently blocked on reused Development databases because additive seed reapply preserves an existing open research order. We need a documented safe preparation path that clears only the blocking state for the seeded research QA target and preserves normal gameplay semantics.

## Implementation steps

1. Audit where research orders and research readiness are stored and queried.
2. Define the safe Development-only preparation boundary for the seeded civilization and planet.
3. Document the contract and the intended QA result in the persisted gameplay and research cockpit checklists.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- src/VoidEmpires.Infrastructure/Research/ResearchQueueService.cs
- docs/dev/research-cockpit-checklist.md

## Expected files to modify

- ai/tasks/in-progress/TASK-30Q-research-qa-state-preparation-contract.md
- ai/tasks/pending/TASK-30R-research-qa-state-preparation-endpoint-and-tests.md
- ai/tasks/pending/TASK-30S-research-qa-preparation-powershell-helper.md
- ai/tasks/pending/TASK-30T-final-research-qa-preparation-closure.md
- docs/dev/research-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- The research QA preparation boundary is documented as Development-only.
- The intended seeded civilization and planet result is explicit.
- The contract does not introduce production behavior or destructive seed reset rules.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
