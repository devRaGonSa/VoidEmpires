# TASK-19F-defenses-taxonomy-and-display-labels

---
id: TASK-19F-defenses-taxonomy-and-display-labels
title: Defenses taxonomy and display labels
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: medium
---

## Goal
Create or consolidate Spanish player-facing labels for defense structures, readiness categories, statuses, and defense actions so the Defenses cockpit does not expose raw enums, backend catalog keys, or DTO terminology.

## Purpose
Give the Defenses cockpit the same presentation quality baseline already established in Planet, Construction, Research, and Shipyard, with centralized helpers instead of inline label mapping.

## Current Problem
Defense data may exist only as backend categories or technical names today. If later UI tasks render those values directly, the cockpit will leak raw implementation details and feel unfinished.

## Context
- The frontend already uses presentation helpers such as `planetPresentation`, `researchPresentation`, and `shipyardPresentation`.
- Construction and Planet module work showed that Spanish-first labels and fallback wording are essential for cockpit readability.
- Defenses needs its own taxonomy layer without renaming persisted enum values or backend contracts.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Domain/Buildings/`
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`

## Implementation Requirements
1. Inspect the actual defense-related catalog and enum values discovered in `TASK-19E`.
2. Add or extend frontend presentation helpers for defense-focused labels, for example:
   - `getDefenseStructureLabel(...)`
   - `getDefenseCategoryLabel(...)`
   - `getDefenseReadinessLabel(...)`
   - `getDefenseStatusLabel(...)`
   - `getDefenseActionLabel(...)`
   - `formatDefenseCost(...)`
   - `formatDefenseDuration(...)`
3. Use Spanish player-facing labels for known defense concepts.
4. Suggested labels may include these if they match real data:
   - `Malla defensiva`
   - `Generador de escudo`
   - `Bateria orbital`
   - `Plataforma antiaerea`
   - `Centro de defensa`
   - `Fortificacion terrestre`
   - `Radar defensivo`
5. Suggested category labels may include:
   - `Proteccion planetaria`
   - `Escudos`
   - `Defensa orbital`
   - `Defensa terrestre`
   - `Sensores`
   - `Infraestructura defensiva`
6. Unknown values must fall back to player-safe wording such as `Defensa pendiente de clasificar`, not raw enum names.
7. Keep raw technical values available only through diagnostics or developer-only detail sections.

## UI/UX Requirements
- Primary cockpit copy must be Spanish-first.
- Labels must stay consistent between Defenses, Construction handoff messaging, and any Planet summary cards that reference defensive state.
- Copy must not imply battle resolution or active combat behavior.

## Backend/API Requirements
- Prefer frontend-only mapping unless the backend already exposes safe display metadata that should be reused.
- Do not rename persisted enum values or alter backend storage contracts just to improve labels.

## Safety Constraints
- No gameplay rule changes.
- No combat stat invention.
- No mutation endpoints.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/` defense presentation helper files
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx` only if wiring the helper becomes necessary during the task

## Acceptance Criteria
- Defense labels and categories are centralized.
- Known defense items render with Spanish player-facing names.
- Unknown values degrade gracefully without surfacing raw technical identifiers.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore`
- `dotnet test --no-build` if backend code changes are introduced unexpectedly

## Notes / Residual Risks
- Final lore naming can evolve later, but this task should remove obviously technical naming from the cockpit path.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task scoped to labeling and presentation helpers.
