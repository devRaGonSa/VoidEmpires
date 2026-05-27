# VoidEmpires

VoidEmpires is a game-project repository with an initial .NET service foundation.

The repository defines the project direction, keeps the AI-assisted task workflow under `ai/`, and now includes a minimal buildable web entrypoint for future game-service work.

## Current Status

- AI Platform workflow assets are installed under `ai/`
- repository context, roadmap, and architecture planning documents are in place
- `VoidEmpires.sln` contains the initial Web, Application, Domain, Infrastructure, and Tests projects
- `VoidEmpires.Web` exposes `/` and `/health` as minimal deterministic endpoints

## Development

### Prerequisites

- .NET 8 SDK
- Git

### Validate

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

### Run The Web App

```powershell
dotnet run --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

The initial runtime checks are:

- `GET /` returns a minimal `VoidEmpires` response.
- `GET /health` returns `{ "status": "ok", "service": "VoidEmpires.Web" }`.

### Database Configuration

PostgreSQL 16 is the selected primary database engine for future persistence work. The repository stores only an empty placeholder at `ConnectionStrings:DefaultConnection`; real connection strings must be supplied outside source control.

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

Do not commit real passwords, NAS hostnames, VPN IPs, private infrastructure addresses, or production connection strings. The real PostgreSQL database is reachable only through private infrastructure or VPN, and normal validation must not require access to it.

## Key Documents

- `AGENTS.md`: repository workflow rules for task execution
- `ai/repo-context.md`: project purpose, product assumptions, workflow, and constraints
- `ai/current-state.md`: current repository reality and validation status
- `ai/roadmap.md`: high-level delivery phases for VoidEmpires
- `ai/architecture-index.md`: intended module boundaries for the future solution
- `ai/reports/solution-bootstrap-plan.md`: actionable plan for the first implementation task

## Workflow

Tasks are processed from `ai/tasks/pending` and moved through the lifecycle directories under `ai/tasks/`.

The current operating model is:

`Context -> Roadmap -> Task -> Implementation -> Validation -> Commit -> Push`

AI workers should process the first pending task, keep changes scoped to that task, run the relevant validation commands, commit the task result, push when the branch tracks a remote, and then continue until no pending tasks remain.

## Next Step

The next planned implementation milestones are to keep hardening the technical foundation before adding gameplay, persistence, authentication, or UI complexity.
