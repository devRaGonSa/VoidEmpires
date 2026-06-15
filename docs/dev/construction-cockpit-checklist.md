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

## Completion materialization contract

- Persisted model: `PlanetConstructionOrder`.
- Open states: `Pending` and `Active`.
- Terminal states: `Completed` and `Cancelled`.
- Due field: `EndsAtUtc`.
- Existing service: `ConstructionOrderCompletionService`.
- Current route: `POST /api/dev/buildings/construction-orders/complete-due`.
- Current route classification: global, not cockpit-safe.
- Completion effect: create or upgrade the target `PlanetBuilding` to the order `TargetLevel`, then mark the order `Completed`.
- Resource rule: the construction cost was already spent when enqueue succeeded; completion must not spend resources again.
- Safe future `/construction` action must be scoped by the current `civilizationId` and `planetId`, process only due open orders for that owned planet, skip terminal rows, return backend-confirmed completed ids, and refresh `/api/dev/planets/ui-state` afterward.
- Ordinary page load, local-session continuation, and read-state refresh must not materialize construction orders.

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

### Reused-development QA preparation contract

- Default scoped IDs for QA prep:
  - `civilizationId = 00000000-0000-0000-0000-000000000001`
  - `planetId = 40000000-0000-0000-0000-000000000001`
- Construction blocking rows are scoped to that pair and only include queue rows with `Pending` or `Active` status.
- Preparation operation steps:
  - clears/neutralizes blocking open orders for the scoped target only
  - preserves completed/history rows on the same planet
  - preserves unrelated planets/civilizations and their rows
  - tops up stockpile to `Credits >= 1000`, `Metal >= 1000`, `Crystal >= 1000`, `Gas >= 1000` when stockpile exists
- This is a Development-only helper and does not run automatically.

Backend-only persisted QA helper:

- `.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed`
- Add `-BuildingType MetalMine` or another available building type to force the exact enqueue target.
- The helper talks directly to `POST /api/dev/buildings/construction-orders/enqueue`, then re-reads `/api/dev/planets/ui-state` to print queue and reserve deltas.

Frontend-confirmed persisted QA path:

