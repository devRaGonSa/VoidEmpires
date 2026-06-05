# TASK-28P Final Ranking Cockpit Closure

---
id: TASK-28P
title: Close Ranking cockpit block and finalize workflow
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: high
---

## Purpose
Close Block 28A-28P with final validation, task movement, and documented non-regression outcome.

## Current problem
Implementation tasks need explicit closure checks to preserve queue discipline and avoid ambiguity.

## Context from current implementation
Ranking should complete the strategic read-only cockpit suite and still respect the no-public-ladder constraints.

## Goal
Finalize task queue, validation, and state consistency for the Ranking block.

## Files to inspect first
- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/ranking-cockpit-checklist.md

## Expected files to modify
- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Move TASK-28A through TASK-28P to `ai/tasks/done`.
- Keep `ai/tasks/pending` with only `.gitkeep`.
- Run final checks:
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
- Record final output summary:
- commit list
- visual QA status
- test count
- bundle/lazy status
- constraint compliance.
- Do not create broad follow-up tasks; at most 3 specific if blocking.

## UI/UX requirements
- No additional feature work.
- No visual QA automation required for closure.

## Backend/API requirements
- No new scope without explicit blockers.

## Safety constraints
- No public ranking implementation claimed unless verified.
- No ladder or matchmaking introduced.

## Acceptance criteria
- /ranking exists as read-only power index cockpit and is lazy-loaded.
- Ranking includes summary, categories, comparison, future placeholders, handoffs, diagnostics.
- Existing accepted cockpits remain routable.
- QA and lazy guard pass.
- `ai/tasks/pending` effectively empty.
- Working tree clean after final commit.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1

## Notes / residual risks
- If blockers remain, create no more than three concrete follow-up tasks.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
