# TASK-10Q

---
id: TASK-10Q
title: Phase 10Q - Seeded fleet estimate API scenario
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10Q"
priority: medium
---

## Goal
Add seeded integration-style API coverage proving orbital travel estimate works against the minimal-validation seed dataset without mutating state.

## Scope
Use the minimal-validation seed dataset and the existing WebApplicationFactory patterns to cover seed setup, `ui-state`, and orbital travel estimate.

## Work
- Apply the minimal-validation seed in the test host or use the equivalent service-level setup.
- Exercise `POST /api/dev/seeds/apply`, `GET /api/dev/fleets/ui-state`, and `POST /api/dev/fleets/orbital-travel/estimate`.
- Assert success, returned route or cost data, and that no transfer, status change, or resource change occurs.

## Acceptance
- Estimate succeeds for a seeded stationed group and valid destination.
- No `OrbitalTransfer` is created.
- `OrbitalGroup` status and resource balances remain unchanged.
- No frontend changes or EF migrations.
