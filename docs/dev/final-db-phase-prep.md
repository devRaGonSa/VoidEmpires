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
- No SQL Server integration or smoke tests are configured today.

## Current Honest Status

This is a documentation-only prep note. It does not add migrations, SQL Server scripts, or automatic provider-specific validation.
