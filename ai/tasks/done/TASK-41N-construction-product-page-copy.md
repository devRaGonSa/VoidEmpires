# TASK-41N

---
id: TASK-41N
title: Construction product page copy
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Construction page product-facing.

## Context
Construction is a real playable system in the current shell and should use game language while preserving existing backend behavior.

## Implementation steps

1. Inspect Construction page sections, cards, modals, diagnostics, and error states.
2. Remove development copy from primary UI.
3. Rename sections to Cola de obras, Catálogo de edificios, Edificios actuales, and Recursos disponibles.
4. Keep the confirmation modal and authoritative refresh behavior.
5. Preserve existing gameplay semantics.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Construction page copy is product-facing.
- Confirmation modal remains intact.
- No development copy appears in product mode.
- Copy regression guard passes.

## Constraints

- Do not change resource, queue, or building semantics.
- Do not optimistic-update authoritative resources, queues, or buildings.
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
