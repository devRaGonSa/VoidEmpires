# TASK-23B

---
id: TASK-23B
title: Phase 23B - Market taxonomy and display labels
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Create centralized Spanish player-facing labels and formatting helpers for Market resources, economy signals, price confidence, trade states, and future market actions.

## Purpose

Market should speak in gameplay language instead of surfacing raw enums, DTO names, capability keys, or technical affordability labels in the primary UI.

## Current problem

The current repository has shared vocabulary and status helper patterns, but there is not yet a Market-specific taxonomy that can translate backend values into coherent Spanish-first economy language without implying that active trade exists.

## Context

Cross-cockpit polish introduced shared vocabulary, route helpers, and secondary diagnostics patterns. Market should follow that same standard and keep technical terms out of the main experience.

## Files to read first

- `docs/dev/cockpit-copy-guidelines.md`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`

## Component discovery

Inspect existing label mappers, status helpers, amount formatting utilities, and cockpit copy conventions first. Prefer extending shared presentation helpers rather than hardcoding strings in the Market page.

## Dependency analysis

Map where display labels are currently applied in accepted cockpits:

- API client or DTO -> view-model mapper -> presentation helpers -> page sections
- shared status helpers -> badge or summary copy -> cockpit cards

The Market taxonomy should fit that same flow.

## Implementation requirements

1. Add or extend frontend presentation helpers for concepts such as:
   - `getMarketResourceLabel(...)`
   - `getMarketSignalLabel(...)`
   - `getPriceConfidenceLabel(...)`
   - `getTradeStateLabel(...)`
   - `getMarketActionLabel(...)`
   - `formatMarketResourceAmount(...)`
   - `formatMarketRatio(...)`
2. Centralize Spanish-first labels for the initial Market vocabulary, including examples such as:
   - `Creditos`
   - `Metal`
   - `Cristal`
   - `Gas`
   - `Deuterio`
   - `Energia`
   - `Reserva local`
   - `Produccion estimada`
   - `Presion de demanda`
   - `Excedente visible`
   - `Referencia de precio`
   - `Ruta comercial futura`
   - `Operacion no disponible`
3. Provide non-technical fallback labels such as:
   - `Recurso pendiente de clasificar`
   - `Senal economica pendiente de clasificar`
4. Keep raw values available only in diagnostics or developer-facing metadata.
5. Do not rename persisted enum values or backend DTO properties.
6. Reuse existing shared helper conventions wherever possible.

## UI/UX requirements

- Spanish-first
- Gameplay language instead of endpoint language
- Primary labels must not imply that live market trading already exists
- Fallback labels must remain readable and non-technical

## Backend/API requirements

- Prefer frontend-only helpers unless backend display metadata already exists and is clearly the shared convention.

## Expected files to modify

- shared Market presentation helpers under `src/VoidEmpires.Frontend/src/utils/`
- Market-facing page or view-model files only if needed to wire the helpers
- docs only if copy guidance needs a small Market note

## Safety constraints

- No gameplay rule changes
- No endpoint behavior changes unless absolutely required for display metadata
- No transaction execution

## Acceptance criteria

- Market labels and categories are centralized.
- Known seeded economy values can render with player-facing names.
- Unknown values no longer fall back to raw enum or camelCase text in the primary UI.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation

Run from repository root:

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Run backend validation only if backend files are touched:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- Final economy lore or naming can still evolve later; this task should establish a reusable baseline, not freeze long-term fiction.
- If a value appears in both Planet and Market, prefer consistency with already accepted cockpit vocabulary.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm changed files are limited to intended frontend helper or doc scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer helper extraction over broad copy churn.
- If the task starts spilling into many pages, split the remaining copy work into a follow-up task instead.
