# TASK-33D

---
id: TASK-33D
title: Session-aware route entry fallback
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Make cockpit pages recover gracefully when query ids are missing but a local playable session exists.

## Context
Seeded validation URLs must keep working, but pages should no longer feel broken when the user has a recent Development-safe playable start saved locally.

## Implementation steps

1. Inspect how Planet, Construction, Research, and Shipyard currently parse query ids and gate backend fetches.
2. Add a reusable hook/helper such as `usePlayableRouteContext` if it reduces duplication.
3. For required-id pages, use query ids when present.
4. When query ids are missing and local session exists, show a Spanish action or link to continue with the latest playable session.
5. Do not silently mutate the URL unless it is explicitly safe and documented in the code/docs.
6. Do not fetch backend data with missing or invalid ids.
7. Keep existing seeded validation query-param flows unchanged.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx

## Acceptance criteria

- Missing-id pages become recoverable through the local playable session.
- Existing query-param flows still work.
- Backend is never called with missing or invalid ids.
- No production auth claims are introduced.

## Constraints

- Do not add production session/auth behavior.
- Do not perform or claim browser/visual QA.
- Do not change gameplay mutations.
- Preserve lazy loading.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33D message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If the expected file list is exceeded, explain why in the commit message or split follow-up work.
