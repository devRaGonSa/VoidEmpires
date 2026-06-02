# TASK-10P

---
id: TASK-10P
title: Phase 10P - Fleet command API error contract consistency
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10P"
priority: medium
---

## Goal
Make fleet command dev endpoints return consistent, client-friendly error contracts without changing gameplay rules.

## Scope
Compare the existing response records and error handling for the fleet command dev endpoints, then align the contracts where practical without redesigning the API layer.

## Work
- Inspect response records and error handling across the fleet command dev endpoints.
- Identify inconsistencies in success payloads, failure payloads, and status-code mappings.
- Update affected tests and docs if the explicit contract changes.

## Acceptance
- Failure responses expose a consistent errors collection where practical.
- Success responses expose a clear payload.
- 400 for malformed input, 503 for missing persistence, 404 for not found, and conflict for business rejections where applicable.
- No frontend changes or EF migrations.
