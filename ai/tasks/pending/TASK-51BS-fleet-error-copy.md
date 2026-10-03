# TASK-51BS

---
id: TASK-51BS
title: "Use clear Spanish fleet errors"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 71
dependencies: ["TASK-51N-fleet-launch-validation.md", "TASK-51AI-fleet-launch-api.md", "TASK-51AJ-fleet-recall-api.md", "TASK-51AN-frontend-fleet-api-client.md", "TASK-51AO-fleet-command-page-rework.md", "TASK-51AZ-colonization-ui-flow.md"]
---

## Goal

Add Spanish product-facing errors.

Examples:
- No hay naves seleccionadas.
- No tienes suficientes naves.
- Combustible insuficiente.
- Capacidad de carga insuficiente.
- No quedan espacios de flota.
- Ese planeta ya tiene propietario.
- Necesitas una nave colonial.
- No puedes desplegar en un planeta ajeno.
- La misión ya no puede retirarse.

No backend exception text in primary UI.

## Context

Map the backend's structured fleet error codes in the established presentation layer. Existing fleetCommandPresentation and accountErrorCopy demonstrate local error handling; avoid surfacing English legacy service exception strings.

Execution prerequisites (resolve by full filename in the task folders, not a bare numeric ID or lexical filename order):

- `TASK-51N-fleet-launch-validation.md`
- `TASK-51AI-fleet-launch-api.md`
- `TASK-51AJ-fleet-recall-api.md`
- `TASK-51AN-frontend-fleet-api-client.md`
- `TASK-51AO-fleet-command-page-rework.md`
- `TASK-51AZ-colonization-ui-flow.md`

Use the mission architecture decisions in `docs/dev/fleet-mission-engine-v1.md` from the new audit task. Existing completed TASK-51A/B/C files belong to older work and do not satisfy these prerequisites. Proposed paths below are names for predecessor/new outputs, not claims that those files already exist; reconcile them with the actual predecessor design before implementation.

## Implementation steps

1. Read `AGENTS.md` and `ai/architecture-index.md` before discovery, then the listed files; apply `ai/orchestrator/component-discovery.md` and, for changed services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Confirm the prerequisites are complete.
2. Inventory N/AI/AJ structured validation codes and define concise Spanish product copy for empty composition, unavailable ships, fuel, cargo capacity, slots, ownership, colony ship and expired recall.
3. Associate errors with relevant fields where possible and retain backend authority; UI validation improves feedback but never permits a launch rejected by the server.
4. Provide safe generic Spanish text for unknown/server/network errors while retaining diagnostic codes only in existing operator tooling; do not interpolate raw exceptions or endpoint URLs.
5. Document code-to-copy mapping for BT guard coverage and verify errors do not introduce combat actions, misleading refunds or optimistic stock state.

## Files to read first

- `src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/accountErrorCopy.ts`
- `src/VoidEmpires.Frontend/src/components/FleetMovementComposer.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `scripts/check-frontend-copy-regressions.ps1`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts`
- `src/VoidEmpires.Frontend/src/components/FleetMovementComposer.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `docs/dev/fleet-api-contracts.md`

The current task's own status/lifecycle metadata and authorized move between task folders are also expected. Do not change unrelated task files. Resolve proposed paths to concrete files before editing; if the necessary scope changes, explain the required allowlist adjustment and split work before exceeding budget.

## Acceptance criteria

- The requested example cases have clear Spanish copy and match actual backend error semantics.
- Unknown failures have a safe fallback with no raw exception text in primary UI.
- Error presentation preserves selection for correction and does not conceal authoritative launch/recall failure.
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
