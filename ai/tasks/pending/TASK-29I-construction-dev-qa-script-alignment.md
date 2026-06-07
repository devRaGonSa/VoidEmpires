# TASK-29I

---
id: TASK-29I-construction-dev-qa-script-alignment
title: Align backend QA script and frontend flow docs
status: pending
type: platform
team: platform
supporting_teams: [platform]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Document two sanctioned gameplay paths (backend script and frontend UI) as both real Development DB operations.

## Context
No behavior changes, doc alignment only.

## Files to read first

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/construction-cockpit-checklist.md
- scripts/dev-qa-create-construction-order.ps1

## Expected files to modify

- ai/tasks/pending/TASK-29I-construction-dev-qa-script-alignment.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/construction-cockpit-checklist.md

## Implementation steps

1. Update both docs to mention backend-only and frontend-confirmation UI paths.
2. Clarify both paths persist data in Development database.
3. Keep script logic unchanged unless strictly needed.

## Acceptance criteria

- Clear two-path QA documentation.
- No accidental script behavior change.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
