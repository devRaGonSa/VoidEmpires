# Current State

## Phase

The repository is in `Phase 2C.2 - Persisted galaxy generation foundation` while retaining the AI Platform workflow assets from Phase 0.

## Repository Reality

The AI Platform template has been adapted into a VoidEmpires-specific project workspace.

Current repository contents are centered on:

- workflow rules in `AGENTS.md`
- planning and orchestration documents under `ai/`
- task lifecycle folders under `ai/tasks/`
- helper scripts under `scripts/`
- the initial `.NET` solution and projects under `src/` and `tests/`

## Application Status

The repository now contains `VoidEmpires.sln` with these projects:

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Domain`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

`VoidEmpires.Web` is a minimal ASP.NET Core host. It exposes:

- `GET /` for a simple product identity response
- `GET /health` for deterministic health checks, including whether persistence is configured
- `POST /api/auth/register` for minimal user registration
- `GET /api/auth/confirm-email` for minimal email confirmation
- `POST /api/dev/galaxies/generate` for controlled development or test galaxy generation when development endpoints and persistence are configured

Phase 2A identity and email foundation work has completed its initial setup. Phase 2C.2 persisted galaxy generation foundation work is now present. The repository now has:

- PostgreSQL 16 selected as the primary relational database engine.
- EF Core with Npgsql package references in `VoidEmpires.Infrastructure`.
- An empty `ConnectionStrings:DefaultConnection` placeholder in web appsettings files.
- A `VoidEmpiresDbContext` in the Infrastructure persistence boundary using ASP.NET Core Identity tables.
- EF Core migrations for the Identity schema and initial galaxy model. Migrations exist in source but are not automatically applied to the real database.
- Infrastructure service registration that enables PostgreSQL only when a non-empty connection string is configured.
- ASP.NET Core Identity registration with unique-email and confirmed-email defaults.
- Application contracts for user registration, email confirmation, and transactional email.
- Infrastructure services for registration and email confirmation backed by ASP.NET Core Identity.
- Brevo transactional email sender wiring behind the provider-agnostic email contract.
- A disabled, placeholder-only `Brevo` configuration section for transactional email integration.
- Deterministic in-memory galaxy generation through `IGalaxyGenerator`, registered in Infrastructure dependency injection.
- Application contracts for persisted galaxy generation through `IGalaxyGenerationService`.
- An Infrastructure `IGalaxyGenerationService` implementation that uses `IGalaxyGenerator` and `VoidEmpiresDbContext` to generate and persist a galaxy aggregate.

There is still no player or civilisation ownership, economy, fleets, combat, alliances, deployed environment definition, login/session endpoints, background processing, or full gameplay implementation.

PostgreSQL remains the persistence target. Real database and Brevo configuration are external to the repository and must not be committed. Brevo is the transactional email provider for user creation and email confirmation, but secrets such as API keys and sender credentials must come from environment variables, user secrets, deployment secrets, or private infrastructure configuration. CI and tests run without requiring the real NAS PostgreSQL database, private network access, or Brevo network calls.

## Task Workflow Status

The repository is actively using the AI task lifecycle:

- `ai/tasks/pending`
- `ai/tasks/in-progress`
- `ai/tasks/review`
- `ai/tasks/done`
- `ai/tasks/blocked`
- `ai/tasks/obsolete`

Inherited template history has been moved out of `ai/tasks/done` into `ai/tasks/obsolete` so future project tracking reflects VoidEmpires work only.

## Planning Status

The repository has established:

- a VoidEmpires-specific repository context
- an initial roadmap
- an initial architecture index
- the first bootstrap implementation plan
- the initial `VoidEmpires` solution structure

## Validation Status

Repository-specific application validation now exists through the .NET solution.

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current tests include assembly-boundary coverage, smoke checks for `/` and `/health`, auth endpoint tests with fake services, development galaxy endpoint tests with fake services, persistence and identity registration checks, application contract tests, deterministic galaxy generation tests, persisted galaxy generation service tests with EF Core InMemory, registration and email confirmation service tests with EF Core InMemory, Brevo sender tests with fake HTTP handlers, and verification that health output does not expose connection string values. Tests do not use the real NAS PostgreSQL database.

If a task later introduces integration boundaries before tests exist, record `No integration tests configured.`

## Constraints

Current constraints remain:

- do not add application behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- avoid adding persistence behavior beyond the current Identity foundation until explicit tasks introduce it
- avoid login/session endpoints, deployment, or gameplay complexity beyond the current galaxy generation foundation until explicit tasks introduce them
- never commit real database secrets, Brevo secrets, private hostnames, VPN details, NAS connection information, or production email configuration
