# TASK-10I

---
id: TASK-10I
title: Phase 10I - Orbital travel estimate contract hardening
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10I"
priority: medium
---

## Goal
Harden the read-only orbital travel estimate behavior so it is reliable for future command execution and UI integration.

## Context
Target the estimate service, route profile service, fuel readiness service, API endpoint, and related tests. Keep the path conservative, validated, and read-only.

## Implementation steps

1. Inspect the estimate service, route profile service, fuel readiness service, endpoint, and fleet travel estimate tests.
2. Tighten validation for invalid civilization IDs, missing IDs, unknown entities, foreign groups, active-transfer conflicts, and destination-equals-current-planet cases where applicable.
3. Add tests proving the estimate path stays read-only and works for seeded or minimal realistic routes.

## Files to read first

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `tests/VoidEmpires.Tests`
- `ai/architecture-index.md`

## Expected files to modify

- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- Validation covers the listed invalid and conflict cases.
- Estimate remains read-only and does not mutate stockpiles, group status, or transfer state.
- No frontend changes or EF migrations.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end: run `git status`, stage the intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
