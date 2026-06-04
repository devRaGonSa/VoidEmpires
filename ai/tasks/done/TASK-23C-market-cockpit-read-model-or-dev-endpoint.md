# TASK-23C

---
id: TASK-23C
title: Phase 23C - Market cockpit read model or dev endpoint
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Provide a stable read-only `MarketUiState` source for the frontend by reusing current resource and production services or, if needed, adding a Development-only Market endpoint.

## Purpose

The cockpit needs one coherent source of truth for reserves, production, reference ratios, trade signals, future-action availability, and diagnostics instead of reconstructing that state ad hoc in the frontend from multiple unrelated payloads.

## Current problem

Planet and other cockpits already expose economy data, but Market needs an economy-oriented shape. Directly stitching multiple payloads in the browser would be fragile, duplicate normalization logic, and make later QA harder.

## Context

Existing accepted cockpits already use typed read models or Development-only UI state endpoints. Market should follow the same pattern and remain explicit about Development-only gating if a new route is introduced.

## Files to read first

- planet UI state services and endpoints
- construction, research, and shipyard read-model services
- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/`
- `docs/dev/market-cockpit-checklist.md` if created by Task 23A

## Component discovery

Identify whether existing planet, stockpile, production, affordability, and fleet logistics read services already produce enough input to assemble a Market cockpit state without adding a new persistence model.

## Dependency analysis

If a new endpoint is needed, document and implement the wiring using the existing conventions:

- `DevEndpointMappings` -> Market endpoint -> query service or read-model assembler
- read-model assembler -> current economy or fleet read services
- seed profile -> deterministic data -> endpoint smoke tests

## Implementation requirements

1. Reuse existing resource or planet read-state if it is already sufficient for Market.
2. If not sufficient, add a Development-only endpoint such as:
   - `GET /api/dev/market/ui-state?civilizationId={id}&planetId={optionalId}`
3. Ensure the read model includes, where supported by current backend foundations:
   - civilization id
   - selected planet context if provided
   - known planets with safe stockpile visibility
   - resource reserves
   - estimated production if available
   - future market capability states
   - reference price or ratio rows derived deterministically from current resource concepts
   - demand or supply hints if derivable
   - route placeholders or limitations
   - diagnostics and limitations
4. Keep the endpoint read-only and side-effect free.
5. Add or update tests for:
   - Development-only gating
   - invalid civilization id
   - optional invalid planet id
   - seeded success via `cockpit-validation`
   - non-empty resource summary
   - no mutation of persisted state on read
   - seed reapply does not duplicate Market-supporting data if the seed is touched
6. Reuse existing DTO and dev endpoint conventions instead of inventing a separate infrastructure style.

## UI/UX requirements

- The contract should support a Spanish-first cockpit without forcing literal UI copy into backend DTO fields.
- The payload should expose enough meaning for summary cards, reserve panels, ratio tables, disabled action placeholders, and collapsed diagnostics.

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
- No transactions
- No buy or sell orders
- No auctions
- No resource movement
- No fleet movement changes

## Acceptance criteria

- Market can fetch one stable cockpit read state.
- The read model is deterministic under the documented seed profile.
- Development-only gating is enforced and tested.
- Read operations do not mutate stockpiles, fleets, transfers, or other gameplay state.
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

- If backend support is intentionally thin, the correct result is an honest read model that exposes limitations rather than inventing pseudo-commerce.
- Later tasks may still need frontend normalization even if the backend endpoint is clean.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm changed files match the expected backend surface.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a narrow read-model addition rather than a broad economy refactor.
- If the endpoint needs too many upstream changes, stop and create a follow-up task instead of widening this one.
