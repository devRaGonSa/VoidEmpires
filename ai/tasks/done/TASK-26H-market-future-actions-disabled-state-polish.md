# TASK-26H

---
id: TASK-26H
title: Phase 26H - Market future actions disabled state polish
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: medium
---

## Goal

Ensure future market operations are visibly disabled, visually secondary, and impossible to mistake for active commands.

## Current problem

Market naturally suggests actions such as buying, selling, exporting, and route creation. If those placeholders are too prominent or too bright, they can mislead users into expecting real execution.

## Context from current implementation

- The Market cockpit already includes future-operation placeholders.
- The product boundary is explicit: no real market transactions, no offers, and no route execution.
- This task should strengthen disabled-state communication without expanding functionality.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md
- docs/dev/cockpit-copy-guidelines.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md

## Implementation requirements

1. Review the future action cards for:
   - `Comprar recursos`
   - `Vender recursos`
   - `Crear oferta`
   - `Crear ruta comercial`
   - `Exportar recursos`
   - `Importar recursos`
2. Ensure each card or action placeholder clearly communicates:
   - `No disponible en esta version.`
   - `Solo lectura en esta cabina.`
3. Confirm no click handler or hidden action path performs mutation.
4. Adjust styling so disabled future operations are never the brightest or most primary elements on the page.
5. Keep the placeholders useful as product-signpost content, not as near-future pseudo-buttons.

## UI/UX requirements

- Disabled future operations should read as roadmap hints, not blocked primary work.
- Visual treatment should remain consistent with the accepted cockpit suite.
- Primary focus must stay on the current read-only economy state.

## Backend/API requirements

- No backend changes are expected.
- No mutation handlers should be added or reintroduced.

## Safety constraints

- No transactions.
- No offer creation.
- No import or export execution.

## Acceptance criteria

- Future market actions are clearly disabled and visually secondary.
- No control suggests that a real transaction can be started.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If some placeholders currently reuse generic card components with hover or active affordances, the styling may need narrow overrides to avoid mixed signals.

