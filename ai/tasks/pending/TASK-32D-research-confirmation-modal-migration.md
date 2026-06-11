# TASK-32D

---
id: TASK-32D
title: Migrate Research confirmation flow to reusable modal
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Move the Research confirmation UX onto the shared modal foundation without weakening the current persisted enqueue and conflict-handling behavior.

## Context
Research already supports a backend-first enqueue path and a safe no-op or conflict flow when an open order exists. This task must preserve those guarantees while making the confirmation UI consistent with the other gameplay cockpits.

## Implementation steps

1. Review the current Research page selection, confirmation, submit, and refresh flow.
2. Replace the current confirmation surface with the shared modal component.
3. Preserve the explicit user-confirm boundary so only the modal primary action mutates state.
4. Keep the current conflict and open-order behavior safe, Spanish, and user-readable.
5. Preserve selected candidate state on failure where the current UX depends on it.
6. Validate the page build and copy guard behavior.

## Files to read first

- `ai/orchestrator/component-discovery.md`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/components/GameModal.tsx`
- `src/VoidEmpires.Frontend/src/api`
- `docs/dev/research-cockpit-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css` if only minor modal support is needed
- `docs/dev/research-cockpit-checklist.md` only if accepted interaction notes need updating

## Acceptance criteria

- Research uses the reusable modal for explicit confirmation.
- Submission occurs only through the modal primary action.
- Open-order conflict and no-op behavior remains safe and Spanish-first.
- Success still refreshes from the backend instead of assuming frontend state.
- Raw payloads stay out of the primary UI.

## Constraints

- Do not add auto-complete.
- Do not change backend contracts in this frontend task unless strictly required and separately justified.
- Keep the existing diagnostics behavior limited to secondary technical areas.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(research): migrate confirmation to gameplay modal`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split backend or QA helper changes into later tasks.
