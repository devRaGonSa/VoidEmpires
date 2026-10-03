# TASK-51BJ

---
id: TASK-51BJ
title: "Generate a review-only idempotent SQL Server fleet script"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 62
dependencies: ["TASK-51BI-sqlserver-migration-generation.md"]
---

## Goal

Generate idempotent/manual-review SQL Server deployment script.

Path:
artifacts/sqlserver/

Include:
- new mission tables;
- composition;
- cargo;
- indexes;
- constraints;
- event tables if added.

Do not include secrets.

## Context

scripts/sqlserver-script-migration.ps1 currently scripts only SqlServerInitialBaseline and defaults to artifacts/sql, while the accepted baseline artifact and safety guard live in artifacts/sqlserver. Extend the existing helper conservatively for the reviewed fleet migration.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51BI-sqlserver-migration-generation.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Add explicit reviewed from/to migration inputs to the current generation-only helper while preserving its safe baseline defaults and environment restoration; reject unexpected migration targets/output extensions.
3. Generate the fleet delta with --idempotent into artifacts/sqlserver using BI's exact IDs and passwordless design-time metadata; include manual-review/no-auto-execution header and required migration history checks.
4. Review tables, composition, cargo, lifecycle events, constraints and indexes against the migration, with clear prerequisites for baseline and any acknowledged schema hotfixes. Do not embed a USE database switch or real connection details.
5. Update static helper checks and review documentation. Handoff the script path to BK for generalized safety validation; if existing baseline-specific guard cannot accept the delta yet, document that limitation rather than claiming it passed.

## Files to read first

- `scripts/sqlserver-script-migration.ps1`
- `scripts/check-dev-qa-scripts.ps1`
- `scripts/check-sqlserver-generated-script-safety.ps1`
- `docs/dev/sql-server-migration-strategy.md`
- `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql`

## Expected files to modify

- `scripts/sqlserver-script-migration.ps1`
- `artifacts/sqlserver/VoidEmpires_Dev_FleetMissionEngineV1.sql`  (proposed generated script)
- `docs/dev/sql-server-migration-review.md`
- `scripts/check-dev-qa-scripts.ps1`

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- The requested SQL Server script exists under artifacts/sqlserver and includes idempotent history guards for the exact reviewed delta.
- Generating it cannot apply SQL, run seed, connect to the user's server or log secrets.
- Baseline scripting remains supported; manual target selection/review remains explicit and repeat execution is a deferred operator check.
- The implementation or documented decision satisfies every requirement in Goal; unresolved dependencies are recorded honestly rather than marked implemented.
- Relevant validation succeeds, no build artifacts are committed, and the final diff stays within the approved task scope.

## Constraints

- Follow the existing layered architecture and extend the authoritative fleet components; do not create competing movement/accounting engines.
- Keep gameplay decisions on the backend, deterministic UTC timing, persisted idempotency, concurrency protection and multiplayer isolation. Frontend countdowns/controls must not materialize gameplay or mutate stock.
- Keep Spanish-first player-facing UI, authenticated sidebar/resource bar and public login/register separation.
- No combat, excluded mission types, market behavior, final images/assets, secrets, passwords, tokens or real connection strings.
- SQL Server is the real development persistence target; checked-in defaults and root migration history currently use Npgsql. Preserve provider support and isolated SQL Server artifacts. Never automatically apply migrations/generated SQL, run database update, or seed a real database.
- Ordinary automated tests must not require SQL Server; InMemory tests do not prove SQL transaction/isolation behavior. Do not claim manual/browser QA unless actually performed.

## Validation

- Run `dotnet build --no-restore` and `dotnet test --no-build`; report actual failures/counts rather than the historical expected baseline.
- Run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` and `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`; helpers must be checked offline without invoking real mutations.
- For persistence/service/worker boundaries, inspect the integration configuration. `scripts/run-integration-tests.ps1` is currently an unadapted placeholder: do not treat it as integration evidence. Unless a genuine adapted suite has since been configured, record exactly: `No integration tests configured.` Run any genuinely configured integration suite before completion.
- Run `git diff --stat` and `git diff --name-only`; compare changes with Expected files to modify, inspect the diff, and verify file/line/commit budgets before marking complete. Include staged changes when reviewing an already staged task.

## Commit and push

1. Synchronize with `git pull` before starting only when the branch has a configured upstream and the repository workflow calls for it; preserve unrelated user changes.
2. Run `git status`, stage only the intended files for this task after successful validation, and review `git diff --cached --stat`.
3. Commit with a clear task-specific message on `codex/block-51a-51cz-fleet-movement-mission-engine-v1`; keep related task commits together on that branch.
4. Move this task to `ai/tasks/done` only when its acceptance/validation requirements are met, preserving the filename and recording its completed status.
5. Commit the lifecycle move if needed and push the configured feature-branch upstream when the repository workflow expects automatic pushes. Do not mix another task's gameplay changes into this commit.

## Change Budget

- Prefer fewer than 5 modified files, under 200 changed lines of code and fewer than 3 commits for this task.
- Include generated code in the measured diff; do not hide snapshot/script churn from review.
- If the necessary work exceeds limits, stop implementation, refine the remaining scope into a focused follow-up under the repository task template, then continue in that task. Do not implement a broad cross-task refactor here.
- This planning pass creates this pending file only; executing these implementation/lifecycle steps belongs to a later implementation run.
