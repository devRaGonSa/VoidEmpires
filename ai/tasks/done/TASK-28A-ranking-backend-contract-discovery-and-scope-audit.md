# TASK-28A Ranking Backend Contract Discovery And Scope Audit

---
id: TASK-28A
title: Audit ranking-related backend contracts and define v1 scope
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: high
---

## Purpose
Audit existing backend/domain/application/web surfaces so Ranking can be implemented as a read-only power index foundation without introducing real public competitive systems.

## Current problem
Ranking remains a placeholder module, and it is not yet clear which existing metric sources can feed a safe read-only power index.

## Context from current implementation
The codebase already has accepted read-only strategic and resource cockpits. Ranking v1 should aggregate available read-state and stay non-persistent, non-public, and non-competitive.

## Goal
Document a safe v1 scope covering read-only power computation from existing data and exclusions for unsupported gameplay.

## Files to inspect first
- src/VoidEmpires.Domain/
- src/VoidEmpires.Domain/Players/
- src/VoidEmpires.Domain/Planets/
- src/VoidEmpires.Domain/Fleets/
- src/VoidEmpires.Domain/Assets/
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- tests/VoidEmpires.Tests/
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/current-state.md

## Expected files to modify
- docs/dev/ranking-cockpit-checklist.md
- ai/current-state.md

## Implementation requirements
- Identify metric sources for:
- civilization identity
- planet ownership
- resource reserves
- production
- colony development
- research progress
- shipyard/industrial stock
- fleet readiness and transport state
- defensive readiness
- ground army readiness
- intelligence/visibility context
- market read state
- alliance read state
- Detect whether any existing names already represent ranking/power concepts.
- Define explicit v1 boundaries:
- read-only own civilization power index
- category score breakdown from available read state
- demo-only comparison rows where safe
- no public global ladder
- no rewards/matchmaking/real public profiles
- no persistence or recalculation worker for ranking.
- Document all findings and decisions in a single docs artifact.

## UI/UX requirements
- No UI changes in this task.
- Define what can be shown safely in cockpit language and comparison wording.

## Backend/API requirements
- Prefer documentation-first audit.
- If code paths are modified, include tests for read-path behavior.

## Safety constraints
- No real public ranking behavior.
- No matchmaking or persistent competition model introduced.
- No authentication behavior changes.
- No action or mutation endpoints.

## Acceptance criteria
- Ranking read-state scope is explicit and safe.
- v1 boundary is documented for future implementation tasks.
- Validation remains green.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend if frontend is touched

## Notes / residual risks
- If ranking inputs are incomplete, fallback to a minimal deterministic score model and clearly document it.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
