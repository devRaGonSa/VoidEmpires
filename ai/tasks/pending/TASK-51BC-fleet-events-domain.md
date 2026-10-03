# TASK-51BC

---
id: TASK-51BC
title: "Persist concise mission lifecycle events"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 55
dependencies: ["TASK-51B-fleet-mission-domain-model.md", "TASK-51R-fleet-arrival-materialization-service.md", "TASK-51S-deploy-mission-arrival.md", "TASK-51T-transport-mission-arrival.md", "TASK-51AB-return-materialization.md", "TASK-51X-colonization-arrival.md", "TASK-51AA-fleet-recall-service.md"]
---

## Goal

Add lightweight player event records for mission lifecycle.

Events:
- Fleet launched
- Fleet arrived
- Cargo delivered
- Fleet returned
- Fleet recalled
- Colony founded
- Mission failed if applicable

Do not create full notification system yet if unnecessary.

## Context

Create lightweight player events at the authoritative mission transition boundary selected in the audit. This is a lifecycle record, not a notification delivery system; event persistence must participate in the same operation as its state transition.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51B-fleet-mission-domain-model.md`
- `TASK-51R-fleet-arrival-materialization-service.md`
- `TASK-51S-deploy-mission-arrival.md`
- `TASK-51T-transport-mission-arrival.md`
- `TASK-51AB-return-materialization.md`
- `TASK-51X-colonization-arrival.md`
- `TASK-51AA-fleet-recall-service.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Inspect the aggregate/transition seam actually produced by B/R/AB and use it to append typed events for launch, arrival, delivery, return, recall, colony foundation and meaningful failure.
3. Persist mission/civilization association, event kind, UTC occurrence, phase/leg identity and only safe deterministic display data. Keep event uniqueness tied to a logical transition so retries cannot append duplicates.
4. Save transition and event together; an aborted arrival or failed transaction must not leave a successful event. Use owned/navigation mapping and existing configuration discovery instead of creating another processing engine.
5. Test each lifecycle kind, repeated transitions, rollback/failure behavior and cross-civilization ownership. If wiring touches many handlers, stop and split that work before exceeding the task budget.

## Files to read first

- `docs/dev/fleet-mission-engine-v1.md` (created by TASK-51A-fleet-domain-and-existing-system-audit.md; required predecessor output)
- `src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferCompletionService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `tests/VoidEmpires.Tests/OrbitalTransferCompletionServiceTests.cs`

## Expected files to modify

- `src/VoidEmpires.Domain/Fleets/FleetMission.cs`  (proposed predecessor-owned aggregate; use the actual B path)
- `src/VoidEmpires.Domain/Fleets/FleetMissionEvent.cs`  (proposed new file)
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/FleetMissionEventConfiguration.cs`  (proposed new file)
- `tests/VoidEmpires.Tests/FleetMissionEventTests.cs`  (proposed new file)

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- Lifecycle events are durable and attributable to the correct civilization and mission.
- A committed logical transition produces one event; retries and concurrent completion do not duplicate records.
- No email, push notifications, combat event system or broad dashboard implementation is introduced.
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
