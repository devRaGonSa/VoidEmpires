# TASK-10N

---
id: TASK-10N
title: Phase 10N - Create orbital transfer endpoint contract hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10N"
priority: medium
---

## Goal
Harden the HTTP contract for creating orbital transfers so service outcomes map to predictable API status codes and response bodies.

## Scope
Focus on `/api/dev/fleets/orbital-transfers/create`, its request or response records, and the tests that verify the API contract. Keep the route and gameplay behavior stable unless a clear bug is found.

## Work
- Add or tighten WebApplicationFactory coverage for disabled behavior, persistence-disabled behavior, invalid payloads, not found, forbidden or rejected ownership cases, conflict cases, and success.
- Ensure the success response exposes the client data currently available from the API.

## Acceptance
- Tests cover the listed status-code mappings.
- Success includes the expected transfer details.
- No frontend execution controls, combat, interception, or EF migrations.
