# TASK-44I-docs-and-current-state-sidebar-correction

---
id: TASK-44I
title: Docs and current state sidebar correction
status: pending
type: docs
team: platform
supporting_teams: [frontend]
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: medium
---

## Goal
Update docs/current state with corrected shell decision.

## Requirements

- Record that Block 43 accidentally removed/failed to show sidebar and Block 44 restores it.
- Clarify product decision:
  - public auth pages standalone;
  - authenticated game pages always use left sidebar on desktop;
  - duplicated in-page navigation remains removed.
- No manual/browser QA claim.

## Files to read first

- ai/current-state.md
- docs/dev/product-readiness-report.md
- docs/dev/deferred-visual-qa-master-checklist.md

## Expected files to modify

- ai/current-state.md
- docs/dev/product-readiness-report.md
- docs/dev/deferred-visual-qa-master-checklist.md

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
