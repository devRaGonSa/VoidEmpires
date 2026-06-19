# TASK-38AZ

---
id: TASK-38AZ
title: Final SQL Server DB catalog closure
status: done
type: documentation
team: platform
supporting_teams: [backend]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Close the current documentation-first SQL Server database and catalog preparation subset without overstating completion.

## Context
This task belongs to the final SQL Server database and catalog consolidation block. The final product database target is SQL Server on user-managed infrastructure, but this block must keep secrets out of the repository, avoid applying migrations automatically to the real server, preserve the current Development and test flow, and keep gameplay expansion out of scope.

## Implementation steps
1. Read every file listed in "Files to read first" before editing.
2. Use ai/orchestrator/component-discovery.md to identify the smallest related component set.
3. Use ai/orchestrator/di-analysis.md before changing persistence registration, seed wiring, scripts, or composition roots.
4. Review the current final DB readiness notes, current-state note, and pending task list.
5. Add a concise closure summary for the completed documentation-first SQL Server/catalog subset and the next implementation categories still pending.
6. Keep the closure factual and explicitly note that runtime provider cutover, migration replay, and final relational catalog ownership are not complete.
7. Run the validation commands listed below before moving the task to done.

## Files to read first
- AGENTS.md
- ai/current-state.md
- docs/dev/final-db-phase-readiness-report.md
- ai/tasks/pending/

## Expected files to modify
- ai/current-state.md
- Optional: docs/dev/final-db-phase-readiness-report.md

## Acceptance criteria
- The task goal is completed or narrowed with explicit blockers and safe next steps.
- All changed files match the Expected files to modify section or the commit message explains why.
- No passwords, secrets, unsafe connection strings, build artifacts, or local machine state are committed.
- The current Development and automated test flow remains intact unless the task explicitly documents a safe conditional path.
- No real SQL Server migration or destructive database change is applied automatically.
- No combat, fleet movement, market transactions, alliance mutations, or production-auth expansion is introduced.
- Required validation commands pass and results are recorded in the task or commit notes where appropriate.
- Closure notes clearly separate the completed documentation-first subset from the still-pending provider, migration, and catalog implementation work.

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

- `git diff --name-only`
- `git status`
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
