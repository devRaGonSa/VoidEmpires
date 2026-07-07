# TASK-42V-sql-server-registration-readiness-doc

---
id: TASK-42V
title: SQL Server registration readiness doc
status: done
type: docs
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Document how registration behaves on SQL Server.

## Context
The SQL Server real database exists and the initial baseline has been manually applied. Registration should use Identity tables and bootstrap the needed universe state without manual seed.

## Implementation steps

1. Review the SQL Server runbook and migration strategy docs.
2. Document that Identity tables are used for account storage.
3. Document that new account creation requires no manual seed when bootstrap creates required galaxy/system/planet records.
4. Note what remains deferred, such as manual operational verification and production hardening.
5. Avoid claiming manual QA was performed.

## Files to read first

- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-test-strategy.md
- docs/dev/final-db-phase-readiness-report.md
- docs/dev/user-account-auth-readiness.md

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/user-account-auth-readiness.md
- docs/dev/product-readiness-report.md

## Acceptance criteria

- SQL Server registration behavior is documented.
- Identity table usage is documented.
- Initial world bootstrap requirements are documented.
- Deferred/manual checks are clearly labeled as not performed.

## Constraints

- Documentation only unless a tiny reference update is required.
- Do not commit connection strings or secrets.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
