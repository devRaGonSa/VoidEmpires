# TASK-35C

---
id: TASK-35C
title: QA script output consistency
status: pending
type: tooling
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Make QA helper output consistent and easier to copy/paste.

## Context
The Development helper scripts should provide predictable sections, useful ids, and copy-ready next commands without placeholder fragments that fail when pasted.

## Implementation steps

1. Review current output from all Development QA helper scripts.
2. Standardize output sections where applicable:
   - warning;
   - target base URL;
   - target civilization/planet;
   - action being performed;
   - result summary;
   - next suggested command or URL.
3. For scripts that print ids, print copy-ready commands without angle-bracket placeholders.
4. Avoid placeholders like `<printed CivilizationId>` unless clearly not meant for direct execution.
5. Ensure materialization script prints processed counts, not-due counts, notes, and suggested next read/check step.
6. Preserve existing parameters and behavior compatibility.

## Files to read first

- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/dev-qa-prepare-construction-ui-state.ps1
- scripts/dev-qa-prepare-research-ui-state.ps1
- scripts/dev-qa-prepare-orbital-production-ui-state.ps1
- scripts/dev-qa-materialize-due-queues.ps1
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/dev-qa-prepare-construction-ui-state.ps1
- scripts/dev-qa-prepare-research-ui-state.ps1
- scripts/dev-qa-prepare-orbital-production-ui-state.ps1
- scripts/dev-qa-materialize-due-queues.ps1
- Optional: scripts/check-dev-qa-scripts.ps1

## Acceptance criteria

- QA script output is clearer and copy-paste safe.
- Scripts parse.
- Existing script behavior remains compatible.
- Validation passes.

## Constraints

- Do not auto-run mutating steps by default.
- Do not perform or claim visual QA.
- Do not change backend endpoint semantics.

## Validation

Before completing the task run:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35C message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
