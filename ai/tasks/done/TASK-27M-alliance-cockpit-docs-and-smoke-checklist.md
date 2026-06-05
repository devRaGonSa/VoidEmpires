# TASK-27M Alliance Cockpit Docs And Smoke Checklist

---
id: TASK-27M
title: Document Alliance read-only cockpit QA and exclusions
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: medium
---

## Purpose
Create and update docs so Alliance can be validated consistently and safely by future reviewers.

## Current problem
Alliance does not yet have a dedicated smoke checklist documenting read-only guarantees and expected seeded state.

## Context from current implementation
The project already uses route-level smoke checklists and development seed docs. We need parallel coverage for Alliance.

## Goal
Add `docs/dev/alliance-cockpit-checklist.md`, update foundation checklist, and include seed references.

## Files to inspect first
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/development-seed-profiles.md
- docs/dev/planet-module-boundaries.md
- ai/current-state.md

## Expected files to modify
- docs/dev/alliance-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/development-seed-profiles.md
- docs/dev/planet-module-boundaries.md

## Implementation requirements
- Create Alliance checklist with at least:
- /alliance?civilizationId=00000000-0000-0000-0000-000000000001
- civilization context visible
- diplomatic summary visible
- contacts and readiness visible
- future actions disabled
- handoff links visible
- diagnostics collapsed
- explicit no alliance/pact/invitation/messaging/roles behavior
- Update shared smoke checklist with Alliance-specific smoke entry.
- Update seed profile docs if seed changes are introduced.

## UI/UX requirements
- Documentation should be practical for manual route smoke and future screenshot tests.

## Backend/API requirements
- Document seed and API assumptions accurately.

## Safety constraints
- No doc claims of gameplay systems not implemented.
- Keep exclusions explicit.

## Acceptance criteria
- Alliance can be tested from docs alone.
- No claim of enabled alliance gameplay in checklist language.
- Docs reflect current route and seed URL.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- If ranking remains placeholder, checklist should mark it as future reference only.
