# TASK-9F

---

id: TASK-9F
title: Ignore frontend install and build artifacts
status: done
type: hardening
team: frontend
supporting_teams:
  - docs
roadmap_item: "Phase 9F - Frontend artifact ignore rules"
priority: medium

---

## Goal

Prevent accidental commits of frontend-generated artifacts such as `node_modules` and `dist`.

## Context

Phase 9B through 9E introduced `src/VoidEmpires.Frontend`, but the repository ignore rules still only cover `.NET` artifacts and Visual Studio local files.

Running the documented frontend workflow would create untracked dependency and build output folders that should never be committed.

## Implementation steps

1. Inspect current root `.gitignore`.
2. Add ignore rules for frontend dependency and build output folders.
3. Keep the change minimal and repo-wide.
4. Update docs only if necessary.

## Files to read first

- .gitignore
- src/VoidEmpires.Frontend/README.md
- AGENTS.md

## Expected files to modify

- .gitignore

## Acceptance criteria

- Frontend `node_modules` is ignored.
- Frontend build output such as `dist` is ignored.
- The change is minimal and does not affect unrelated ignore rules.

## Constraints

- Keep the change small.
- Do not modify unrelated files.

## Validation

Before completing the task, run:

```powershell
git status --short
```

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits per task.
