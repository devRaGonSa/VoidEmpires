# TASK-26D

---
id: TASK-26D
title: Phase 26D - Market primary copy and read-only boundary polish
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: high
---

## Goal

Make the Market cockpit primary copy clearly Spanish, gameplay-facing, and unmistakably read-only.

## Current problem

Market must feel like an economy command cockpit rather than a generic Development page. It also needs stronger wording that makes clear no purchases, sales, or trade execution happen here.

## Context from current implementation

- Market already presents economy state, reference values, and future-operation placeholders.
- The repository has cockpit copy guidelines and existing hero, status, and layout patterns.
- This task is visual and narrative polish only, not feature expansion.

## Files to read first

- docs/dev/cockpit-copy-guidelines.md
- docs/dev/market-cockpit-checklist.md
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts
- docs/dev/market-cockpit-checklist.md

## Implementation requirements

1. Polish the visible copy for:
   - hero area
   - summary cards
   - reserves and production section
   - reference prices section
   - future operations section
2. Ensure the primary wording includes or aligns with:
   - `Mercado`
   - `Lectura economica`
   - `Referencias orientativas`
   - `Operaciones no disponibles en esta version`
   - `Esta cabina no ejecuta compras ni ventas.`
3. Move raw technical wording out of the primary UI and into diagnostics where appropriate.
4. Keep copy concise, readable, and consistent with the existing cockpit suite.
5. Do not introduce new controls or mutate existing read-only boundaries.

## UI/UX requirements

- Primary copy must be Spanish-first.
- The page should feel like a strategy-economy overview, not a debug surface.
- Any disabled operations should read as intentional future placeholders rather than missing implementations.

## Backend/API requirements

- No backend changes are expected.
- Do not change Market API behavior or add mutation handlers.

## Safety constraints

- No transactions.
- No mutation handlers.
- No new action flows hidden behind polished copy.

## Acceptance criteria

- Market primary copy is clearly Spanish, gameplay-facing, and read-only.
- Primary UI no longer depends on technical endpoint or payload wording.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If existing API field names leak directly into presentation helpers, this task may need small helper cleanups to keep the user-facing copy clean.

