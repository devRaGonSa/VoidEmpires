# TASK-31P

---
id: TASK-31P
title: Final orbital production closure
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Close the orbital production or military preparation gameplay-v1 block and leave the repository in a clean, validated, reviewable final state.

## Context
Once the block is complete, the task lifecycle folders, pending queue, commits, push state, and closure summary must all reflect the final accepted scope: Shipyard real production if supported, Defenses either safe real enqueue or explicit read-only limitation, Fleets readiness-only, Planet summary-only, Development-only QA preparation, and continued exclusions around combat, movement, missions, and auto-complete.

## Implementation steps

1. Move `TASK-31A` through `TASK-31P` to `ai/tasks/done`.
2. Verify that `ai/tasks/pending` contains only `.gitkeep`.
3. Ensure the working tree is clean after the final commit and push.
4. Prepare the final closeout summary including:
   - commit list;
   - validation results;
   - test count;
   - exact QA preparation command;
   - whether visual QA remains user-driven.

## Files to read first

- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/tasks/pending/
- ai/tasks/done/

## Expected files to modify

- ai/tasks/done/TASK-31A-orbital-production-contract-audit.md
- ai/tasks/done/TASK-31B-orbital-production-frontend-api-contracts.md
- ai/tasks/done/TASK-31C-shipyard-selection-review-state.md
- ai/tasks/done/TASK-31D-shipyard-confirmation-panel.md
- ai/tasks/done/TASK-31E-shipyard-real-production-submit.md
- ai/tasks/done/TASK-31F-shipyard-backend-first-refresh-and-stock.md
- ai/tasks/done/TASK-31G-defense-production-scope-and-readiness.md
- ai/tasks/done/TASK-31H-defense-errors-and-blocked-affordance.md
- ai/tasks/done/TASK-31I-fleets-readiness-from-orbital-production.md
- ai/tasks/done/TASK-31J-planet-military-orbital-activity-summary.md
- ai/tasks/done/TASK-31K-orbital-production-qa-state-preparation.md
- ai/tasks/done/TASK-31L-orbital-production-qa-powershell-helper.md
- ai/tasks/done/TASK-31M-cross-cockpit-safety-and-guardrails.md
- ai/tasks/done/TASK-31N-runtime-qa-docs-and-visual-checklist.md
- ai/tasks/done/TASK-31O-current-state-and-final-validation.md
- ai/tasks/done/TASK-31P-final-orbital-production-closure.md

## Acceptance criteria

- All `31A-31P` tasks are moved to `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- The final repository state is committed and pushed.
- The final block summary covers commits, validations, test count, the exact QA preparation command, and the fact that visual QA remains user-driven.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not close the block while pending tasks remain or the worktree is dirty

## Validation

Before completing the task ensure:

- the repository-relevant final validation bundle for the block has already passed
- `git status` is clean after the final commit
- `ai/tasks/pending` contains only `.gitkeep`

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
