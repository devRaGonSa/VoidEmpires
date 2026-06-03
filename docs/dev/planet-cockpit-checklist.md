# Planet Cockpit Checklist

Planet cockpit v1 is the first playable 2D colony-management surface for the current development build.

## Acceptance boundary

- `/planet` is a real route.
- `planetId` is query-param driven.
- The page stays 2D and development-safe.
- Construction enqueue is allowed only through an explicit confirmation flow.
- Completing due constructions remains disabled in this build because the current backend endpoint is global rather than planet-scoped.

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Then confirm on `/planet`:

- The route loads with Spanish loading, error, and empty states.
- When `civilizationId` is present and `planetId` is omitted, the primary or first owned planet is selected.
- The page header prioritizes planet identity, system, ownership, type, and colony state.
- Resources and production are readable without leading with raw DTO names.
- Buildings are grouped into readable categories.
- The construction queue is readable and uses `No hay construcciones en cola.` when empty.
- Construction candidates show readable availability states such as available, blocked, insufficient resources, or missing capacity data.
- The enqueue flow requires explicit confirmation and refreshes the cockpit after success.
- The complete-due area stays visibly disabled and reads `No disponible en esta build`.
- Links back to Galaxy and toward Fleets preserve context without introducing shared global state.
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
