# User Account Auth Readiness

This note documents the current account/auth boundary for the playable shell. It prepares the product handoff without claiming production authentication.

It does not add bearer tokens, localStorage auth tokens, role claims, production account selection, final authorization, combat, fleet movement, market transactions, alliance mutations, or final DB/model consolidation.

## Current Backend Foundation

- `POST /api/auth/register` exists and uses `IUserRegistrationService` when persistence is configured.
- `GET /api/auth/confirm-email` exists and uses `IEmailConfirmationService` when persistence is configured.
- `POST /api/accounts/register` creates an Identity account and initial player world.
- `POST /api/accounts/login` validates credentials with ASP.NET Core Identity and returns a safe current player summary.
- `GET /api/accounts/me` resolves the current account from the Identity application cookie and returns the safe player/civilization/home planet summary.
- `POST /api/accounts/logout` clears the current Identity application cookie session.
- The selected session approach is an HTTP-only ASP.NET Core Identity application cookie named `VoidEmpires.Auth`; no bearer token or localStorage token is introduced.
- Local Vite development origins `http://localhost:5173` and `http://127.0.0.1:5173` are explicitly allowed to send credentials through CORS.
- Cookie security remains production-biased: cookies are HTTP-only, `SameSite=Lax`, and `SecurePolicy=Always` outside Development.
- `IdentityAccountService` creates ASP.NET Core Identity users, generates email confirmation tokens, and sends transactional email through the configured email sender.
- Identity is registered only when `ConnectionStrings:DefaultConnection` is non-empty; ordinary tests still avoid any real SQL Server dependency.
- `/health` reports auth configured when persistence is configured.
- SQL Server uses the same Identity tables and account bootstrap workflow after the SQL Server baseline schema has been manually applied; no real SQL Server registration QA is claimed in this note.

## Current Playable Start Boundary

- `/register` is the primary product account entry and posts to `POST /api/accounts/register`.
- `/onboarding` is retained as a route alias to registration.
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
5. The frontend signs in after successful registration, resolves the generated world route from the registration response, and can refresh current account state through `/api/accounts/me`.
6. The frontend stores only non-sensitive navigation context for fallback; no password, bearer token, or cookie value is written to browser storage.
7. Planet and cockpit reads still need final ownership/visibility authorization before production readiness can be claimed.

### Home planet allocation

The account bootstrap allocator first selects an existing unowned Terran planet ordered by system and orbital slot. If no candidate exists, it creates `Account Bootstrap Galaxy`, adds a new system at the next unused `(x, 0, 0)` coordinate, and creates a Terran home planet in slot 1. Ownership is created by the bootstrap workflow in the same persistence operation, and existing owned planets, including validation seed planets, are not reassigned.

## Current UI Boundary

The account UI now uses `/register`, `/login`, `/api/accounts/me`, and `/api/accounts/logout` for the primary account flow:

- Registration creates credentials and an initial world, then routes to the generated home planet after login succeeds.
- The shell and guarded game routes use backend-backed current account state.
- Local memory stores navigation ids only.
- Backend reads remain authoritative for ownership and cockpit state.

## Final Auth Dependencies

Before claiming production auth, the product still needs:

1. Server-side authorization on gameplay reads and mutations.
2. Account-to-player/civilization selection rules beyond one initial civilization.
3. Email confirmation UX and resend/recovery behavior.
4. Clear environment gating for Development-only playable-start endpoints.
5. Tests proving unauthenticated users cannot read or mutate protected gameplay state.
6. Registration-to-world smoke coverage proving account creation, session resolution, and home planet UI-state access without SQL Server.
7. Manual SQL Server registration verification after the operator-applied baseline exists.

## Validation

- This is a documentation audit only.
- Required validation for the task: `dotnet build --no-restore` and `dotnet test --no-build`.
- No browser, screenshot, auth integration, production session, SQL Server connection, or final authorization validation was performed for this note.
