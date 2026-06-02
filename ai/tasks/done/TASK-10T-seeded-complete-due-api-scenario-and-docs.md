# TASK-10T

---
id: TASK-10T
title: Phase 10T - Seeded complete-due API scenario and docs
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10T"
priority: medium
---

## Goal
Add seeded API scenario coverage proving complete-due transfer processing behaves correctly and document the verified seeded API scenario flow.

## Scope
Use the minimal-validation seed dataset, the current fleet API surface, and a focused docs update only if the verified flow needs to be recorded.

## Work
- Apply the seed and create or use a due transfer.
- Call `POST /api/dev/fleets/orbital-transfers/complete-due` and then `GET /api/dev/fleets/ui-state`.
- Update `docs/dev/fleet-api-contracts.md` or a focused dev doc only if needed.

## Acceptance
- Complete-due returns the success contract and is idempotent.
- Due transfers complete, future transfers do not complete early, and completed groups move to the destination with the expected status.
- Any docs update stays concise and uses placeholder connection string text only.
- No frontend changes or EF migrations.
