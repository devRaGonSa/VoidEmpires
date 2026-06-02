# VoidEmpires

VoidEmpires is a game-project repository with an initial .NET service foundation.

The repository defines the project direction, keeps the AI-assisted task workflow under `ai/`, and now includes a minimal buildable web entrypoint for future game-service work.

## Current Status

- AI Platform workflow assets are installed under `ai/`
- repository context, roadmap, and architecture planning documents are in place
- `VoidEmpires.sln` contains the initial Web, Application, Domain, Infrastructure, and Tests projects
- `VoidEmpires.Web` exposes `/`, `/health`, `/api/auth/register`, `/api/auth/confirm-email`, and a controlled development galaxy generation endpoint
- `VoidEmpires.Infrastructure` contains EF Core/Npgsql persistence, ASP.NET Core Identity, Identity and galaxy migrations, Brevo transactional email wiring, deterministic galaxy generation, and a persisted galaxy generation service

## Development

### Prerequisites

- .NET 8 SDK
- Git
- Node.js 20+ with npm for `src/VoidEmpires.Frontend`

### Validate

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

For the frontend prototype:

```powershell
cd src/VoidEmpires.Frontend
npm install
npm run build
```

### Run The Web App

```powershell
dotnet run --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

The initial runtime checks are:

- `GET /` returns a minimal `VoidEmpires` response.
- `GET /health` returns service status plus `persistence.configured` and `persistence.provider` metadata without exposing connection string values.
- `POST /api/auth/register` accepts an email and password, creates a user through the registration service, and sends confirmation email through the transactional email abstraction.
- `GET /api/auth/confirm-email` accepts `userId` and `token` query parameters and delegates confirmation to the email confirmation service.
- `POST /api/dev/galaxies/generate` is available only in Development, or when `VoidEmpires:DevEndpoints:Enabled` is explicitly set to `true`, and requires configured persistence.
- `POST /api/dev/seeds/apply` is available only in Development, or when `VoidEmpires:DevEndpoints:Enabled=true`, and is the explicit opt-in entrypoint for local validation seed profiles.

Example development galaxy generation request:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5000/api/dev/galaxies/generate" `
  -ContentType "application/json" `
  -Body '{"name":"Void Prime","seed":"alpha-001","solarSystemCount":12,"minPlanetsPerSystem":2,"maxPlanetsPerSystem":5}'
```

Do not enable development endpoints against production data. Use a local or disposable development database for generated test galaxies.

### Development Seed Convention

Development seed data is opt-in and must never run automatically on startup. Use a disposable local database such as `VoidEmpireDB_Dev`, or any explicitly configured private development database.

Run the web app with:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=VoidEmpireDB_Dev;Username=postgres;Password=<local-password>"
dotnet run --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

Then apply the current development seed profile explicitly:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5000/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"minimal-validation"}'
```

Current convention notes:

- The `minimal-validation` profile is the stable profile name for development validation data.
- The profile currently seeds a deterministic strategic-map dataset for civilization `00000000-0000-0000-0000-000000000001`.
- Deterministic IDs:
- System `20000000-0000-0000-0000-000000000001`
- Planet `40000000-0000-0000-0000-000000000001` (`Aurelia`, owned)
- Planet `40000000-0000-0000-0000-000000000002` (`Cinder Reach`)
- Planet `40000000-0000-0000-0000-000000000003` (`Frost Hollow`)
- Re-running the same profile should remain safe and idempotent as later seed tasks add data.
- Re-applying `minimal-validation` validates that the baseline rows exist, but it does not wipe later mutations, delete extra transfers or groups, refund spent resources, or rebuild stockpiles that already exist.
- If `create transfer` or other dev-only mutations changed the local validation state and you need a true clean slate, switch to a fresh disposable local database before applying the seed again. The repository does not currently provide a destructive reset command.
- Keep seed execution in `Development`, or explicitly set `VoidEmpires:DevEndpoints:Enabled=true` for non-production validation environments only.

### Manual Validation Flow

Backend:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=VoidEmpireDB_Dev;Username=postgres;Password=<local-password>"
dotnet run --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

Apply the explicit seed:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"minimal-validation"}'
```

Frontend:

```powershell
$env:VITE_VOIDEMPIRES_API_BASE_URL = "http://localhost:5142"
npm run dev --prefix src/VoidEmpires.Frontend
```

Validation URLs:

- `/health`
- `/api/dev/strategic-map?civilizationId=00000000-0000-0000-0000-000000000001`
- `/api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001`
- `/api/dev/planets/40000000-0000-0000-0000-000000000001/visual-state`
- `/api/dev/planets/40000000-0000-0000-0000-000000000002/visual-state`
- `/api/dev/planets/40000000-0000-0000-0000-000000000003/visual-state`

Current frontend mutation boundary:

