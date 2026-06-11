# TASK-32E

---
id: TASK-32E
title: Migrate Shipyard confirmation flow to reusable modal
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Adopt the shared gameplay modal for Shipyard production confirmation while preserving the accepted backend-first orbital production flow.

## Context
Shipyard is already technically accepted for persisted production enqueue. This task should only standardize the confirmation UX and must not drift into fleet movement, mission creation, or combat behavior.

## Implementation steps

1. Review the current Shipyard confirmation and submit flow.
2. Replace the existing confirmation presentation with the reusable modal.
3. Keep review and selection non-mutating until the explicit confirm action.
4. Preserve backend-first refresh, fallback handling, and current success and error messaging.
5. Keep raw payloads out of the primary UI and preserve Spanish-first interaction copy.
6. Run the frontend build and copy guard validations.

## Files to read first

- `ai/orchestrator/component-discovery.md`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/components/GameModal.tsx`
- `src/VoidEmpires.Frontend/src/api`
- `docs/dev/shipyard-cockpit-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css` if modal support needs minor styling
- `docs/dev/shipyard-cockpit-checklist.md` only if accepted interaction notes need updating

## Acceptance criteria

- Shipyard uses the shared modal for explicit confirmation.
- Submission occurs only on the modal primary action.
- Backend-first refresh and fallback behavior remain intact.
- No movement, combat, or mission behavior is introduced.
- Frontend build and copy guard validations pass.

## Constraints

- Do not add fleet mission creation.
- Do not add auto-complete.
- Keep changes limited to the confirmation UX and adjacent copy or state wiring.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(shipyard): migrate confirmation to gameplay modal`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Defer any backend contract work to separate tasks.
