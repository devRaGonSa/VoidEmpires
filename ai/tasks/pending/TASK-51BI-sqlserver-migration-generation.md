# TASK-51BI

---
id: TASK-51BI
title: "Generate the additive SQL Server fleet migration"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 61
dependencies: ["TASK-51BH-mission-persistence-indexes.md", "TASK-51BC-fleet-events-domain.md", "TASK-51Y-colony-bootstrap-profile.md"]
---

## Goal

Generate SQL Server migration/model changes for Fleet Mission system.

Requirements:
- migration source code;
- no automatic DB apply;
- preserve current SQL Server provider support;
- normal tests provider-independent.

## Context

SQL Server has an isolated baseline under Persistence/Migrations/SqlServer with SqlServerDesignTimeMigrationsAssembly. Root migrations and checked-in runtime default remain Npgsql; SQL Server is the user's real development target. Generate an additive migration from the accepted SQL snapshot, never a replacement baseline.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51BH-mission-persistence-indexes.md`
- `TASK-51BC-fleet-events-domain.md`
- `TASK-51Y-colony-bootstrap-profile.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Compare the accepted SQL baseline snapshot with current mappings, including existing manual hotfix drift, and identify all proposed differences before generation; do not silently fold unrelated catch-up changes into this migration.
3. Use explicit SQL Server provider selection and the passwordless design-time generation fallback. Generate AddFleetMissionEngineV1 into the isolated SQL Server namespace/output folder; restore environment variables afterward without printing them.
4. Review mission/composition/cargo/events/colony-support schema, UTC/decimal/Guid types, FK/delete rules, constraints and indexes. Verify no Npgsql annotations or accidental full-baseline recreation and no modifications to the root PostgreSQL snapshot.
5. Record the exact generated migration ID and offline command for BJ. Generated changes count toward the budget: measure before completion, and split/refine generation work if needed instead of excluding generated files from review.

## Files to read first

- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs`
- `src/VoidEmpires.Infrastructure/Persistence/SqlServerDesignTimeMigrationsAssembly.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/VoidEmpiresDbContextModelSnapshot.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/20260706131610_SqlServerInitialBaseline.cs`
- `docs/dev/sql-server-migration-strategy.md`
- `tests/VoidEmpires.Tests/PersistenceRegistrationTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/<timestamp>_AddFleetMissionEngineV1.cs`  (proposed generated migration)
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/<timestamp>_AddFleetMissionEngineV1.Designer.cs`  (proposed generated metadata)
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/VoidEmpiresDbContextModelSnapshot.cs`
- `docs/dev/sql-server-migration-strategy.md`

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- Reviewed SQL Server migration source and isolated snapshot represent the intended additive model.
- Normal build/tests remain database-independent and the configured default provider is not silently switched.
- No connection to a real database, database update, automatic migration apply or credential-bearing artifact is performed.
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
