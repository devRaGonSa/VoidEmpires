# TASK-51BF

---
id: TASK-51BF
title: "Harden all fleet authorization boundaries"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 58
dependencies: ["TASK-51AI-fleet-launch-api.md", "TASK-51AJ-fleet-recall-api.md", "TASK-51AK-fleet-active-api.md", "TASK-51AL-fleet-history-api.md", "TASK-51AM-fleet-destination-api.md", "TASK-51BD-player-event-query.md", "TASK-51X-colonization-arrival.md"]
---

## Goal

Audit authorization across every new fleet endpoint.

A user must never:
- launch another civilization's fleet;
- recall another player's mission;
- inspect hidden cargo;
- deploy to someone else's planet;
- colonize owned planet;
- manipulate another player's resources.

Add negative tests.

## Context

Existing legacy fleet routes are Development-only and accept civilization identifiers, while AccountEndpoints provides the real account session. Audit every new read and mutation endpoint, including preview/UI state/events and any still-reachable legacy bypass.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51AI-fleet-launch-api.md`
- `TASK-51AJ-fleet-recall-api.md`
- `TASK-51AK-fleet-active-api.md`
- `TASK-51AL-fleet-history-api.md`
- `TASK-51AM-fleet-destination-api.md`
- `TASK-51BD-player-event-query.md`
- `TASK-51X-colonization-arrival.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Build an endpoint matrix covering launch, preview, recall, active/history/destinations/events/UI state and any production-accessible legacy routes; identify the authenticated account-to-civilization check for each.
3. Add adversarial tests swapping civilization, planet and mission IDs between two authenticated users and attempting unauthenticated access.
4. Verify service rules reject deploying to a foreign world, colonizing any owned world, launching another player's stock, modifying foreign stockpiles and reading hidden fleet composition/cargo.
5. Repair the shared boundary/validator first so individual endpoints cannot drift; preserve intentionally gated developer tooling while preventing it from becoming a normal-player bypass. Document any remaining targeted follow-up before exceeding budget.

## Files to read first

- `src/VoidEmpires.Web/AccountEndpoints.cs`
- `src/VoidEmpires.Web/Program.cs`
- `src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs`
- `tests/VoidEmpires.Tests/DevOrbitalTransferEndpointTests.cs`
- `tests/VoidEmpires.Tests/AccountSessionEndpointTests.cs`
- `docs/dev/fleet-mission-engine-v1.md` (created by TASK-51A-fleet-domain-and-existing-system-audit.md; required predecessor output)

## Expected files to modify

- `src/VoidEmpires.Web/FleetMissionEndpoints.cs`  (proposed predecessor-owned module; reconcile actual fleet endpoint files)
- `src/VoidEmpires.Infrastructure/Fleets/FleetLaunchValidationService.cs`  (proposed predecessor-owned validator; reconcile N)
- `tests/VoidEmpires.Tests/FleetMissionAuthorizationTests.cs`  (proposed new tests)
- `docs/dev/fleet-mission-engine-v1.md` (created by TASK-51A-fleet-domain-and-existing-system-audit.md; required predecessor output)

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- All new fleet surfaces authorize the caller before returning private data or mutating gameplay.
- Rejected requests leave missions, resources, ownership and ship stock unchanged.
- Negative tests cover both direct calls and ID tampering; user-visible errors reveal no private opponent state.
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
