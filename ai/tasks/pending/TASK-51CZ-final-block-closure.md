# TASK-51CZ

---
id: TASK-51CZ
title: "final block closure"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 104
dependencies: ["TASK-51CY-clean-task-state.md"]
---

## Goal

Close Block 51.

Final validation:
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1

Also:
git status
dir ai\tasks\pending

Expected final state:
- real fleet launch exists;
- ships are persisted in movement;
- active movements exist;
- countdown exists;
- resource transport exists;
- deployment exists;
- recall exists;
- return exists;
- colonization exists;
- new colonies become playable;
- mission processing is idempotent;
- multi-player isolation is tested;
- SQL Server migration/script is generated but not auto-applied;
- no combat added;
- no secrets committed;
- ai/tasks/pending contains only .gitkeep.

Final output required:
- task completion summary;
- architecture decisions;
- mission types implemented;
- commits;
- final test count;
- frontend build result;
- script validation result;
- SQL Server migration/script paths;
- list of manual SQL actions still required;
- known deferred items;
- explicit statement that combat was NOT implemented;
- explicit statement that manual/browser QA was NOT claimed unless actually performed.

## Context

This is the implementation block's final closure, not the planning pass. Confirm every predecessor and any necessary split task is complete, then run final validation and report evidence. All requested tasks including CZ belong in done only after their own completion is proven.

- This file was created in a planning-only pass; execute gameplay work only in the later ai-platform task run.
- Prerequisites (exact filenames): `TASK-51CY-clean-task-state.md`.
- Execute this block by `execution_order`, resolving prerequisites first. Plain filename ordering puts AA before B and is not the requested sequence; `scripts/codex-runner.ps1` does not parse this metadata.
- Planning base: `472c6d29` on the existing Block 54 feature history, which was eight commits ahead of main. Reinspect the actual checkout and predecessor output before execution.
- Older done tasks named TASK-51A, TASK-51B and TASK-51C concern unrelated frontend work; identify this block by complete filenames and roadmap item, never by ID alone.
- `docs/dev/fleet-mission-engine-v1.md` is the shared decision/evidence record produced by TASK-51A. New mission/service/test names below are proposed paths; use the audited equivalent recorded there instead of creating duplicate components.

## Implementation steps

1. Verify the exact requested task manifest, architecture decision record and validation evidence. Ensure launch, persisted composition/cargo, active movements/countdowns, transport, deploy, explore foundation, recall/return, colonization and playable colonies meet their acceptance criteria.
2. Run every final command in the original Goal below, plus the adapted generated SQL safety check against the fleet deployment script; inspect actual exit codes and counts. Resolve failures before closure without applying SQL or changing unrelated files.
3. Inspect conservation, idempotency, authorization and multi-player coverage; verify mission processing has one authoritative path and no combat execution or secrets were added.
4. Record the final architecture decisions, actual mission types, task summary, implementation commits, test count, frontend/guard results, SQL migration/script paths, manual SQL actions and deferred items. Explicitly state combat was NOT implemented and state whether manual/browser QA was actually performed.
5. Move this task itself from in-progress to done only after successful validation; then verify git status and dir ai\tasks\pending show the intended clean state with only .gitkeep pending. Include that lifecycle move in the final commit and push per repository workflow; never remove unfinished follow-ups to satisfy the check.

## Files to read first

- `ai/current-state.md`
- `docs/dev/fleet-mission-engine-v1.md`
- `docs/dev/product-readiness-report.md`
- `docs/dev/fleet-mission-consistency-recovery.md`
- `scripts/check-sqlserver-generated-script-safety.ps1`
- `AGENTS.md`

Before implementation also follow `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, for services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Read the predecessor's actual mission/query implementation named in the decision record before modifying it.

## Expected files to modify

- ai/current-state.md
- docs/dev/fleet-mission-engine-v1.md
- This exact task file, only for the normal pending -> in-progress -> done lifecycle and status/evidence updates.

If a validation/review task finds a code defect outside this allowlist, document its owning component and create a bounded follow-up instead of silently broadening the task. Do not replace already implemented equivalent files.

## Acceptance criteria

- All expected final gameplay outcomes in the original Goal are supported by implementation and tests, including no duplicate processing and playable secondary colonies.
- Backend/frontend/static validations pass; SQL migration and deployment script are generated/reviewable but have not been auto-applied; final report includes actual counts, commits, paths and manual actions.
- After CZ's own completed lifecycle, the requested Block 51 manifest is in done, pending contains only .gitkeep, and manual/browser QA and combat boundaries are reported honestly.
- Required validation succeeds and results are recorded honestly. No unrelated files or build artifacts are committed.

## Constraints

- Reuse existing Domain/Application/Infrastructure/Web/Frontend boundaries and dependency registration conventions. Backend owns time, authorization, movement state, resources and orbital stock.
- Preserve Spanish-first player copy, authenticated sidebar, top resources, public login/register separation and normal gameplay.
- No combat, attack resolution, random expedition loot, market/trade routes or final images/assets.
- Never apply SQL Server migrations or generated SQL automatically. Normal tests must not require SQL Server. Do not commit secrets, passwords, tokens or real credential-bearing connection strings.
- Keep this task narrow; do not reimplement accepted predecessor work or claim manual/browser QA without actual execution.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- Run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-sqlserver-generated-script-safety.ps1 -ScriptPath "<actual fleet script path from TASK-51BJ>"`; substitute the documented path and inspect the TASK-51BK interface first. This validates text only; do not execute generated SQL.
- For storage, background-job or service-boundary changes, run configured repository-specific integration tests only if genuinely adapted. The current `scripts/run-integration-tests.ps1` is a placeholder; otherwise log exactly: `No integration tests configured.`
- Run `git diff --check`, `git diff --stat` and `git diff --name-only`; inspect the staged equivalents after staging. Compare every path to Expected files to modify plus this task lifecycle and verify the change budget.

## Commit and push

1. Before starting, inspect `git status`, current branch and upstream; pull only when a configured upstream and the repository workflow make it appropriate.
2. After validation, inspect the intended diff and task/file budget. Stage only this task's allowlisted changes and verified lifecycle moves.
3. Commit with a clear scoped message, then complete the task's move to `ai/tasks/done` with status `done`; ensure the lifecycle move is committed as well.
4. Push the feature branch `codex/block-51a-51cz-fleet-movement-mission-engine-v1` when configured for the remote workflow. Do not merge unrelated feature branches or mark unresolved work complete.

## Change Budget

- Prefer fewer than 5 changed files, under 200 lines of code and fewer than 3 commits per task; include tests/docs and lifecycle changes in the diff review.
- If implementation exceeds this budget, stop and split the smallest remaining work into a follow-up using `ai/task-template.md`; prefer at most 3 follow-ups at once and never duplicate completed tasks.
- This planning pass creates only the exact requested 104 task files; follow-up creation belongs to later execution if evidence requires it.
