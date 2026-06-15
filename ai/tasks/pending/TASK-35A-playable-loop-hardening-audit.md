# TASK-35A

---
id: TASK-35A
title: Playable loop hardening audit
status: pending
type: platform
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Audit the current end-to-end playable loop and identify hardening gaps before making behavior changes.

## Context
The playable loop is technically complete through queue materialization. This block should stabilize scripts, diagnostics, copy, and deferred QA preparation without adding a new gameplay domain.

## Implementation steps

1. Review the technical loop:
   - onboarding start creation;
   - local session persistence;
   - Planet hub;
   - resource materialization;
   - construction enqueue;
   - research enqueue;
   - shipyard enqueue;
   - due queue materialization;
   - post-materialization refresh/read state.
2. Identify duplicated diagnostics, inconsistent copy, unclear empty states, script encoding issues, and unclear QA steps.
3. Review whether QA scripts consistently:
   - warn about Development DB mutation;
   - use UTF-8 clean output;
   - handle backend offline;
   - print useful ids;
   - fail with actionable messages.
4. Document hardening scope and boundaries in:
   - `docs/dev/persisted-gameplay-flow-checklist.md`;
   - `docs/dev/frontend-foundation-smoke-checklist.md`.
5. Make no gameplay or runtime behavior changes.

## Files to read first

- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- scripts/check-dev-qa-scripts.ps1
- scripts/dev-qa-materialize-due-queues.ps1
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Hardening scope is documented.
- Script/copy/diagnostic gaps are identified.
- No gameplay behavior is changed.
- No visual QA is performed or claimed.
- Validation passes.

## Constraints

- Do not add new gameplay domains.
- Do not add production auth, login, WebSockets, workers, combat, movement, missions, market, alliances, or espionage execution.
- Do not add instant-complete buttons or auto-complete on normal page load.
- Backend remains source of truth.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35A message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
