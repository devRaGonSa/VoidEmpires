# TASK-11S

---
id: TASK-11S
title: Phase 11S - Controlled mutation non-visual regression checklist
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11S"
priority: medium
---

## Goal
Add a focused non-visual regression checklist for the estimate, confirm, and create-transfer flow.

## Context
The controlled mutation path now spans read-only estimate, explicit confirmation, create-transfer execution, and post-success refresh. A compact checklist helps keep that sequence stable without requiring manual visual review for every iteration.

## Implementation steps

1. Review `docs/dev/fleet-api-contracts.md` and any existing smoke checklist or validation notes for a place to keep the new regression checklist compact.
2. Add or update a checklist that covers backend build, backend tests, frontend build, seed apply, fleet ui-state read, estimate behavior, create-transfer execution, refreshed ui-state, and stale or duplicate rejection behavior.
3. Explicitly note that manual visual validation is not required for this block unless a clear regression appears.
4. Keep the checklist aligned with the existing dev-only workflow and prototype boundary.
5. Update related docs only if a lightweight consistency fix is needed.
6. Run the required validation commands before completing the task.

## Files to read first

- docs/dev/fleet-api-contracts.md
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/README.md
- README.md
- ai/current-state.md

## Expected files to modify

- docs/dev/fleet-api-contracts.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-controlled-mutation-checklist.md
- README.md

## Acceptance criteria

- The checklist covers build, tests, seed apply, ui-state read, estimate, create-transfer, refreshed state, and stale or duplicate rejection checks.
- The checklist clearly states that manual visual validation is not required for this block.
- The checklist stays focused on non-visual validation and dev-only behavior.
- No screenshots are introduced.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched.

## Constraints

- Keep the work documentation and test-alignment focused.
- Do not add frontend execution for additional mutation commands.
- Do not apply EF migrations.
- Add or update backend or frontend tests only if a clear gap is discovered.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single commit for this task.
- If the checklist grows into broader test suite work, split the extra work into a follow-up task.
