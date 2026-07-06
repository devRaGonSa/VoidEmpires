# TASK-41I

---
id: TASK-41I
title: Dev diagnostics secondary only
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Ensure diagnostics panels are collapsed, hidden, or secondary and not part of normal gameplay hierarchy.

## Context
Primary cards should never show raw ids, raw JSON, endpoint payloads, or technical backend details.

## Implementation steps

1. Locate diagnostics panels, raw JSON renderers, raw id displays, and payload details.
2. Move them to operator mode or collapsed secondary details.
3. Remove raw ids from primary cards and normal labels.
4. Keep technical details useful for operators without polluting product mode.
5. Preserve API calls and authoritative refresh behavior.

## Files to read first

- docs/dev/product-mode-visibility-contract.md
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Product mode primary UI contains no raw ids, raw JSON, or endpoint payloads.
- Diagnostics are operator-only or collapsed secondary details.
- Copy regression guard passes.

## Constraints

- Do not fake or hide actual gameplay failures.
- Do not remove useful internal diagnostics from operator paths.
- Keep UI Spanish-first.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
