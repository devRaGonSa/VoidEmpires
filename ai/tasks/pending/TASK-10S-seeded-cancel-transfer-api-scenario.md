# TASK-10S

---
id: TASK-10S
title: Phase 10S - Seeded cancel transfer API scenario
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10S"
priority: medium
---

## Goal
Add seeded API scenario coverage proving cancel transfer returns the seeded fleet state to a consistent non-active-transfer state.

## Scope
Use the minimal-validation seed dataset and the current fleet API surface to verify cancel-transfer behavior before and after the mutation.

## Work
- Apply the seed and create a transfer, or use a deterministic seeded active transfer if available.
- Call `POST /api/dev/fleets/orbital-transfers/cancel` and then `GET /api/dev/fleets/ui-state`.
- Confirm repeat cancellation is rejected and does not mutate state again.

## Acceptance
- Cancel returns the success contract.
- The transfer is cancelled or otherwise no longer active.
- The group returns to the expected available or stationed status.
- Resources are not refunded if that is the current rule.
- No frontend changes or EF migrations.
