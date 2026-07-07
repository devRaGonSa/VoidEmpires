# TASK-41C

---
id: TASK-41C
title: Remove global development header copy
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Remove product-facing header copy that says Development, prototype, local suite, backend details, or fake dev resources.

## Context
The global shell should read like a game product rather than a development console.

## Implementation steps

1. Inspect the global app shell and header components.
2. Replace development/prototype/backend copy with game-shell language.
3. Show the game title, current civilization or planet when available, and product-like status.
4. Remove fake resource bars or mock user labels unless tied to authoritative backend state.
5. Keep backend state as source of truth and preserve lazy loading.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css
- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Header no longer exposes Development, prototype, local suite, backend profile, provider, localhost, or fake dev resource language.
- Header remains useful as a product shell.
- RaulG/mock user copy is absent unless backed by real session support.
- Copy regression guard passes.

## Constraints

- Do not fake backend state in frontend.
- Do not add production auth.
- Do not add final images or generated assets.

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
