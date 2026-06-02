# Repository Context

## Project Overview

VoidEmpires is the future home of the `VoidEmpires` game and its supporting services.

At this stage the repository is in Phase 9N frontend UI hardening and backend foundation refinement. It contains AI Platform workflow assets, project-planning documentation, the buildable `.NET` solution, and the `src/VoidEmpires.Frontend` prototype.

The current purpose of the repository is to preserve the product direction, architecture direction, and delivery workflow while incrementally building the first service foundation.

## Product Direction

VoidEmpires is being planned as a strategy game focused on building, expanding, and sustaining an empire in a hostile void setting.

Until gameplay specifications are written in more detail, future tasks should treat the following as the working product pillars:

- empire growth through expansion and territorial control
- strategic decision-making around economy, technology, and military power
- conflict between competing powers in a persistent science-fiction setting
- systems that can later support both moment-to-moment gameplay loops and longer-term progression

These pillars are planning assumptions for repository setup. They should be refined by later product and design tasks rather than treated as final game design.

## Technical Direction

The current implementation direction is a modular .NET-based solution named `VoidEmpires`. The solution currently includes:

- `VoidEmpires.Web` as the first ASP.NET Core host
- `VoidEmpires.Application` for future use-case orchestration
- `VoidEmpires.Domain` for future domain rules
- `VoidEmpires.Infrastructure` for future adapters and infrastructure concerns
- `VoidEmpires.Tests` for baseline unit and smoke coverage
- `VoidEmpires.Frontend` for the current Vite + React prototype shell

The early technical bias is:

- start with a small, modular solution rather than a monolith with premature infrastructure
- keep domain rules isolated from delivery and infrastructure concerns
- establish clear boundaries for gameplay/domain logic, application orchestration, infrastructure, and external interfaces
- prefer API-first and service-oriented seams so additional clients or tools can be added later
- document architecture decisions before introducing heavy dependencies

The selected primary relational database engine is PostgreSQL 16. The intended .NET persistence stack is EF Core with Npgsql unless a later decision supersedes it. Real database configuration must be supplied externally through safe environment-specific mechanisms such as environment variables, user secrets, deployment secrets, or private infrastructure configuration. Do not commit real connection strings, passwords, private hostnames, VPN details, or NAS addresses. CI and automated tests must not depend on the real PostgreSQL database.

Current implemented runtime surfaces already include the web host health and authentication flows, controlled development galaxy generation endpoints, persisted infrastructure services, and the frontend prototype shell. Future tasks should extend those existing boundaries instead of assuming the repository is still at an empty bootstrap stage.

## Workflow Expectations

This repository uses the AI Platform task lifecycle under `ai/tasks/`.

Expected operating model:

`Context -> Roadmap -> Task -> Implementation -> Validation -> Commit -> Push`

Rules for future work:

- process the first pending task first
- keep changes small and directly related to the active task
- prefer updating existing docs or components over creating parallel structures
- run repository-relevant validation for each task
- record durable project knowledge in `ai/` rather than leaving it implicit in commits

## Team Workflow

The documented team model under `ai/teams/` is used for planning and review guidance.

Current practical expectations:

- Platform owns repository scaffolding, workflow conventions, and technical foundation tasks
- Product and Docs clarify game direction and durable repository knowledge
- Backend, Frontend, Database, DevOps, QA, and Security guidance become active once real application work starts

There is no automated team routing yet. Task metadata and human review still provide coordination.

## Constraints

Current constraints for repository work:

- the repository has a service foundation plus a frontend prototype, but gameplay systems remain incremental and task-driven
- application behavior should be added only through explicit tasks
- workflow scripts should remain stable unless a task explicitly targets them
- repository context documents should stay deterministic, concise, and easy for future agents to reuse

## Immediate Priorities

The next milestones are:

1. keep the .NET validation path reliable
2. evolve domain and application boundaries through small explicit tasks
3. introduce PostgreSQL persistence through small explicit tasks without committing secrets or requiring the real database in CI
4. defer authentication, deployment, and gameplay complexity until the foundation is ready

Future tasks should avoid speculative implementation and keep repository guidance aligned with the intended product direction.
