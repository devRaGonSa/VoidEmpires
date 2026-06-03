# TASK-15E

---
id: TASK-15E
title: Module aware building action filtering and guardrails
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Prevent accidental execution of building actions from the wrong module screen.

## Current problem
Once the catalog is split, the UI must ensure hidden actions are not still executable through stale components or copied cards. Filtering has to happen before action buttons are rendered.

## Context from current implementation
Construction actions are controlled and confirmation-based. That must remain true. The frontend can help by filtering earlier, but the backend remains authoritative.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`

## Implementation requirements
- Ensure action lists are filtered by owning module before render.
- Ensure hidden actions do not have active buttons on the wrong screen.
- If an action belongs to another module, display a safe handoff link instead of an execute button.
- Add helper functions such as:
  - `canRenderActionInModule(action, module)`
  - `getActionHandoffTarget(action)`
  - `getWrongModuleMessage(action)`
- For wrong-module actions, use copy such as:
  - `Esta orden pertenece a Defensas.`
  - `Gestionar desde Astillero.`
  - `Disponible en una cabina futura.`
- Keep the construction endpoint authoritative; the frontend guards are UX support, not security.

## UI/UX requirements
- Wrong-module items must never look directly actionable.
- The user should understand where to go next.

## Backend/API requirements
- No backend change expected unless current action data lacks enough classification.

## Safety constraints
- Do not weaken the existing confirmation flow.
- Do not bypass backend validation.

## Acceptance criteria
- `/construction` only offers correct-module actions.
- Specialized module placeholders do not execute hidden construction actions.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Backend validation remains the true authority; these guardrails are UX and safety support.
