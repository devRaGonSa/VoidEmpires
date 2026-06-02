# TASK-13J

---
id: TASK-13J
title: Phase 13J - Strategic map playable cockpit docs and QA
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13E-13J"
priority: medium
---

## Goal
Document the Galaxia and strategic map playable cockpit milestone and add manual visual QA instructions.

## Context
The strategic map work in this block should finish with clear docs and a practical visual QA checklist that matches the current read-only cockpit behavior.

## Implementation steps

1. Update the documentation and validation checklists that mention the strategic map.
2. Describe the current readable strategic map behavior and the remaining read-only boundary.
3. Add a final manual visual QA checklist for layout, selection, markers, and technical surfaces.
4. Keep the documentation free of secrets and local-only infrastructure details.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `src/VoidEmpires.Frontend/README.md`
- the root `README.md`, if relevant
- strategic map docs, if present

## Expected files to modify

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `src/VoidEmpires.Frontend/README.md`
- `README.md`, if relevant
- strategic map docs, if present

## Acceptance criteria

- The docs describe the current strategic map behavior clearly.
- The read-only boundary is documented.
- The final visual QA checklist covers layout, selection, planets, markers, and technical panels.
- Validation commands are documented.
- No screenshots or secrets are added.

## Constraints

- Do not include secrets, passwords, real connection strings, or private IPs.
- Do not include screenshots.
- Do not add unrelated scope.
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
