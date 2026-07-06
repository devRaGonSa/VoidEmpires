# TASK-41AC

---
id: TASK-41AC
title: Product status badges
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Standardize product-facing badges.

## Context
Status badges should not expose Development, QA, Solo desarrollo, or Read-only in normal UI.

## Implementation steps

1. Search pages and components for status badges and badge-like labels.
2. Replace dev/test badges with product statuses: Disponible, En curso, Bloqueado, Pendiente, Preparado, Requiere recursos, and Próximamente.
3. Keep technical badges only in operator mode.
4. Avoid misleading readiness claims.
5. Update the copy guard if needed.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css
- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Product mode uses standardized status badges.
- Technical/dev badges are operator-only.
- Copy regression guard passes.

## Constraints

- Do not obscure real errors or blocked states.
- Do not fake availability.
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
