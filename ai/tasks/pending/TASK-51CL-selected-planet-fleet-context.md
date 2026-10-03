# TASK-51CL

---
id: TASK-51CL
title: "selected planet fleet context"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 90
dependencies: ["TASK-51CK-countdown-consistency.md", "TASK-51BB-planet-selector-multi-colony.md", "TASK-51AG-fleet-command-ui-state.md", "TASK-51AO-fleet-command-page-rework.md"]
---

## Goal

Ensure Fleet Command respects selected planet.

Changing current planet changes:
- available ships;
- mission origin;
- resource cargo;
- active local context.

## Context

Block54 FleetsPage derives requestedPlanetId from route/account context but its asynchronous reload lacks a stale-response guard. Switching worlds must invalidate composition, cargo, preview and confirmation state before any new origin is submitted.

- This file was created in a planning-only pass; execute gameplay work only in the later ai-platform task run.
- Prerequisites (exact filenames): `TASK-51CK-countdown-consistency.md`, `TASK-51BB-planet-selector-multi-colony.md`, `TASK-51AG-fleet-command-ui-state.md`, `TASK-51AO-fleet-command-page-rework.md`.
- Execute this block by `execution_order`, resolving prerequisites first. Plain filename ordering puts AA before B and is not the requested sequence; `scripts/codex-runner.ps1` does not parse this metadata.
- Planning base: `472c6d29` on the existing Block 54 feature history, which was eight commits ahead of main. Reinspect the actual checkout and predecessor output before execution.
- Older done tasks named TASK-51A, TASK-51B and TASK-51C concern unrelated frontend work; identify this block by complete filenames and roadmap item, never by ID alone.
- `docs/dev/fleet-mission-engine-v1.md` is the shared decision/evidence record produced by TASK-51A. New mission/service/test names below are proposed paths; use the audited equivalent recorded there instead of creating duplicate components.

## Implementation steps

1. Trace selected-planet propagation from the authenticated shell and the authoritative owned-world list into fleet UI-state, local availability and origin resource balances.
2. On planet/civilization change, clear incompatible selected ships, mission preview, cargo and confirmation state; reject late responses from a previous request scope.
3. Ensure launch always uses the current server-authorized origin and current selection snapshot, with availability/cargo bounded by that planet. Keep civilization-wide active mission/slot data clearly distinguished from local stock.
4. Exercise rapid A-to-B-to-A selection, pending preview/launch responses and an invalid/foreign route planet. Verify resources, ships and submission origin never show stale mixed contexts.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/api/fleetTypes.ts`
- `src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs`
- `docs/dev/fleet-mission-engine-v1.md`

Before implementation also follow `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, for services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Read the predecessor's actual mission/query implementation named in the decision record before modifying it.

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- scripts/check-frontend-copy-regressions.ps1
- This exact task file, only for the normal pending -> in-progress -> done lifecycle and status/evidence updates.

If a validation/review task finds a code defect outside this allowlist, document its owning component and create a bounded follow-up instead of silently broadening the task. Do not replace already implemented equivalent files.

## Acceptance criteria

- Changing current planet updates available ships, mission origin, cargo resources and local context together.
- Late responses and open confirmations from a previous planet cannot launch or display a mission using stale origin data.
- Required validation succeeds and results are recorded honestly. No unrelated files or build artifacts are committed.

## Constraints

- Reuse existing Domain/Application/Infrastructure/Web/Frontend boundaries and dependency registration conventions. Backend owns time, authorization, movement state, resources and orbital stock.
- Preserve Spanish-first player copy, authenticated sidebar, top resources, public login/register separation and normal gameplay.
- No combat, attack resolution, random expedition loot, market/trade routes or final images/assets.
- Never apply SQL Server migrations or generated SQL automatically. Normal tests must not require SQL Server. Do not commit secrets, passwords, tokens or real credential-bearing connection strings.
- Keep this task narrow; do not reimplement accepted predecessor work or claim manual/browser QA without actual execution.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- For storage, background-job or service-boundary changes, run configured repository-specific integration tests only if genuinely adapted. The current `scripts/run-integration-tests.ps1` is a placeholder; otherwise log exactly: `No integration tests configured.`
- Run `git diff --check`, `git diff --stat` and `git diff --name-only`; inspect the staged equivalents after staging. Compare every path to Expected files to modify plus this task lifecycle and verify the change budget.

## Commit and push

1. Before starting, inspect `git status`, current branch and upstream; pull only when a configured upstream and the repository workflow make it appropriate.
2. After validation, inspect the intended diff and task/file budget. Stage only this task's allowlisted changes and verified lifecycle moves.
3. Commit with a clear scoped message, then complete the task's move to `ai/tasks/done` with status `done`; ensure the lifecycle move is committed as well.
4. Push the feature branch `codex/block-51a-51cz-fleet-movement-mission-engine-v1` when configured for the remote workflow. Do not merge unrelated feature branches or mark unresolved work complete.

## Change Budget

- Prefer fewer than 5 changed files, under 200 lines of code and fewer than 3 commits per task; include tests/docs and lifecycle changes in the diff review.
- If implementation exceeds this budget, stop and split the smallest remaining work into a follow-up using `ai/task-template.md`; prefer at most 3 follow-ups at once and never duplicate completed tasks.
- This planning pass creates only the exact requested 104 task files; follow-up creation belongs to later execution if evidence requires it.
