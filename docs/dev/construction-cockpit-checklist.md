# Construction Cockpit Checklist

Construction cockpit v1 is the focused route for the current guarded planet construction flow.
Use `docs/dev/planet-module-boundaries.md` to keep `/construction` limited to general infrastructure and to explain the specialized handoff cards.
Use `docs/dev/development-seed-profiles.md` for the standard Development-only QA commands, ids, and reseed guidance.

## Acceptance boundary

- `/construction` is a real route.
- The route stays development-safe and 2D-only.
- Only `enqueue construction` is executable from this cockpit.
- Queue completion remains disabled here because the backend complete-due endpoint is still global.
- Diagnostics stay collapsed unless explicitly opened.

## Seeded QA scenario

Use `planet-full-validation` for the richer deterministic Construction QA baseline:

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Expected comparison baseline:
  - Existing visible buildings include `Centro de mando`, `Distrito habitacional`, `Planta solar`, and `Mina de metal`.
  - At least one action is immediately available in the catalog.
  - Several actions remain blocked.
  - The queue shows one completed construction-history row and no open order.

Backend-only persisted QA helper:

- `.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed`
- Add `-BuildingType MetalMine` or another available building type to force the exact enqueue target.
- The helper talks directly to `POST /api/dev/buildings/construction-orders/enqueue`, then re-reads `/api/dev/planets/ui-state` to print queue and reserve deltas.

Verified refresh behavior:

- After a successful backend `201`, the cockpit re-reads `/api/dev/planets/ui-state` before leaving the user with the final visible queue and stockpile state.
- The main success copy remains grounded in backend confirmation: `La cabina se actualizo con el estado confirmado por la API.`
- If that follow-up read fails, the cockpit keeps the success or failure technically honest and asks the user to refresh the view instead of inventing a local queue row.

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Then confirm on `/construction`:

- The deterministic seeded scenario can open as `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- The route should keep specialized modules in the secondary handoff section rather than showing full research, army, shipyard, or defenses catalogs.
- The first viewport prioritizes active planet context, reserves, local economy, queue state, and the construction catalog.
- The catalog shows readable building names, readable categories, readable availability badges, and next-step language in Spanish.
- Blocked cards do not dominate the screen and their disabled buttons remain visually secondary.
- The current seeded `Aurelia` state shows both available and blocked actions for direct comparison.
- Preparing an available action opens the guarded confirmation panel with planet, building, action, level, cost, and duration.
- Sending the confirmed order refreshes the queue and surfaces a readable success message.
- A successful enqueue reduces the visible planet reserves immediately by the full order cost; this cockpit does not use a reservation-only placeholder rule.
- Command failures show Spanish guidance in the main panel while technical details remain secondary inside diagnostics.
- The queue remains readable after refresh, including building name, action, level, timing, and cost.
- Links toward `Planeta`, `Galaxia`, and `Flotas` preserve the current civilization and planet context.
- The diagnostics drawer starts collapsed.

## Intentional exclusions

- No 3D renderer.
- No complete-due execution from this cockpit.
- No combat, espionage, or alliance gameplay.
- No production authentication changes.
