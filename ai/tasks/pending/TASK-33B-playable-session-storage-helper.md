# TASK-33B

---
id: TASK-33B
title: Playable session storage helper
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Add a typed frontend helper for remembering the latest Development-safe playable start as non-sensitive navigation context.

## Context
Later pages need a shared way to load the last playable start without duplicating localStorage parsing or treating local data as authoritative gameplay state.

## Implementation steps

1. Read the documented contract from TASK-33A.
2. Add `src/VoidEmpires.Frontend/src/utils/playableSession.ts` or the nearest equivalent helper location.
3. Implement `savePlayableSession(...)`, `loadPlayableSession()`, `clearPlayableSession()`, and `hasPlayableSession()`.
4. Store only non-sensitive fields: `civilizationId`, `planetId`, player/display name if available, `civilizationName` if available, `planetName` if available, and client-side `createdAt`/`updatedAt` timestamps if useful.
5. Validate shape on load and ignore invalid, corrupt, or partial localStorage data safely.
6. Ensure the helper does not store credentials, tokens, cookies, passwords, or auth/session assertions.
7. Add lightweight tests or guards if the frontend already has an appropriate test setup. If not, document the static guard in existing validation docs or scripts.

## Files to read first

- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/package.json

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- Optional: existing lightweight frontend test or guard script if one already covers utility contracts.
- Optional: docs/dev/persisted-gameplay-flow-checklist.md if a static guard must be documented.

## Acceptance criteria

- Helper compiles.
- Invalid stored data is ignored safely.
- No credentials, tokens, or auth claims are stored.
- No UI behavior changes are introduced yet.

## Constraints

- Do not wire the helper into pages yet.
- Do not fake backend state.
- Do not introduce production auth.
- Do not add a heavy frontend test framework solely for this helper.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33B message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
