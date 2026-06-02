# TASK-10W

---
id: TASK-10W
title: Phase 10W - Split and merge endpoint contract hardening
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10W"
priority: medium
---

## Goal
Harden the HTTP/API contracts for split and merge orbital group dev endpoints.

## Scope
Inspect `/api/dev/fleets/orbital-groups/split` and `/api/dev/fleets/orbital-groups/merge`, and align the tests with the response conventions used in Block 10M-10P.

## Work
- Add or tighten WebApplicationFactory coverage for disabled behavior, persistence-disabled behavior, payload validation, not found, conflict, wrong-civilization rejection, and success.
- Keep response shapes consistent with the fleet command endpoints.
- Update `docs/dev/fleet-api-contracts.md` only if the contract is clarified.

## Acceptance
- Split and merge map to the current API conventions.
- Success returns payload data and failures return errors.
- No frontend changes, combat, interception, or EF migrations.
