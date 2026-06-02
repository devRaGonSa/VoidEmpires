# TASK-10Z

---
id: TASK-10Z
title: Phase 10Z - Fleet quantity conservation invariants
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10Z"
priority: medium
---

## Goal
Add cross-command invariant coverage proving fleet or orbital asset quantities are conserved across split, merge, create transfer, cancel, complete-due, and estimate flows where conservation is expected.

## Scope
Inspect fleet or orbital models, services, endpoint tests, and seeded scenario tests. Focus on quantity-bearing concepts and reusable helpers.

## Work
- Cover estimate, create, cancel, complete-due, split, merge, and rejected-command conservation cases.
- Prefer service-level tests when easier; use seeded API or WebApplicationFactory scenarios where existing helpers make it practical.

## Acceptance
- Quantity is conserved where expected and unchanged for rejected flows.
- Tests stay reusable and avoid brittle status or visibility assertions.
- No frontend changes or EF migrations.
