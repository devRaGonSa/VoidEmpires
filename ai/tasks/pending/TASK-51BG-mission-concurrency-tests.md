# TASK-51BG

---
id: TASK-51BG
title: "Exercise mission races and idempotency"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 59
dependencies: ["TASK-51P-orbital-stock-reservation-hardening.md", "TASK-51Q-resource-cargo-reservation-hardening.md", "TASK-51R-fleet-arrival-materialization-service.md", "TASK-51X-colonization-arrival.md", "TASK-51AA-fleet-recall-service.md", "TASK-51AB-return-materialization.md", "TASK-51AC-gameplay-refresh-fleet-integration.md", "TASK-51AD-optional-fleet-worker.md"]
---

## Goal

Add heavy concurrency/idempotency coverage.

Cases:
- same mission arrival processed twice;
- simultaneous refresh requests;
- worker + read refresh race;
- two colonizers reach same planet;
- two fleet launches compete for same ship stock;
- two launches compete for same resources;
- recall races with arrival.

No duplicates or negative stock.

## Context

The ordinary test project uses EF InMemory, which does not prove relational locks or isolation. Add deterministic competing-context/service tests around the actual claim strategy and explicitly separate logical invariants from manual SQL Server evidence.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51P-orbital-stock-reservation-hardening.md`
- `TASK-51Q-resource-cargo-reservation-hardening.md`
- `TASK-51R-fleet-arrival-materialization-service.md`
- `TASK-51X-colonization-arrival.md`
- `TASK-51AA-fleet-recall-service.md`
- `TASK-51AB-return-materialization.md`
- `TASK-51AC-gameplay-refresh-fleet-integration.md`
- `TASK-51AD-optional-fleet-worker.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Use fixed UTC times and barriers/fault injection to force competing logical operations through independent service scopes/contexts; never share one DbContext across threads or rely on sleeps.
3. Cover duplicate arrival, parallel refresh, worker/read race, competing colonizers, stock contention, resource/fuel contention and recall/arrival conflict. Assert one winner or defined retry result.
4. After each race, reload persisted state and verify one ownership, one delivery, exact ship totals, no negative balances, no duplicate events and terminal/return state consistent with the winning transition.
5. Test the claim/concurrency mechanism directly with a provider-neutral fixture or repository-supported relational substitute; document the remaining SQL isolation gap and manual checklist rather than enabling real SQL in ordinary tests.

## Files to read first

- `tests/VoidEmpires.Tests/OrbitalTransferCompletionServiceTests.cs`
- `tests/VoidEmpires.Tests/OrbitalStockGroupServiceTests.cs`
- `tests/VoidEmpires.Tests/ResourceSpendServiceTests.cs`
- `tests/VoidEmpires.Tests/GameplayRefreshServiceTests.cs`
- `tests/VoidEmpires.Tests/VoidEmpires.Tests.csproj`
- `docs/dev/sql-server-test-strategy.md`

## Expected files to modify

- `tests/VoidEmpires.Tests/FleetMissionConcurrencyTests.cs`  (proposed new tests)
- `tests/VoidEmpires.Tests/FleetMissionConcurrencyFixture.cs`  (proposed only if needed)
- `docs/dev/fleet-mission-engine-v1.md` (created by TASK-51A-fleet-domain-and-existing-system-audit.md; required predecessor output)
- `docs/dev/sql-server-test-strategy.md`

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- All listed races have deterministic assertions and repeatable success in the normal test suite.
- Sequential double-invocation coverage is not mislabeled as concurrent database validation.
- No ordinary test requires SQL Server, external credentials or an opt-in worker running indefinitely.
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
