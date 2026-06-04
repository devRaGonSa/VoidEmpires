# Market Cockpit Checklist

Market cockpit v1 is accepted as a read-only economy cockpit foundation for the current development build.
Use `docs/dev/development-seed-profiles.md` for deterministic QA setup and `docs/dev/planet-module-boundaries.md` to keep Market separate from Planet, Construction, Shipyard, Fleets, and Galaxy ownership boundaries.

Closure note for block `26C-26L`:

- the read-only visual polish baseline is implemented
- this document is the source of truth for manual Market QA
- final screenshot-backed acceptance remains user-driven

## Acceptance boundary

- `/market` is a real route.
- `civilizationId` is required and `planetId` remains optional.
- The page stays development-safe and read-only.
- Market may expose reserves, production, advisory ratios, trade signals, disabled future operations, and cross-cockpit handoffs.
- Market must not buy, sell, transfer, auction, mutate resources, or execute trade routes.
- Diagnostics stay collapsed or clearly secondary.

## Task 26C visual readiness audit

Component discovery for this audit:

- Entrypoint: [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx)
- Frontend presentation owners: [src/VoidEmpires.Frontend/src/utils/marketPresentation.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/marketPresentation.ts) and [src/VoidEmpires.Frontend/src/utils/marketViewModel.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/marketViewModel.ts)
- Route context helpers used by Market handoffs: [src/VoidEmpires.Frontend/src/utils/routeUrls.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/routeUrls.ts)

Dependency map for the current Market read surface:

- `/market` page -> `fetchMarketUiState` -> `/api/dev/market/ui-state`
- API response -> `mapMarketUiStateToViewModel`
- View model -> Market summary, reserves, production, references, signals, future-action placeholders, diagnostics
- Route helpers -> Planet, Construction, Shipyard, Fleets, and Galaxy handoffs with preserved `civilizationId` and optional `planetId`

Section-presence result from the audit:

- hero or header: present
- economy summary: present
- reserves and production: present
- reference prices or ratios: present
- trade signals and future routes: present
- disabled future operations: present
- handoffs: present
- collapsed diagnostics: present, but currently duplicated

Primary visual QA target list for the rest of the block:

| Priority | Risk | Why it can block acceptance | Current owner |
|---|---|---|---|
| high | The first viewport is too dev-loader-heavy before the actual economy story begins. `Entrada de mercado`, `Resumen de cabina`, `Limite actual`, suspicious-context handling, loading, and empty/error states all sit ahead of the main Market summary. | The route can read more like a tooling shell than an accepted cockpit, especially on narrower viewports. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx) |
| high | Several primary labels still read like action or console language: `Accion principal`, `Apertura de Mercado`, `Operaciones futuras`, `Acciones de mercado`, and button-style future cards. | Market can look closer to a blocked transaction console than a read-only economy analysis cabin. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx), [src/VoidEmpires.Frontend/src/utils/marketViewModel.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/marketViewModel.ts) |
| high | Disabled future operations are rendered as a full grid of buttons, one per commercial action. | Even disabled, the button grid visually over-promises buying, selling, route creation, import, and export. It should stay visibly secondary to the read-only economy panels. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx) |
| medium | Diagnostics are collapsed, but there are two separate technical disclosures on the page. | The current layout repeats support content and increases page length and visual noise, making diagnostics feel more prominent than they should. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx) |
| medium | Read-only boundary messaging is truthful but repeated in many places: hero, top rule card, references, signals, future operations, and handoff copy. | The repetition helps safety, but too much of it can crowd out the actual economy story and flatten hierarchy. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx) |
| medium | The trade-signal and route block may visually outweigh the core reserves and production story. | Market should read economy-first, with logistics and future routes as secondary interpretation rather than the main event. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx) |
| medium | Placeholder fallbacks such as `Recurso pendiente de clasificar`, `Senal economica pendiente de clasificar`, `Confianza de precio pendiente de clasificar`, `Operacion pendiente de clasificar`, and `Accion de mercado pendiente de clasificar` are still player-visible if new API values appear. | These are acceptable as diagnostics, but they fail visual QA if they escape into primary cards. | [src/VoidEmpires.Frontend/src/utils/marketPresentation.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/marketPresentation.ts) |
| medium | `reasonKey` values for future actions are normalized through `getTradeStateLabel`, which can fall back to `Operacion pendiente de clasificar`. | If backend reason keys expand, Market can surface placeholder wording in always-visible cards instead of a contained limitation note. | [src/VoidEmpires.Frontend/src/utils/marketViewModel.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/marketViewModel.ts) |
| low | The page hardcodes a future-action list instead of rendering only the backend-provided `futureActions`. | This is still safe today, but it is a frontend assumption that can drift from the API and make the page look more transactional than the current contract actually is. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx) |
| low | Market currently keeps two navigation areas near the end: a full handoff card block plus a generic related-cabins strip. | The duplication is not wrong, but it spends valuable vertical space after an already long page. | [src/VoidEmpires.Frontend/src/pages/MarketPage.tsx](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/pages/MarketPage.tsx) |

