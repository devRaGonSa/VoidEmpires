# TASK-29S

---
id: TASK-29S
title: Construction QA preparation PowerShell helper
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 29Q-29T"
priority: medium
---

## Goal
Add a development-only PowerShell helper that calls the new preparation endpoint and prints a concise summary.

## Context
Manual QA currently requires a brittle pre-check sequence. This helper should standardize preparation and make the backend mutation explicit and obvious.

## Implementation steps

1. Add `scripts/dev-qa-prepare-construction-ui-state.ps1` with default base URL and seeded IDs.
2. Show the required warning about Development DB mutation.
3. fail clearly when backend is unreachable or call fails.
4. parse and print summary (succeeded, blocking order counts, optional resource before/after).
5. include the script in `scripts/check-dev-qa-scripts.ps1`.
6. add the command to the relevant doc checklist.

## Files to read first

- `scripts/dev-qa-baseline.ps1`
- `scripts/dev-qa-create-construction-order.ps1`
- `scripts/dev-qa-common.ps1`
- `scripts/check-dev-qa-scripts.ps1`

## Expected files to modify

- `scripts/dev-qa-prepare-construction-ui-state.ps1`
- `scripts/check-dev-qa-scripts.ps1`
- `docs/dev/construction-cockpit-checklist.md`

## Acceptance criteria

- Helper runs with default parameters and prints concise success or clear error.
- Required warning message is shown.
- Script is included in parser/check validation.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
