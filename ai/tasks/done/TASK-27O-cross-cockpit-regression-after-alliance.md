# TASK-27O Cross Cockpit Regression After Alliance

---
id: TASK-27O
title: Run regression pass across accepted cockpits after Alliance integration
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Validate that Alliance implementation did not regress existing accepted routes and standard checks.

## Current problem
Alliance touches route navigation, docs, and seed workflows, creating potential regressions in Galaxy/Planet/Research and other cockpits.

## Context from current implementation
All previous accepted cockpits rely on route stability and lazy-loading. This pass confirms unchanged behavior and checks documentation continuity.

## Goal
Run required checks and ensure acceptance route list is updated with Alliance.

## Files to inspect first
- ai/current-state.md
- docs/dev/frontend-foundation-smoke-checklist.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/current-state.md

## Implementation requirements
- Update route regression list to include:
- /galaxy
- /planet
- /construction
- /research
- /shipyard
- /fleets
- /defenses
- /ground-army
- /espionage
- /market
- /alliance
- Re-run standard checks:
- dotnet build
- dotnet test
- npm build
- check-dev-qa-scripts
- route-lazy-import guard
- No visual QA required for technical closure.

## UI/UX requirements
- None besides preserving current behavior.

## Backend/API requirements
- None unless checks reveal route data dependency regressions.

## Safety constraints
- No gameplay additions in this pass.
- No changes to acceptance behavior required unless blocked.

## Acceptance criteria
- Existing route checks remain green.
- Alliance route is included in regression scope.
- Validation outputs are documented.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1

## Notes / residual risks
- If regressions surface in non-alliance modules, split follow-ups by route area and keep this block focused.