What already reads well and should be preserved:

- The route clearly states the read-only boundary and never exposes live buy, sell, or route-execution actions.
- Core economy sections already exist, so the rest of the block can focus on hierarchy and copy instead of adding major new surfaces.
- Handoff links already preserve context toward the neighboring cockpit owners.
- Diagnostics are already collapsed by default rather than opening directly into raw support detail.

Follow-up path from this audit:

- `26D`: tighten primary copy and reduce action-like or execution-like wording
- `26E`: improve production and reserve layout so the economy story appears earlier and reads more clearly
- `26F`: reframe reference ratios as advisory comparisons only
- `26G`: demote trade-signal and route placeholder weight relative to the economy summary
- `26H`: keep future operations visible but clearly secondary and non-interactive
- `26I`: simplify handoff density and clarify cockpit ownership boundaries
- `26J`: contain diagnostics and raw fallback wording more aggressively
- `26K`: finish docs and smoke wording once the visual pass stabilizes
- `26L`: closure-only confirmation after the final Market polish pass

## Task 26D copy focus

Primary Market wording should now align with these anchors:

- `Mercado`
- `Lectura economica`
- `Referencias orientativas`
- `Operaciones no disponibles en esta version`
- `Esta cabina no ejecuta compras ni ventas.`

During manual QA, treat any return to console-like labels such as `Accion principal`, `Apertura de Mercado`, `placeholder`, or generic `operacion` wording in the first viewport as a regression for this block.

## Task 26E layout focus

Reserve and production panels should now read with these scoping cues:

- `Lectura de civilizacion`
- `Reservas de Aurelia` when the seeded planet is active
- `Produccion estimada`
- compact resource-state labels such as `Excedente visible`, `Reserva ajustada`, and `Estable`

Treat dense ledger-like cards or reserve and production rows that no longer distinguish local versus broader scope as a regression for this block.

## Task 26F reference safety focus

Reference and ratio cards should now keep these safety cues visible:

- `Referencia de intercambio`
- `Ratio orientativo`
- `No es una oferta activa`
- `Solo lectura`

Treat any ratio card that reads like a confirmation step, a live offer, or a bright primary action as a regression for this block.

## Task 26G signals and route focus

Trade-signal and route-placeholder content should now keep these cues visible:

- `Senales comerciales`
- `Rutas comerciales futuras`
- `Transferencia de recursos no disponible`
- `Crear ruta comercial no disponible`
- `Revisar logistica en Flotas`
- `Ver contexto de ruta en Galaxia`

Treat any route placeholder that reads like a builder, a transfer setup, or a primary command surface as a regression for this block.

## Task 26H disabled future-actions focus

Future Market operations should now keep these disabled-state cues visible:

