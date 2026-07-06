# SQL Server Baseline Migration Review

Status date: 2026-07-06

## Current Generation Result

`SqlServerInitialBaseline` is now available for review.

`TASK-40D` generated a migration, but that scaffold was rejected and removed because it reused the root PostgreSQL-shaped `VoidEmpiresDbContextModelSnapshot` and produced a provider-transition migration instead of an initial SQL Server schema baseline.

`TASK-40D1` added SQL Server design-time migration history isolation. `TASK-40D2` then regenerated the baseline and accepted these files:

- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/20260706131610_SqlServerInitialBaseline.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/20260706131610_SqlServerInitialBaseline.Designer.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/VoidEmpiresDbContextModelSnapshot.cs`

Initial generation checks:

- `Up` has initial-schema shape with 33 `CreateTable` calls and 67 `CreateIndex` calls.
- `Down` contains the corresponding `DropTable` cleanup path.
- generated metadata uses SQL Server types and annotations, not Npgsql metadata.
- root PostgreSQL migrations and the root PostgreSQL snapshot remain unchanged.
- no automatic migration apply was run.

## Detailed Review Result

Decision: the generated baseline is complete enough to proceed to idempotent SQL script generation for manual review.

Covered persisted tables include ASP.NET Core Identity tables, galaxy/solar-system/star/planet tables, player profile and civilization tables, ownership, resource stockpiles, production profiles, building state, construction orders, research projects and orders, orbital asset stock, orbital groups and transfers, exploration missions and knowledge, diplomacy, alliances, and population profiles.

Index review:

- construction due-order lookup exists as `IX_PlanetConstructionOrders_Status_EndsAtUtc_Sequence`
- research due-order lookup exists as `IX_ResearchOrders_Status_EndsAtUtc_Sequence`
- shipyard or orbital production due-order lookup exists as `IX_AssetProductionOrder_Target_Status_EndsAtUtc_Sequence`
- planet ownership lookup indexes exist as `ix_planet_ownerships_civilization_status` and `ux_planet_ownerships_planet_id`
- normalized-name indexes exist as `ux_player_profiles_normalized_display_name`, `ix_civilizations_normalized_name`, and `ux_civilizations_profile_normalized_name`
- catalog-key-style uniqueness exists for code-owned stock/project keys where applicable, including orbital asset stock, planetary asset stock, and research projects
- no final relational catalog seed tables are expected in this baseline because final catalog seed ownership remains a later task

Precision and type review:

- economy decimal values use explicit `decimal(18,4)` precision and scale
- GUID values use `uniqueidentifier`
- UTC `DateTime` values use `datetime2`
- Identity lockout timestamps use `datetimeoffset`

String length review:

- gameplay names and normalized names use bounded lengths such as `nvarchar(128)`
- alliance tags use `nvarchar(16)`
- diplomatic contact source uses `nvarchar(64)`
- research type uses `nvarchar(100)`
- ASP.NET Core Identity normalized fields use the expected `nvarchar(256)` or key-sized `nvarchar(450)`
- unbounded `nvarchar(max)` usage is limited to Identity extensibility fields such as claims, stamps, phone number, and password hash

Cascade review:

- cascade deletes are present for ASP.NET Core Identity dependent rows
- cascade deletes are present for core aggregate ownership paths: galaxy to solar systems, solar system to planets and star, and player profile to civilizations
- no cascade path was found that deletes broad gameplay state from a queue, resource, fleet, research, diplomacy, or ownership table outside those parent-child relationships

Secret and connection-string review:

- no seed data, real usernames, real passwords, full connection strings, or SQL Server target strings were found in generated migration files
- the only password-related generated field is the ASP.NET Core Identity `PasswordHash` column definition

## Required Next Step

## Static SQL Script Safety Review

`scripts/check-sqlserver-generated-script-safety.ps1` now provides a focused static guard for the committed generated review artifact at `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql`.

The guard checks that the script is the expected idempotent baseline review script, contains the manual-review header, and uses `__EFMigrationsHistory` gating. It fails on obvious unsafe SQL or credential-bearing fragments such as `DROP DATABASE`, `DROP LOGIN`, `DROP USER`, `TRUNCATE TABLE`, login/user creation statements, password assignments, and connection-string server or username values.

The guard intentionally allows normal EF migration-history operations, including `__EFMigrationsHistory` table creation, reads, and migration-history insertion. It does not prove that every schema detail is semantically correct, that the target database is ready, or that execution is safe for a live environment.

Manual review is still required before an operator opens the script in SSMS. That review must verify the target database, backups, expected schema shape, object ownership, operational timing, and rollback plan.

## Required Next Step

Run the static SQL script safety guard through the dev QA script, then perform manual SQL review. The script still must not be applied automatically, and any manual SSMS apply remains a later operator-controlled step.

No SQL Server migration was applied automatically, no generated SQL was run, and no real SQL Server connection string or credential was added.
