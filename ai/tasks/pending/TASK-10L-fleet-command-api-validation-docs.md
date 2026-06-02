# TASK-10L

---
id: TASK-10L
title: Phase 10L - Fleet command API validation documentation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10L"
priority: medium
---

## Goal
Document the fleet command lifecycle validation flow for local development without adding new UI or visual testing requirements.

## Context
Update the existing documentation surface so developers can validate the fleet command API flow locally with PowerShell examples and clear behavior notes.

## Implementation steps

1. Find the most appropriate documentation or README location for the fleet command flow.
2. Add manual PowerShell examples for health, seed, fleet UI state, travel estimate, create transfer, cancel transfer, and complete-due calls.
3. Document the expected outcomes and the development-only warning.

## Files to read first

- `README.md`
- `ai/architecture-index.md`
- `src/VoidEmpires.Web`

## Expected files to modify

- `README.md` or another existing docs file

## Acceptance criteria

- The doc includes the requested PowerShell examples using placeholder connection string text only.
- The doc explains the read-only estimate behavior and the transfer lifecycle outcomes.
- The doc warns that the endpoints are development-only and not gameplay UI or combat/interception execution.
- No frontend changes or screenshots.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` only if frontend files are touched

## Commit and push

At the end: run `git status`, stage the intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
