# TASK-10U

---
id: TASK-10U
title: Phase 10U - Split orbital group service lifecycle hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10U"
priority: medium
---

## Goal
Harden the orbital group split service lifecycle so a stationed group can be split safely and all rejection paths are non-mutating.

## Scope
Inspect the split service, domain model, request or result contracts, and tests. Preserve the current domain naming and status conventions.

## Work
- Cover success, quantity updates, new-group properties, and all listed rejection cases.
- Confirm repeated invalid calls do not mutate the source group or create extra groups.

## Acceptance
- Successful split creates a compatible new group and reduces the source quantity.
- Invalid calls are rejected without mutation.
- No frontend changes, movement, combat, interception, or EF migrations.
