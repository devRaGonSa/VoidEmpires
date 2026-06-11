# TASK-32C

---
id: TASK-32C
title: Migrate Construction confirmation flow to reusable modal
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Replace the current inline or panel confirmation UX in Construction with the reusable modal while preserving the already accepted persisted enqueue behavior.

## Context
Construction already has a real backend-backed enqueue flow. The main risk in this task is accidentally mutating while selecting or reviewing an item, or weakening the current backend-first refresh behavior after confirmation.

## Implementation steps

1. Review the current Construction selection, confirmation, submission, success, and error states.
2. Swap the current confirmation presentation for the shared modal component.
3. Keep modal open and selection state read-only until the explicit primary confirm action runs.
4. Preserve success, failure, no-op, diagnostics, and backend-refresh behavior.
5. Keep Spanish-first copy and avoid raw payloads in the primary UI.
6. Verify copy-regression guard coverage after the migration.

## Files to read first

- `ai/orchestrator/component-discovery.md`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/components/GameModal.tsx`
- `src/VoidEmpires.Frontend/src/api`
- `docs/dev/construction-cockpit-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css` if the modal migration needs minor shared styling support
- `docs/dev/construction-cockpit-checklist.md` only if the accepted interaction notes need updating

## Acceptance criteria

- Construction uses the reusable modal for explicit confirmation.
- Selecting or reviewing a construction candidate does not mutate backend state.
- Submission occurs only on the modal primary action.
- Existing success, error, no-op, backend refresh, and diagnostics behavior remains safe.
- Spanish-first copy is preserved and copy guards pass.

## Constraints

- Do not add auto-complete behavior.
- Do not optimistic-update resources before a backend refresh.
- Keep the backend as the source of truth.
- Do not broaden this task into resource-economy changes.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(construction): migrate confirmation to gameplay modal`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split any backend changes into separate follow-up tasks.
