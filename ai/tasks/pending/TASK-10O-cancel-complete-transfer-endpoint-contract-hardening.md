# TASK-10O

---
id: TASK-10O
title: Phase 10O - Cancel and complete transfer endpoint contract hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10O"
priority: medium
---

## Goal
Harden HTTP contracts for cancelling transfers and completing due transfers.

## Scope
Focus on `/api/dev/fleets/orbital-transfers/cancel`, `/api/dev/fleets/orbital-transfers/complete-due`, and the WebApplicationFactory tests that confirm their HTTP behavior.

## Work
- Add or tighten coverage for disabled behavior, persistence-disabled behavior, invalid payloads, not found, ownership rejection, conflict, success, and idempotency.
- Keep the current transfer lifecycle rules intact, including any no-refund convention.

## Acceptance
- Cancel and complete-due map to the current API conventions.
- Complete-due remains idempotent at the HTTP level.
- No frontend changes or EF migrations.
