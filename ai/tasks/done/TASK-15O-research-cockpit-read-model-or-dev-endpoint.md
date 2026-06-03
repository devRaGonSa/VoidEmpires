# TASK-15O-research-cockpit-read-model-or-dev-endpoint

---
id: TASK-15O-research-cockpit-read-model-or-dev-endpoint
title: Research cockpit read model or dev endpoint
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Provide a safe read model for the Research cockpit using existing backend services or a new Development-only endpoint if required.

## Purpose
The frontend needs one coherent Research state, not a set of fragile raw endpoints stitched together in the UI.

## Current Problem
The Research page needs catalog entries, queue state, completion state, availability hints and diagnostics in one place. If that data is scattered, the UI becomes brittle and difficult to keep safe.

## Context
- Follow the same pattern used by the other dev cockpit surfaces.
- Keep this read path free of mutation.
- The endpoint should remain development-safe and never become a production auth surface.

## Files to Inspect First
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Infrastructure/Research/`
- `src/VoidEmpires.Web/`
- `tests/VoidEmpires.Tests/`
- Existing dev endpoint patterns for Planet, Fleets and StrategicMap

## Implementation Requirements
1. Reuse an existing read endpoint if it already provides enough state.
2. If no adequate endpoint exists, add a Development-only Research UI state endpoint.
3. Suggested route only if needed:
   - `GET /api/dev/research/ui-state?civilizationId={id}&planetId={optional}`
4. The read model should include:
   - requesting civilization id;
   - selected planet id and name if available;
   - research catalog;
   - category or group;
   - technology id or key;
   - display metadata if backend owns it;
   - current status;
   - prerequisites or requirements;
   - resource costs if applicable;
   - duration if applicable;
   - active queue items;
   - completed or researched items if available;
   - action availability hints;
   - diagnostics and limitations.
5. Keep all mutations out of this read endpoint.
6. Add tests for:
   - Development-only gating;
   - persistence unavailable if matching existing patterns;
   - invalid civilization id;
   - optional invalid planet id;
   - success with minimal-validation seed;
   - no mutation of resources or queues during read.

## UI/UX Requirements
- The DTO should support Spanish presentation, but it does not need to contain final copy if the frontend handles labels.
- The shape should be stable enough to support a cockpit, not just a debug panel.

## Backend/API Requirements
- Follow existing endpoint conventions.
- Do not add a production endpoint.
- Do not require auth.
- Do not add a migration unless impossible to avoid.

## Safety Constraints
- Read endpoint must not enqueue research.
- Read endpoint must not complete research.
- Read endpoint must not apply technology effects.
- Do not expose hidden balances or global unlock state unless the backend already does so safely.

## Expected Files to Modify
- One application contract or query handler if needed.
- One infrastructure service or adapter if needed.
- One web endpoint/controller if needed.
- One or more tests in `tests/VoidEmpires.Tests/`.

## Acceptance Criteria
- Research cockpit can fetch stable state.
- Tests cover the endpoint or service if added.
- Build and tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend types are touched.

## Notes / Residual Risks
- If backend catalog is very limited, return deterministic placeholders only when they are clearly marked as readiness metadata.
- Keep the data model easy to expand later without breaking the cockpit contract.
- If the safe path already exists, prefer wiring it rather than adding a duplicate.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer one dev-only endpoint rather than multiple narrow ones.
