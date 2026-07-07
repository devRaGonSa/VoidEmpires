# User Account Auth Readiness

This note documents the current account/auth boundary for the playable shell. It prepares the product handoff without claiming production authentication.

It does not add bearer tokens, localStorage auth tokens, role claims, production account selection, final authorization, combat, fleet movement, market transactions, alliance mutations, or final DB/model consolidation.

## Current Backend Foundation

- `POST /api/auth/register` exists and uses `IUserRegistrationService` when persistence is configured.
- `GET /api/auth/confirm-email` exists and uses `IEmailConfirmationService` when persistence is configured.
- `POST /api/accounts/register` creates an Identity account and initial player world.
- `POST /api/accounts/login` validates credentials with ASP.NET Core Identity and returns a safe current player summary.
- The selected session approach is an HTTP-only ASP.NET Core Identity application cookie named `VoidEmpires.Auth`; no bearer token or localStorage token is introduced.
- Local Vite development origins `http://localhost:5173` and `http://127.0.0.1:5173` are explicitly allowed to send credentials through CORS.
- Cookie security remains production-biased: cookies are HTTP-only, `SameSite=Lax`, and `SecurePolicy=Always` outside Development.
- `IdentityAccountService` creates ASP.NET Core Identity users, generates email confirmation tokens, and sends transactional email through the configured email sender.
- Identity is registered only when `ConnectionStrings:DefaultConnection` is non-empty; ordinary tests still avoid any real SQL Server dependency.
- `/health` reports auth configured when persistence is configured.
- These endpoints are account foundation only. They do not currently bootstrap the active playable civilization in the frontend shell.

## Current Playable Start Boundary

- `/onboarding` calls `POST /api/dev/players/starting-civilization`.
- `StartingCivilizationService` creates a `PlayerProfile`, `Civilization`, generated galaxy/system/home planet, `PlanetOwnership`, starting resource stockpile, production profile, population profile, building capacity, and initial buildings.
- The response returns explicit `civilizationId` and `homePlanetId`.
- The frontend stores those ids in local browser navigation memory through `playableSession.ts`.
- Cockpit routes continue to use URL query ids and backend reads; local storage is not auth, ownership, a bearer token, a cookie, a role claim, or production session state.

## Registration Product Contract

The final product entry should replace `/onboarding` as the primary new-player path:

1. A player registers with email, password, commander name, civilization name, and optional home planet name.
2. The account service creates the ASP.NET Core Identity user and enforces email/password policy.
3. The bootstrap workflow creates or links one `PlayerProfile` for that user.
4. The workflow creates the initial `Civilization`, normalizes its lookup name, assigns or generates a home planet, creates active `PlanetOwnership`, and initializes starting economy/production/building state.
5. A confirmed login/session resolves the current account and active civilization server-side.
6. The frontend navigates to the game using session-owned data from `/api/accounts/me`, not client-trusted raw ids.
7. Planet and cockpit reads validate ownership/visibility against the authenticated account before returning gameplay state.

### Home planet allocation

The account bootstrap allocator first selects an existing unowned Terran planet ordered by system and orbital slot. If no candidate exists, it creates `Account Bootstrap Galaxy`, adds a new system at the next unused `(x, 0, 0)` coordinate, and creates a Terran home planet in slot 1. Ownership is created by the bootstrap workflow in the same persistence operation, and existing owned planets, including validation seed planets, are not reassigned.

## Current UI Boundary

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
9. Registration-to-world smoke coverage proving account creation, session resolution, and home planet UI-state access without SQL Server.

## Validation

- This is a documentation audit only.
- Required validation for the task: `dotnet build --no-restore` and `dotnet test --no-build`.
- No browser, screenshot, auth integration, production session, SQL Server connection, or final authorization validation was performed for this note.
