# Seed Data Architecture

Status date: 2026-06-19

This note defines the recommended final seed-data architecture for gameplay catalogs and metadata in the final database phase.

It does not add migrations, auto-apply database behavior, production-auth changes, combat data, fleet movement execution, market mutations, or alliance mutations.

## Decision

Decision: use a hybrid model.

Authoritative source:

- backend-owned, versioned JSON content files in the repository for final catalog rows and starting-data templates

Safety and execution layer:

- backend code loaders and validators that deserialize those files into typed contracts
- explicit manual seed application through a future repository service and operator-invoked script

Not chosen as the primary source of truth:

- raw SQL files
- EF `HasData(...)`
- hardcoded C# catalog rows as the long-term final source

## Why This Choice Fits VoidEmpires

The repository already has these constraints:

1. gameplay metadata must stay backend-owned
2. SQL Server is the intended final target, but real apply steps must remain manual
3. Development seed profiles must remain separate from final production initialization
4. frontend copy must not become the hidden source of truth for catalog labels or descriptions
5. ordinary `dotnet build` and `dotnet test` runs must stay provider-independent

A hybrid JSON-plus-loader model fits those constraints better than the alternatives.

## Architecture Shape

### 1. Source layer

Store final seed inputs as repository-owned JSON files for:

- buildings
- research
- orbital assets
- planetary assets
- defenses
- resources
- starting civilization and home-planet templates

Each file should carry:

- stable backend key
- Spanish display text
- structured category and role metadata
- module ownership metadata
- placeholder image and icon keys
- sort order
- tags
- revision or version marker

These files are content inputs, not executable SQL.

### 2. Contract layer

Define typed backend contracts that represent the accepted seed shapes.

Those contracts should validate:

- required fields
- duplicate keys
- unknown keys
- enum compatibility where the domain still uses enums
- cross-file references such as requirements or prerequisite keys

This keeps failures inside ordinary backend validation rather than discovering them only during SQL script review.

### 3. Loader layer

Add a backend loader that:

- reads the JSON files
- validates them
- converts them into typed seed models
- exposes deterministic errors when files drift from the domain

The loader should be callable from tests without requiring a real database.

### 4. Apply layer

Later tasks should add a dedicated final seed service that:

- receives the validated in-memory seed models
- compares them to the target relational rows
- inserts or updates catalog rows deterministically
- remains explicit and operator-invoked

No app startup path should call that service automatically.

### 5. Operator layer

For the real SQL Server target, keep the final apply path manual:

- operator chooses the target connection string externally
- operator reviews the seed plan or script output
- operator invokes the seed script explicitly

This matches the repository's existing no-auto-apply safety posture.

## Why Not The Other Options

### Code-only catalogs

Current code-owned catalogs were acceptable for early development, but they are not the best final seed architecture because:

- editorial metadata stays too tightly coupled to runtime code releases
- non-trivial text changes become code edits everywhere
- production catalog rows would still need a relational apply path later

Code can still own domain enums and fallback safeguards, but not the long-term final content source.

### Raw SQL as the primary source

Raw SQL is not the best primary source because:

- it is harder to review as content
- it is easier to drift across providers and naming assumptions
- it pushes structural validation too late into the apply step

SQL may still be a generated or operator-reviewed apply artifact later, but not the editorial source of truth.

### EF `HasData(...)`

`HasData(...)` is not a good fit because:

- large metadata payloads become noisy migrations
- minor copy changes can force migration churn
- it couples content updates to migration generation too aggressively
- it is awkward for cross-row validation and content review

### UI-owned copy

Frontend helpers must not own final labels or descriptions because:

- the backend already serves read models across multiple cockpits
- duplicated UI copy drifts from seed content quickly
- SQL Server seed readiness requires one backend-owned source

## Authority Rules

Final authority should be split like this:

- domain enums and hard invariants: C# code
- final catalog metadata and starting-data templates: backend-owned JSON
- validation and mapping rules: C# loader and tests
- persisted final relational rows: database state after explicit manual apply
- cockpit presentation and fallback labels: frontend only as a temporary compatibility layer

## Relationship To Current Development Seeds

Development seed profiles remain separate from final production seed data.

Development seed profiles should continue to provide:

- deterministic QA ids
- richer cockpit screenshots and demos
- additive and idempotent local state preparation

They should not become the source for:

- final production catalog rows
- final starting civilization balances
- final live starting research/building packs

## SQL Server Readiness

This architecture is SQL Server-friendly because:

1. JSON files are provider-neutral
2. loader and validation tests do not require SQL Server
3. final apply remains explicit and manual
4. SQL generation or relational upsert logic can be reviewed separately from editorial content
5. content changes do not require automatic migration replay

## Current SQL Server Operator Sequence

After the SQL Server baseline schema exists, catalog work remains dry-run-first:

1. Set the SQL Server connection context outside the repository.
2. Run `scripts/sqlserver-final-catalog-seed.ps1` without `-Apply`.
3. Review the reported catalog file names and row counts.
4. Treat missing files, shape errors, or unexpected row counts as blockers.
5. Do not apply final relational catalog rows yet; the backend still reports `ApplyDeferred` for non-dry-run requests.

The future apply path must add final catalog tables, deterministic upsert behavior, reviewable output, and explicit operator confirmation before it can write to SQL Server. Development seed profiles remain separate and must not be used as the final production catalog seed path.

## Recommended Next Sequence

1. Add the JSON-or-source-file layout and naming convention.
2. Add typed loader contracts and validation tests.
3. Add the final seed service that consumes validated content.
4. Add the explicit operator-invoked final seed script.
5. Keep frontend fallbacks only until cockpit reads can rely on backend-served catalog metadata everywhere.

## Honest Current Status

- Current authoritative gameplay catalogs are still partly code-owned.
- Current Development seed profiles are still QA scaffolding only.
- The chosen final architecture is now defined, but not yet implemented.
- The current SQL Server catalog helper validates versioned JSON sources in dry-run mode, but final relational apply remains deferred.
- The remaining Block 38 seed tasks should implement this hybrid model, not replace it with SQL-first or UI-owned content.
