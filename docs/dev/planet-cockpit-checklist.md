# Planet Cockpit Checklist

Planet cockpit v1 is the first playable 2D colony-management surface for the current development build.
Use `docs/dev/planet-module-boundaries.md` to keep the dashboard role separate from construction and specialized placeholder cabinas.
Use `docs/dev/development-seed-profiles.md` for the standard Development-only QA commands, ids, and recovery guidance.

## Acceptance boundary

- `/planet` is a real route.
- `planetId` is query-param driven.
- The page stays 2D and development-safe.
- Construction enqueue is allowed only through an explicit confirmation flow.
- Completing due constructions remains disabled in this build because the current backend endpoint is global rather than planet-scoped.

## Queue materialization boundary

- Planet may summarize construction, research, and orbital/asset queue readiness from backend read models, but it must not materialize queued work from ordinary page load.
- Current global routes remain out of the Planet cockpit action surface:
  - `POST /api/dev/buildings/construction-orders/complete-due`
  - `POST /api/dev/research/orders/complete-due`
  - `POST /api/dev/assets/production/process-due`
- A future Planet refresh action may call only scoped Development materialization routes after they exist, using the active `civilizationId` and selected owned `planetId`, and must re-read backend state before showing changed buildings, research progress, stock, or queues.
- Completion does not spend resources again because Construction, Research, and Shipyard enqueue already deduct the visible cost immediately.

## Seeded QA scenario

Use `planet-full-validation` for the richer deterministic Planet QA baseline:

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Expected comparison baseline:
  - Existing visible buildings include `Centro de mando`, `Distrito habitacional`, `Planta solar`, and `Mina de metal`.
  - The queue shows one completed construction-history row and no open order.
  - At least one construction option is immediately available.
  - At least one construction option is blocked by `Recursos insuficientes`.

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Then confirm on `/planet`:

- The deterministic seeded scenario can open as `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- The dashboard route should stay focused on `Planeta`, `Construccion`, `Flotas`, and `Galaxia` handoff cards instead of showing the full construction catalog.
- The dashboard route should also show a compact read-only orbital or military preparation summary that points toward `Astillero`, `Defensas`, and `Flotas`.
- The route loads with Spanish loading, error, and empty states.
- When `civilizationId` is present and `planetId` is omitted, the primary or first owned planet is selected.
- The page header prioritizes planet identity, system, ownership, type, and colony state.
- Resources, production, and building capacity are readable without leading with raw DTO names.
- Buildings are grouped into readable categories.
- The construction queue is readable and uses `No hay construcciones en cola.` when empty.
- Construction candidates show readable names, categories, availability badges, and next-step language rather than raw backend errors.
- The seeded `Aurelia` scenario exposes both an available action and at least one blocked action for direct comparison.
- The enqueue flow requires explicit confirmation and refreshes the cockpit after success.
- The complete-due area stays visibly disabled and reads `No disponible en esta build`.
- Links back to Galaxy and toward Fleets preserve context without introducing shared global state.
- The orbital or military hub summary uses backend-backed counts such as `orbitalContext` and honest limitation copy instead of inventing stock or queue rows that Planet does not receive directly.
- Handoff links toward `Astillero`, `Defensas`, and `Flotas` preserve `civilizationId` and `planetId`.
- Diagnostics stay collapsed by default.
- Desktop layout avoids horizontal overflow.
- For the orbital-preparation runtime pass, revisit `Planeta` after `Shipyard -> Defenses -> Fleets` and confirm the same colony context survives the full loop without granting any new orbital or military mutation authority.

## Intentional exclusions

- No 3D.
- No combat.
- No espionage.
- No alliances.
- No WebSockets.
- No production auth.
- No Galaxy mutations.
- No fleet split or merge enablement from Planet.

## Current contract audit

- Real persisted mutation now:
  - Construction enqueue only, through the existing guarded planet or construction flow
- Current onboarding and context reality:
  - Planet still depends on explicit `civilizationId` query context instead of an authenticated session handoff.
  - When `planetId` is omitted, the backend selects the owned home or first owned planet for the provided civilization.
  - Current QA and sidebar helper flows still center on the seeded civilization `00000000-0000-0000-0000-000000000001` and owned planet `40000000-0000-0000-0000-000000000001`.
- Read-only orbital or military summary now:
  - `orbitalContext` counts for stationed groups, active departures, and active arrivals
  - building, queue, stockpile, and ownership context that Defenses or Shipyard can safely summarize without inventing mutations
  - Planet may explain defense readiness from visible defense-category construction data, but it must hand off any deeper flow to `Defensas`
  - Planet may explain Fleet readiness from visible orbital counts, but it must not claim that local orbital stock or Shipyard queue rows are exposed here
- Current resource-economy audit:
  - local balances come from persisted `PlanetResourceStockpile`
  - production summaries come from persisted `PlanetProductionProfile`
  - elapsed-time accrual exists in backend services, but Planet read-state does not auto-apply a live tick during page load
  - the accepted v1 accrual contract is explicit backend materialization through the existing economy tick service, not frontend timers and not silent `GET /planet` mutation
  - current seeded and playable-start baseline production remains `18` credits, `14` metal, `6` crystal, and `3` gas per hour before any research multiplier
  - current `ResourceExtraction` research is the only documented production-rate multiplier in scope for v1
- Future backend work required before Planet can claim more:
  - explicit resource-accrual apply path that materializes elapsed production safely before Planet claims live growth
  - authoritative refresh flow that shows backend-confirmed balances after an accrual step
  - direct orbital production submit
  - defense-specific mutation
  - fleet command execution, allocation, or combat actions

## Resource accrual contract note

- Planet must stay honest about balance freshness.
- Until the explicit accrual path exists, the page may show persisted reserves and persisted per-hour production together, but it must not imply that balances are increasing live in the browser.
- Ordinary page load must not mutate stockpiles just because the user opened or refreshed `/planet`.
- Any future resource refresh shown in Planet must come from a backend-confirmed accrual step plus a follow-up read, using the same authoritative pattern already used for Construction, Research, and Shipyard refreshes.
