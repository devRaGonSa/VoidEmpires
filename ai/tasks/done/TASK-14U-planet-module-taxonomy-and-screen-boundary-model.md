# TASK-14U

---
id: TASK-14U
title: Planet module taxonomy and screen boundary model
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Define a deterministic frontend taxonomy for planet-related screens so the sidebar modules stop competing with each other.

## Current problem
`/planet` and `/construction` currently behave like broad catch-all pages. The same building and action data can appear in the wrong conceptual place, which makes the UI feel mixed even though the sidebar already exposes separate cabins for construction, research, ground army, shipyard and defenses.

## Context from current implementation
The previous block improved labels and cost readability, so the UI is clearer at the card level. The remaining problem is boundary design: the same read-model data is still being presented in screens that do not own it conceptually. The taxonomy should be frontend-first unless backend metadata already exists and is safer to reuse.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`

## Implementation requirements
- Introduce a centralized taxonomy for screen ownership:
  - `PlanetOverview`
  - `GeneralConstruction`
  - `Research`
  - `GroundArmy`
  - `Shipyard`
  - `Defenses`
  - `Logistics`
  - `UnknownOrDiagnostics`
- Add helper functions such as:
  - `getPlanetModuleForBuilding(...)`
  - `getPlanetModuleLabel(...)`
  - `isGeneralConstructionAction(...)`
  - `isSpecializedModuleAction(...)`
- Keep the taxonomy deterministic and easy to reuse in future tasks.
- Do not rename persisted enums or backend domain concepts.
- If an item can appear as buildable infrastructure but is owned elsewhere as gameplay, document that distinction in code comments or helper docs.

## UI/UX requirements
- Primary labels must be Spanish.
- Use the sidebar names for module labels:
  - `Planeta`
  - `Construcción`
  - `Investigación`
  - `Ejército Tierra`
  - `Astillero`
  - `Defensas`
- Avoid technical names in the primary view.
- Unknown categories should not leak raw numbers; only diagnostics may show `Pendiente de clasificar`.

## Backend/API requirements
- Prefer no backend changes.
- If backend display metadata is already available and clearly better, use it only with tests and without changing persisted values.

## Safety constraints
- No research execution.
- No troop training.
- No ship production.
- No defense execution.
- No new migrations.

## Acceptance criteria
- A clear taxonomy exists in code.
- `/planet` and `/construction` can use it to determine what belongs in each screen.
- Raw category numbers no longer decide screen ownership.
- The taxonomy is documented enough for later Codex tasks to reuse safely.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` only if backend files change.
- `dotnet test --no-build` only if backend files change.

## Notes / residual risks
- This taxonomy is an interim presentation boundary, not the final product catalog model.
- Later backend work can formalize the same concepts without changing the UI intent.
