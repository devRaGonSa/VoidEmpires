# TASK-10V

---
id: TASK-10V
title: Phase 10V - Merge orbital groups service lifecycle hardening
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10V"
priority: medium
---

## Goal
Harden the orbital group merge service lifecycle so compatible stationed groups merge safely and all rejection paths are non-mutating.

## Scope
Inspect the merge service, domain model, request or result contracts, and tests. Preserve the current domain naming and status conventions.

## Work
- Cover success, quantity combination, target/source lifecycle, and all listed rejection cases.
- Confirm repeated invalid calls do not mutate quantities or remove extra groups.

## Acceptance
- Successful merge combines quantities into the target group.
- Invalid calls are rejected without mutation.
- No frontend changes, movement, combat, interception, or EF migrations.
