# Architecture Index

This file describes the high-level architecture direction for VoidEmpires after the first application projects have been created.

The boundaries below are planning targets for the current modular service foundation and early implementation tasks.

## Repository Type

Game-service and operations repository for `VoidEmpires`, currently in early technical foundation mode.

## Planned Core Modules

| Module | Responsibility | Initial data ownership | Future scalability concern |
|---|---|---|---|
| Solution bootstrap and hosts | Provide the composition roots for the first executable applications such as an API host, worker host, or future admin host. | Host configuration, startup wiring, environment-specific settings. | Keep startup thin so new delivery surfaces can be added without duplicating business logic. |
| Domain core | Define the canonical rules and concepts for empires, sectors, fleets, resources, research, diplomacy, and conflict. | Domain entities, value objects, invariants, and domain events. | Prevent gameplay rules from leaking into controllers, persistence models, or transport contracts. |
| Application layer | Coordinate use cases, commands, queries, validation, and transactional workflows around the domain. | Application contracts, use-case orchestration, DTO boundaries, and authorization-aware workflows. | Support increasing use-case breadth without coupling every feature directly to infrastructure. |
| World and empire management | Own strategic world-state operations such as empire creation, territorial expansion, ownership changes, and map-level progression. | World snapshots, sector ownership state, empire-level progression metadata. | Handle larger world sizes, partitioning, and concurrency around shared territorial state. |
| Economy and progression | Manage resources, production, research, unlock progression, and long-term empire growth systems. | Resource ledgers, production queues, research trees, unlock state. | Maintain consistency when progression systems become more asynchronous, interdependent, or content-heavy. |
| Military and conflict resolution | Resolve fleet state, combat preparation, engagements, loss calculation, and post-conflict outcomes. | Fleet compositions, combat inputs, battle outcomes, readiness or cooldown state. | Support deterministic resolution, replays or audits, and higher combat throughput under load. |
| Player identity and access | Represent player accounts, sessions, authorization rules, and their relation to empires or administrative roles. | Player profiles, identity links, role assignments, access policies. | Keep authentication and authorization replaceable as external identity or platform requirements evolve. |
| Transactional email | Send account lifecycle and operational messages through provider-agnostic application contracts with Infrastructure-owned delivery adapters. | Email request contracts, delivery results, provider configuration metadata, and template selection. | Keep providers such as Brevo replaceable, testable, and disabled in CI without weakening account workflows. |
| Persistence and infrastructure | Implement data access, external integrations, file or cache adapters, and other infrastructure concerns. PostgreSQL 16 is the selected primary relational database, with EF Core and Npgsql as the intended .NET persistence stack. | Persistence models, repository implementations, integration clients, infrastructure mappings. Real configuration is supplied externally and secrets must not be committed. | Prevent infrastructure shortcuts from becoming the de facto domain model as data volume grows. CI and tests must not depend on the real PostgreSQL database. |
| Background jobs and world simulation | Execute scheduled ticks, asynchronous recalculations, notifications, and long-running world updates. | Job payloads, scheduling metadata, simulation checkpoints, retry state. | Coordinate idempotency, distributed execution, and observability for world-changing background work. |
| Admin, telemetry, and live operations | Expose the operational tooling needed to inspect state, balance systems, audit changes, and monitor platform health. | Audit trails, diagnostics, moderation actions, operational dashboards, telemetry metadata. | Add live-ops capabilities without granting unsafe direct access to gameplay state. |

## Cross-Cutting Principles

- Keep domain decisions independent from transport, UI, and storage details.
- Prefer explicit module boundaries over a single catch-all application project.
- Introduce infrastructure only when a concrete use case needs it.
- Preserve deterministic behavior where gameplay outcomes affect progression or conflict.
- Treat observability, validation, and operational safety as first-class concerns once state becomes persistent.

## Near-Term Build Order

1. create the solution and initial host or hosts
2. establish the domain core and application layer boundaries
3. add the first gameplay-focused modules for world, economy, and military behavior
4. add persistence and background processing once the first durable workflows exist
5. add admin and live-ops capabilities after the core game loop becomes inspectable

## Current Reality

The initial `.NET` solution now exists with these implemented project boundaries:

- `src/VoidEmpires.Web`: minimal ASP.NET Core host with `/`, `/health`, registration, and email confirmation endpoints
- `src/VoidEmpires.Application`: application-layer assembly boundary marker plus provider-agnostic identity and transactional email contracts
- `src/VoidEmpires.Domain`: domain assembly boundary marker
- `src/VoidEmpires.Infrastructure`: infrastructure assembly boundary marker with EF Core/Npgsql persistence, ASP.NET Core Identity, initial Identity migration, and Brevo transactional email adapter
- `tests/VoidEmpires.Tests`: baseline assembly, endpoint, persistence, identity, and email tests

The gameplay, background job, admin, and live-ops modules remain planning targets. Future tasks should introduce those capabilities incrementally without bypassing the established project boundaries. Persistence tasks must keep real database configuration outside the repository and avoid requiring the real PostgreSQL database for CI or ordinary test runs. Brevo is the transactional email provider for account creation and confirmation flows, but provider-specific details belong in Infrastructure behind application-level email contracts.
