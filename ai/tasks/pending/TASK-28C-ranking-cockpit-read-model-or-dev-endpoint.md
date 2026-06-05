# TASK-28C Ranking Cockpit Read Model Or Dev Endpoint

---
id: TASK-28C
title: Add a stable Ranking UI read model or development endpoint
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: high
---

## Purpose
Ensure Ranking has a stable, deterministic read payload without introducing any persistent public ladder behavior.

## Current problem
Composing Ranking directly from multiple unrelated payloads would be fragile and hard to QA.

## Context from current implementation
Other cockpits use dedicated read models for stable dev UI state; Ranking should follow this pattern.

## Goal
Reuse existing read surfaces when possible, otherwise add a development-only read endpoint.

## Files to inspect first
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs

## Expected files to modify
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/

## Implementation requirements
- Reuse existing read model or services if sufficient.
- Add development-only endpoint if necessary, for example:
- GET /api/dev/ranking/ui-state?civilizationId={id}
- Include in response:
- civilization id
- civilization display identity
- total power index
- category scores
- demo comparison rows
- publication state (read-only/not public)
- future placeholders for leaderboard/season/rewards
- disabled actions list
- diagnostics and limitations
- Ensure endpoint is deterministic and read-only.
- Add tests for:
- development gating
- invalid civilization id
- successful payload
- non-empty summary
- zero mutation

## UI/UX requirements
- Response shape supports Spanish summaries and cards.

## Backend/API requirements
- Keep endpoint in development surface.
- No production auth changes.
- No migrations.

## Safety constraints
- Read-only only.
- No global ladder persistence.
- No combat, rewards, matchmaking, or player-profile competition exposure.

## Acceptance criteria
- Ranking page can consume one consistent read model.
- Covered by tests for invalid and valid inputs.
- Build/test passes.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend if frontend types are updated

## Notes / residual risks
- If a full endpoint is added too heavy for v1, prefer a minimal safe adapter that is explicit in scope.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
