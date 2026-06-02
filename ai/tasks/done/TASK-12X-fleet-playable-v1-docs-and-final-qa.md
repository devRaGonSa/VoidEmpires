# TASK-12X

---
id: TASK-12X
title: Phase 12X - Fleet playable v1 docs and final QA
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12S-12X"
priority: medium
---

## Goal
Document Fleet cockpit playable v1 and update the final visual QA checklist for the command set now supported by the frontend.

## Context
The Fleet work in this block is meant to end with clear documentation and a final manual QA checklist. The docs should describe what is currently supported, what remains prototype-only, and how to validate the screen visually before deciding whether one last polish pass is needed.

## Implementation steps

1. Update the dev and frontend docs that describe Fleet actions and validation.
2. Record the supported controlled actions and the still-prototype actions.
3. Add a final visual QA checklist for Spanish copy, readable labels, active transfers, and controlled mutations.
4. Keep the documentation free of secrets, screenshots, and private infrastructure details.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/fleet-controlled-mutation-checklist.md`
- `docs/dev/fleet-api-contracts.md`
- `src/VoidEmpires.Frontend/README.md`
- the root `README.md`, if it mentions Fleet workflows

## Expected files to modify

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/fleet-controlled-mutation-checklist.md`
- `docs/dev/fleet-api-contracts.md`
- `src/VoidEmpires.Frontend/README.md`
- `README.md`, if relevant

## Acceptance criteria

- The docs clearly describe Fleet cockpit v1 supported actions.
- Prototype-only actions are still called out as disabled or experimental.
- The final manual QA checklist covers the visible Fleet UX goals from this block.
- Validation commands are documented.
- No screenshots or secrets are added.

## Constraints

- Keep the changes documentation-focused.
- Do not include secrets, passwords, real connection strings, or private IPs.
- Do not add unrelated product scope.
- Keep the change minimal.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

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
