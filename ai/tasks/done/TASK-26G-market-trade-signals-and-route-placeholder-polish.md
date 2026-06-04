# TASK-26G

---
id: TASK-26G
title: Phase 26G - Market trade signals and route placeholder polish
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: medium
---

## Goal

Polish Market trade signals and future route placeholders without implying that logistics or trade routes can be executed from this cockpit.

## Current problem

Commercial signals and future route placeholders are useful context, but they can accidentally look like active logistics tools if the labels or emphasis are too strong.

## Context from current implementation

- Market already references trade signals and future route concepts.
- Fleets and Galaxy remain separate cockpits for logistics and map context.
- This block must preserve those boundaries while making the Market page feel coherent and intentional.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md

## Implementation requirements

1. Polish labels for trade signals and future route placeholders so they stay informative but non-executable.
2. Ensure the visible language includes or aligns with:
   - `Senales comerciales`
   - `Rutas comerciales futuras`
   - `Transferencia de recursos no disponible`
   - `Crear ruta comercial no disponible`
3. Add or polish clear handoff language pointing users toward:
   - `Revisar logistica en Flotas`
   - `Ver contexto de ruta en Galaxia`
4. Keep disabled actions visually secondary and clearly separate from primary summary content.
5. Do not add actual route creation, transfer setup, or fleet movement.

## UI/UX requirements

- Future-route sections should feel like planning notes, not actionable route builders.
- Handoff links should be discoverable without overshadowing the current Market read.
- Disabled states should remain visibly subordinate to the read-only content.

## Backend/API requirements

- No backend changes are expected.
- Do not introduce route execution or logistics commands.

## Safety constraints

- No trade route execution.
- No fleet movement.
- No resource transfer creation from Market.

## Acceptance criteria

- Trade signals and route placeholders are readable, secondary, and clearly non-executable.
- Cross-cockpit handoffs are clearer without breaking existing navigation.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If route helpers currently use generic labels, the polish work may need to centralize a small amount of Market-specific wording without affecting other cockpits.

