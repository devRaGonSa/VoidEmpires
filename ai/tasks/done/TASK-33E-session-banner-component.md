# TASK-33E

---
id: TASK-33E
title: Session banner component
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Add a reusable session/context banner for main cockpit pages.

## Context
Planet and cockpit pages need consistent Spanish-first context for the selected Development-safe playable session without showing raw ids as the main experience.

## Implementation steps

1. Review existing component conventions and page styling.
2. Add `src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx` or the nearest equivalent component.
3. Display civilization name and planet name when available.
4. Include concise copy that this is a Development-safe playable session when applicable.
5. Provide actions/links for:
   - `Ir al planeta`
   - `Cambiar/iniciar otra sesión`
   - `Limpiar sesión local`
6. Ensure clearing the session only clears localStorage and does not call backend mutation endpoints.
7. Keep raw ids out of the primary banner.
8. Avoid broad visual redesign and reuse existing styles where possible.

## Files to read first

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Reusable banner compiles.
- Banner displays friendly session context without primary raw ids.
- Clear action only removes local navigation context.
- No page migration is required in this task unless necessary for compilation.

## Constraints

- Do not redesign the cockpit pages.
- Do not add backend mutations.
- Do not introduce production auth claims.
- Keep Spanish-first copy.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33E message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
