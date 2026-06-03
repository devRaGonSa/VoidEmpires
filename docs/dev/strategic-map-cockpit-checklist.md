# Strategic Map Cockpit Checklist

Galaxy cockpit v1 is accepted as a polished read-only development surface after the 13K-13P pass.

Historical note:

- The earlier 13E-13J block was technically valid and buildable.
- The later 13K-13P pass was still required to fix gameplay hierarchy, Spanish-first copy, diagnostics placement, and map readability.

## Regression audit - 2026-06-03

Confirmed root cause for the reported "empty Galaxy screen" after the `cockpit-validation` seed:

- The current frontend only mounts `StrategicMapPage` on `/`, not on `/galaxy`.
- `src/VoidEmpires.Frontend/src/App.tsx` registers Galaxy at `Route path="/"`, and there is no `/galaxy` alias or fallback route.
- `src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx` also treats only `location.pathname === "/"` as the strategic-map route, so `/galaxy` falls back to the generic shell framing.
- When a user opens `/galaxy`, the shared shell still renders, but no child route matches, so the accepted strategic cockpit body never mounts. This matches the "shell only" regression report more closely than a backend data failure.

Related but separate behavior on `/`:

- `StrategicMapPage` reads `civilizationId`, `systemId`, and `planetId` from the query string.
- If `civilizationId` is missing, the load effect returns early and no strategic-map request is sent.
- In that no-context case, the hero, query form, summary placeholder, and read-only rules render, but every main cockpit section behind `result && ...` stays hidden.
- This means `/` without query context is not the same failure as `/galaxy`: it is a route match with no visible loaded cockpit, not a no-match shell.
- API request failures are not swallowed in the audited path. `getStrategicMap(...)` is wrapped in `try/catch`, unsuccessful responses set `error`, and the query card renders that error text.
- No explicit missing-context banner exists yet when `civilizationId` is absent. A later UX task should surface that state more directly near the query form.

Seeded strategic data status:

- Existing automated coverage still shows the strategic read model is populated for the seeded civilization `00000000-0000-0000-0000-000000000001`.
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
  - `StrategicMapServiceReturnsNonEmptySystemsFromSeededDataset`
  - `CockpitValidationSeedKeepsStrategicMapFleetContextVisible`
- `docs/dev/development-seed-profiles.md` and `src/VoidEmpires.Application/Development/IDevelopmentSeedService.cs` still advertise the Galaxy QA URL on `/`, not `/galaxy`.

Reproduction notes used for this audit:

- Code inspection traced the full path from `App.tsx` route matching through `StrategicMapPage` query parsing, API loading, and `result`-gated render branches.
- A local Vite frontend server was started successfully for spot-checking, but the Browser runtime was unavailable in this session (`Browser is not available: iab`), so the final confirmation is code-and-test backed rather than screenshot-backed.

## Acceptance boundary

- Galaxy remains read-only.
- No Galaxy mutation buttons are enabled.
- No 3D renderer, WebGL, Three.js, Babylon.js, or similar runtime is introduced.
- Technical diagnostics stay collapsed or clearly secondary.

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Then confirm on `/`:

- The first viewport reads like a playable strategic cockpit, not a raw dev panel.
- Spanish-first copy dominates the primary flow.
- The 2D map remains the dominant surface.
- System focus prioritizes name, coordinates, control or visibility, star type, and counts.
- Planet focus prioritizes name, type, ownership or control, and colonization state.
- Fleet markers and transfer overlays are visually distinguishable.
- Galaxy quick links can open `/planet?civilizationId=...&planetId=...`.
- Quick links toward `/fleets` preserve civilization or planet context.
- Full ids, raw capability keys, request payloads, and renderer payloads remain secondary.
- The technical drawer starts collapsed.

## Known non-goals

- No Galaxy mutations.
- No combat or interception execution.
- No espionage gameplay.
- No alliances gameplay UI.
- No WebSockets.
- No production authentication.
