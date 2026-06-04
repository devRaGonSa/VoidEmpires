# TASK-22C

---
id: TASK-22C
title: Phase 22C - Espionage cockpit read model or dev endpoint
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Provide a stable read-only `EspionageUiState` source for the frontend by reusing strategic visibility services or, if needed, adding a Development-only Espionage endpoint.

## Purpose

The cockpit needs one coherent source of truth for intelligence coverage, targets, confidence, limitations, and diagnostics instead of reconstructing that state ad hoc in the frontend from multiple unrelated payloads.

## Current problem

`Galaxy` already has strategic read-state, but Espionage needs an analysis-oriented shape. Directly stitching strategic-map payloads in the browser would be fragile, duplicate normalization logic, and make later QA harder.

## Context

Existing cockpits already use typed read models or Development-only UI state endpoints. Espionage should follow the same pattern and remain explicit about Development-only gating if a new route is introduced.

## Files to read first

- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- strategic map read-model services and DTOs
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/`
- `docs/dev/strategic-map-cockpit-checklist.md`
- `ai/current-state.md`

## Component discovery

Identify whether the existing strategic map, detection, sensor, interception-readiness, and fleet UI state services already produce enough input to assemble an Espionage cockpit state without adding a new persistence model.

## Dependency analysis

If a new endpoint is needed, document and implement the wiring using the existing conventions:

- `DevEndpointMappings` -> Espionage endpoint -> query service or read-model assembler
- read-model assembler -> existing strategic visibility and fleet or transfer services
- seed profile -> deterministic data -> endpoint smoke tests

## Implementation requirements

1. Reuse the strategic map read-state if it is already sufficient for Espionage.
2. If not sufficient, add a Development-only endpoint such as:
   - `GET /api/dev/espionage/ui-state?civilizationId={id}`
3. Ensure the read model includes, where supported by current backend foundations:
   - civilization id
   - known, visible, owned, observed, and partial target summaries
   - target systems and planets
   - confidence or coverage summaries
   - passive fleet or transfer signal summaries
   - recommended intelligence focus
   - disabled future mission action availability and reasons
   - diagnostics and limitations
4. Keep the endpoint read-only and side-effect free.
5. Add or update tests for:
   - Development-only gating
   - invalid civilization id
   - civilization not found or empty state behavior
   - seeded success via `cockpit-validation`
   - non-empty intelligence target list where the seed supports it
   - no mutation of persisted state on read
6. Reuse existing DTO and development endpoint conventions instead of inventing a separate infrastructure style.

## UI/UX requirements

- The contract should support a Spanish-first cockpit without forcing literal UI copy into backend DTO fields.
- The payload should expose enough meaning for summary cards, grouped targets, confidence cues, disabled action placeholders, and collapsed diagnostics.

## Backend/API requirements

- Development-only endpoint only
- No production auth work
- No automatic migrations
- No new gameplay command surface
- Prefer read-model composition over new persistence

## Expected files to modify

- read-model service and DTO files under `src/VoidEmpires.Application/` or `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- backend tests for the new or extended read surface
- seed tests only if required for deterministic coverage

## Safety constraints

- Read endpoint must not mutate state
- No spy mission creation
- No sabotage
- No counter-espionage
- No combat
- No fleet movement changes
- No Galaxy mutation

## Acceptance criteria

- Espionage can fetch one stable cockpit read state.
- The read model is deterministic under the documented seed profile.
- Development-only gating is enforced and tested.
- Read operations do not mutate stockpiles, fleets, transfers, knowledge, or map state.
- Build and tests pass.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if contract types are shared into frontend code during the task.

If integration checks are reviewed and no configured integration suite exists, record:

`No integration tests configured.`

## Notes / residual risks

- If backend support is intentionally thin, the correct result is an honest read model that exposes limitations rather than inventing pseudo-intelligence.
- Later tasks may still need frontend normalization even if the backend endpoint is clean.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm changed files match the expected backend surface.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a narrow read-model addition rather than a broad strategic-map refactor.
- If the endpoint needs too many upstream changes, stop and create a follow-up task instead of widening this one.
