# Final DB Phase Readiness Report

Status date: 2026-06-19

This report summarizes the current readiness of the `VoidEmpires` final database phase while the repository still remains PostgreSQL-first in checked-in runtime and design-time behavior.

The final intended production database target is an external, user-managed SQL Server instance. That target is documented here as an operations path, not as an already-completed repository cutover.

## Ready Now

- safe SQL Server documentation exists for connection setup, security posture, backup and restore planning, and migration dry-run expectations
- checked-in appsettings remain placeholder-safe and do not contain a real SQL Server password
- the repository does not auto-apply migrations during startup, tests, or helper-script execution
- completed Block 38 work did not run a real SQL Server migration, update, backup, restore, or seed apply automatically against a user-managed server
- the repository includes one opt-in SQL Server connection smoke test gate, while ordinary validation remains provider-independent
- the latest Block 38 cross-stack validation gate passed:
  - `dotnet build --no-restore`
  - `dotnet test --no-build`
  - `npm run build --prefix src/VoidEmpires.Frontend`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
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

- configurable provider selection instead of hard-coded Npgsql runtime and design-time wiring
- a defined SQL Server migration-baseline strategy
- a validated SQL Server script-generation helper
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
Server=192.168.178.28,1433;Database=VoidEmpires;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

This template is placeholder-only. Real values must remain external to the repository.

Explicit SQL Server selection must also remain external, for example through `VoidEmpires__Persistence__Provider=sqlserver`.

## Current Migration And Seed Position

Migration position:

- the checked-in EF migration history is PostgreSQL-shaped today
- current migrations are not validated for direct SQL Server replay
- SQL Server migration work must remain manual and explicitly reviewed

Seed position:

- Development seed profiles are QA scaffolding only
- static gameplay catalogs remain code-owned, not final relational seed data
- production-owned final seed data is still pending later tasks

## Current Safety Posture

- no real SQL Server migration was applied automatically
- no helper script in the repository applies SQL Server changes automatically to a real server
- no real SQL Server password is committed in checked-in docs, scripts, or config
- the current repository secret scan is green and only allows documented placeholder values for passwords or other obvious secrets
- SQL Server guidance remains manual by default for connection setup, backups, restore, script review, and apply
- existing PowerShell helpers remain validation, guidance, or explicitly invoked Development utilities rather than hidden SQL Server mutation automation

## Risks Before Go-Live

1. PostgreSQL remains the checked-in default, so operators must still select SQL Server explicitly in external configuration.
2. Existing migration history is PostgreSQL-specific and not yet re-baselined for SQL Server.
3. Catalog and seed ownership remain incomplete for final production initialization.
4. The current SQL Server smoke test proves connection-only behavior, not schema replay readiness.
5. Any real SQL Server rollout still depends on careful operator-managed credentials, backups, restore drills, and manual script review.

## Current Honest Decision

Decision: documentation and validation prep for the final SQL Server phase is ready.

Decision not granted: repository-level SQL Server runtime readiness, SQL Server migration replay readiness, final relational seed readiness, or production cutover readiness.
