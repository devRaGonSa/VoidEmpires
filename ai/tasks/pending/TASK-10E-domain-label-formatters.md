# Phase 10E - Domain label formatters for seeded UI data

## Summary

Add frontend-safe formatting helpers for domain enums and identifier display used by the seeded development UI surfaces so Galaxia, Flotas, and PlanetVisualState are readable without backend changes.

## Goal

Create reusable frontend label and formatting helpers that convert technical seeded values into deterministic, conservative Spanish presentation labels.

## Scope

- Frontend-focused.
- Add reusable label and format helpers under `src/VoidEmpires.Frontend/src` in an appropriate folder such as `utils`, `domain`, or `presentation`.
- Cover at least:
  - `PlanetType` numeric values to Spanish labels.
  - `ColonizationStatus` numeric values to Spanish labels.
  - `SpaceAssetType` numeric values to Spanish labels.
  - `OrbitalGroupStatus` numeric values to Spanish labels.
  - `ResourceType` numeric values to Spanish labels.
  - `VisibilityLevel` numeric values to Spanish labels.
  - Common block, reason, and status values used in strategic-map and fleet command hints where practical.
- Add helper for compact GUID display:
  - show full GUID only in a secondary or dev details area;
  - otherwise show a short form such as the first 8 characters, or a known friendly name if resolved.
- Keep labels deterministic and conservative.
- Do not change backend contracts.
- Do not add new API endpoints.
- Do not add gameplay behavior.
- Add tests if frontend test tooling already exists; otherwise keep helpers simple and validate through the frontend build.

## Files to Read First

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts`
- fleet-related frontend pages/components already used by Flotas
- any existing frontend utility or formatting helpers under `src/VoidEmpires.Frontend/src`

## Expected Files to Modify

- one or more new or existing frontend helper files under `src/VoidEmpires.Frontend/src`
- only small frontend call sites if needed to validate helper integration

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