- `No disponible en esta version.`
- `Solo lectura en esta cabina.`

Treat any future Market operation card that still reads like a clickable primary command, a blocked CTA, or a near-future execution step as a regression for this block.

## Task 26J diagnostics containment focus

Market diagnostics should now remain contained under one collapsed `Diagnostico secundario` surface.

Treat repeated technical drawers, raw fallback wording in the first viewport, or limitation notes that escape into the main economy flow as a regression for this block.

## Seeded QA scenario

Use `cockpit-validation` for the richer deterministic Market QA baseline:

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Solar system: `Helios Gate`
- Deterministic QA URL:
  `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Expected comparison baseline:
  - civilization and selected-planet reserves are non-zero
  - selected-planet production is visible
  - advisory reference ratios are present
  - at least one economy signal plus one future-route signal are visible
  - future Market actions remain disabled placeholders

## Final manual QA

This visual QA pass remains user-driven. Use the checklist below as the accepted read-only baseline, but do not treat it as screenshot-backed approval unless the browser pass is actually performed.

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

If integration checks are reviewed for this cockpit block, record:

`No integration tests configured.`

Then apply the richer shared seed twice:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5142/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"cockpit-validation"}'

Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5142/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"cockpit-validation"}'
```

Then confirm on `/market`:

- The deterministic seeded scenario can open as `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- The first viewport reads like an economy cockpit, not a transaction console or shell-only placeholder.
- The page reads as `Mercado`, `Lectura economica`, and `Referencias orientativas`, not as a dev tool or transaction console.
- The page loads with Spanish loading, missing-context, and failure states.
- Civilization and selected-planet context are visible.
- Summary cards are visible before diagnostics.
- Resource summary and reserve posture are visible.
- Reserves remain clearly split between `Lectura de civilizacion` and `Reservas de Aurelia`.
- Production or economy summary is visible when the seeded profile is applied.
- Reference prices or ratios are visible as advisory-only data and read as `No es una oferta activa`.
- Trade signals and future-route placeholders are visible as secondary context.
- Future Market actions stay disabled and visually secondary.
- Handoff links to Planet, Construction, Shipyard, Fleets, and Galaxy preserve context.
- Diagnostics stay collapsed by default.
- Buying is not available.
- Selling is not available.
- Resource mutation is not available.
- Trade-route execution is not available.
- Raw backend wording does not dominate the first viewport.

## Intentional exclusions

- No buying.
- No selling.
- No player-to-player trading.
- No auctions.
- No resource mutation.
- No trade-route execution.
- No WebSockets.
- No production authentication.
- No hidden Fleet or Galaxy mutation through Market handoffs.

## Backend scope audit

This section keeps the original backend-scope contract that guided Market v1 implementation.

### Accepted boundary

- `/market` is not backed by a dedicated production market service or endpoint.
- Market v1 stays read-only, development-safe, and honest about missing commerce mechanics.
- Market may summarize civilization and selected-planet economy context by reusing existing read models and persisted economy data.
- Market may show deterministic advisory ratios or placeholder price signals only as derived guidance.
- Market must not buy, sell, transfer, auction, list, match orders, or execute trade routes.

### Backend inventory

#### Resource taxonomy

Implemented canonical resource types:

- `Credits`
- `Metal`
- `Crystal`
- `Gas`

Primary components:

- `src/VoidEmpires.Domain/Economy/ResourceType.cs`
- `src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs`
- `src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs`

#### Planet reserves and production

Implemented now:

- Per-planet persisted stockpiles in `PlanetResourceStockpile`.
- Per-planet persisted hourly production in `PlanetProductionProfile`.
- Production application through `ResourceProductionService` and `PlanetEconomyTickService`.
- Research-based production multiplier through `ResearchBonusCalculator`.

Primary components:

- `src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs`
- `src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs`
- `src/VoidEmpires.Domain/Economy/ResourceProductionService.cs`
- `src/VoidEmpires.Infrastructure/Economy/PlanetEconomyTickService.cs`
- `src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs`

Current limit:

- No civilization-level treasury or shared reserve aggregate is persisted today. The real reserve source is planet-scoped stockpile data.

#### Cost and affordability foundations

Implemented now:

- Construction costs from `BuildingCatalog`.
- Research costs from `ResearchCatalog`.
- Orbital and planetary asset costs from `OrbitalAssetCatalog` and `PlanetaryAssetCatalog`.
- Fleet travel costs from `OrbitalTravelEstimator`.
- Shared affordability and spend validation through `PlanetResourceStockpile` and `ResourceSpendService`.

Primary components:

- `src/VoidEmpires.Infrastructure/Buildings/PlanetConstructionQueueService.cs`
- `src/VoidEmpires.Infrastructure/Research/ResearchEnqueueReadinessEvaluator.cs`
- `src/VoidEmpires.Infrastructure/Assets/AssetProductionQueueService.cs`
- `src/VoidEmpires.Infrastructure/Economy/ResourceSpendService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs`

#### Queue and production state

Implemented now:

- Construction queue persistence in `PlanetConstructionOrder`.
- Research queue persistence in `ResearchOrder`.
- Asset production queue persistence in `AssetProductionOrder`.
- Orbital stock persistence in `OrbitalAssetStock`.
- Planetary unit stock persistence in `PlanetaryAssetStock`.

#### Fleet and logistics-adjacent signals

Implemented now:

- Civilizations can inspect orbital groups, active transfers, route profile metadata, travel cost previews, and placeholder fuel readiness.
- Fleet UI state already exposes current-planet resource contexts for fleet groups.
- Planet UI state already exposes orbital activity counts for stationed groups, departures, and arrivals.

Current limit:

- These signals describe logistics readiness and travel cost pressure, not trade execution or resource shipping.

### Safe Market v1 scope

Market v1 may safely do the following:

- read civilization context
- read selected planet context when `planetId` is provided
- summarize planet-scoped reserves for `Credits`, `Metal`, `Crystal`, and `Gas`
- summarize selected-planet production per hour when a production profile exists
- summarize resource pressure implied by construction, research, shipyard, and ground-army costs
- summarize logistics pressure implied by fleet travel-cost previews and active transfer context
- derive deterministic advisory ratios or reference prices from existing static costs or seeded economy rules
- show future trade-route, listing, import, export, and exchange actions only as disabled placeholders
- expose diagnostics that clarify when values are advisory, development-only, selected-planet-only, or unavailable

Market v1 must not do the following:

- mutate stockpiles
- spend resources
- create or complete fleet transfers
- create resource shipments
- create buy or sell orders
- create listings or auctions
- execute player-to-player trades
- imply that civilization-wide reserves already exist as a backend truth when only planet stockpiles are persisted

### Ownership boundaries

- `Planet` remains the source for local colony reserves, production, building capacity, and orbital-activity context.
- `Construction` remains the source for building queue management, building costs, and guarded construction enqueue.
- `Research` remains the source for technology queue state, technology costs, and research readiness.
- `Shipyard` remains the source for orbital production queue state, local orbital stock, and orbital build requirements.
- `Defenses` remains the source for defense-specific construction readiness.
- `Ground Army` remains the source for local troop stock and terrestrial production readiness.
- `Fleets` remains the source for orbital groups, transfers, route previews, and travel-cost logistics hints.
- `Galaxy` remains read-only strategic context and must not be duplicated as a market execution surface.

### Decision summary

- The repository already has enough read-only economy and logistics foundations to support a truthful Market cockpit.
- The repository does not have a real market subsystem.
- The correct `Mercado v1` scope is an economy-analysis cockpit assembled from existing reserves, production, cost, affordability, and logistics-readiness data.
- All transaction gameplay, commerce execution, and resource movement remain explicitly disabled in this phase.
