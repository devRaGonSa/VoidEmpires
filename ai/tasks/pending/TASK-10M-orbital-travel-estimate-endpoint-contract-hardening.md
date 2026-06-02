# TASK-10M

---
id: TASK-10M
title: Phase 10M - Orbital travel estimate endpoint contract hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10M"
priority: medium
---

## Goal
Harden the HTTP contract for the development orbital travel estimate endpoint so it consistently exposes validation and service outcomes.

## Scope
Focus on the estimate endpoint, related records, and WebApplicationFactory tests. Keep the endpoint read-only and aligned with current development conventions.

## Work
- Tighten coverage for disabled behavior, persistence-disabled behavior, malformed input, not found, conflict, and success.
- Keep the response shape consistent with the current pattern.

## Acceptance
- Tests cover the listed status-code mappings.
- Success returns an estimate payload and failures return errors.
- No mutation, frontend changes, or EF migrations.
