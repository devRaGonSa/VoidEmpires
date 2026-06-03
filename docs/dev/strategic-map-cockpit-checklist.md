# Strategic Map Cockpit Checklist

Galaxy cockpit v1 is accepted as a polished read-only development surface after the 13K-13P pass.

Historical note:

- The earlier 13E-13J block was technically valid and buildable.
- The later 13K-13P pass was still required to fix gameplay hierarchy, Spanish-first copy, diagnostics placement, and map readability.

## Regression audit - 2026-06-03

Resolved root cause for the reported "empty Galaxy screen" after the `cockpit-validation` seed:

- `StrategicMapPage` now mounts on both `/galaxy` and `/`.
- `/galaxy` is the canonical route and `/` remains a compatibility alias.
- The shell now recognizes both paths as Galaxy and keeps the `Galaxia` navigation state highlighted.
- Missing, invalid, API-error, and empty-read-model states now render explicit panels instead of degrading into a shell-only or near-empty experience.

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
- `docs/dev/development-seed-profiles.md` now advertises the canonical Galaxy QA URL on `/galaxy`.

Reproduction notes used for this audit:

- Code inspection traced the full path from `App.tsx` route matching through `StrategicMapPage` query parsing, API loading, and `result`-gated render branches.
- A local Vite frontend server was started successfully for spot-checking, but the Browser runtime was unavailable in this session (`Browser is not available: iab`), so the final confirmation is code-and-test backed rather than screenshot-backed.

## Acceptance boundary

- Galaxy remains read-only.
- No Galaxy mutation buttons are enabled.
- Galaxy may expose readiness, route, and command metadata, but never direct
  mutation execution from the strategic cockpit itself.
- Any actionable handoff from Galaxy must navigate into the owning cockpit with
  preserved context instead of calling a mutating API directly.
- No 3D renderer, WebGL, Three.js, Babylon.js, or similar runtime is introduced.
- Technical diagnostics stay collapsed or clearly secondary.

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Then apply the required seed profile and open the canonical route:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5142/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"cockpit-validation"}'
```

- Canonical QA URL:
  `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Compatibility alias:
  `/?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`

Then confirm:

- The first viewport reads like a playable strategic cockpit, not a raw dev panel or a generic shell placeholder.
- The page does not stop after the shared shell header and intro cards.
- Spanish-first copy dominates the primary flow.
- The 2D map remains the dominant surface.
- The seeded `Helios Gate` system is visible and focusable from the map or system chip rail.
- System focus prioritizes name, coordinates, control or visibility, star type, and counts.
- Planet focus prioritizes name, type, ownership or control, and colonization state.
- The seeded `Aurelia`, `Cinder Reach`, and `Aether Crown` planets are visible in the current cockpit context.
- Fleet markers and transfer overlays are visually distinguishable.
- The legend, focus system panel, planet intelligence list, and transfer summary all render together.
- Galaxy quick links can open `/planet?civilizationId=...&planetId=...`.
- Quick links toward `/fleets` preserve civilization or planet context.
- The read-only boundary remains visible and Galaxy never exposes direct mutation execution.
- Full ids, raw capability keys, request payloads, and renderer payloads remain secondary.
- The technical drawer starts collapsed.

## Known non-goals

- No Galaxy mutations.
- No combat or interception execution.
- No espionage gameplay.
- No alliances gameplay UI.
- No WebSockets.
- No production authentication.
