# TASK-11A

---
id: TASK-11A
title: Phase 11A - Fleet ownership and authorization invariant audit
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11A"
priority: medium
---

## Goal
Ensure every fleet command consistently rejects cross-civilization access and does not leak or mutate another civilization's groups, transfers, or resources.

## Scope
Audit the fleet command family across estimate, create, cancel, complete-due, split, and merge flows. Keep HTTP status mapping aligned with the existing convention.

## Work
- Add or tighten cross-civilization rejection tests for each command path.
- Confirm rejected calls do not mutate groups, transfers, or resource balances.
- Document the complete-due behavior if it is global by design.

## Acceptance
- Wrong-civilization access is rejected consistently.
- Rejected calls do not mutate state or leak data.
- No production auth and no EF migrations.
