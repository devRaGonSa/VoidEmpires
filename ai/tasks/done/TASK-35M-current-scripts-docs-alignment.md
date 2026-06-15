# TASK-35M

---
id: TASK-35M
title: Current scripts docs alignment
status: pending
type: docs
team: platform
supporting_teams: [frontend]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: medium
---

## Goal
Align docs with current scripts and remove stale command placeholders.

## Context
Docs and task artifacts should be copy-paste safer, especially around PowerShell commands and generated ids.

## Implementation steps

1. Search docs and task documentation for stale command placeholders such as:
   - `<printed CivilizationId>`;
   - `<printed HomePlanetId>`;
   - `<guid>`;
   - malformed copy-paste commands that would error in PowerShell.
2. Replace risky placeholders with:
   - explicit example values;
   - clear prose;
   - copy-safe command templates with variable assignment.
3. Ensure major helper scripts are documented:
   - `dev-qa-prepare-playable-session-state.ps1`;
   - `dev-qa-materialize-due-queues.ps1`;
   - `dev-qa-get-playable-session-diagnostics.ps1`;
   - `dev-qa-playable-loop-guide.ps1`.
4. Do not alter gameplay behavior.

## Files to read first

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/current-state.md
- ai/tasks/done/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- Optional: docs/dev/deferred-visual-qa-master-checklist.md
- Optional: ai/current-state.md

## Acceptance criteria

- Docs are copy-paste safer.
- Major helper scripts are documented.
- Copy regression guard passes.
- No gameplay behavior changes are made.

## Constraints

- Do not edit completed task files unless repository conventions allow documentation cleanup there.
- Do not perform browser/visual QA.
- Keep command examples PowerShell-safe.

## Validation

Before completing the task run:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35M message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
