# User Account Auth Readiness

This note documents the current account/auth boundary for the playable shell. It prepares the product handoff without claiming production authentication.

It does not add login, session cookies, bearer tokens, role claims, production account selection, final authorization, combat, fleet movement, market transactions, alliance mutations, or final DB/model consolidation.

## Current Backend Foundation

- `POST /api/auth/register` exists and uses `IUserRegistrationService` when persistence is configured.
- `GET /api/auth/confirm-email` exists and uses `IEmailConfirmationService` when persistence is configured.
- `IdentityAccountService` creates ASP.NET Core Identity users, generates email confirmation tokens, and sends transactional email through the configured email sender.
- `/health` reports auth configured when persistence is configured.
- These endpoints are account foundation only. They do not currently bootstrap the active playable civilization in the frontend shell.

## Current Playable Start Boundary

- `/onboarding` calls `POST /api/dev/players/starting-civilization`.
- The backend creates a Development-only player profile, civilization, homeworld, resources, buildings, and local context.
- The response returns explicit `civilizationId` and `homePlanetId`.
- The frontend stores those ids in local browser navigation memory through `playableSession.ts`.
- Cockpit routes continue to use URL query ids and backend reads; local storage is not auth, ownership, a bearer token, a cookie, a role claim, or production session state.

## UI Boundary Added In This Task

`/onboarding` now includes a visible account/access boundary panel:

- Account foundation exists in backend registration and email confirmation.
- This screen does not create credentials or call the registration endpoint.
- Local memory stores navigation ids only.
- Backend reads remain authoritative for ownership and cockpit state.

## Final Auth Dependencies

Before claiming production auth, the product needs:

1. Login and logout flows.
2. Authenticated session or token strategy.
3. Active civilization resolution from the authenticated account.
4. Server-side authorization on gameplay reads and mutations.
5. Account-to-player/civilization selection rules.
6. Email confirmation UX and resend/recovery behavior.
7. Clear environment gating for Development-only playable-start endpoints.
8. Tests proving unauthenticated users cannot read or mutate protected gameplay state.

## Validation

- Frontend build: `npm run build --prefix src/VoidEmpires.Frontend`.
- Lazy route guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`.
- Copy guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, auth integration, production session, or final authorization validation was performed for this note.
