# TASK-29O

---
id: TASK-29O-construction-visual-qa-checklist
title: Add visual QA checklist for construction enqueue
status: pending
type: platform
team: platform
supporting_teams: [platform]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Prepare visual/manual verification checklist for the new construction workflow.

## Context
No visual QA execution is done here; the checklist only.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- ai/tasks/pending/TASK-29O-construction-visual-qa-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation steps

1. Add visual checks for:
   - action selection
   - confirmation
   - loading
   - success
   - blocked cards and reasons
   - resource/queue refresh
   - no auto-complete
   - no raw payloads in primary UI
2. Keep checklist items marked as pending/manual.

## Acceptance criteria

- Checklist is complete and ready for human execution.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
