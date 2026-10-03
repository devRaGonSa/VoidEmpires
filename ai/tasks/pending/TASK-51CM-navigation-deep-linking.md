# TASK-51CM

---
id: TASK-51CM
title: "navigation deep linking"
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: medium
execution_order: 91
dependencies: ["TASK-51CL-selected-planet-fleet-context.md", "TASK-51AY-strategic-map-fleet-actions.md", "TASK-51AQ-fleet-destination-selector.md", "TASK-51AM-fleet-destination-api.md"]
---

## Goal

Support safe deep links from Galaxy to Fleet Command with destination preselected.

No raw data trust:
backend revalidates destination.

## Context

buildFleetsUrl currently carries civilizationId and planetId; the selected map destination must be a separate hint so it cannot accidentally replace the owned origin. Earlier TASK-51AY may already implement the path; this task closes gaps and verifies it.

- This file was created in a planning-only pass; execute gameplay work only in the later ai-platform task run.
- Prerequisites (exact filenames): `TASK-51CL-selected-planet-fleet-context.md`, `TASK-51AY-strategic-map-fleet-actions.md`, `TASK-51AQ-fleet-destination-selector.md`, `TASK-51AM-fleet-destination-api.md`.
- Execute this block by `execution_order`, resolving prerequisites first. Plain filename ordering puts AA before B and is not the requested sequence; `scripts/codex-runner.ps1` does not parse this metadata.
- Planning base: `472c6d29` on the existing Block 54 feature history, which was eight commits ahead of main. Reinspect the actual checkout and predecessor output before execution.
- Older done tasks named TASK-51A, TASK-51B and TASK-51C concern unrelated frontend work; identify this block by complete filenames and roadmap item, never by ID alone.
- `docs/dev/fleet-mission-engine-v1.md` is the shared decision/evidence record produced by TASK-51A. New mission/service/test names below are proposed paths; use the audited equivalent recorded there instead of creating duplicate components.

## Implementation steps

1. Trace Galaxy's selected-planet action to the Fleet route helper and destination selector; use the explicit destinationPlanetId/mission hint contract agreed in TASK-51AY.
2. Parse deep links defensively, preserve the authenticated selected origin, and resolve any preselection through the backend's visible destination/capability response.
3. Ignore or show a concise Spanish error for malformed, hidden, stale, same-origin or now-owned destinations. No URL field authorizes identity, mission type, ship quantity, cargo or launch.
4. Test or statically guard round-trip link generation, URL encoding and user changes after preselection; confirm the launch endpoint revalidates the destination at execution time.

## Files to read first

- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetMovementComposer.tsx`
- `docs/dev/fleet-mission-engine-v1.md`

Before implementation also follow `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, for services/entrypoints/wiring, `ai/orchestrator/di-analysis.md`. Read the predecessor's actual mission/query implementation named in the decision record before modifying it.

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- scripts/check-frontend-copy-regressions.ps1
- This exact task file, only for the normal pending -> in-progress -> done lifecycle and status/evidence updates.

If a validation/review task finds a code defect outside this allowlist, document its owning component and create a bounded follow-up instead of silently broadening the task. Do not replace already implemented equivalent files.

## Acceptance criteria

- Galaxy can open Fleet Command with a valid destination preselected while preserving the owned origin.
- Untrusted URL values cannot bypass visibility, ownership or mission eligibility and never automatically launch a mission.
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
