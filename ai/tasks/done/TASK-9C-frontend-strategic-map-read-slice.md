# TASK-9C

---

id: TASK-9C
title: Add frontend strategic map read slice
status: done
type: feature
team: frontend
supporting_teams:
  - backend
  - tests
  - docs
roadmap_item: "Phase 9C - Frontend strategic map read slice"
priority: high

---

## Goal

Add the first frontend strategic map read slice consuming the existing Development-only strategic map endpoint.

This phase should display a simple, readable strategic map data view. It must not implement final UI, real 3D map rendering, gameplay commands, production auth, or backend changes.

## Context

The backend exposes:

* `GET /api/dev/strategic-map?civilizationId={id}`
* strategic map action manifest
* strategic map payload including systems, planets, visibility, fleet presence, transfer overlays, sensor/detection/interception metadata, diplomacy/alliance/pact readiness metadata

The frontend should now consume the strategic map read endpoint and show a basic data-driven page.

This is a prototype/read slice, not final gameplay UI.

## Implementation steps

1. Inspect frontend shell from Phase 9B.
2. Add TypeScript DTOs for strategic map response:

   * only model the fields currently needed by the UI
   * tolerate extra backend fields
3. Add API client method:

   * `getStrategicMap(civilizationId: string)`
4. Add frontend state:

   * civilization id input
   * loading state
   * error state
   * successful result state
5. Add simple strategic map display:

   * list/count of systems
   * per-system name, visibility, coordinates
   * planet count
   * fleet presence count
   * transfer overlay count
   * readiness badges/sections for sensors, detection, interception, diplomacy/alliance/pacts if present
6. Add a clear note:

   * endpoint is dev-only
   * data is read-only
   * readiness metadata is not gameplay authorization
7. Do not add command buttons that mutate gameplay.
8. Add lightweight component tests only if the frontend test framework is introduced; otherwise keep to build/typecheck validation.
9. Update docs if needed.
10. Update `ai/current-state.md` to document Phase 9C.

## Files to read first

* src/VoidEmpires.Frontend/src/App.tsx
* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/config.ts
* docs/dev/strategic-map-api-contract.md
* docs/dev/pre-frontend-contract-checkpoint.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/styles.css
* docs/dev/pre-frontend-contract-checkpoint.md if frontend usage docs need updating
* ai/current-state.md

May add:

* src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts
* src/VoidEmpires.Frontend/src/components/StatusBadge.tsx
* src/VoidEmpires.Frontend/src/components/StrategicMapSystemCard.tsx

## Acceptance criteria

* Frontend can call strategic map dev endpoint.
* User can enter or configure a civilization id.
* Page shows loading/error/success states.
* Page displays systems/planets/fleet/transfer summary data.
* Page surfaces readiness metadata conservatively.
* No gameplay mutation commands are added.
* No final 3D map is added.
* Backend validation remains green.
* Frontend build/typecheck passes where environment supports it.
* `ai/current-state.md` documents Phase 9C.

## Constraints

* Do not add gameplay mutations.
* Do not add production auth.
* Do not add backend endpoints.
* Do not add backend gameplay changes.
* Do not add final UI/visual design.
* Do not add 3D rendering yet.
* Do not treat readiness metadata as authorization.
* Keep UI simple and robust to missing data.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Also run frontend validation if npm is available:

```powershell
cd src/VoidEmpires.Frontend
npm install
npm run build
cd ../..
```

Expected result:

* backend clean build
* backend tests passing
* frontend build passing where environment supports it

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `feat(frontend): add strategic map read slice`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
