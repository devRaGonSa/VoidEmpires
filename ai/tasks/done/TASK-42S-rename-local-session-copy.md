# TASK-42S-rename-local-session-copy

---
id: TASK-42S
title: Rename local session copy
status: done
type: frontend
team: frontend
supporting_teams: [docs]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Remove remaining product-facing local session, local game, and new game wording.

## Context
Normal UI should use account/player language such as Cuenta, Comandante, Civilizacion, Mundo principal, and Continuar. Technical local storage mentions may remain only in hidden/operator docs.

## Implementation steps

1. Search frontend source for local-session and new-game wording in Spanish and English.
2. Replace product-facing copy with online account language.
3. Keep operator-only or docs/dev technical mentions only when necessary.
4. Run the copy regression guard and frontend build.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Product-facing forbidden local game/session copy is removed.
- Replacement copy is Spanish-first and aligned with online multiplayer account flow.
- Copy regression guard passes.

## Constraints

- Do not remove operator docs unless they are player-facing.
- Keep diff scoped to copy and guard updates.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