- Open `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Choose an action already marked `Disponible`
- Review the guarded confirmation section and send the order explicitly
- This route also creates a real persisted Development database row; it is not a mock or optimistic-only UI flow
- The route still depends on the same backend refresh contract and must not invent queue rows or resource deltas locally

## Audited persisted enqueue contract

- Mutation route: `POST /api/dev/buildings/construction-orders/enqueue`
- Read route used for option selection and refresh: `GET /api/dev/planets/ui-state?civilizationId={id}&planetId={id}`
- Development exposure rule:
  - The route is mapped only when `Program.cs` enables Development endpoints.
  - Current gate: `environment.IsDevelopment()` or `VoidEmpires:DevEndpoints:Enabled=true`.
  - If persistence is not configured through `ConnectionStrings:DefaultConnection`, the route returns `503 Service Unavailable`.
- Request body fields:
  - `planetId: Guid`
  - `civilizationId: Guid`
  - `action: ConstructionQueueItemAction`
  - `buildingType: BuildingType`
  - `requestedAtUtc: DateTime` with `Utc` kind required
- Success response:
  - `201 Created`
  - body fields: `succeeded`, `orderId`, `startsAtUtc`, `endsAtUtc`, `errors`
- Request validation response:
  - `400 Bad Request` for missing ids, missing action, missing building type, missing `requestedAtUtc`, or non-UTC `requestedAtUtc`
- Service rejection response:
  - `409 Conflict` with backend error text in `errors`

## Safe UI boundary

- The cockpit should only submit actions already surfaced by `/api/dev/planets/ui-state` with `AvailabilityStatus = "Available"`.
- The cockpit should treat the planet UI-state response as the source of truth for `action`, `buildingType`, `targetLevel`, displayed cost, and duration.
- The cockpit must send backend-compatible enum values from the read model, not localized labels.
- The cockpit may allow explicit foreign-planet viewing, but it must not present enqueue as executable there.
- The cockpit should always refresh from `/api/dev/planets/ui-state` after a `201` instead of inserting an optimistic local queue row.
- The cockpit should keep `complete-due` unavailable here because the backend completion route is still global.
- Current confirmation audit:
  - `/construction` now uses the reusable `GameModal` confirmation component.
  - The guarded submit step still lives in the shared `PlanetPage` construction variant and stays read-only until the modal primary action runs.
  - The explicit acknowledgement checkbox remains required before submit.
- Current playable-session audit:
  - `/construction` is still the shared `PlanetPage` construction variant, so session context, the playable-session banner, and missing-id local-session continuation are owned by that shared route implementation.
  - Local playable-session memory only rebuilds navigation URLs; it does not change construction selection, modal opening, enqueue confirmation, resource spend, or backend-first refresh behavior.
  - Handoffs back to `Planeta` use the active `civilizationId` and `planetId` from the loaded construction context.

## Audited rejection and guardrail matrix

- Proven enqueue conflicts from the persisted POST:
  - `Planet is not owned by the requesting civilization.`
  - `Planet already has an open construction order.`
  - `Insufficient resources.`
  - `Planet building capacity would be exceeded.`
  - `Planet resource stockpile was not found.`
  - `Building was not found.` for an upgrade request whose building row does not exist
- Proven read-state guardrails from `/api/dev/planets/ui-state`:
  - Missing `civilizationId` returns `400 Bad Request`
  - Unknown `planetId` returns `404 Not Found` with `Planet was not found.`
  - Explicit foreign-planet selection stays readable but hides stockpile and management data, and the action summary stays blocked
  - Open-order and affordability blocking are already surfaced before submit through action availability and summary state
- Important contract note:
  - The current enqueue service does not emit a distinct `civilization was not found` or `planet was not found` conflict. Unknown ids currently collapse into request validation, read-state `404`, or the ownership conflict path.

## Immediate resource semantics

- Successful enqueue spends the full visible construction cost immediately from `PlanetResourceStockpile`.
- The persisted row is created immediately with `Status = Active`.
- This flow does not use a reservation-only placeholder or deferred spend model.

Verified refresh behavior:

- After a successful backend `201`, the cockpit re-reads `/api/dev/planets/ui-state` before leaving the user with the final visible queue and stockpile state.
- The main success copy remains grounded in backend confirmation: `La cabina se actualizo con el estado confirmado por la API.`
- If the backend accepts the order but the refreshed queue does not expose the new row yet, the cockpit says: `La orden fue aceptada por el backend; la cola visible se actualizara con la siguiente lectura disponible.`
- If that follow-up read fails, the cockpit keeps the success or failure technically honest and asks the user to refresh the view instead of inventing a local queue row.

## Minimal regression guards

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- Manual guard for `/construction`:
  - selecting an action must not submit anything by itself
  - sending the order still requires the explicit confirmation checkbox and button
  - post-submit queue or resource changes must come from the refreshed backend read, not from fabricated local state

## Runtime QA sequence

1. Start the backend:

```powershell
dotnet run --project .\src\VoidEmpires.Web
```

2. Reapply the shared baseline twice when you want a predictable reused-database starting point:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
```

3. Start the frontend:

```powershell
npm run dev --prefix .\src\VoidEmpires.Frontend
```

4. Choose one sanctioned runtime path:

- Backend-only fallback:

```powershell
.\scripts\dev-qa-prepare-construction-ui-state.ps1
.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed
```

- Frontend-confirmed path:
  - Open `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
  - Prepare one available action
  - Confirm the checkbox and send the order

5. Expected frontend runtime outcome:

- the confirmation step remains mandatory
- success copy stays tied to backend confirmation
- queue and resources refresh from the backend read model
- accepted-but-not-yet-visible state may show `La orden fue aceptada por el backend; la cola visible se actualizara con la siguiente lectura disponible.`

6. Reused database warning:

- repeated runs can leave a real open construction order in place
- reseeding helps restore read-model context but does not delete manual active orders
- when you need the exact pre-enqueue baseline again, switch to a fresh disposable local database or run the scoped preparation helper before re-testing

```powershell
.\scripts\dev-qa-prepare-construction-ui-state.ps1
```

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
