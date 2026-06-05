# TASK-28N Ranking Cockpit Docs And Smoke Checklist

---
id: TASK-28N
title: Document Ranking cockpit docs and smoke checks
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Create validation docs that make Ranking QA repeatable and explicit.

## Current problem
Ranking has no dedicated checklist and no seeded check path in docs.

## Context from current implementation
Other cockpits have dedicated smoke docs and shared foundation checklist entries.

## Goal
Create `docs/dev/ranking-cockpit-checklist.md`, then integrate it into shared smoke docs.

## Files to inspect first
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/development-seed-profiles.md
- docs/dev/planet-module-boundaries.md
- ai/current-state.md

## Expected files to modify
- docs/dev/ranking-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/development-seed-profiles.md

## Implementation requirements
- Include seeded URL:
- /ranking?civilizationId=00000000-0000-0000-0000-000000000001
- Checks:
- page loads
- civilization context visible
- power summary visible
- category scores visible
- demo comparison visible
- future actions disabled
- handoffs to Galaxy/Market/Espionaje/Alianzas
- diagnostics collapsed
- explicit no public ranking
- no matchmaking
- no rewards
- no public profiles
- Update shared checklist with Ranking entry and route.

## UI/UX requirements
- Checklist should be practical for manual QA and optional screenshot checks.

## Backend/API requirements
- Document seed dependencies and any assumptions.

## Safety constraints
- Do not claim implemented features that are still placeholders.

## Acceptance criteria
- Ranking can be validated from documentation.
- Docs are aligned with implementation constraints.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- If Ranking routes are not yet available in navigation, checklist should record that status explicitly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
