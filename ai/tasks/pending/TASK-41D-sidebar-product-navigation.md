# TASK-41D

---
id: TASK-41D
title: Sidebar product navigation
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make sidebar navigation product-facing.

## Context
Primary navigation should present the game surface, not the development cockpit terminology.

## Implementation steps

1. Inspect sidebar/navigation configuration and route labels.
2. Remove "bucle jugable", "preparacion", "solo lectura", and similar dev-like wording where inappropriate.
3. Use product labels: Inicio, Galaxia, Planeta, Construcción, Investigación, Astillero, Defensas, Flotas, Espionaje, Alianza, Mercado, Ranking.
4. For unavailable systems, use subtle lock/readiness indicators without technical language.
5. Preserve route params, routing behavior, and lazy loading.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Sidebar labels match product terminology.
- Unavailable systems are clearly but non-technically marked.
- No normal navigation item exposes development/test/prototype language.
- Route lazy-import guard passes.

## Constraints

- Do not remove internal routes unless explicitly safe.
- Do not add gameplay mutations.
- Keep UI Spanish-first.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

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
