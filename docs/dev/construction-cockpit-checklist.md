# Construction Cockpit Checklist

Construction cockpit v1 is the focused route for the current guarded planet construction flow.

## Acceptance boundary

- `/construction` is a real route.
- The route stays development-safe and 2D-only.
- Only `enqueue construction` is executable from this cockpit.
- Queue completion remains disabled here because the backend complete-due endpoint is still global.
- Diagnostics stay collapsed unless explicitly opened.

## Seeded QA scenario

Use the current `minimal-validation` seed for deterministic Construction checks:

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Expected comparison baseline:
  - Existing visible buildings include `Centro de mando` and `Distrito habitacional`.
  - At least one action is immediately available in the catalog.
  - At least one action is blocked by `Recursos insuficientes`.
  - The queue starts empty and should read `Sin cola` or `No hay construcciones en cola.`

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Then confirm on `/construction`:

- The deterministic seeded scenario can open as `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- The first viewport prioritizes active planet context, reserves, local economy, queue state, and the construction catalog.
- The catalog shows readable building names, readable categories, readable availability badges, and next-step language in Spanish.
- Blocked cards do not dominate the screen and their disabled buttons remain visually secondary.
- The current seeded `Aurelia` state shows both available and blocked actions for direct comparison.
- Preparing an available action opens the guarded confirmation panel with planet, building, action, level, cost, and duration.
- Sending the confirmed order refreshes the queue and surfaces a readable success message.
- Command failures show Spanish guidance in the main panel while technical details remain secondary inside diagnostics.
- The queue remains readable after refresh, including building name, action, level, timing, and cost.
- Links toward `Planeta`, `Galaxia`, and `Flotas` preserve the current civilization and planet context.
- The diagnostics drawer starts collapsed.

## Intentional exclusions

- No 3D renderer.
- No complete-due execution from this cockpit.
- No combat, espionage, or alliance gameplay.
- No production authentication changes.
