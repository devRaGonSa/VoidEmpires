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
- Read-only orbital or military summary now:
  - `orbitalContext` counts for stationed groups, active departures, and active arrivals
  - building, queue, stockpile, and ownership context that Defenses or Shipyard can safely summarize without inventing mutations
  - Planet may explain defense readiness from visible defense-category construction data, but it must hand off any deeper flow to `Defensas`
  - Planet may explain Fleet readiness from visible orbital counts, but it must not claim that local orbital stock or Shipyard queue rows are exposed here
- Future backend work required before Planet can claim more:
  - direct orbital production submit
  - defense-specific mutation
  - fleet command execution, allocation, or combat actions
