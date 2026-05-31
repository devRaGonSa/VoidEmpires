# TASK-6X

---
id: TASK-6X
title: Refresh visual sandbox validation baseline
status: pending
type: documentation
team: backend
supporting_teams:

* docs

roadmap_item: "Phase 6X - Visual sandbox docs validation baseline refresh"
priority: low
---

## Goal

Update the visual state sandbox development guide so its validation baseline matches the current repository state.

## Context

The review loop after Phase 6W found that `docs/dev/visual-state-sandbox.md` still documents the older Phase 6H/6I test baseline of `289 passing tests`, while the current validated baseline in `ai/current-state.md` is Phase 6W with `364` passing tests.

This is a documentation-only correction.

## Implementation steps

1. Inspect the visual sandbox guide validation section.
2. Inspect the current repository state baseline.
3. Update the visual sandbox guide to reference the current baseline.
4. Update `ai/current-state.md` to document Phase 6X if needed.

## Files to read first

* docs/dev/visual-state-sandbox.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

* docs/dev/visual-state-sandbox.md
* ai/current-state.md

## Acceptance criteria

* The visual sandbox guide no longer reports the stale Phase 6H/6I baseline.
* The documented validation baseline matches the current repository state.
* `ai/current-state.md` documents Phase 6X.
* No code or behavior changes are introduced.

## Constraints

* Documentation only.
* Keep the change minimal.
* Do not modify endpoint behavior, tests, UI, migrations, or gameplay logic.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Expected result:

* clean build
* 0 errors
* no new warnings
* all tests passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected documentation/state files changed.
4. Commit with a clear message, for example:
   `docs(dev): refresh visual sandbox validation baseline`
5. Push the branch to the remote.

## Change Budget

* Prefer modifying fewer than 5 files.
* Prefer changes under 200 lines of code.
