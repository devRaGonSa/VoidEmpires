# TASK-51BD

---
id: TASK-51BD
title: "Query recent player mission events"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 56
dependencies: ["TASK-51BC-fleet-events-domain.md", "TASK-51AK-fleet-active-api.md", "TASK-51AL-fleet-history-api.md"]
---

## Goal

Expose recent mission events for the player.

Useful for:
- Home activity;
- future notifications.

Bound results.

Tests required.

## Context

Expose bounded recent events through the authenticated fleet boundary already introduced by AI-AK. Follow existing query DTO patterns and keep caller identity as the authorization source.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51BC-fleet-events-domain.md`
- `TASK-51AK-fleet-active-api.md`
- `TASK-51AL-fleet-history-api.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Define an event DTO with friendly summary data, kind, timestamp and safe route references; exclude raw persistence objects, hidden cargo and unrelated player state.
3. Project only the authenticated civilization's recent events with deterministic UTC/id ordering and a server-enforced maximum limit; reject invalid limits and return an empty result normally.
4. Add the read endpoint to the existing fleet module and reuse its dependency registration convention. Do not create a parallel auth mechanism; if a separate DI edit is required, refine the allowlist/budget before editing.
5. Test unauthorized and wrong-civilization requests, empty results, ties, maximum limits, event ordering and redaction.

## Files to read first

- `docs/dev/fleet-mission-engine-v1.md` (created by TASK-51A-fleet-domain-and-existing-system-audit.md; required predecessor output)
- `src/VoidEmpires.Web/AccountEndpoints.cs`
- `src/VoidEmpires.Infrastructure/Fleets/FleetOperationalOverviewService.cs`
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`
- `tests/VoidEmpires.Tests/AccountSessionEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Application/Fleets/IFleetMissionEventQueryService.cs`  (proposed new contract)
- `src/VoidEmpires.Infrastructure/Fleets/FleetMissionEventQueryService.cs`  (proposed implementation)
- `src/VoidEmpires.Web/FleetMissionEndpoints.cs`  (proposed predecessor-owned endpoint module; reconcile AI/AK path and wiring)
- `tests/VoidEmpires.Tests/FleetMissionEventQueryTests.cs`  (proposed new tests)

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- Recent events are bounded, stable and private to the authenticated civilization.
- The result can feed Home and later notifications without coupling it to either presentation.
- No event read changes gameplay or produces duplicate lifecycle events.
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
