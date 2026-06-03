# Strategic Map Cockpit Checklist

Galaxy cockpit v1 is accepted as a polished read-only development surface after the 13K-13P pass.

Historical note:

- The earlier 13E-13J block was technically valid and buildable.
- The later 13K-13P pass was still required to fix gameplay hierarchy, Spanish-first copy, diagnostics placement, and map readability.

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
