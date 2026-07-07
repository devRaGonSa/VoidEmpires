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

## Current UI Boundary

`PlayableSessionBanner`, `/`, `/onboarding`, and `/account-settings` may display friendly commander, civilization, and planet labels from local storage for convenience.

Those surfaces must not treat local storage as login, cookie, token, role, permission, or proof of ownership. Each cockpit still re-reads backend state before showing playable state.

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

## Replacement Target

The product replacement for local memory is server-owned account resolution:

1. Registration creates the Identity user and initial world bootstrap in one account-owned workflow.
2. Login establishes the session or token.
3. `/api/accounts/me` returns safe commander, civilization, and home planet summary data for the authenticated user.
4. The frontend uses that response to route into Planet and related cockpits.
5. Client-stored ids become optional navigation hints only; backend ownership and visibility remain authoritative.

## Validation

- This is a documentation audit only.
- Required validation for the task: `dotnet build --no-restore` and `dotnet test --no-build`.
- No browser, screenshot, auth integration, production session, SQL Server connection, or final authorization validation was performed for this note.
