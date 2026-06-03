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

Use the current `research-validation` seed for richer deterministic Research checks:

```powershell
POST /api/dev/seeds/apply
{"profile":"research-validation"}
```

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Seeded parent system: `Helios Gate`
- Seeded QA URL: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Expected summary baseline before enqueue:
  - `Disponibles: 1`
  - `Bloqueadas: 7`
  - `Cola: 1`
  - `En espera de cierre: 0`
  - `Proyectos: 1`
- Expected comparison baseline:
  - `Ingenieria planetaria` is immediately available.
  - `Extraccion de recursos`, `Sistemas energeticos`, and other higher-cost technologies remain blocked by `Recursos insuficientes en Aurelia.`
  - The queue shows one completed `Sistemas energeticos` history row and no active order.
- Current backend limitation:
  - The readiness evaluator only distinguishes `Available`, `InResearch`, `InsufficientResources`, and missing-planet fallback states.
  - This seed does not invent hidden prerequisites or new blocker mechanics just to diversify labels.
- Audit note:
  - If `/research` shows `Disponibles: 0` after reapplying `research-validation`, the primary root cause is usually an existing active research order, not the research UI-state mapper.
  - The profile intentionally resets the owned-planet stockpile to `125` credits, `110` metal, `70` crystal, and `30` gas so `Ingenieria planetaria` remains the deterministic available item while higher-cost research stays blocked.
  - Reapplying the seed preserves completed history but does not delete an already enqueued active research order. For an exact pre-enqueue baseline, use a fresh disposable local database before applying the seed.

## Browser checkpoints

- Checkpoint 1: available card before enqueue
  - `Ingenieria planetaria` is visible as the seeded available card.
  - `Disponibles` is at least `1`.
- Checkpoint 2: confirmation panel before submit
  - The guarded confirmation panel is open for the selected available item.
  - `Confirmar` is still blocked until the acknowledgement checkbox is checked.
- Checkpoint 3: queue after one successful enqueue
  - The queue shows one active research row with Spanish technology label, status, and close time.
  - The primary feedback reads `Investigacion enviada a la cola.`
- Checkpoint 4: blocked comparison card
  - `Extraccion de recursos` remains blocked with readable Spanish guidance, and multiple higher-cost cards remain blocked for the same truthful resource reason.
- Checkpoint 5: complete-due placeholder
  - `Completar vencidas no disponible` stays visibly disabled with its explanation text.

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

- The deterministic seeded scenario opens from the exact QA URL above after applying `research-validation`.
- The active context corresponds to `Aurelia` in the seeded `Helios Gate` system.
- The summary shows `Disponibles >= 1` and `Bloqueadas >= 1` before any enqueue.
- The route loads with Spanish loading, error, and empty states.
- The first viewport prioritizes cabina context, resumen, cola y progreso, and the catalog.
- The catalog shows Spanish technology names and meaningful category groupings.
- At least one available research card and multiple blocked cards are visible together for comparison.
- Blocked research shows a readable reason rather than raw backend text.
- `Requisito pendiente de clasificar` does not appear in the main seeded blocker text.
- Requirement chips, cost, duration, and next-level data wrap cleanly without horizontal overflow.
- Preparing an available item opens the guarded confirmation panel before any mutation happens.
- The confirmation panel shows civilizacion, planeta, tecnologia, categoria, coste, duracion, and `Requisitos: Listos para envio`.
- `Confirmar` and `Cancelar` appear before any enqueue mutation.
- Sending the confirmed order refreshes the queue and updates the catalog from the read model instead of adding optimistic local entries.
- After one successful enqueue, `Ingenieria planetaria` no longer appears as immediately available and the queue shows one active order.
- Any generic validation rejection during this guarded enqueue flow is a QA failure, not an acceptable fallback state.
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

## Block closure gate

Close the current Research enqueue block only after all of the following are true:

- `/research` loads from the seeded QA URL and shows `Disponibles >= 1` before enqueue.
- The seeded available item opens confirmation and the guarded submit succeeds.
- The queue refreshes from backend state after success and the enqueued technology no longer appears ready.
- Blocked research remains non-mutating and complete-due remains visibly disabled.
- Diagnostics stay collapsed, and no 3D, WebGL, combat, interception, or espionage execution appears in this cockpit.

## Intentional exclusions

- No 3D renderer.
- No complete-due execution from this cockpit.
- No combat, espionage, or alliance gameplay.
- No production authentication changes.
- No hidden technology effects beyond the current backend-supported queue and completion state.
