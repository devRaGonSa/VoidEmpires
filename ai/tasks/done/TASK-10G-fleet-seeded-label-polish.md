# Phase 10G - Fleet seeded label polish

## Summary

Use the frontend formatting helpers to make seeded Flotas data readable, especially fleet card labels, planet references, and resource balances.

## Goal

Polish the Fleet seeded UI so groups, transfers, and resource context display readable Spanish labels instead of raw enum numbers and full GUIDs.

## Scope

- Frontend Flotas / Fleet page only.
- Replace numeric or raw labels where possible:
  - `assetType` to label.
  - group status to label.
  - `resourceType` to label.
  - command availability to readable Spanish labels.
- Use known map or seed data where available to display planet names instead of full planet GUIDs:
  - `Aurelia` for `40000000-0000-0000-0000-000000000001`
  - `Cinder Reach` for `40000000-0000-0000-0000-000000000002`
  - `Aether Crown` for `40000000-0000-0000-0000-000000000003`
- If the fleet payload does not include planet names, implement a small frontend lookup for deterministic seed planet IDs only as a development presentation fallback, or show compact IDs while documenting the limitation.
- Improve fleet group cards:
  - group type or name from asset type;
  - quantity;
  - current planet label;
  - origin planet label;
  - status label;
  - active transfer summary if present.
- Resource context should show resource names rather than `1/2/3/4`.
- Keep action manifest read-only/mutation separation from previous phases.
- Do not add real command execution controls.
- Do not add backend behavior unless a tiny DTO addition is clearly safer; prefer frontend-only.

## Files to Read First

- fleet-related frontend page and components already used by Flotas
- fleet API type definitions under `src/VoidEmpires.Frontend/src`
- helpers created by `TASK-10E`
- any existing strategic map planet-name resolution reused by the frontend

## Expected Files to Modify

- Fleet frontend page and related components only
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

- Seeded Flotas shows 4 groups, 1 transfer, and resources with readable labels.
- No prominent raw `resourceType` or `assetType` numbers.
- No `NetworkError`.

