# TASK-35B

---
id: TASK-35B
title: PowerShell encoding hardening
status: pending
type: tooling
team: platform
supporting_teams: []
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Fix PowerShell script encoding/output issues such as mojibake in Spanish text.

## Context
Recent script validation showed corrupted Spanish output such as `ÃƒÆ’Ã‚Â³rdenes`. QA helper scripts should be UTF-8 safe where feasible without changing endpoint or gameplay semantics.

## Implementation steps

1. Review these scripts:
   - `scripts/dev-qa-prepare-playable-session-state.ps1`;
   - `scripts/dev-qa-prepare-construction-ui-state.ps1`;
   - `scripts/dev-qa-prepare-research-ui-state.ps1`;
   - `scripts/dev-qa-prepare-orbital-production-ui-state.ps1`;
   - `scripts/dev-qa-materialize-due-queues.ps1`;
   - `scripts/check-dev-qa-scripts.ps1`.
2. Ensure source strings use clean Spanish text and are saved as UTF-8.
3. Add safe console encoding initialization if appropriate:
   - `[Console]::OutputEncoding = [System.Text.Encoding]::UTF8`;
   - `$OutputEncoding = [System.Text.Encoding]::UTF8`.
4. Preserve Windows PowerShell 5.1 compatibility unless the repository already requires PowerShell 7.
5. Keep messages clear and Spanish-first.
6. Do not change endpoint or gameplay semantics.

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
- scripts/check-dev-qa-scripts.ps1

## Acceptance criteria

- Scripts parse.
- Spanish warnings no longer contain corrupted strings in source.
- Console encoding setup is safe for supported PowerShell versions.
- `check-dev-qa-scripts.ps1` passes.

## Constraints

- Do not change endpoint semantics.
- Do not add hidden mutations.
- Do not break Windows PowerShell 5.1 compatibility without an explicit repository decision.

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
4. Commit with a clear TASK-35B message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
