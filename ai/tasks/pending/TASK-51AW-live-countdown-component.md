# TASK-51AW

---
id: TASK-51AW
title: Shared live countdown component
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 49
dependencies: ["TASK-51AV-active-movement-cards.md"]
---

## Goal

Create reusable countdown component.

Use for:
- construction;
- research;
- shipyard;
- defense;
- fleet movement.

Input authoritative EndsAtUtc.

Display examples:
- 42s
- 4m 12s
- 1h 03m
- 2d 4h

When reaching zero:
- show “Finalizando…”
- trigger safe state refresh;
- backend determines completion.

Do not materialize gameplay in frontend.

## Context

This file was created in the task-plan pass only; its implementation belongs to a later ai-platform run.

LiveQueueCountdown and utils/countdown.ts already serve construction, research, shipyard, defense and fleet. Extend these shared pieces; do not add a fifth/sixth timer implementation.

Prerequisites: `TASK-51AV-active-movement-cards.md`. Read the decisions produced by `TASK-51A-fleet-domain-and-existing-system-audit.md` before implementation. Match prerequisites by their full filenames: the existing completed TASK-51A/B/C live-queue tasks are unrelated to this mission block. Follow execution_order and prerequisites: A-Z with Y before X for colony bootstrap, then AA-AZ, BA-BZ, CA-CZ; do not use raw lexicographic suffix sorting.

The plan targets the inspected Block 54 fleet foundation at `472c6d29`; those eight commits were ahead of `origin/main` during planning. Reinspect current code and preserve valid changes if the branch has advanced.

## Implementation steps

1. Read the files below and the completed prerequisite contracts. Use `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, for services/entrypoints/wiring, `ai/orchestrator/di-analysis.md` before editing; do not treat the architecture index's PostgreSQL-only description as the current SQL Server target.
2. Audit existing call sites and preserve UTC parsing of Z, explicit offsets and legacy timezone-less SQL Server timestamps.
3. Provide the required compact seconds/minutes/hours/days formatting centrally and show Finalizando… at zero; maintain one safe expiry callback per mission phase/order and allow a new callback for a subsequent return phase.
4. Update the existing executable countdown checks inside the copy-regression guard for positive examples, exact zero, expired/invalid timestamps, offset normalization and boundary transitions. Keep broad call-site conformance for CK.
5. Implement only this slice, preserve the A-decided OrbitalTransfer/OrbitalGroup/OrbitalAssetStock conservation model, and add the focused validation described below. New paths in the expected list are proposed homes; if A selected an existing component instead, document that exact substitution before editing.

## Files to read first

- `src/VoidEmpires.Frontend/src/components/LiveQueueCountdown.tsx`
- `src/VoidEmpires.Frontend/src/utils/countdown.ts`
- `src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx`
- `src/VoidEmpires.Frontend/src/components/ActiveFleetMovements.tsx`
- `scripts/check-frontend-copy-regressions.ps1`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/components/LiveQueueCountdown.tsx`
- `src/VoidEmpires.Frontend/src/utils/countdown.ts`
- `scripts/check-frontend-copy-regressions.ps1`

The lifecycle update/move of this exact task file is the only additional administrative change expected. Do not alter other task files or overwrite the earlier unrelated completed TASK-51A/B/C records. Persistence migrations/scripts are owned by their dedicated later tasks; no database apply belongs here.

## Acceptance criteria

- Shared output covers 42s, 4m 12s, 1h 03m and 2d 4h and uses one consistent Finalizando… expiry text without duplicated due labels.
- Timezone-equivalent timestamps produce the same remaining time; invalid timestamps are safe; repeated renders do not repeat a phase's automatic expiry request.
- Construction/research/shipyard/defense/fleet keep using the shared timer; completion remains determined exclusively by backend refresh.
- Required validation passes, changes are limited to the stated slice, and no build artifacts are committed.

## Constraints

- Extend existing components and the completed prerequisite contracts; do not introduce a competing fleet inventory, mission engine, countdown or authorization system.
- Backend owns movement, timestamps, stock/cargo/resource changes and completion. Keep UTC persisted/serialized consistently, including SQL Server `datetime2` materialization.
- Preserve the authenticated sidebar/top resource bar and public login/register separation. Player-facing UI/errors are Spanish-first; raw IDs and technical diagnostics stay out of normal player flows.
- No combat, final images/assets, market gameplay, real credentials, automatic SQL Server migration application or destructive repair.
- Normal automated tests must not require real SQL Server. Do not claim concurrent relational guarantees from an in-memory-only sequential test.
- Respect dependencies and the small-change budget. If prerequisite contracts or budget make the task too large, stop and refine/split minimally at execution time; this planning pass must add no extra task files.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend` (includes TypeScript validation).
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` and `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`.
- Exercise the concrete interaction/error cases in the acceptance criteria using any configured frontend harness and the relevant existing executable guards. There is no standalone frontend test runner configured at planning time; do not invent a passing test count or claim browser/manual QA unless actually performed.
- This UI/API-client boundary consumes real authenticated services. At planning time `scripts/run-integration-tests.ps1` is an unadapted placeholder: skip it and log exactly `No integration tests configured.` Run any genuinely configured integration checks if they exist when executing.
- Run `git diff --stat` and `git diff --name-only` before completion. Compare every changed file with the expected list, include staged changes when applicable, explain an A-approved path substitution and remove only your unrelated edits.
- Check `git status`, review for secrets and confirm change-budget compliance. A remaining required failure means the task is not complete.

## Commit and push

1. At execution start, inspect the configured upstream and synchronize with `git pull` when the repository workflow requires it; continue on the feature branch, preserving unrelated work.
2. After required validations pass, stage only the scoped implementation/tests and commit with a clear message. Prefer fewer than 3 commits for this task.
3. Move this exact task from in-progress to done and set lifecycle metadata consistently only after completion; commit that administrative update without claiming other pending tasks are done.
4. Push the feature branch when its configured remote workflow expects it. Do not apply SQL Server schema or publish assets as part of pushing.

## Change Budget

- Prefer fewer than 5 implementation files and fewer than 200 changed lines of code, with fewer than 3 commits.
- The expected files above are the focused scope, not permission to expand it. If exceeded, stop implementation and split the smallest necessary follow-up during execution, reusing planned later slices where possible.
- Generate no more than 3 follow-ups at once, do not repeat completed work, and do not create additional tasks during this plan-generation pass.
