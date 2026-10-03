# TASK-51BE

---
id: TASK-51BE
title: "Show concise fleet activity on Inicio"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 57
dependencies: ["TASK-51BD-player-event-query.md", "TASK-51AX-home-fleet-movement-summary.md"]
---

## Goal

Optionally display concise recent fleet events on Inicio.

Only if events exist.

Avoid large dashboard noise.

## Context

HomePage already hosts compact module summaries. Extend the real movement summary from AX with a short recent-event list only when useful events exist; avoid restoring removed dashboard clutter.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51BD-player-event-query.md`
- `TASK-51AX-home-fleet-movement-summary.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Consume BD's bounded authenticated event endpoint using the established API client and response types; do not synthesize events from countdowns or local launch clicks.
3. Render at most a small, explicitly chosen number of events with concise Spanish wording, friendly planet labels and occurrence time; omit the whole section for no events.
4. Refresh after authoritative fleet state refresh, preserve active-movement prominence and avoid duplicate requests or stale cross-account results.
5. Keep network failure local to the event section without blocking Inicio or exposing raw errors. Verify the conditional empty state through build/static guards; record visual checks as deferred unless actually run.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/HomePage.tsx`
- `src/VoidEmpires.Frontend/src/components/ActiveFleetMovements.tsx`
- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
- `src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts`
- `scripts/check-frontend-copy-regressions.ps1`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/HomePage.tsx`
- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
- `src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts`
- `src/VoidEmpires.Frontend/src/components/RecentFleetEvents.tsx`  (proposed new component)

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- Inicio displays real recent fleet events only when present and remains usable if the event query fails.
- GUIDs, endpoint URLs and backend exceptions are absent from primary copy.
- Active movement countdowns retain their existing behavior and no large empty event panel is rendered.
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
- Run `npm run build --prefix src/VoidEmpires.Frontend`, `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` and `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
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
