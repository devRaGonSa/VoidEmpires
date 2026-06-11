# TASK-32M

---
id: TASK-32M
title: Add development QA helper for onboarding and resource economy state
status: pending
type: feature
team: gameplay
supporting_teams: [backend, frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: medium
---

## Goal
Provide a development-only QA helper that prepares a playable session state for onboarding and resource-economy verification without manual database editing.

## Context
This block introduces more backend-backed state. QA needs a repeatable setup path that can create or prepare a playable start, print the relevant ids and resource snapshot, and optionally adjust resource timestamps if that can be done safely through development-only flows.

## Implementation steps

1. Review the existing development QA scripts and their parsing conventions.
2. Add a new development-only script or endpoint flow for preparing playable-session state.
3. Default the helper to `http://localhost:5142`.
4. Print returned ids, starting or current resource snapshot, and a clear warning that the helper mutates the development database.
5. Update the QA script parser coverage so the new helper is validated alongside the existing scripts.

## Files to read first

- `scripts/dev-qa-baseline.ps1`
- `scripts/check-dev-qa-scripts.ps1`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests`
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Expected files to modify

- `scripts/dev-qa-prepare-playable-session-state.ps1`
- `scripts/check-dev-qa-scripts.ps1`
- `src/VoidEmpires.Web/DevEndpointMappings.cs` only if a development-only helper endpoint is required
- `tests/VoidEmpires.Tests/*` if a new endpoint is added

## Acceptance criteria

- QA can prepare a playable session without manual SQL.
- The helper prints ids and resource snapshot information clearly.
- The helper warns that it mutates the development database.
- Script parsing checks pass.
- Backend tests pass if a new endpoint is introduced.

## Constraints

- Keep the helper development-only.
- Do not replace existing construction, research, or orbital QA scripts.
- Avoid unsafe direct database manipulation outside established development endpoints.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(dev): add playable session QA helper`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split endpoint work from script work if the change budget is exceeded.
