# TASK-23D

---
id: TASK-23D
title: Phase 23D - Market frontend API types and view models
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Add typed frontend API access and normalized view-model helpers for the Market cockpit.

## Purpose

Market should not render raw backend DTOs directly. It needs typed request and response handling plus normalized view-model helpers for reserves, production, reference ratios, signals, disabled future actions, and diagnostics.

## Current problem

Other accepted cockpits already use typed API clients and view-model normalization. Market currently has no dedicated typed client layer, which would make the page brittle and harder to evolve safely.

## Context

Espionage, Defenses, Ground Army, Shipyard, and Research already follow the repository's typed frontend pattern. Market should fit the same approach and reuse shared route and presentation conventions.

## Files to read first

- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Component discovery

Inspect current cockpit API clients, response type definitions, data normalization helpers, and recommendation or summary mappers. Prefer extending those conventions instead of inventing a Market-only data layer pattern.

## Dependency analysis

Map the expected frontend flow:

- typed fetch helper -> `fetchMarketUiState(...)`
- raw DTO -> Market mapper -> page-ready view model
- shared label helpers -> presentational sections and cards

The Market page should consume normalized state rather than branching on raw backend field names.

## Implementation requirements

1. Add frontend types for Market UI state, such as:
   - `MarketUiState`
   - `MarketEconomySummary`
   - `MarketResourceReserve`
   - `MarketProductionFlow`
   - `MarketReferencePrice`
   - `MarketSignal`
   - `MarketFutureAction`
   - `MarketRoutePlaceholder`
   - `MarketDiagnostics`
2. Add a typed API function such as:
   - `fetchMarketUiState(...)`
3. Add view-model helpers such as:
   - `mapMarketUiStateToViewModel(...)`
   - `groupMarketSignals(...)`
   - `selectRecommendedMarketFocus(...)`
   - `getMarketPrimaryAction(...)`
4. Normalize at least:
   - resource labels
   - stockpile scope
   - production or flow labels
   - reference price or ratio labels
   - action availability
   - disabled or future reasons
5. Keep raw details available only in diagnostics or debug sections.
6. Reuse existing API error and route conventions where possible.

## UI/UX requirements

- The view model must support:
  - dashboard summary
  - reserve and production panels
  - reference price tables or cards
  - disabled future market actions
  - handoffs to Planet, Construction, Shipyard, Fleets, and Galaxy
- Spanish-first
- Diagnostics should remain secondary

## Backend/API requirements

- No backend change is expected unless the endpoint DTO shape requires a narrow fix.

## Expected files to modify

- Market API client files under `src/VoidEmpires.Frontend/src/api/`
- Market view-model or helper files under `src/VoidEmpires.Frontend/src/utils/` or `src/VoidEmpires.Frontend/src/pages/`
- related type definitions used by the Market page

## Safety constraints

- Do not call mutation endpoints
- No optimistic updates
- No transaction flows
- No silent fallback to invented economy data

## Acceptance criteria

- Typed client and view model exist for Market.
- Frontend compiles with no ad hoc raw DTO rendering requirement on the page.
- The Market page can consume typed state through the same pattern used by accepted cockpits.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Run backend validation only if backend files are touched.

## Notes / residual risks

- Some fields may remain optional until backend support matures; the mapper should handle that honestly instead of manufacturing false precision.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm only intended frontend typing and helper files changed.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer extracting shared helpers rather than expanding the page component with inline mapping logic.
- If frontend typing spreads too broadly, split remaining normalization into a follow-up task.
