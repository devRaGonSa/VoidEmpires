# TASK-38B

---
id: TASK-38B
title: SQL Server provider dependency
status: done
type: backend
team: platform
supporting_teams: [backend]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: high
---

## Goal
Prepare infrastructure dependencies so SQL Server provider support can be introduced safely without breaking current Development and test flows.

## Context
This task belongs to the final SQL Server database and catalog consolidation block. The final product database target is SQL Server on user-managed infrastructure, but this block must keep secrets out of the repository, avoid applying migrations automatically to the real server, preserve the current Development and test flow, and keep gameplay expansion out of scope.

## Implementation steps
1. Read every file listed in "Files to read first" before editing.
2. Use ai/orchestrator/component-discovery.md to identify the smallest related component set.
3. Use ai/orchestrator/di-analysis.md before changing persistence registration, seed wiring, scripts, or composition roots.
4. Inspect current project references and packages that own EF Core persistence.
5. Add or adjust package dependencies required for SQL Server provider support while keeping the current checked-in runtime behavior unchanged.
6. Update code only as needed to keep the solution compiling after dependency changes.
7. Do not switch the default provider in this task.
8. Run the validation commands listed below before moving the task to done.

## Files to read first
- AGENTS.md
- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/orchestrator/di-analysis.md
- src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj
- src/VoidEmpires.Web/VoidEmpires.Web.csproj
- tests/VoidEmpires.Tests/VoidEmpires.Tests.csproj

## Expected files to modify
- src/VoidEmpires.Infrastructure/VoidEmpires.Infrastructure.csproj
- Optional: tests/VoidEmpires.Tests/VoidEmpires.Tests.csproj
- Optional: src/VoidEmpires.Web/VoidEmpires.Web.csproj

## Acceptance criteria
- The task goal is completed or narrowed with explicit blockers and safe next steps.
- All changed files match the Expected files to modify section or the commit message explains why.
- No passwords, secrets, unsafe connection strings, build artifacts, or local machine state are committed.
- The current Development and automated test flow remains intact unless the task explicitly documents a safe conditional path.
- No real SQL Server migration or destructive database change is applied automatically.
- No combat, fleet movement, market transactions, alliance mutations, or production-auth expansion is introduced.
- Required validation commands pass and results are recorded in the task or commit notes where appropriate.
- SQL Server EF Core provider dependencies are present in the relevant project files without switching the default runtime provider yet.

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
