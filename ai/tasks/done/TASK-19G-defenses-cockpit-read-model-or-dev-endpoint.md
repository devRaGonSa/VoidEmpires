# TASK-19G-defenses-cockpit-read-model-or-dev-endpoint

---
id: TASK-19G-defenses-cockpit-read-model-or-dev-endpoint
title: Defenses cockpit read model or dev endpoint
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - qa
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Provide a stable Defenses cockpit read model using existing services when possible, or a new Development-only endpoint when a dedicated aggregate is required.

## Purpose
Give the frontend one coherent source of truth for selected planet context, defensive infrastructure, readiness summaries, available or blocked options, queue state, and current limitations instead of forcing the UI to stitch together fragile raw endpoints.

## Current Problem
The Defenses cockpit cannot become a real cabin without coherent state. Existing backend data may be spread across buildings, queues, catalogs, and seed data. A dedicated read model may be necessary to present that state safely and clearly.

## Context
- Planet, Research, Shipyard, Fleet, and Galaxy already rely on cockpit-oriented read models or UI-state endpoints.
- Development-only endpoint patterns already exist in `DevEndpointMappings.cs`.
- Seed-driven QA depends on deterministic URLs and stable response shapes.

## Files to Inspect First
- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/`
- current dev UI-state services for Planet, Research, Shipyard, and Fleets
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`

## Implementation Requirements
1. Reuse existing read endpoints if they already provide enough information for a Defenses cockpit without heavy frontend stitching.
2. If the existing contracts are insufficient, add a Development-only aggregate read endpoint, preferably:
   - `GET /api/dev/defenses/ui-state?civilizationId={id}&planetId={id}`
3. The read model should aim to include:
   - civilization id
   - planet id, name, and local system context
   - planet control or ownership state
   - local resources relevant to defense preparations
   - current defense-related structures or buildings
   - readiness or protection summary
   - available and blocked defense options if supported
   - queue items if defense-related queue state exists
   - complete-due capability or limitation metadata
   - diagnostics and explicit limitations
4. Keep the response honest if the backend can only expose partial readiness or placeholder action availability.
5. Add tests for:
   - Development-only gating
   - invalid civilization id
   - invalid planet id
   - not controlled planet if applicable
   - successful seeded response under `cockpit-validation`
   - read path non-mutation
   - available versus blocked state distribution if supported by the data

## UI/UX Requirements
- The DTO should support a Spanish-first frontend without embedding every final label in the backend.
- The contract should make disabled or unavailable states easy for the cockpit to explain truthfully.

## Backend/API Requirements
- Follow existing Development-only route conventions.
- Do not add a production endpoint.
- Do not require auth.
- Avoid migrations unless a truly minimal persistence addition is unavoidable and justified by the audit.

## Safety Constraints
- The read endpoint must not mutate state.
- No combat behavior.
- No defensive damage or interception simulation.
- No fleet or galaxy mutation.

## Expected Files to Modify
- relevant application query or DTO files
- `src/VoidEmpires.Infrastructure/` service implementation files
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- related tests under `tests/VoidEmpires.Tests/`

## Acceptance Criteria
- Defenses cockpit can fetch a stable, development-safe aggregate state.
- Tests cover the endpoint or service path if it is added.
- The contract is honest about limitations rather than inventing unsupported behavior.
- Build and tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend types or clients are touched

## Notes / Residual Risks
- If backend support is still thin after the audit, the best outcome may be a clearly limited readiness-focused response rather than an overgrown aggregate.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split follow-up shaping work into later tasks rather than overloading this endpoint task.
