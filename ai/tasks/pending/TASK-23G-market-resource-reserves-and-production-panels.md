# TASK-23G

---
id: TASK-23G
title: Phase 23G - Market resource reserves and production panels
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Render reserve and production panels that present Market resources in a clear read-only economy view.

## Purpose

Market should aggregate and interpret current resources and economy flow without replacing Planet or other specialized cockpits that already own local management actions.

## Current problem

The cockpit needs concrete reserve and production sections so the player can actually see what the Market dashboard is summarizing. Without them, the page would remain abstract and not clearly useful.

## Context

Planet and other accepted cockpits already expose resources. Market should aggregate and interpret them without duplicating active management behavior or inventing unavailable numbers.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- Market view-model and presentation helpers
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Component discovery

Inspect how current cockpits render resource cards, grouped summary panels, and fallback states when values are unavailable. Prefer adapting those patterns instead of creating a one-off economy grid.

## Dependency analysis

Expected rendering flow:

- Market UI state -> normalized reserve and production view model
- presentation helpers -> resource labels and amount formatting
- page sections -> reserve and flow cards

## Implementation requirements

1. Render a reserves panel that can show, where available:
   - `Creditos`
   - `Metal`
   - `Cristal`
   - `Gas`
   - `Deuterio`
   - `Energia`
2. Render a production or flow panel that can show:
   - estimated hourly production if available
   - honest fallback text when production is unknown or unsupported
3. Show scope framing such as:
   - `Reservas de Aurelia`
   - `Lectura de civilizacion`
   - `Produccion estimada`
4. Add signal text when derivable, for example:
   - `Excedente visible`
   - `Reserva ajustada`
   - `Recurso estable`
5. Do not invent unavailable data or pretend every resource has an hourly number if the backend does not provide it.
6. Do not add mutation buttons.

## UI/UX requirements

- Cards should stay compact and readable
- Spanish labels only in primary UI
- Avoid technical enum names
- Keep the panels visually secondary to the page title but primary over diagnostics

## Backend/API requirements

- No backend change is expected unless the read model lacks essential reserve data that already exists elsewhere.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- Market helper or mapper files
- `src/VoidEmpires.Frontend/src/styles.css` if needed for a small layout addition

## Safety constraints

- No resource mutation
- No transaction actions
- No fabricated production values

## Acceptance criteria

- Reserves and production are visible and understandable from the Market page.
- Missing values are handled honestly instead of leaking raw backend fields or blank spaces.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Economy balance values can evolve later; this task should prioritize readability and truthfulness over analytical depth.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to Market reserve or production presentation.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer reusing existing resource card patterns instead of building a large new component family.
- If additional backend shaping is required, split that work into a follow-up task rather than inflating this frontend panel task.
