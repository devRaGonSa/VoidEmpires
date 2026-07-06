# TASK-41AW

---
id: TASK-41AW
title: Final SQL Server product readiness note
status: pending
type: docs
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Update SQL Server docs with product-facing state.

## Context
SQL Server support remains accepted, and the product UI work should be documented without changing database behavior.

## Implementation steps

1. Review SQL Server checklist and product readiness docs.
2. Record that schema was manually applied and `cockpit-validation` seed was accepted against SQL Server.
3. Record that product UI now hides development wording by default.
4. Record remaining final seed, assets, QA, and correction phases.
5. Keep all real credentials and connection strings out of the repository.

## Files to read first

- docs/dev/sql-server-user-checklist.md
- docs/dev/product-readiness-report.md
- ai/current-state.md

## Expected files to modify

- docs/dev/sql-server-user-checklist.md
- docs/dev/product-readiness-report.md
- ai/current-state.md

## Acceptance criteria

- SQL Server docs reflect schema and seed acceptance status.
- Product UI development-wording removal is documented.
- Remaining final phases are explicit.

## Constraints

- Do not apply migrations automatically.
- Do not run real SQL Server mutations.
- Do not commit secrets or resolved connection strings.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
