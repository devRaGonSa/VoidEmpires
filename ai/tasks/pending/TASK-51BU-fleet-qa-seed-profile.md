# TASK-51BU

---
id: TASK-51BU
title: "Extend the internal fleet QA seed"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 73
dependencies: ["TASK-51E-ship-capability-metadata.md", "TASK-51Y-colony-bootstrap-profile.md", "TASK-51BC-fleet-events-domain.md", "TASK-51BM-existing-orbital-group-integration.md"]
---

## Goal

Extend internal QA seed profile to support fleet testing.

Seed:
- origin planet;
- another owned planet;
- one unowned colonizable planet;
- several ship types;
- enough fuel/resources;
- optional completed history.

Internal only.

Normal player registration must remain unaffected.

## Context

DevelopmentSeedService already has an additive fleet-validation profile with deterministic group/transfer state. Extend that profile's authoritative mission scenario and metadata; never apply it to normal registration or automatically to SQL Server.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51E-ship-capability-metadata.md`
- `TASK-51Y-colony-bootstrap-profile.md`
- `TASK-51BC-fleet-events-domain.md`
- `TASK-51BM-existing-orbital-group-integration.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Reuse the fleet-validation profile with documented deterministic IDs for an origin, a second owned planet and an unowned colonizable destination visible under the real destination rules.
3. Seed several valid ship types including colony capability, enough fuel/resources and mandatory planetary baseline records; keep any completed history optional and clearly separated from active missions.
4. Preserve additive/idempotent reapply semantics: do not reset missions, erase ownership or refund spent cargo/fuel. Define how a previously colonized target affects reruns and recommend a fresh disposable database when necessary.
5. Test profile isolation and repeat application using ordinary test persistence; verify registration's home-planet bootstrap and production startup never execute the fleet QA seed.

## Files to read first

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Application/Development/IDevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevDevelopmentSeedEndpointTests.cs`
- `docs/dev/development-seed-profiles.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Application/Development/IDevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `docs/dev/development-seed-profiles.md`

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- The internal fixture can exercise transport, deploy, recall and colonization with real contracts.
- Reapplying does not duplicate ships/resources/missions or steal an existing colony.
- The profile is Development-only, documented and not run against a real database by this task.
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
