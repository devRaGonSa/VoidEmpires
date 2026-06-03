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

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Seeded parent system: `Helios Gate`
- Seeded QA URL: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Expected comparison baseline:
  - `Ingenieria planetaria` is immediately available.
  - `Extraccion de recursos` remains blocked by `Recursos insuficientes`.
  - The queue starts empty and should read `No hay ordenes activas en la cola.`

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
- The route loads with Spanish loading, error, and empty states.
- The first viewport prioritizes cabina context, resumen, cola y progreso, and the catalog.
- The catalog shows Spanish technology names and meaningful category groupings.
- At least one available research card and at least one blocked card are visible together for comparison.
- Blocked research shows a readable reason rather than raw backend text.
- Requirement chips, cost, duration, and next-level data wrap cleanly without horizontal overflow.
- Preparing an available item opens the guarded confirmation panel before any mutation happens.
- Sending the confirmed order refreshes the queue and updates the catalog from the read model instead of adding optimistic local entries.
- The success state can show order details returned by the API without exposing raw payloads in the main cockpit.
- `Completar investigaciones vencidas` stays disabled with a clear placeholder because the current backend route is not scoped safely to this cabin.
- The diagnostics drawer starts collapsed and keeps technical details secondary.
- Links toward `Planeta`, `Construccion`, `Flotas`, and `Galaxia` preserve `civilizationId` and `planetId`.
- No 3D, WebGL, production auth, combat, fleet effects, or espionage execution is introduced from this route.

## Intentional exclusions

- No 3D renderer.
- No complete-due execution from this cockpit.
- No combat, espionage, or alliance gameplay.
- No production authentication changes.
- No hidden technology effects beyond the current backend-supported queue and completion state.
