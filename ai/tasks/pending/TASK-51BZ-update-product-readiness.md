# TASK-51BZ

---
id: TASK-51BZ
title: "Update fleet product readiness from evidence"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 78
dependencies: ["TASK-51BF-mission-authorization-hardening.md", "TASK-51BG-mission-concurrency-tests.md", "TASK-51BL-existing-orbital-transfer-migration.md", "TASK-51BN-fleet-stock-accounting-invariants.md", "TASK-51BO-resource-accounting-invariants.md", "TASK-51BR-frontend-responsive-fleet-ui.md", "TASK-51BX-fleet-sqlserver-manual-checklist.md", "TASK-51BY-fleet-browser-qa-checklist.md"]
---

## Goal

Update product-readiness docs.

Record:
- fleet movement status;
- mission types implemented;
- colonization status;
- combat still deferred;
- known remaining gaps.

## Context

product-readiness-report.md currently describes the earlier Development-only fleet productization, while the requested block adds an authenticated mission engine. Update claims using predecessor code/tests and distinguish working behavior from manual SQL/browser acceptance.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51BF-mission-authorization-hardening.md`
- `TASK-51BG-mission-concurrency-tests.md`
- `TASK-51BL-existing-orbital-transfer-migration.md`
- `TASK-51BN-fleet-stock-accounting-invariants.md`
- `TASK-51BO-resource-accounting-invariants.md`
- `TASK-51BR-frontend-responsive-fleet-ui.md`
- `TASK-51BX-fleet-sqlserver-manual-checklist.md`
- `TASK-51BY-fleet-browser-qa-checklist.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Inventory implemented Deploy, Transport, Explore/Expedition, Colonize and internal Return behavior against code/tests, including recall, cargo, multi-colony selection and refresh/worker paths.
3. Record actual readiness per capability, authenticated boundary and invariant evidence; list remaining architectural/performance gaps and any task still blocked instead of labeling the whole feature complete.
4. Link the isolated SQL migration/manual script and unexecuted SQL/browser checklists; distinguish ordinary automated checks from operator/manual validation.
5. State combat and all excluded mission types remain deferred. Leave final actual test-count recording to CX and final closure to CZ so this report does not bake in the old 799 estimate.

## Files to read first

- `docs/dev/product-readiness-report.md`
- `ai/current-state.md`
- `docs/dev/fleet-mission-engine-v1.md` (created by TASK-51A-fleet-domain-and-existing-system-audit.md; required predecessor output)
- `docs/dev/fleet-api-contracts.md`
- `docs/dev/fleets-cockpit-checklist.md`
- `docs/dev/sql-server-test-strategy.md`

## Expected files to modify

- `docs/dev/product-readiness-report.md`
- `docs/dev/fleet-api-contracts.md`

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- Readiness accurately names supported mission types, colonization behavior and known limits.
- Combat remains explicitly deferred; SQL apply and browser QA are not asserted without evidence.
- The report links authoritative contracts/checklists and avoids stale expected test totals.
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
- Check every referenced path/contract and readiness claim against the actual predecessor outputs; leave browser/SQL checklist evidence pending unless it was truly collected.
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
