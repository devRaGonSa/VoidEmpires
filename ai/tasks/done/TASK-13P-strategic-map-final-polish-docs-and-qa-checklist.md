# TASK-13P

---
id: TASK-13P
title: Phase 13P - Strategic map final polish docs and QA checklist
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Document the final Galaxy v1 read-only cockpit acceptance criteria after the 13K-13P polish pass.

## Context
Block 13E-13J established a technically valid strategic cockpit, but later visual QA identified polish gaps in language, hierarchy, diagnostics, and map readability. The documentation should record that distinction and define the final acceptance checklist for the polished read-only cockpit.

## Implementation steps

1. Update or create the strategic map cockpit QA checklist document.
2. Record that the 13E-13J block was technically valid but required the 13K-13P polish pass.
3. Add the final build, test, and manual visual acceptance checklist for Galaxy v1.
4. Update `ai/current-state.md` or equivalent current-state documentation if present.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/strategic-map-cockpit-checklist.md`, if present
- `src/VoidEmpires.Frontend/README.md`
- `ai/current-state.md`, if present
- `README.md`

## Expected files to modify

- `docs/dev/strategic-map-cockpit-checklist.md`, create or update
- `docs/dev/frontend-foundation-smoke-checklist.md`, if needed
- `src/VoidEmpires.Frontend/README.md`, if needed
- `ai/current-state.md`, if needed

## Acceptance criteria

- The docs record the need for the 13K-13P Galaxy polish pass.
- The final Galaxy v1 checklist covers build, test, npm build, Spanish-first copy, map priority, clear system focus, readable planets, understandable fleet or transfer markers, read-only behavior, no 3D, and collapsed diagnostics.
- The current-state docs stay consistent with the documented frontend milestone.
- No task implementation is performed in this documentation task.

## Constraints

- Keep the scope documentation-only.
- Do not add screenshots or secrets.
- Do not imply Galaxy mutations are supported.
- Keep the read-only boundary explicit.

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
