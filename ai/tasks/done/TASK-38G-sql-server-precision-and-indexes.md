# TASK-38G

---
id: TASK-38G
title: SQL Server precision and indexes
status: done
type: backend
team: platform
supporting_teams: [backend]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: high
---

## Goal
Audit and tighten EF model precision and index configuration where SQL Server compatibility is likely to matter, without broadening into a migration rollout.

## Context
This task belongs to the final SQL Server database and catalog consolidation block. The final product database target is SQL Server on user-managed infrastructure, but this block must keep secrets out of the repository, avoid applying migrations automatically to the real server, preserve the current Development and test flow, and keep gameplay expansion out of scope.

## Implementation steps
1. Read every file listed in "Files to read first" before editing.
2. Use ai/orchestrator/component-discovery.md to identify the smallest related component set.
3. Use ai/orchestrator/di-analysis.md before changing persistence registration, seed wiring, scripts, or composition roots.
4. Inspect entity configurations for decimals, timestamps, unique indexes, filtered indexes, and provider-sensitive mappings.
5. Tighten only the model configuration that is clearly needed for provider compatibility or later migration stability.
6. Add or update tests only if they directly protect the adjusted model behavior.
7. Do not generate or apply new migrations in this task.
8. Run the validation commands listed below before moving the task to done.

## Files to read first
- AGENTS.md
- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/orchestrator/di-analysis.md
- src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- tests/VoidEmpires.Tests/

## Expected files to modify
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- Optional: tests/VoidEmpires.Tests/

## Acceptance criteria
- The task goal is completed or narrowed with explicit blockers and safe next steps.
- All changed files match the Expected files to modify section or the commit message explains why.
- No passwords, secrets, unsafe connection strings, build artifacts, or local machine state are committed.
- The current Development and automated test flow remains intact unless the task explicitly documents a safe conditional path.
- No real SQL Server migration or destructive database change is applied automatically.
- No combat, fleet movement, market transactions, alliance mutations, or production-auth expansion is introduced.
- Required validation commands pass and results are recorded in the task or commit notes where appropriate.
- Precision or index configuration changes are minimal, intentional, and compatible with keeping PostgreSQL as the current default provider.

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
- `git diff --name-only`
- `git status`

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
