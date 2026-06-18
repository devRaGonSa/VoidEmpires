# TASK-38ABBC

---
id: TASK-38ABBC
title: Normalized name column readiness
status: pending
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Add a provider-agnostic, index-friendly normalization strategy for display-name and civilization-name uniqueness checks.

## Context
`StartingCivilizationService` currently enforces case-insensitive uniqueness with `ToLower()` comparisons. That query shape is not index-friendly and is not honestly fixed by adding plain indexes alone. A safe readiness fix needs schema-backed normalized values or an equivalent persisted strategy.

## Implementation steps
1. Read the files listed below before editing.
2. Choose the smallest persisted normalization approach that fits the current repository conventions.
3. Update the relevant persistence configuration and service query shapes.
4. Add or update tests for case-insensitive duplicate detection.
5. Keep the change safe for local Development flow and avoid automatic real-database migration execution.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/PlayerProfileConfiguration.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/CivilizationConfiguration.cs
- tests/VoidEmpires.Tests/StartingCivilizationServiceTests.cs

## Expected files to modify
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria
- Case-insensitive duplicate detection no longer relies on non-sargable lowercase comparisons.
- The chosen strategy is compatible with the SQL Server readiness direction.
- Validation commands pass.
- No unrelated files are modified.

## Constraints
- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation
Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push
At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only`.
3. Stage the intended files.
4. Commit with a clear message.
5. Push the branch to the remote.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
