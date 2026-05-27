# Current State

## Phase

The repository has moved into `Phase 1 - Technical foundation` while retaining the AI Platform workflow assets from Phase 0.

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

Phase 1B persistence foundation work has completed its initial setup. The repository now has:

- PostgreSQL 16 selected as the primary relational database engine.
- EF Core with Npgsql package references in `VoidEmpires.Infrastructure`.
- An empty `ConnectionStrings:DefaultConnection` placeholder in web appsettings files.
- A `VoidEmpiresDbContext` skeleton in the Infrastructure persistence boundary.
- Infrastructure service registration that enables PostgreSQL only when a non-empty connection string is configured.
- A disabled, placeholder-only `Brevo` configuration section for future transactional email integration.

There are still no gameplay entities, migrations, deployed environment definition, public authentication endpoints, Brevo sender implementation, background processing, or gameplay implementation.

Real database and Brevo configuration are external to the repository and must not be committed. CI and tests run without requiring the real NAS PostgreSQL database, private network access, or Brevo network calls.

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

Current tests include assembly-boundary coverage, smoke checks for `/` and `/health`, persistence and identity registration checks, application contract tests, and verification that health output does not expose connection string values.

If a task later introduces integration boundaries before tests exist, record `No integration tests configured.`

## Constraints

Current constraints remain:

- do not add application behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- avoid adding persistence behavior beyond the current DbContext skeleton until explicit tasks introduce it
- avoid public authentication endpoints, deployment, or gameplay complexity until explicit tasks introduce them
- never commit real database secrets, Brevo secrets, private hostnames, VPN details, NAS connection information, or production email configuration
