# TASK-30I

---
id: TASK-30I-research-dev-qa-script-alignment
title: Align QA scripts and docs with research enqueue flow
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Document and align backend and frontend QA paths for the new persisted research enqueue behavior.

## Context
Research now has two approved execution paths that should stay explicit and safe:
backend helper script and frontend confirmation path.

## Implementation steps

1. Update `docs/dev/research-cockpit-checklist.md` with explicit research UX scope and constraints.
2. Update `docs/dev/persisted-gameplay-flow-checklist.md` with:
   - backend-only script path
   - frontend `/research` confirmation path
3. Note both paths persist to Development DB.
4. Note open-order no-op behavior for reused DB scenarios.
5. Keep script behavior unchanged unless strictly necessary.

## Files to read first

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `scripts/dev-qa-create-research-order.ps1`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`

## Expected files to modify

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/research-cockpit-checklist.md`

## Acceptance criteria

- Both QA paths are documented as real persisted operations.
- Reused DB no-op behavior is documented.
- No script logic change unless explicitly required.

## Constraints

- No gameplay expansion outside Research real enqueue.
- Keep copy aligned with Spanish-first review expectations.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `docs: align QA scripts and research enqueue checklist`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
