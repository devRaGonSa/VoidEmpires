# TASK-15D

---
id: TASK-15D
title: Construction related modules handoff panel
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Add a handoff panel in `/construction` for specialized modules instead of rendering their catalogs inside Construction.

## Current problem
Users need to know where defense, research, army and shipyard content went after filtering `/construction`. Removing categories without explanation would feel like content vanished.

## Context from current implementation
The construction page already has resource, cost and action-card layouts. It should now focus on allowed construction categories and explain the specialized boundaries clearly.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Add a `Cabinas especializadas` or `Gestión avanzada` section to `/construction`.
- Include handoff cards for:
  - Investigación
  - Ejército Tierra
  - Astillero
  - Defensas
- Each card should show:
  - module purpose;
  - whether the cabin is placeholder or read-only;
  - a link to the route with context.
- If there are building actions assigned to those modules, show a count such as `3 elementos gestionados en Defensas`.
- Do not show full action details for specialized items inside `/construction`.
- Keep construction focused on general, civil, economic and infrastructure actions.

## UI/UX requirements
- Spanish-first.
- Handoff cards should be secondary to the main construction catalog.
- Do not make them look like direct build buttons.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- No specialized module mutations.

## Acceptance criteria
- `/construction` explains where specialized content belongs.
- Specialized categories are not full sections.
- Links work and preserve context.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Later module implementation can replace the handoff cards with richer status previews.
