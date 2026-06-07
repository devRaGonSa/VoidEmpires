# TASK-30L

---
id: TASK-30L-research-runtime-qa-command-docs
title: Add runtime QA commands for research enqueue
status: pending
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Document executable commands for manual QA of persisted Research enqueue from both UI and backend paths.

## Context
Manual operators need a deterministic checklist with known resource constraints and DB reuse warning.

## Implementation steps

1. Add runtime QA section to relevant docs with commands:
   - start backend
   - apply cockpit-validation script twice
   - start frontend
   - open `/research`
2. Add expected behavior sequence:
   - select available research
   - confirm
   - success
   - resources/queue/progress refreshed
3. Include reused DB warning about repeated runs and existing open order.
4. Include backend-only fallback script reference.

## Files to read first

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `scripts/dev-qa-create-research-order.ps1`
- `scripts/check-dev-qa-scripts.ps1`

## Expected files to modify

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/research-cockpit-checklist.md`

## Acceptance criteria

- A complete, ordered runtime QA command sequence is documented.
- Warnings and expected outcomes are explicit.
- Backend-only fallback path remains documented.

## Constraints

- Do not alter script behavior unless required.
- Keep runtime docs user-driven.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `docs: add runtime QA docs for research enqueue`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
