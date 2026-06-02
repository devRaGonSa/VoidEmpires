# TASK-13D

---
id: TASK-13D
title: Phase 13D - Fleet v1 final QA docs
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12Y-13D"
priority: medium
---

## Goal
Update the final documentation and visual QA checklist for accepting Fleet v1 as a dev-cockpit screen.

## Context
This block should end with clear documentation that describes the accepted Fleet command set, the remaining prototype actions, and the final visual checks needed before moving on to another major area.

## Implementation steps

1. Update the Fleet-related documentation and validation checklists.
2. Document the supported controlled actions and the remaining prototype-only actions.
3. Add a final visual QA checklist that matches the current Fleet polish goals.
4. Keep the documentation free of secrets, screenshots, and local-only infrastructure details.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/fleet-controlled-mutation-checklist.md`
- `docs/dev/fleet-api-contracts.md`
- `src/VoidEmpires.Frontend/README.md`
- the root `README.md`, if relevant

## Expected files to modify

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/fleet-controlled-mutation-checklist.md`
- `docs/dev/fleet-api-contracts.md`
- `src/VoidEmpires.Frontend/README.md`
- `README.md`, if relevant

## Acceptance criteria

- The docs describe the accepted Fleet v1 command set clearly.
- Prototype-only actions are still called out explicitly.
- The final QA checklist covers compact development surfaces, clean labels, and clear order flow.
- Validation commands are documented.
- No screenshots or secrets are added.

## Constraints

- Do not include secrets, passwords, real connection strings, or private IPs.
- Do not add unrelated scope.
- Do not touch PostgreSQL directly.
- Keep the changes documentation-focused.

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
