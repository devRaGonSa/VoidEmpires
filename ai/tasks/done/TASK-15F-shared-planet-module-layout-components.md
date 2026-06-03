# TASK-15F

---
id: TASK-15F
title: Shared planet module layout components
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Reduce duplicated markup between Planet, Construction and the module placeholders by extracting lightweight reusable layout pieces.

## Current problem
As more module screens are created, copying large card structures can produce inconsistent behavior, copy drift and style regressions.

## Context from current implementation
The app already uses a consistent dark cockpit style. This task should extract only obvious shared pieces, not introduce a heavy design system rewrite.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/components/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/components/*`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/*.tsx` for module placeholders as needed

## Implementation requirements
- Identify duplicated patterns such as:
  - active planet hero card;
  - local resources summary;
  - module status card;
  - related cabin card;
  - collapsed diagnostics panel.
- Extract only small reusable React components where it clearly reduces duplication.
- Suggested components may include:
  - `PlanetContextHeader`
  - `ModuleStatusCard`
  - `ModuleHandoffGrid`
  - `LocalResourceSummary`
  - `CollapsedDiagnosticsPanel`
- Keep props typed.
- Do not create over-abstracted generic components.
- Preserve the existing visual output.

## UI/UX requirements
- No visible regression.
- Layout remains consistent across modules.

## Backend/API requirements
- No backend change.

## Safety constraints
- Do not rewrite the entire frontend architecture.
- Do not move unrelated fleet or galaxy components unless necessary.

## Acceptance criteria
- Shared module screens use consistent components.
- Frontend build passes.
- The code is easier to maintain.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Keep this task disciplined. If extraction grows too large, leave a follow-up note instead of broadening scope.
