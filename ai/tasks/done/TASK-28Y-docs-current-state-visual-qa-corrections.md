# TASK-28Y

---
id: TASK-28Y
title: Update Docs After Visual QA Corrections
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Goal
Document applied visual QA corrections and pending user re-validation status across ranking/alliance/market.

## Context
After copy/fallback edits, repository docs should reflect true acceptance state and avoid overstating final UX status before user screenshot review.

## Implementation steps

1. Update cockpit-specific checklists and smoke checklist with current findings.
2. Update `ai/current-state.md` with corrected visual status and remaining user-driven confirmation.
3. Keep claims accurate and aligned to latest validations.

## Files to read first

- docs/dev/ranking-cockpit-checklist.md
- docs/dev/alliance-cockpit-checklist.md
- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/current-state.md

## Expected files to modify

- docs/dev/ranking-cockpit-checklist.md
- docs/dev/alliance-cockpit-checklist.md
- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/current-state.md

## Acceptance criteria

- Documentation mirrors actual validation state.
- No false-positive acceptance language without user confirmation.
- `dotnet build --no-restore`, `dotnet test --no-build`, and frontend build succeed.
