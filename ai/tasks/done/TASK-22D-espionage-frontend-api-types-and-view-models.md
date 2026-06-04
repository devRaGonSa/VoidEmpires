# TASK-22D

---
id: TASK-22D
title: Phase 22D - Espionage frontend API types and view models
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Add typed frontend API access and normalized view models for the Espionage cockpit.

## Purpose

The frontend needs a stable translation layer between backend DTOs and the player-facing cockpit so the page can group targets, label uncertainty correctly, and keep diagnostics separate from primary gameplay copy.

## Current problem

Rendering raw backend DTOs directly would couple the page to transport details, duplicate label logic in JSX, and make uncertainty or disabled-action rules harder to maintain.

## Context

Accepted cockpits such as `Research`, `Shipyard`, `Defenses`, and `Ground Army` already use typed clients and view-model helpers. Espionage should follow the same pattern and preserve route-context and diagnostics conventions.

## Files to read first

- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- Espionage endpoint contract or shared DTO notes from `TASK-22C`

## Component discovery

Inspect existing frontend API clients and view-model helpers for specialized cockpits. Prefer matching their file layout, fetch wrapper usage, error handling, and normalization style.

## Implementation requirements

1. Add typed frontend API contracts for Espionage UI state, for example:
   - `EspionageUiState`
   - `IntelligenceSummary`
   - `IntelligenceTarget`
   - `IntelligenceSystemTarget`
   - `IntelligencePlanetTarget`
   - `IntelligenceSignal`
   - `IntelligenceCoverage`
   - `EspionageActionAvailability`
   - `EspionageDiagnostics`
2. Add an API function such as `fetchEspionageUiState(...)`.
3. Add normalization helpers such as:
   - `mapEspionageUiStateToViewModel(...)`
   - `groupIntelTargetsBySystem(...)`
   - `selectRecommendedIntelTarget(...)`
   - `getEspionagePrimaryAction(...)`
4. Normalize player-facing properties for:
   - target labels
   - visibility labels
   - confidence levels
   - action availability
   - blocked or future reasons
5. Keep raw backend details available only inside diagnostics-oriented structures.
6. Preserve optionality where backend support is still maturing.

## UI/UX requirements

- The view model must support a cockpit summary, grouped target catalog, confidence cues, signals panel, disabled future mission panel, and contextual handoffs.
- Spanish-first presentation must be possible without pushing copy into raw transport contracts.
- Unknown or partial data must remain visibly incomplete rather than looking confirmed.

## Backend/API requirements

- No backend behavior change is expected unless the new contract reveals a mismatch.
- If a mismatch is found, resolve it in the smallest possible way and add tests in the backend task that owns the endpoint.

## Expected files to modify

- Espionage API client under `src/VoidEmpires.Frontend/src/api/`
- Espionage type definitions under `src/VoidEmpires.Frontend/src/api/` or `src/VoidEmpires.Frontend/src/types/`
- Espionage view-model or presentation helpers under `src/VoidEmpires.Frontend/src/utils/`

## Safety constraints

- No mutation calls
- No optimistic updates
- No mission execution
- No direct dependence on raw DTO names in the visible UI

## Acceptance criteria

- Typed API access exists for the Espionage cockpit.
- A normalized view model exists and is reusable.
- The page can consume the state without depending on raw transport details in JSX.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Run backend validation only if backend files are touched while resolving a contract mismatch.

## Notes / residual risks

- Some fields may need to stay optional until the backend read model reaches the exact shape desired by later presentation tasks.
- If the target grouping logic grows too large, extract it early instead of leaving a monolithic page mapper.

## Commit and push

1. Run `git status`.
2. Verify only intended frontend API and helper files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep the typed client and view model small and dedicated.
- If helper sprawl appears, create a follow-up task rather than packing unrelated presentation cleanup into this one.
