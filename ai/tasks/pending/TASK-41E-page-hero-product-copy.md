# TASK-41E

---
id: TASK-41E
title: Page hero product copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Rewrite all page hero/header copy into concise product language.

## Context
Page headers currently include development cockpit phrases. They should describe in-game command surfaces.

## Implementation steps

1. Inspect all frontend route/page hero and header sections.
2. Remove "Bucle jugable Development", "Endpoints Development", "mutaciones Development", and similar language.
3. Use product concepts such as Centro de mando imperial, Gestión de colonia, Mando de construcción, Laboratorio de investigación, Astillero orbital, Defensa planetaria, and Mando de flotas.
4. Keep copy concise and Spanish-first.
5. Preserve existing behavior and route loading.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/

## Acceptance criteria

- All normal page hero/header copy uses product language.
- Development endpoint/mutation/cockpit language is absent from normal UI.
- Build and copy regression guard pass.

## Constraints

- Do not change backend behavior.
- Do not add fake readiness or fake gameplay claims.
- No browser/manual QA claim.

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
