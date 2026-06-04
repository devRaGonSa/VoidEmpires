# TASK-26E

---
id: TASK-26E
title: Phase 26E - Market reserves and production layout polish
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: high
---

## Goal

Improve the readability and economy-specific framing of the Market reserves and production panels.

## Current problem

Resource panels can become dense or feel too similar to Planet and Construction. Market should interpret economic state clearly without duplicating every other cockpit's layout.

## Context from current implementation

- Market already surfaces reserves and production-related information.
- The accepted cockpit suite favors compact cards, strong labels, and readable comparisons.
- This block must stay read-only and presentation-focused.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts
- src/VoidEmpires.Frontend/src/api/marketTypes.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts
- src/VoidEmpires.Frontend/src/styles.css

## Implementation requirements

1. Improve reserve and production panel labeling so the user can quickly distinguish local versus broader economy context.
2. Ensure visible labels support or align with:
   - `Creditos`
   - `Metal`
   - `Cristal`
   - `Gas`
   - other current resource labels when present
3. Make resource scope explicit with wording such as:
   - `Reservas de Aurelia`
   - `Lectura de civilizacion`
   - `Produccion estimada`
4. Add or polish compact resource-signal text such as:
   - `Excedente visible`
   - `Reserva ajustada`
   - `Estable`
5. Keep the layout compact and readable at common desktop widths.
6. Do not invent production values or derived claims the backend does not already expose.

## UI/UX requirements

- Reserve panels should scan quickly without looking like raw ledgers.
- Market should feel economy-focused rather than like a duplicate Planet dashboard.
- Compact signal text should help interpretation without crowding the cards.

## Backend/API requirements

- No backend changes are expected.
- If a field is missing, do not fabricate it in the UI.

## Safety constraints

- Read-only only.
- No new calculations that imply unsupported economic mechanics.
- No resource mutation.

## Acceptance criteria

- Reserves and production panels are more readable and clearly scoped.
- Labels and signal text remain truthful to current data.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If the current data model omits one of the expected resource categories in a given seed state, the layout should degrade cleanly rather than leaving awkward gaps.

