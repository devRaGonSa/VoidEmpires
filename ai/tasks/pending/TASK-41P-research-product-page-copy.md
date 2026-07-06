# TASK-41P

---
id: TASK-41P
title: Research product page copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Research page product-facing.

## Context
Research is a real playable system in the current shell and should use scientific game language without endpoint notes.

## Implementation steps

1. Inspect Research page sections, cards, confirmation modal, diagnostics, and endpoint notes.
2. Use product labels: Laboratorio, Cola de investigación, Tecnologías disponibles, Tecnologías bloqueadas, and Progreso científico.
3. Remove development copy and endpoint notes from product mode.
4. Keep the confirmation modal and backend behavior.
5. Preserve authoritative refresh behavior after mutations.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Research page uses requested product language.
- Development and endpoint notes are absent from product mode.
- Confirmation modal and backend behavior remain intact.
- Copy regression guard passes.

## Constraints

- Do not change research queue/resource semantics.
- Do not optimistic-update authoritative research state.
- No raw ids in primary UI.

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
