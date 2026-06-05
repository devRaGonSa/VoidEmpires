# Frontend Performance Notes

## Route Loading Baseline

Observed on `2026-06-05` from `npm run build --prefix src/VoidEmpires.Frontend`:

- `dist/assets/index-DuplAnn0.js`: `551.88 kB` minified, `136.02 kB` gzip
- `dist/assets/index-D7aZozWO.css`: `45.44 kB` minified, `7.20 kB` gzip
- Vite output: `Some chunks are larger than 500 kB after minification`
- Current build shape: one CSS asset and one JS application chunk for the whole frontend shell plus every implemented cockpit route

## Current Eager Route Surface

`src/App.tsx` currently imports these route components synchronously:

- `StrategicMapPage`
- `PlanetPage`
- `ConstructionPage`
- `ResearchPage`
- `ShipyardPage`
- `FleetsPage`
- `MarketPage`
- `DefensesPage`
- `GroundArmyPage`
- `EspionagePage`
- `ModuleCabinPage`

Large page modules currently pulled into the initial chunk include:

- `StrategicMapPage.tsx` at about `69.8 kB`
- `FleetsPage.tsx` at about `67.0 kB`
- `ShipyardPage.tsx` at about `56.6 kB`
- `PlanetPage.tsx` at about `54.0 kB`
- `MarketPage.tsx` at about `43.9 kB`
- `DefensesPage.tsx` at about `37.8 kB`
- `EspionagePage.tsx` at about `34.7 kB`
- `ResearchPage.tsx` at about `30.4 kB`
- `GroundArmyPage.tsx` at about `21.9 kB`

## Shared Shell Versus Route-Local Code

Keep synchronous:

- `src/main.tsx`
- `src/App.tsx`
- `src/styles.css`
- `src/components/ui/AppShell.tsx`
- `src/config.ts`
- `src/utils/routeUrls.ts`
- lightweight route metadata such as `specializedPlanetModuleRoutes`

Move behind route-level lazy loading:

- page components under `src/pages/` that fetch cockpit-specific backend state
- cockpit-specific panels and view-model helpers that are only referenced from one route tree
- the specialized route branches that users do not need on first load

Watch-outs:

- `ConstructionPage` only re-exports `PlanetPage`, so splitting both routes naively can preserve behavior but should not duplicate the same heavy implementation in separate chunks.
- `specializedPlanetModuleRoutes` is shared navigation metadata and should remain available before any route content is loaded.
- Shared navigation links and accepted seeded URLs must stay stable while route components move to `React.lazy`.

## Recommended Follow-up

- Introduce route-level `React.lazy` loading in `App.tsx` first, with a small shared fallback that preserves the current shell framing.
- Keep the map shell, sidebar, top resource bar, route metadata, and URL builders synchronous.
- Rebuild immediately after the lazy-route pass and compare the new entry chunk against the `551.88 kB` baseline before considering `manualChunks`.
- Evaluate `manualChunks` only after route-level splitting lands, so any Vite tuning reflects the real post-lazy import graph.

## Post-Lazy Evaluation

Observed on `2026-06-05` after route-level lazy loading and the route-metadata extraction:

- transformed modules: `87`
- current entry chunk: `dist/assets/index-CDfREE6c.js` at `179.32 kB` minified and `58.48 kB` gzip
- current CSS asset: `dist/assets/index-DnIPhZw-.css` at `45.97 kB` minified and `7.26 kB` gzip
- current build shape: shared entry chunk plus per-route chunks for `StrategicMapPage`, `PlanetPage`, `ConstructionPage`, `ResearchPage`, `ShipyardPage`, `FleetsPage`, `DefensesPage`, `GroundArmyPage`, `EspionagePage`, `MarketPage`, and `ModuleCabinPage`
- current Vite result: no `500 kB` chunk-size warning

Largest async cockpit chunks in the current build:

- `FleetsPage-TLboUIOD.js`: `48.43 kB` minified, `11.41 kB` gzip
- `MarketPage-IQHCiS20.js`: `41.22 kB` minified, `8.79 kB` gzip
- `StrategicMapPage-H1U1EWf4.js`: `40.09 kB` minified, `9.75 kB` gzip
- `ShipyardPage-CsI210CZ.js`: `38.15 kB` minified, `7.88 kB` gzip

Decision:

- keep the default Vite chunking
- do not add `manualChunks` in `vite.config.ts`

Rationale:

- route-level splitting already removed the warning and reduced the entry chunk from `551.88 kB` to `179.32 kB`
- no remaining build symptom justifies adding manual chunk rules
- preserving Vite defaults keeps chunk behavior simpler and avoids brittle grouping rules that would need upkeep as cockpit routes evolve

## Validation Command

Run from repository root:

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Interpret the result this way:

- route-level lazy loading is healthy when Vite emits one shared entry chunk plus cockpit-specific async chunks
- a renewed single oversized entry chunk or a returned `500 kB` warning should be treated as a regression worth investigating
- this block changes route-loading architecture only; it does not change gameplay rules, backend contracts, or accepted cockpit URLs
- after any route-loading change, rerun the accepted cockpit smoke checks because a clean build does not replace route-level QA
