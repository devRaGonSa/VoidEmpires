# TASK-11F

---
id: TASK-11F
title: Phase 11F - Fleet command execution guardrails
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11F"
priority: medium
---

## Goal
Add frontend guardrails and backend documentation alignment for future command execution so mutation actions cannot be accidentally triggered as regular gameplay UI.

## Scope
Inspect the Fleet page, action manifest panel, readiness display, and `docs/dev/fleet-api-contracts.md`. Keep mutation actions clearly separated from read-only actions.

## Work
- Label mutation metadata as development or prototype only.
- Prevent accidental click-to-execute mutation behavior.
- Update docs to explain read-only versus mutation commands and the current development-only posture.

## Acceptance
- Guardrails keep mutation actions distinct and clearly labeled.
- No full command execution UI or backend endpoints are added.
- If frontend is touched, validate it with the frontend build.
