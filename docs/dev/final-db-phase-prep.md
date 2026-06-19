# Final DB Phase Prep

Status date: 2026-06-18

This note is the lightweight entrypoint for the final database/model phase.

The detailed phase plan remains in:

- `docs/dev/product-readiness-report.md`

The SQL Server-specific test-position note for that phase is:

- `docs/dev/sql-server-test-strategy.md`

## Entry Rule

Start final DB/model work only if the repository keeps these boundaries intact:

- ordinary `dotnet test` runs remain provider-independent
- no real SQL Server migration or update is applied automatically
- secrets and connection strings stay external to the repository
- Development-only demo and QA flows remain intact unless a task explicitly narrows them

## Required References

Use these documents together before expanding SQL Server-specific work:

- `docs/dev/product-readiness-report.md`
- `docs/dev/sql-server-test-strategy.md`
- `docs/dev/catalog-metadata-readiness.md`
- `docs/dev/building-catalog-final-db-readiness.md`
- `docs/dev/research-catalog-final-db-readiness.md`
- `docs/dev/resource-economy-final-db-readiness.md`
- `docs/dev/ship-catalog-final-db-readiness.md`
- `docs/dev/defense-catalog-final-db-readiness.md`

## Current Testing Position

- Default build/test validation remains:
  - `dotnet build --no-restore`
  - `dotnet test --no-build`
- SQL Server-specific smoke coverage is optional and should be explicitly gated.
- The repository currently has one opt-in SQL Server connection smoke test, but no broader SQL Server integration-test suite.

Latest Block 38 cross-stack validation gate:

- `dotnet build --no-restore` succeeded with `0` errors; one rerun reported transient `MSB3026` copy-retry warnings while `testhost` held test output DLLs, but the build still completed successfully
- `dotnet test --no-build` succeeded with `725` passing tests, `0` failed, and `0` skipped
- `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules and the shared entry chunk at `181.33 kB` minified / `59.14 kB` gzip
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded, including the frontend lazy-import guard, copy-regression guard, and the new repository secret scan
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded

## Current Persistence Audit

Status date: 2026-06-18

Current persistence facts before any SQL Server cutover work:

- EF Core provider package in source today: `Npgsql.EntityFrameworkCore.PostgreSQL` in `src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj`.
- Runtime registration today: `AddVoidEmpiresPersistence` in `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs` calls `UseNpgsql(connectionString)` directly.
- Design-time registration today: `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs` also calls `UseNpgsql(...)`.
- Web startup reads `ConnectionStrings:DefaultConnection` and only wires persistence, Identity, and queue workers when that value is non-empty.
- Repository appsettings keep `ConnectionStrings:DefaultConnection` empty by default in both `appsettings.json` and `appsettings.Development.json`; real connection strings are expected outside source control.
- Repository appsettings may include a non-operative `ConnectionStrings:SqlServerTargetTemplate` reference string for documentation only; startup does not read it and it must never be replaced with a real password in source control.
- Design-time fallback sources are external environment variables first:
  - `ConnectionStrings__DefaultConnection`
  - `VOIDEMPIRES_CONNECTION_STRING`
  - fallback local design value: `Host=localhost;Database=voidempires_design`

Current EF model and migration facts:

- The single EF context is `VoidEmpiresDbContext`, which inherits from `IdentityDbContext<VoidEmpiresUser>`.
- Current DbSets cover identity-linked player state, galaxy/system/planet state, ownership, economy, buildings, research, diplomacy, exploration, orbital transfers, and related read/write gameplay rows.
- Migrations live under `src/VoidEmpires.Infrastructure/Persistence/Migrations`.
- The current migration history is additive and code-first; the latest checked-in migration is `20260618152037_AddNormalizedPlayerLookupColumns.cs`.
- The repository does not apply migrations automatically at app startup.

Current catalog and seed sources:

- Development seed setup is owned by `DevelopmentSeedService` and `DevelopmentSeedProfiles`; it creates deterministic Development-only rows and does not represent final production initialization.
- Static gameplay catalog metadata is still code-owned, not relationally seeded:
  - `BuildingCatalog`
  - `ResearchCatalog`
  - `OrbitalAssetCatalog`
  - `PlanetaryAssetCatalog`
- Current frontend/read-model notes already document that final relational seed work still needs Spanish labels, short descriptions, category metadata, module ownership, image keys, and revision metadata.

Current PostgreSQL-specific assumptions that remain relevant while preparing for the final SQL Server target:

- Provider selection is hard-coded to Npgsql in both runtime and design-time paths.
- Existing checked-in migrations contain PostgreSQL-native store types such as `uuid`, `timestamp with time zone`, and `character varying(...)`.
- Identity migrations and snapshot output also assume Npgsql annotations and PostgreSQL identity conventions.
- Some current model configuration is provider-sensitive:
  - decimal resource/production values use `HasPrecision(18, 4)`
  - filtered indexes use PostgreSQL-style quoted filter SQL, for example `\"PlanetId\" IS NULL` in `ExplorationKnowledgeConfiguration`
  - recent migration backfill SQL includes explicit PostgreSQL branches and separate SQL Server branches for normalized-name rollout
- Test coverage for provider wiring is still Npgsql-specific today; `PersistenceRegistrationTests` explicitly assert the Npgsql options extension.

Current SQL Server target constraints and cutover risks:

1. Provider selection must become configurable without changing the meaning of ordinary local/test runs.
2. Existing migration history is PostgreSQL-shaped; a SQL Server target may require a separate baseline or carefully regenerated migration set rather than direct reuse.
3. Provider-specific filtered-index SQL and migration SQL branches must be audited case by case before claiming SQL Server replay safety.
4. Static code catalogs and Development seed profiles still sit outside final relational seed ownership, so provider cutover alone is not enough for final DB readiness.
5. Ordinary CI and local tests currently depend on InMemory plus Npgsql configuration assumptions; SQL Server validation must remain explicitly gated.
6. No repository path should auto-apply migrations to a user-managed SQL Server during startup, tests, or helper scripts.

Current honest conclusion:

- The repository is still PostgreSQL-first in package references, runtime wiring, design-time tooling, and existing migration artifacts.
- SQL Server remains a documented future target, but not a configured runtime provider in the current checked-in implementation.
- Final SQL Server cutover work should start only after provider selection, migration-baseline strategy, and catalog/seed ownership are separated into explicit follow-up tasks.

## Current Honest Status

This is a documentation-only prep note. It does not add migrations, SQL Server scripts, or automatic provider-specific validation.
