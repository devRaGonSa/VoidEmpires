# TASK-51AC

---
id: TASK-51AC
title: Gameplay refresh fleet integration
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 29
dependencies: ["TASK-51R-fleet-arrival-materialization-service.md", "TASK-51AB-return-materialization.md"]
---

## Goal

Integrate due mission materialization into normal gameplay refresh.

Existing GameplayRefreshService should be audited.

Normal game reads should:
- accrue resources;
- complete construction;
- complete research;
- complete production;
- materialize due fleet missions.

Must remain idempotent.

Avoid repeated N+1 work where possible.

Tests required.

## Context

This file was created in the task-plan pass only; its implementation belongs to a later ai-platform run.

GameplayRefreshService already accrues resources then materializes construction/research/production. Block 54 Fleet UI reads separately call the global transfer processor when planetId exists. Integrate the new engine through one scoped refresh path after account authorization.

Prerequisites: `TASK-51R-fleet-arrival-materialization-service.md`, `TASK-51AB-return-materialization.md`. Read the decisions produced by `TASK-51A-fleet-domain-and-existing-system-audit.md` before implementation. Match prerequisites by their full filenames: the existing completed TASK-51A/B/C live-queue tasks are unrelated to this mission block. Follow execution_order and prerequisites: A-Z with Y before X for colony bootstrap, then AA-AZ, BA-BZ, CA-CZ; do not use raw lexicographic suffix sorting.

The plan targets the inspected Block 54 fleet foundation at `472c6d29`; those eight commits were ahead of `origin/main` during planning. Reinspect current code and preserve valid changes if the branch has advanced.

## Implementation steps

1. Read the files below and the completed prerequisite contracts. Use `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, for services/entrypoints/wiring, `ai/orchestrator/di-analysis.md` before editing; do not treat the architecture index's PostgreSQL-only description as the current SQL Server target.
2. Extend the existing gameplay refresh request/result with fleet materialization only as needed; choose and document ordering so time boundaries, cargo delivery and production accrue consistently at the supplied UTC instant.
3. Invoke civilization-scoped outbound and return processing in batches and retain construction, research, shipyard and defense behavior; selected-planet filtering must not strand that civilization's return missions.
4. Replace the Fleet UI endpoint's global completion shortcut with the scoped refresh contract; authenticated routes added later must validate identity before calling this path. Avoid per-mission stock/query loops and duplicate read refresh.
5. Implement only this slice, preserve the A-decided OrbitalTransfer/OrbitalGroup/OrbitalAssetStock conservation model, and add the focused validation described below. New paths in the expected list are proposed homes; if A selected an existing component instead, document that exact substitution before editing.

## Files to read first

- `src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs`
- `src/VoidEmpires.Application/Gameplay/IGameplayRefreshService.cs`
- `src/VoidEmpires.Infrastructure/Gameplay/GameplayQueueMaterializationService.cs`
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`
- `tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs`
- `src/VoidEmpires.Application/Gameplay/IGameplayRefreshService.cs`
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`
- `tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs`

The lifecycle update/move of this exact task file is the only additional administrative change expected. Do not alter other task files or overwrite the earlier unrelated completed TASK-51A/B/C records. Persistence migrations/scripts are owned by their dedicated later tasks; no database apply belongs here.

## Acceptance criteria

- Normal owned-world refresh progresses due queues/resources and fleet phases without requiring the worker; future timestamps and other civilizations' missions remain untouched.
- Repeated refresh and overlapping worker/read processing conserve ships, cargo and resources, including when both outbound and return are already due.
- No untrusted civilization/planet identifier triggers world-wide materialization; failures remain observable without claiming successful arrival.
- Required validation passes, changes are limited to the stated slice, and no build artifacts are committed.

## Constraints

- Extend existing components and the completed prerequisite contracts; do not introduce a competing fleet inventory, mission engine, countdown or authorization system.
- Backend owns movement, timestamps, stock/cargo/resource changes and completion. Keep UTC persisted/serialized consistently, including SQL Server `datetime2` materialization.
- Preserve the authenticated sidebar/top resource bar and public login/register separation. Player-facing UI/errors are Spanish-first; raw IDs and technical diagnostics stay out of normal player flows.
- No combat, final images/assets, market gameplay, real credentials, automatic SQL Server migration application or destructive repair.
- Normal automated tests must not require real SQL Server. Do not claim concurrent relational guarantees from an in-memory-only sequential test.
- Respect dependencies and the small-change budget. If prerequisite contracts or budget make the task too large, stop and refine/split minimally at execution time; this planning pass must add no extra task files.

## Validation

- `dotnet build --no-restore`.
- `dotnet test --no-build` including the focused test cases described above; use deterministic clocks and independent contexts for retry/concurrency evidence.
- For storage/API/worker boundaries, inspect current integration configuration. At planning time `scripts/run-integration-tests.ps1` is an unadapted placeholder, so skip it and log exactly `No integration tests configured.` If genuine repository integration tests are configured by execution time, run them successfully before completion. EF InMemory alone is not evidence of relational race/rollback guarantees; use the shared relational test strategy established in the block without requiring real SQL Server.
- Run `git diff --stat` and `git diff --name-only` before completion. Compare every changed file with the expected list, include staged changes when applicable, explain an A-approved path substitution and remove only your unrelated edits.
- Check `git status`, review for secrets and confirm change-budget compliance. A remaining required failure means the task is not complete.

## Commit and push

1. At execution start, inspect the configured upstream and synchronize with `git pull` when the repository workflow requires it; continue on the feature branch, preserving unrelated work.
2. After required validations pass, stage only the scoped implementation/tests and commit with a clear message. Prefer fewer than 3 commits for this task.
3. Move this exact task from in-progress to done and set lifecycle metadata consistently only after completion; commit that administrative update without claiming other pending tasks are done.
4. Push the feature branch when its configured remote workflow expects it. Do not apply SQL Server schema or publish assets as part of pushing.

## Change Budget

- Prefer fewer than 5 implementation files and fewer than 200 changed lines of code, with fewer than 3 commits.
- The expected files above are the focused scope, not permission to expand it. If exceeded, stop implementation and split the smallest necessary follow-up during execution, reusing planned later slices where possible.
- Generate no more than 3 follow-ups at once, do not repeat completed work, and do not create additional tasks during this plan-generation pass.
