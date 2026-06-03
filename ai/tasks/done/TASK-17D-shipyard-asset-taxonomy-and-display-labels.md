# TASK-17D-shipyard-asset-taxonomy-and-display-labels

---
id: TASK-17D-shipyard-asset-taxonomy-and-display-labels
title: Shipyard asset taxonomy and display labels
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Create a centralized Spanish-first presentation layer for orbital assets, production categories, queue statuses, and Shipyard actions.

## Purpose
Ensure Shipyard looks like a playable cockpit instead of a diagnostic panel leaking enum names, DTO field names, capability keys, or raw backend identifiers.

## Current Problem
The repo already has presentation helpers for Planet, Construction, and Research, but Shipyard still risks showing raw technical values if it reads asset data directly from the backend. Without a clear taxonomy and label helper layer, the UI will become inconsistent across Shipyard, Fleets, and docs.

## Context
- Building and research pages already separate backend data from player-facing labels.
- Fleet has accepted wording that should not be broken casually.
- Shipyard introduces a new player-facing vocabulary around orbital production, queue status, stock, and readiness.

## Files to Inspect First
- `src/VoidEmpires.Domain/Assets/`
- `src/VoidEmpires.Domain/Fleets/`
- `src/VoidEmpires.Application/Assets/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`

## Implementation Requirements
1. Inspect the real asset enums, catalog values, and queue statuses before naming anything.
2. Add or extend frontend presentation helpers for Shipyard, for example:
   - `getAssetTypeLabel(...)`
   - `getAssetCategoryLabel(...)`
   - `getAssetRoleLabel(...)`
   - `getAssetProductionStatusLabel(...)`
   - `getShipyardActionLabel(...)`
   - `formatAssetQuantity(...)`
   - `formatAssetProductionCost(...)`
   - `formatAssetProductionDuration(...)`
3. Use Spanish labels that match the domain and existing module tone.
4. If the actual domain values align, prefer labels such as:
   - `Nave exploradora`
   - `Nave de escolta`
   - `Nave de carga`
   - `Fragata ligera`
   - `Nave colonial`
   - `Sonda`
   - `Plataforma orbital`
5. If the actual domain supports categories, prefer Spanish group labels such as:
   - `Exploracion`
   - `Logistica`
   - `Escolta`
   - `Transporte`
   - `Colonial`
   - `Orbital`
   - `Prototipo`
6. Unknown values must fall back to a neutral Spanish string such as `Activo orbital pendiente de clasificar`.
7. Keep raw enum values only in diagnostics or developer notes.
8. Preserve accepted Fleet wording unless a consistency fix is genuinely needed.

## UI/UX Requirements
- Spanish-first copy in primary UI.
- Labels should be reusable across Shipyard, Fleets, and docs.
- Avoid exposing technical identifiers in cards, summaries, buttons, and queue rows.

## Backend/API Requirements
- Prefer frontend-only presentation helpers.
- If backend already exposes safe display metadata, reuse it instead of duplicating logic.

## Safety Constraints
- Do not change persisted enum values.
- Do not change fleet behavior.
- Do not silently rename technical keys used in requests or tests.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/` Shipyard or shared presentation helper files
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Possibly `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx` only if a narrow consistency update is required

## Acceptance Criteria
- Asset labels, categories, and statuses are centralized and reusable.
- No primary Shipyard UI depends on raw enum text.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` only if backend files are touched.

## Notes / Residual Risks
- Final lore naming can evolve later, but this task should remove technical leakage now.
- If multiple taxonomies exist in backend code, document which one Shipyard treats as authoritative for display.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer focused helpers over broad copy rewrites.
