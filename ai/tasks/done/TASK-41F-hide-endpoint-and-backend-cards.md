# TASK-41F

---
id: TASK-41F
title: Hide endpoint and backend cards
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Remove or hide primary UI cards showing backend URL, provider profile, endpoint details, or localhost information.

## Context
Technical endpoint details belong in hidden diagnostics/operator surfaces, not in normal player-facing UI.

## Implementation steps

1. Search frontend pages and components for endpoint, backend URL, provider profile, localhost, and raw technical detail cards.
2. Remove these details from primary UI or move them behind the operator-mode contract.
3. Keep the health endpoint and internal scripts technical where appropriate.
4. Ensure normal UI does not mention localhost, backend profile, provider name, or endpoint URLs.
5. Preserve read and mutation behavior.

## Files to read first

- docs/dev/product-mode-visibility-contract.md
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Normal UI no longer shows backend URL, provider profile, localhost, or endpoint URLs.
- Technical details remain available only where hidden or internal.
- Copy regression guard passes.

## Constraints

- Do not remove health endpoint behavior.
- Do not commit secrets or real connection strings.
- Do not break SQL Server support.

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
