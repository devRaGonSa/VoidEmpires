# Research Cockpit Checklist

Research cockpit v1 is the current development-safe route for browsing technology readiness, queue state, and guarded enqueue actions.
Use `docs/dev/planet-module-boundaries.md` to keep `/research` separate from the planet dashboard, construction flow, and unsupported gameplay systems.

## Acceptance boundary

- `/research` is a real route.
- The route stays development-safe and 2D-only.
- `enqueue research` is allowed only through an explicit confirmation flow when the development endpoint is available.
- Completing due research remains disabled here because the current backend complete-due endpoint is global rather than cockpit-scoped.
- Diagnostics stay collapsed unless explicitly opened.

## Seeded QA scenario

Use the current `minimal-validation` seed for deterministic Research checks:

```powershell
POST /api/dev/seeds/apply
{"profile":"minimal-validation"}
```

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Seeded parent system: `Helios Gate`
- Seeded QA URL: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Expected summary baseline before enqueue:
  - `Disponibles: 1`
  - `Bloqueadas: 7`
  - `Cola: 0`
  - `En espera de cierre: 0`
  - `Proyectos: 0`
- Expected comparison baseline:
  - `Ingenieria planetaria` is immediately available.
  - `Extraccion de recursos` remains blocked by `Recursos insuficientes en Aurelia.`
  - The queue starts empty and should read `No hay ordenes activas en la cola.`
- Audit note:
  - If `/research` shows `Disponibles: 0` after reapplying `minimal-validation`, the primary root cause is stale persisted stockpile state, not the research UI-state mapper. Earlier seed behavior only created the planet stockpile when missing, so consumed resources could survive reseeding and make all cards appear blocked.
  - The development seed now restores the owned-planet stockpile to at least the research QA baseline (`125` credits, `100` metal, `50` crystal, `20` gas) so `Ingenieria planetaria` remains the deterministic available item for the baseline scenario.
  - Reapplying the seed restores the owned-planet stockpile baseline, but it does not delete an already enqueued research order or rebuild the queue to an empty state. For an exact pre-enqueue baseline, use a fresh disposable local database before applying the seed.

## Enqueue contract note

The current dev enqueue route is `POST /api/dev/research/orders/enqueue`.
The frontend-safe request body is:

```json
{
  "civilizationId": "00000000-0000-0000-0000-000000000001",
  "sourcePlanetId": "40000000-0000-0000-0000-000000000001",
  "researchType": "PlanetaryEngineering",
  "requestedAtUtc": "2026-01-01T12:00:00Z"
}
```

- `civilizationId` is required and must be a non-empty GUID.
- `sourcePlanetId` is required and must be a non-empty GUID owned by the civilization.
- `researchType` must use the stable backend enum key such as `PlanetaryEngineering`, not the Spanish label `Ingenieria planetaria`.
- `requestedAtUtc` is required and must be a UTC timestamp.
- `targetLevel`, `planetId`, capability ids, and action metadata are not part of the current backend enqueue contract.
- Root cause from the enqueue audit:
  - The cockpit already sent the correct route and core fields.
  - The validation failure came from enum-string binding: the frontend posted `researchType` as the stable string key, but the web host had not enabled JSON string enum deserialization for the POST contract.

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Then confirm on `/research`:

- The deterministic seeded scenario opens from the exact QA URL above.
- The active context corresponds to `Aurelia` in the seeded `Helios Gate` system.
- The summary shows `Disponibles >= 1` and `Bloqueadas >= 1` before any enqueue.
- The route loads with Spanish loading, error, and empty states.
- The first viewport prioritizes cabina context, resumen, cola y progreso, and the catalog.
- The catalog shows Spanish technology names and meaningful category groupings.
- At least one available research card and at least one blocked card are visible together for comparison.
- Blocked research shows a readable reason rather than raw backend text.
- `Requisito pendiente de clasificar` does not appear in the main seeded blocker text.
- Requirement chips, cost, duration, and next-level data wrap cleanly without horizontal overflow.
- Preparing an available item opens the guarded confirmation panel before any mutation happens.
- The confirmation panel shows civilizacion, planeta, tecnologia, categoria, coste, duracion, and `Requisitos: Listos para envio`.
- `Confirmar` and `Cancelar` appear before any enqueue mutation.
- Sending the confirmed order refreshes the queue and updates the catalog from the read model instead of adding optimistic local entries.
- After one successful enqueue, `Ingenieria planetaria` no longer appears as immediately available and the queue shows one active order.
- The success state can show order details returned by the API without exposing raw payloads in the main cockpit.
- `Completar vencidas no disponible` stays disabled with a clear placeholder because the current backend route is not scoped safely to this cabin.
- The diagnostics drawer starts collapsed and keeps technical details secondary.
- Links toward `Planeta`, `Construccion`, `Flotas`, and `Galaxia` preserve `civilizationId` and `planetId`.
- No 3D, WebGL, production auth, combat, fleet effects, or espionage execution is introduced from this route.

## Closure checklist

- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.
- `/research` loads from the seeded QA URL above.
- The summary shows available `>= 1` before enqueue.
- The summary shows blocked `>= 1`.
- An available card shows `Revisar investigacion`.
- A blocked card cannot mutate and keeps a visually secondary button.
- `Requisito pendiente de clasificar` does not appear in primary seeded blocker text.
- `Completar vencidas no disponible` stays disabled unless a safer cockpit-scoped path is implemented later.
- Confirmation appears before enqueue.
- After exactly one confirmed enqueue, the queue updates and the reviewed technology changes away from the ready state.
- Diagnostics remain collapsed by default.

## Intentional exclusions

- No 3D renderer.
- No complete-due execution from this cockpit.
- No combat, espionage, or alliance gameplay.
- No production authentication changes.
- No hidden technology effects beyond the current backend-supported queue and completion state.
