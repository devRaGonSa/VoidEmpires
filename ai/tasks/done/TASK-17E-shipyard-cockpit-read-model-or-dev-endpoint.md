# TASK-17E-shipyard-cockpit-read-model-or-dev-endpoint

---
id: TASK-17E-shipyard-cockpit-read-model-or-dev-endpoint
title: Shipyard cockpit read model or dev endpoint
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Provide a stable Shipyard cockpit read model by reusing existing endpoints or adding a development-only UI-state endpoint if the current backend surface is too fragmented.

## Purpose
Give the frontend one trustworthy source of Shipyard state so it does not stitch together multiple raw endpoints or duplicate business rules.

## Current Problem
The Shipyard page needs coherent state for planet context, ownership, local resources, production options, queue, stock, readiness, blocked reasons, and diagnostics. If the frontend assembles that state ad hoc from unrelated endpoints, it will be fragile and harder to keep truthful than the Planet, Construction, and Research cockpits.

## Context
- Research, Planet, and Fleet already lean on cockpit-style read models.
- Shipyard must remain development-only if a new route is introduced.
- The read path must support seeded QA and honest disabled states even when mutation support is limited.

## Files to Inspect First
- `src/VoidEmpires.Application/Assets/`
- `src/VoidEmpires.Infrastructure/Assets/`
- `src/VoidEmpires.Web/`
- `tests/VoidEmpires.Tests/`
- Existing dev endpoint patterns for Planet, Fleets, and Research
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`

## Implementation Requirements
1. Inspect existing read endpoints and determine whether they already provide enough Shipyard state.
2. Reuse existing endpoints if they already expose coherent cockpit data.
3. If no adequate read surface exists, add a development-only endpoint such as:
   - `GET /api/dev/shipyard/ui-state?civilizationId={id}&planetId={id}`
4. The read model should include, when the current backend can supply them safely:
   - civilization id;
   - planet id, name, and system context;
   - ownership or control state;
   - local resource stockpile relevant to production;
   - shipyard building or production readiness state;
   - asset production catalog and options;
   - requirements, costs, and durations;
   - current queue items;
   - planetary or orbital asset stock;
   - action availability;
   - blocked reasons;
   - diagnostics and known limitations.
5. The endpoint must not mutate state.
6. Add tests for:
   - dev-only gating;
   - invalid ids;
   - success with minimal-validation seed;
   - no mutation on read;
   - catalog availability versus blocked options when applicable.

## UI/UX Requirements
- The DTO should support Spanish presentation, but the final player copy can live in the frontend.
- The read model must be rich enough to support a dashboard, catalog, queue, stock summary, disabled actions, and collapsed diagnostics.

## Backend/API Requirements
- Follow existing dev endpoint conventions.
- Do not require production auth.
- Do not add a production route.
- Avoid migrations unless impossible.

## Safety Constraints
- Read endpoints must not enqueue production.
- Read endpoints must not complete due production.
- Read endpoints must not create orbital groups.

## Expected Files to Modify
- `src/VoidEmpires.Application/Assets/` relevant query or contract files
- `src/VoidEmpires.Infrastructure/Assets/` relevant service or read-model files
- `src/VoidEmpires.Web/` endpoint registration or route mapping
- `tests/VoidEmpires.Tests/` relevant endpoint or service tests

## Acceptance Criteria
- Shipyard has a stable read model suitable for cockpit UI.
- Any new endpoint is explicitly development-only.
- Tests cover the supported read path.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend types or clients are touched.

## Notes / Residual Risks
- If the backend catalog is incomplete, the read model must expose that honestly instead of pretending production is ready.
- Keep the route and DTO narrowly scoped to Shipyard, not a generic mega-endpoint.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- If the read model grows too broad, split follow-up tasks instead of folding more features into this one.
