# TASK-34G

---
id: TASK-34G
title: Materialization QA helper script
status: pending
type: tooling
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Add a PowerShell helper for Development queue progression/completion QA.

## Context
Developers need a clear script that calls the Development-only materialization endpoint and warns that it mutates Development database state.

## Implementation steps

1. Read existing dev QA helper scripts for conventions.
2. Add `scripts/dev-qa-materialize-due-queues.ps1`.
3. Default backend base URL to `http://localhost:5142`.
4. Accept `BaseUrl`, `CivilizationId`, and optional `PlanetId`.
5. Accept `ElapsedSeconds` or `NowUtc` according to the endpoint contract.
6. Accept switches `IncludeConstruction`, `IncludeResearch`, and `IncludeShipyard`, or default to all supported queues.
7. Print a concise materialization summary.
8. Warn clearly in Spanish: `Este script modifica la base de datos de Development materializando órdenes vencidas.`
9. Fail cleanly if the backend is unreachable or returns non-success.
10. Add parser/check coverage in `scripts/check-dev-qa-scripts.ps1`.
11. Do not replace existing QA preparation scripts.

## Files to read first

- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/dev-qa-prepare-construction-ui-state.ps1
- scripts/dev-qa-prepare-research-ui-state.ps1
- scripts/dev-qa-prepare-orbital-production-ui-state.ps1
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- scripts/dev-qa-materialize-due-queues.ps1
- scripts/check-dev-qa-scripts.ps1
- Optional: docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- Script parses.
- QA helper can call the Development endpoint.
- It warns clearly about Development database mutation.
- `check-dev-qa-scripts.ps1` passes.

## Constraints

- Do not make the script perform browser QA.
- Do not replace existing preparation helpers.
- Do not expose production cheating semantics.

## Validation

Before completing the task run:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34G message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
