# TASK-006

---
id: TASK-006
title: Harden worker automation hygiene
status: done
type: platform
team: platform
supporting_teams:
  - devops
roadmap_item: "Phase 0 - Repository and AI Platform setup"
priority: high
---

## Goal
Reduce accidental automation side effects by tightening local transient file handling and narrowing automated staging behavior.

## Context
The repository currently leaves `ai/worker.lock` unignored, and both local and CI automation use broad staging commands (`git add -A` and `git add .`). That can accidentally sweep unrelated local files into automated commits. The worker script also detects active runner processes using a slash-specific path pattern that can miss Windows command lines.

## Implementation steps

1. Add ignore rules for `ai/worker.lock` if they do not already exist.
2. Update `scripts/codex-runner.ps1` so worker-process detection handles Windows path separators reliably.
3. Narrow automated staging in `scripts/codex-runner.ps1` and `.github/workflows/codex-worker.yml` so the automation commits only intended repository changes.
4. Keep the changes conservative and aligned with the existing workflow.
5. Do not modify unrelated documentation beyond what is necessary for the automation change.

## Files to read first

- `ai/task-template.md`
- `scripts/codex-runner.ps1`
- `.github/workflows/codex-worker.yml`
- `.gitignore`

## Expected files to modify

- `.gitignore`
- `scripts/codex-runner.ps1`
- `.github/workflows/codex-worker.yml`

## Acceptance criteria

- `ai/worker.lock` is ignored by Git.
- Worker lock detection works with Windows-style command paths.
- Automated staging no longer relies on `git add -A` or `git add .`.
- The workflow remains functional for normal task processing.

## Validation

Before completing the task ensure:

- `git check-ignore ai/worker.lock` succeeds.
- `git diff --name-only` shows only the intended automation files.
- No `dotnet build` is required because the application solution does not exist yet.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore: harden worker automation hygiene`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