- `POST /api/dev/fleets/orbital-travel/estimate` may execute from the frontend as a read-only preview.
- `POST /api/dev/fleets/orbital-transfers/create` may execute from the frontend only after explicit development-only confirmation and mutates development data.
- `POST /api/dev/fleets/orbital-transfers/cancel` may execute from the frontend only after explicit development-only confirmation for a visible active transfer, mutates development data, and does not refund previously charged travel resources.
- `complete-due`, `split`, and `merge` remain disabled or prototype-only in the frontend.

Recommended recovery flow after `create transfer` or `cancel transfer` mutates local validation data:

1. Inspect `GET /api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001` to confirm the current stationed groups, active transfers, and resource context.
2. Re-apply `POST /api/dev/seeds/apply` with `{"profile":"minimal-validation"}` only when you need to restore missing baseline rows.
3. If fleet state, transfer rows, or resource balances must return to the original baseline, use a fresh disposable local database and then apply `minimal-validation` again.
4. Re-run `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
5. Keep manual browser review deferred unless a clear frontend regression appears; the current fleet mutation path is validated primarily through build, test, and optional API-level checks.
6. When a browser review is needed for the Fleet cockpit milestone, use `docs/dev/frontend-foundation-smoke-checklist.md` and confirm the command deck, group rail, selected-group detail, guarded command column, disabled prototype controls, and readable feedback areas on the `Flotas` route.
7. For the completed Fleet cockpit milestone, also confirm the visual gameplay checklist: mostly Spanish UI, compact scannable squad rail, readable selected-group panel, simple estimate/create/cancel order flow, visible active transfers with progress, readable resources, secondary compact ids, and collapsed development details.

### Database Configuration

PostgreSQL 16 is the selected primary database engine. EF Core with Npgsql is the current persistence stack in `VoidEmpires.Infrastructure`. The repository stores only an empty placeholder at `ConnectionStrings:DefaultConnection`; real connection strings must be supplied outside source control.

For local development, prefer one of these safe override mechanisms:

```powershell
$env:ConnectionStrings__DefaultConnection = "<local or private PostgreSQL connection string>"
dotnet run --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

or user secrets:

```powershell
dotnet user-secrets init --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<local or private PostgreSQL connection string>" --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

Do not commit real passwords, NAS hostnames, VPN IPs, private infrastructure addresses, or production connection strings. The real PostgreSQL database is reachable only through private infrastructure or VPN, and normal validation and tests run without requiring access to it. Identity tests use local fakes or EF Core InMemory rather than the real database.

### Brevo Email Configuration

Brevo is the planned transactional email provider for account creation and email confirmation flows. The committed `Brevo` configuration section is disabled and contains only safe placeholders.

For local development, prefer environment variables or user secrets:

```powershell
$env:Brevo__Enabled = "true"
$env:Brevo__ApiKey = "<Brevo API key>"
$env:Brevo__SenderEmail = "<verified sender email>"
$env:Brevo__SenderName = "VoidEmpires"
$env:Brevo__ConfirmationBaseUrl = "<public confirmation base URL>"
dotnet run --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

or user secrets:

```powershell
dotnet user-secrets init --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
dotnet user-secrets set "Brevo:Enabled" "true" --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
dotnet user-secrets set "Brevo:ApiKey" "<Brevo API key>" --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
dotnet user-secrets set "Brevo:SenderEmail" "<verified sender email>" --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
dotnet user-secrets set "Brevo:ConfirmationBaseUrl" "<public confirmation base URL>" --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

Do not commit real Brevo API keys, SMTP credentials, verified sender addresses, recipient lists, or production confirmation URLs. Automated tests and CI must not call Brevo; sender tests use fake HTTP handlers.

## Key Documents

- `AGENTS.md`: repository workflow rules for task execution
- `ai/repo-context.md`: project purpose, product assumptions, workflow, and constraints
- `ai/current-state.md`: current repository reality and validation status
- `ai/roadmap.md`: high-level delivery phases for VoidEmpires
- `ai/architecture-index.md`: intended module boundaries for the future solution
- `ai/reports/solution-bootstrap-plan.md`: actionable plan for the first implementation task
- `src/VoidEmpires.Frontend/README.md`: frontend prototype setup, routes, and limitations
- `docs/dev/frontend-foundation-smoke-checklist.md`: frontend smoke validation steps
- `docs/dev/fleet-controlled-mutation-checklist.md`: focused non-visual regression steps for the Fleet estimate, create-transfer, and cancel-transfer flows

## Workflow

Tasks are processed from `ai/tasks/pending` and moved through the lifecycle directories under `ai/tasks/`.

The current operating model is:

`Context -> Roadmap -> Task -> Implementation -> Validation -> Commit -> Push`

AI workers should process the first pending task, keep changes scoped to that task, run the relevant validation commands, commit the task result, push when the branch tracks a remote, and then continue until no pending tasks remain.

## Next Step

The next planned implementation milestones are to keep hardening the technical foundation before adding gameplay, login/session flows, deployment, or UI complexity.
