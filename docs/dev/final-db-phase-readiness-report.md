# Final DB Phase Readiness Report

Status date: 2026-07-06

This report summarizes the current readiness of the `VoidEmpires` final database phase while the repository still remains PostgreSQL-first in checked-in runtime and design-time behavior.

The final intended production database target is an external, user-managed SQL Server instance. That target is documented here as an operations path, not as an already-completed repository cutover.

## Ready Now

- safe SQL Server documentation exists for connection setup, security posture, backup and restore planning, and migration dry-run expectations
- checked-in appsettings remain placeholder-safe and do not contain a real SQL Server password
- the repository does not auto-apply migrations during startup, tests, or helper-script execution
- completed Block 38 work did not run a real SQL Server migration, update, backup, restore, or seed apply automatically against a user-managed server
- the repository includes one opt-in SQL Server connection smoke test gate, while ordinary validation remains provider-independent
- the latest Block 40O final validation gate passed without requiring a real SQL Server:
  - `dotnet build --no-restore`
  - `dotnet test --no-build`
  - `npm run build --prefix src/VoidEmpires.Frontend`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- latest recorded results: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `744` passing tests, `0` failed, and `0` skipped; frontend build succeeded with `106` transformed modules and a `181.33 kB` minified / `59.13 kB` gzip shared entry chunk; the QA, route lazy-import, frontend copy, generated SQL safety, and repository secret guards all passed; `git status --short` before recording results showed only the in-progress task move
- repository-local copy and secret guards now check for unsafe connection-string examples and obvious committed secret patterns without external tooling, including `scripts/check-repo-secret-scan.ps1`

## Manual Operator Steps Still Required

Before any real SQL Server cutover attempt, an operator must still:

1. create or select the target SQL Server database manually
2. provision application credentials outside source control
3. inject connection strings through environment variables or secret storage
4. take and verify backups manually
5. review any future SQL migration scripts manually
6. apply any future SQL migration scripts manually in SSMS or an equivalent explicit operator workflow
7. restore-test backups on a disposable target before treating them as production protection

## Deferred Repository Work

The following work remains incomplete inside the repository:

- manual SSMS apply and post-apply verification of the accepted SQL Server baseline
- final relational catalog ownership for currently code-owned gameplay catalogs
- final seed architecture for production initialization instead of Development-only QA profiles
- any broader SQL Server validation beyond the current opt-in connection smoke check

## Current Connection Setup Position

Current checked-in behavior:

- web startup reads `ConnectionStrings:DefaultConnection`
- web startup reads `VoidEmpires:Persistence:Provider` and stays on PostgreSQL unless `sqlserver` is selected explicitly
- persistence wiring is enabled only when that connection string is non-empty
- runtime persistence now supports explicit provider selection while keeping PostgreSQL as the checked-in default path
- design-time DbContext creation now supports explicit provider selection through environment variables while keeping PostgreSQL as the fallback

Documented SQL Server target shape:

```text
Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

This template is placeholder-only. Real values must remain external to the repository.

Explicit SQL Server selection must also remain external, for example through `VoidEmpires__Persistence__Provider=sqlserver`.

Local app run command shape after manual schema preparation:

```powershell
$env:VoidEmpires__Persistence__Provider="SqlServer"
$env:ConnectionStrings__DefaultConnection="Server=<HOST>,1433;Database=VoidEmpires_Dev;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
$env:VOIDEMPIRES_CONNECTION_STRING=$env:ConnectionStrings__DefaultConnection
dotnet run --project .\src\VoidEmpires.Web\VoidEmpires.Web.csproj
```

This is a local operator validation path only. The SQL Server schema must exist first, and health diagnostics do not prove migration replay or production readiness.

## Current Migration And Seed Position

Migration position:

- the checked-in EF migration history is PostgreSQL-shaped today
- current migrations are not validated for direct SQL Server replay
- `SqlServerInitialBaseline` now exists in the isolated SQL Server migration folder and is not part of the root PostgreSQL-shaped chain
- the idempotent SQL Server baseline script exists as a committed review artifact and has passed static safety and secret scans
- SQL Server schema apply remains manual and explicitly reviewed; no generated SQL was executed by the repository workflow

Accepted SQL Server baseline strategy:

1. Keep the checked-in PostgreSQL-first path and existing root migration history intact.
2. Keep SQL Server migrations isolated under `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer`.
3. Treat the root migration chain as historical PostgreSQL context only; do not replay it directly against SQL Server.
4. Use the reviewed idempotent SQL script only through manual SSMS/operator review.
5. Keep migration apply, backup, restore, and production seeding outside app startup and outside default automated validation.

Seed position:

- Development seed profiles are QA scaffolding only
- static gameplay catalogs remain code-owned, not final relational seed data
- production-owned final seed data is still pending later tasks
- `scripts/sqlserver-final-catalog-seed.ps1` remains dry-run-first for SQL Server context; apply requires explicit confirmation and currently remains safely deferred rather than deleting or mutating user gameplay data
- post-baseline catalog audit confirms the checked-in source files currently validate as `15` buildings, `8` research items, `4` orbital assets, `1` defense item, and `7` resources, but the backend still has no non-dry-run relational apply path and no dedicated final catalog table ownership model in the accepted SQL Server baseline

## Current Safety Posture

- no real SQL Server migration was applied automatically
- no helper script in the repository applies SQL Server changes automatically to a real server
- no real SQL Server password is committed in checked-in docs, scripts, or config
- the current repository secret scan is green and only allows documented placeholder values for passwords or other obvious secrets
- the latest final validation gate did not connect to SQL Server, execute generated SQL, run `dotnet ef database update`, apply seeds, back up, or restore
- SQL Server guidance remains manual by default for connection setup, backups, restore, script review, and apply
- existing PowerShell helpers remain validation, guidance, or explicitly invoked Development utilities rather than hidden SQL Server mutation automation

## Risks Before Go-Live

1. PostgreSQL remains the checked-in default, so operators must still select SQL Server explicitly in external configuration.
2. Existing root migration history remains PostgreSQL-specific; SQL Server uses the isolated baseline path.
3. The accepted SQL Server baseline still needs manual SSMS apply and post-apply verification before any runtime validation can rely on the target schema.
4. Catalog and seed ownership remain incomplete for final production initialization; the next implementation step must decide the relational catalog model and deterministic upsert behavior before any `-Apply` path can be enabled.
5. The current SQL Server smoke test proves connection-only behavior, not schema replay readiness.
6. Any real SQL Server rollout still depends on careful operator-managed credentials, backups, restore drills, and manual script review.

## Current Honest Decision

Decision: the SQL Server initial baseline migration block has passed final repository validation and is ready for manual operator review/apply steps.

Decision not granted: automatic schema apply, repository-level SQL Server runtime cutover, final relational seed readiness, or production cutover readiness.
