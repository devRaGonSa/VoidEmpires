# Local Session vs Auth Boundary

This note documents the visible frontend boundary between local cockpit navigation memory and real authentication.

It does not add login, session cookies, bearer tokens, role claims, production account selection, final authorization, combat, fleet movement, market transactions, alliance mutations, or final DB/model consolidation.

## Current Local Memory

`playableSession.ts` stores only:

- `civilizationId`
- `planetId`
- optional display labels for player, civilization, and planet
- `createdAt` and `updatedAt` timestamps

The storage key is browser-local and non-sensitive. The helper accepts only non-empty ids and valid timestamps when reading existing data.

## What Local Memory Is Not

Local playable memory is not:

- login state
- production authentication
- a bearer token
- a cookie
- a role claim
- ownership proof
- permission to mutate gameplay state
- a server-side active civilization resolver

## UI Boundary Added In This Task

`PlayableSessionBanner` now describes stored context as "Memoria local de navegacion" and states that it keeps only Development ids for cockpit links.

The banner copy now explicitly says the memory is not login, cookie, token, role, or permission, and that each cockpit still re-reads backend state using visible ids before showing playable state.

## Runtime Rule

Cockpits may use local memory only to rebuild URLs when query ids are missing. Backend read models remain authoritative for:

- ownership
- resources
- queues
- buildings
- research
- stock
- fleets
- readiness
- mutation results

Missing, stale, or cleared local memory must fall back to explicit route entry or `/onboarding`, not hidden auth or a different civilization.

## Validation

- Frontend build: `npm run build --prefix src/VoidEmpires.Frontend`.
- Lazy route guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`.
- Copy guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, auth integration, production session, or final authorization validation was performed for this note.
