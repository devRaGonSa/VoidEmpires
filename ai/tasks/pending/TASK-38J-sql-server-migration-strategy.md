# TASK-38J

---
id: TASK-38J
title: SQL Server migration strategy
status: pending
type: docs
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: high
---

## Goal
Define the safe migration strategy for the final SQL Server database target.

## Context
This task belongs to the final SQL Server database and catalog consolidation block. The final product database target is SQL Server on user-managed infrastructure, but this block must keep secrets out of the repository, avoid applying migrations automatically to the real server, preserve the current Development and test flow, and keep gameplay expansion out of scope.

## Implementation steps
1. Read every file listed in "Files to read first" before editing.
2. Use ai/orchestrator/component-discovery.md to identify the smallest related component set.
3. Use ai/orchestrator/di-analysis.md before changing persistence registration, seed wiring, scripts, or composition roots.
4. Implement only the behavior, documentation, scripts, or validation required by this task goal.
5. Keep SQL Server credentials, passwords, and unsafe connection strings out of the repository.
6. Do not run migrations or database updates against the user's real SQL Server automatically.
7. Keep backend state authoritative and do not add new gameplay systems outside database or catalog consolidation.
8. Run the validation commands listed below before moving the task to done.
9. Decide whether the repository should use a separate SQL Server migration set, a provider-specific branch of migrations, or a replacement migration strategy, and justify the choice.
10. Document how to generate migrations, how to produce reviewable scripts, and how rollback and backup expectations should work.

## Files to read first
- AGENTS.md
- ai/tasks/pending/TASK-38A-final-sql-server-database-audit.md
- src/VoidEmpires.Infrastructure/
- docs/dev/final-db-phase-prep.md

## Expected files to modify
- docs/dev/final-db-phase-prep.md
- docs/dev/sql-server-migration-strategy.md

## Acceptance criteria
- The task goal is completed or narrowed with explicit blockers and safe next steps.
- All changed files match the Expected files to modify section or the commit message explains why.
- No passwords, secrets, unsafe connection strings, build artifacts, or local machine state are committed.
- The current Development and automated test flow remains intact unless the task explicitly documents a safe conditional path.
- No real SQL Server migration or destructive database change is applied automatically.
- No combat, fleet movement, market transactions, alliance mutations, or production-auth expansion is introduced.
- Required validation commands pass and results are recorded in the task or commit notes where appropriate.
- The migration strategy clearly states whether SQL Server uses separate or replacement migrations and why.
- Manual review, rollback, and backup considerations are documented and no migration is run automatically.

## Constraints
- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal and within the task scope
- Prefer small commits
- Keep UI and visible product copy Spanish-first where frontend text changes are required
- Preserve lazy loading and copy regression guard coverage when frontend files are touched
- If the change exceeds the task budget, stop and create a follow-up task instead of broadening scope

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
