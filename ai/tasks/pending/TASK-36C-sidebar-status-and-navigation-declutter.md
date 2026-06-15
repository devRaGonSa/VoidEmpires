# TASK-36C

---
id: TASK-36C
title: Sidebar status and navigation declutter
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Update sidebar/navigation to distinguish playable, Development-only, and read-only/preparation pages.

## Context
The sidebar still carries obsolete read-only messaging and does not clearly distinguish the playable Development loop from readiness-only pages.

## Implementation steps

1. Find sidebar/navigation code.
2. Remove or update obsolete copy such as `Solo están habilitadas las rutas de lectura...`.
3. Group or annotate playable Development loop pages:
   - Nuevo inicio;
   - Planeta;
   - Construcción;
   - Investigación;
   - Astillero.
4. Group or annotate readiness/read-only pages:
   - Defensas;
   - Flotas;
   - Espionaje;
   - Mercado;
   - Alianza;
   - Ranking;
   - Ejército Tierra if applicable.
5. Preserve existing routes and session/query handoffs.
6. Avoid making the sidebar visually huge.
7. Keep Spanish-first copy.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- Optional: src/VoidEmpires.Frontend/src/components/
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Sidebar copy reflects real current state.
- Playable vs read-only pages are clearer.
- Routes and session handoffs continue working.
- Frontend build and route guard pass.

## Constraints

- Do not add gameplay mutations.
- Do not remove routes.
- Preserve lazy loading.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36C message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
