# TASK-29L

---
id: TASK-29L-construction-runtime-qa-command-docs
title: Document runtime QA commands for construction enqueue
status: done
type: platform
team: platform
supporting_teams: [platform]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Provide operator-level instructions for manual runtime QA of frontend and backend paths.

## Context
Documentation-only task to reduce onboarding friction for gameplay mutation verification.

## Files to read first

- docs/dev/construction-cockpit-checklist.md
- scripts/dev-qa-create-construction-order.ps1

## Expected files to modify

- ai/tasks/pending/TASK-29L-construction-runtime-qa-command-docs.md
- docs/dev/construction-cockpit-checklist.md

## Implementation steps

1. Add clear startup and validation sequence:
   - start backend
   - apply cockpit-validation twice
   - start frontend
   - open /construction
2. Record expected UI behavior and queue/resource refresh outcomes.
3. Add reset/reused DB warning for repeated runs.
4. Mention backend-only fallback script path.

## Acceptance criteria

- Manual QA is executable directly from docs.

## Validation notes

- `dotnet build --no-restore` passed with transient `MSB3026` copy-retry warnings while `testhost` still held test output DLLs.
- `dotnet test --no-build` passed.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
