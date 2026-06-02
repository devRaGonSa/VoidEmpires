# Phase 10F - Strategic map seeded label polish

## Summary

Use the frontend formatting helpers to make seeded Galaxia data readable and presentation-ready without changing backend contracts or rendering behavior.

## Goal

Polish the Strategic Map seeded UI so real seeded systems and planets display readable Spanish labels instead of raw enum numbers and prominent technical identifiers.

## Scope

- Frontend Galaxia / Strategic Map page only.
- Replace numeric or raw labels where possible:
  - `planetType` to label.
  - `colonizationStatus` to label.
  - `starType` to label if currently visible.
  - `visibilityLevel` and `visibilityReason` to label.
  - command block reasons to label or readable note.
- Make planet selector and planet cards show:
  - planet name prominently;
  - type label;
  - ownership or visibility label;
  - colonization label;
  - short ID only as dev metadata.
- Make selected system panel show:
  - system name;
  - coordinates;
  - planet count;
  - fleet marker count;
  - transfer overlay count.
- Preserve existing Figma-aligned styling.
- Do not add 3D map rendering.
- Do not add Three.js/WebGL.
- Do not change backend.
- Do not execute gameplay mutations.

## Files to Read First

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSystemCard.tsx`
- `src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts`
- helpers created by `TASK-10E`

## Expected Files to Modify

- Strategic Map frontend page and related components only
- shared frontend formatting helpers from `TASK-10E` if a small extension is needed

## Constraints

- Do not commit secrets.
- Do not hardcode database passwords, local private IPs, or real connection strings.
- Do not touch PostgreSQL.
- Do not apply EF migrations.
- Do not add gameplay mutations.
- Do not add WebSockets.
- Do not add Three.js/WebGL.
- Do not introduce production authentication.
- Keep dev/prototype boundaries clear.
- Keep changes incremental and reviewable.
- Preserve the current successful validation baseline.

## Validation

Run:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Manual Validation

- Seeded Galaxia shows `Helios Gate`, `Aurelia`, `Cinder Reach`, and `Aether Crown` with readable labels.
- No prominent raw enum numbers for planet type or colonization status.
- No `NetworkError`.

