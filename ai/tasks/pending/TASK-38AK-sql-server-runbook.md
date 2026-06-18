# TASK-38AK

---
id: TASK-38AK
title: SQL Server runbook
status: pending
type: docs
team: platform
supporting_teams: [backend]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: high
---

## Goal
Create a practical SQL Server runbook for the user-managed final database workflow.

## Context
This task belongs to the final SQL Server database and catalog consolidation block. The final product database target is SQL Server on user-managed infrastructure, but this block must keep secrets out of the repository, avoid applying migrations automatically to the real server, preserve the current Development and test flow, and keep gameplay expansion out of scope.
Safe SQL Server template to preserve in docs and examples: `Server=192.168.178.28,1433;Database=VoidEmpires;User Id=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`

## Implementation steps
1. Read every file listed in "Files to read first" before editing.
2. Use ai/orchestrator/component-discovery.md to identify the smallest related component set.
3. Use ai/orchestrator/di-analysis.md before changing persistence registration, seed wiring, scripts, or composition roots.
4. Implement only the behavior, documentation, scripts, or validation required by this task goal.
5. Keep SQL Server credentials, passwords, and unsafe connection strings out of the repository.
6. Do not run migrations or database updates against the user's real SQL Server automatically.
7. Keep backend state authoritative and do not add new gameplay systems outside database or catalog consolidation.
8. Run the validation commands listed below before moving the task to done.
9. Document how to create the database in SSMS, set environment variables, run the connection smoke script, generate migration scripts, apply scripts manually, seed catalogs, and back up the database.
10. Keep passwords out of the runbook and make manual steps explicit where automation is intentionally withheld.

## Files to read first
- AGENTS.md
- docs/dev/final-db-phase-prep.md
- scripts/
- src/VoidEmpires.Web/
- src/VoidEmpires.Infrastructure/

## Expected files to modify
- docs/dev/sql-server-runbook.md

## Acceptance criteria
- The task goal is completed or narrowed with explicit blockers and safe next steps.
- All changed files match the Expected files to modify section or the commit message explains why.
- No passwords, secrets, unsafe connection strings, build artifacts, or local machine state are committed.
- The current Development and automated test flow remains intact unless the task explicitly documents a safe conditional path.
- No real SQL Server migration or destructive database change is applied automatically.
- No combat, fleet movement, market transactions, alliance mutations, or production-auth expansion is introduced.
- Required validation commands pass and results are recorded in the task or commit notes where appropriate.
- The runbook covers creation, configuration, smoke validation, migration scripting, manual apply, seeding, and backup guidance.
- No password or unsafe credential storage is documented.

## Constraints
- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal and within the task scope
- Prefer small commits
- Keep UI and visible product copy Spanish-first where frontend text changes are required
- Preserve lazy loading and copy regression guard coverage when frontend files are touched
- If the change exceeds the task budget, stop and create a follow-up task instead of broadening scope
- Do not include the real SQL Server password in any file, script, task note, or commit message

## Validation
Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push
At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear task-specific message.
5. Push the branch if the repository workflow expects it.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits per task.
- If the change would exceed these limits, create a follow-up task and stop expanding scope.
