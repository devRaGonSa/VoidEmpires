# TASK-32N

---
id: TASK-32N
title: Keep onboarding and cockpit navigation handoffs coherent
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: medium
---

## Goal
Ensure the new onboarding flow and the existing Planet, Construction, Research, Shipyard, Defenses, and Fleets navigation paths all preserve the ids needed for a coherent playable session.

## Context
This block adds a new entry point into the gameplay loop. The navigation contract must still support the seeded validation routes while also allowing a newly created playable start to hand off cleanly into the main cockpit pages without eager imports or broken query parameters.

## Implementation steps

1. Review current route and query-parameter handoff behavior across Planet and related cockpit pages.
2. Ensure onboarding success navigates to Planet using the ids returned from the backend.
3. Verify Planet links to the main cockpit pages preserve ids and remain compatible with existing seeded validation URLs.
4. Add or adjust lightweight guards or docs where they provide durable safety.
5. Update the frontend smoke checklist with the supported handoff scenarios.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `scripts/check-frontend-route-lazy-imports.ps1`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Acceptance criteria

- Onboarding success can navigate into Planet using returned ids.
- Planet links preserve ids into Construction, Research, Shipyard, Defenses, and Fleets.
- Existing seeded validation URLs still work.
- Lazy route import protections continue to pass.

## Constraints

- Preserve lazy loading.
- Keep handoff changes minimal and explicit.
- Do not redesign routing beyond what the new playable-session flow requires.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(frontend): align playable session handoffs`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If route and page updates exceed budget, split guard or doc work into a follow-up task.
