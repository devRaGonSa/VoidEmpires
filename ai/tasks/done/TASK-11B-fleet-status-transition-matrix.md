# TASK-11B

---
id: TASK-11B
title: Phase 11B - Fleet status transition matrix and tests
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11B"
priority: medium
---

## Goal
Document and test the current fleet or orbital group status transition matrix so future commands do not introduce inconsistent status changes.

## Scope
Inspect the status enums or domain models, the fleet command services, and `docs/dev/fleet-api-contracts.md`. Keep the current naming and avoid inventing new statuses.

## Work
- Add a compact transition matrix in docs or comments where appropriate.
- Add tests for gaps in the expected transitions across estimate, create, cancel, complete-due, split, and merge.
- Ensure unavailable, reserved, or in-transfer groups are rejected where appropriate.

## Acceptance
- The current transition matrix is documented and test-covered.
- Commands do not introduce inconsistent status changes.
- No frontend changes or EF migrations.
