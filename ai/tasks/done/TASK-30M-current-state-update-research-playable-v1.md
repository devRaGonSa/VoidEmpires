# TASK-30M

---
id: TASK-30M-current-state-update-research-playable-v1
title: Record Research as second controlled persisted mutation
status: done
type: platform
team: platform
supporting_teams: [platform]
roadmap_item: ""
priority: medium
---

## Goal
Update `ai/current-state.md` with the accepted Research v1 capabilities and remaining constraints.

## Context
Current state should reflect that Construction is first accepted mutation and Research is second, with explicit behavior boundaries.

## Implementation steps

1. Update `ai/current-state.md` with:
   - Research supports real persisted enqueue UX v1
   - Development-scope
   - explicit confirmation requirement
   - backend-confirmed resource deduction
   - backend state refresh on success
   - no automatic completion
   - Construction remains first accepted mutation
   - other cockpits remain read-only unless previously accepted
2. Keep test count accurate.
3. Keep visual QA as user-driven.
4. Ensure no speculative claims about implemented future scope.

## Files to read first

- `ai/current-state.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/research-cockpit-checklist.md`

## Expected files to modify

- `ai/current-state.md`

## Acceptance criteria

- Current state accurately names Research v1 behavior and boundaries.
- No overclaim regarding combat/multiplayer/other expansions.

## Constraints

- No source code changes in this task.
- Use concise, truthful status language.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `docs: update current-state for research v1 playable`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
