# TASK-41AZ

---
id: TASK-41AZ
title: Final product-facing closure
status: done
type: workflow
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Close Block 41.

## Context
This is the final closure task after all Block 41 work is complete, validated, committed, and pushed.

Queue note: this task must run only after the remaining pending Block 41 product-page tasks are complete. It was requeued after those tasks because closure cannot be accepted while other `TASK-41*` files remain pending.

## Implementation steps

1. Verify all `TASK-41*` tasks are complete and move them to `ai/tasks/done`.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run the full final validation command set.
4. Commit and push the final state.
5. Output the commit list, validation results, test count, pages updated, forbidden UI terms removed, operator mode status, and an explicit statement that no manual/browser QA was performed.

## Files to read first

- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- ai/current-state.md
- docs/dev/no-visible-development-report.md

## Expected files to modify

- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- ai/current-state.md
- docs/dev/

## Acceptance criteria

- Normal UI no longer exposes Development, QA, test, or prototype language.
- Product-facing app shell is coherent.
- Onboarding, Planet, Construction, Research, and Shipyard are product-facing.
- Readiness pages are product-facing without fake mutations.
- Operator/dev tools and diagnostics are hidden or secondary.
- SQL Server support remains intact.
- No gameplay semantics changed.
- No final images generated.
- No manual QA is claimed.
- Build, test, frontend, and guard scripts pass.
- Pending contains only `.gitkeep`.

## Constraints

- Do not close incomplete tasks.
- Do not apply migrations automatically.
- Do not commit secrets.
- Do not add combat, movement, market transactions, alliance mutations, active espionage, payment systems, or production auth.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- `git status`
- `dir ai/tasks/pending`

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
