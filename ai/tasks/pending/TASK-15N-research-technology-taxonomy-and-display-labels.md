# TASK-15N-research-technology-taxonomy-and-display-labels

---
id: TASK-15N-research-technology-taxonomy-and-display-labels
title: Research technology taxonomy and display labels
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Create or consolidate a player-facing taxonomy for research technologies so the Research cabin can render meaningful Spanish names, categories and statuses.

## Purpose
Research cannot feel like a cockpit if it exposes raw enum values, ids or backend codes as the primary UI. This task establishes the presentation layer that the later Research page will rely on.

## Current Problem
Other module cockpits already taught us that raw technical labels degrade usability. Research needs the same readable treatment from the beginning instead of a second inconsistent labeling model.

## Context
- Use existing backend metadata if it is already good enough.
- If backend values are raw codes, normalize them in frontend view models.
- Keep persisted enum values untouched.
- Keep raw values only for diagnostics and developer detail panes.

## Files to Inspect First
- `src/VoidEmpires.Domain/Research/`
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`, if present

## Implementation Requirements
1. Inspect the actual Research domain or catalog values before choosing labels.
2. Add centralized presentation helpers for research, such as:
   - `getResearchTechnologyLabel(...)`
   - `getResearchCategoryLabel(...)`
   - `getResearchStatusLabel(...)`
   - `getResearchRequirementLabel(...)`
   - `formatResearchDuration(...)`
   - `formatResearchCost(...)`
3. Use Spanish labels aligned with the game concept.
4. Suggested categories, adapted to actual data:
   - Economia
   - Energia
   - Logistica
   - Exploracion
   - Militar terrestre
   - Militar espacial
   - Defensa
   - Colonizacion
   - Administracion
5. Suggested status labels:
   - Disponible
   - En investigacion
   - Completada
   - Bloqueada
   - Recursos insuficientes
   - Requisito pendiente
   - No disponible en esta build
6. Unknown technologies must degrade gracefully to a safe fallback such as:
   - `Tecnologia pendiente de clasificar`
7. Keep labels consistent across catalog, queue and result messages.

## UI/UX Requirements
- The primary Research UI must never show raw enum names as the main label.
- Unknown values must not render as camelCase or numeric IDs.
- If backend display metadata already exists and is sufficient, reuse it instead of duplicating labels.
- Keep diagnostics separate from player-facing copy.

## Backend/API Requirements
- Prefer frontend helper only.
- If backend DTOs need display metadata, keep it Development-only and add tests.
- Do not change persisted enum values or research progression rules.

## Safety Constraints
- Do not add tech effects.
- Do not change research behavior.
- Do not broaden scope into combat, fleets, economy or unlock logic.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts` or equivalent helper file.
- Possibly `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx` for wiring.
- Backend files only if display metadata must be added there.

## Acceptance Criteria
- Research display helper exists.
- Labels are Spanish and gameplay-readable.
- Unknown values degrade gracefully.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` only if backend changes are made.

## Notes / Residual Risks
- Final names and lore can be refined later by a writing or design pass.
- This task only establishes functional labels and presentation defaults.
- If the catalog is incomplete, placeholders must still look intentional and safe.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single presentation helper module.
