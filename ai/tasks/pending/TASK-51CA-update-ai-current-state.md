# TASK-51CA

---
id: TASK-51CA
title: "update ai current state"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 79
dependencies: ["TASK-51BZ-update-product-readiness.md", "TASK-51A-fleet-domain-and-existing-system-audit.md", "TASK-51BL-existing-orbital-transfer-migration.md"]
---

## Goal

Update ai/current-state.md comprehensively.

Include:
- architecture;
- fleet mission model;
- movement rules;
- mission lifecycle;
- SQL Server migration status;
- tests;
- exclusions.

## Context

The latest architecture index still describes an early PostgreSQL foundation, while the implemented development target is SQL Server. Existing current-state notes also contain historical transfer-only and pre-Block54 descriptions. Update the current truth without erasing history or claiming future validation.

- This file was created in a planning-only pass; execute gameplay work only in the later ai-platform task run.
- Prerequisites (exact filenames): `TASK-51BZ-update-product-readiness.md`, `TASK-51A-fleet-domain-and-existing-system-audit.md`, `TASK-51BL-existing-orbital-transfer-migration.md`.
- Execute this block by `execution_order`, resolving prerequisites first. Plain filename ordering puts AA before B and is not the requested sequence; `scripts/codex-runner.ps1` does not parse this metadata.
- Planning base: `472c6d29` on the existing Block 54 feature history, which was eight commits ahead of main. Reinspect the actual checkout and predecessor output before execution.
- Older done tasks named TASK-51A, TASK-51B and TASK-51C concern unrelated frontend work; identify this block by complete filenames and roadmap item, never by ID alone.
- `docs/dev/fleet-mission-engine-v1.md` is the shared decision/evidence record produced by TASK-51A. New mission/service/test names below are proposed paths; use the audited equivalent recorded there instead of creating duplicate components.

## Implementation steps

1. Read the Block 51 audit and completed task evidence; trace the chosen mission aggregate, composition, cargo, movement legs, legacy OrbitalTransfer/OrbitalGroup integration, authoritative queries and authenticated endpoints.
2. Replace or clearly supersede current-state claims that contradict the implemented architecture. Summarize departure, due arrival, transport overflow, deploy, exploration, recall, return, colony bootstrap and failure semantics with links to the audit decision record.
3. Record migration and generated SQL paths, manual application status, executed tests and deferred browser/SQL QA. Distinguish generated/reviewed from applied and tested against SQL Server.
4. List explicit exclusions including combat, random expedition rewards, trade routes, market movement and final assets; retain the authenticated shell and Spanish-first product scope.

## Files to read first

- `ai/current-state.md`
- `ai/architecture-index.md`
- `docs/dev/fleet-mission-engine-v1.md`
- `docs/dev/product-readiness-report.md`
- `src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs`

Before implementation also follow `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, for services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Read the predecessor's actual mission/query implementation named in the decision record before modifying it.

## Expected files to modify

- ai/current-state.md
- This exact task file, only for the normal pending -> in-progress -> done lifecycle and status/evidence updates.

If a validation/review task finds a code defect outside this allowlist, document its owning component and create a bounded follow-up instead of silently broadening the task. Do not replace already implemented equivalent files.

## Acceptance criteria

- Current-state documentation accurately describes the final persisted fleet model, mission lifecycle, movement rules, integration boundaries and exclusions.
- Every claim about tests, migration application or manual/browser QA is tied to actual evidence; no baseline number is copied as an expectation.
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
- Verify documentation against current code and predecessor evidence; run `git diff --check` and inspect links/paths. No manual/browser or SQL validation is implied.
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
