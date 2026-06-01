# TASK-9G

---

id: TASK-9G
title: Add frontend strategic map 2D view
status: pending
type: feature
team: frontend
supporting_teams:
  - backend
  - architecture
  - tests
  - docs
roadmap_item: "Phase 9G - Frontend strategic map 2D view"
priority: high

---

## Goal

Add a simple read-only 2D strategic map visualization in the frontend.

This phase should render strategic map systems using backend coordinates from the existing Development-only strategic map endpoint.

It must not add final 3D rendering, Three.js, WebSockets, gameplay mutations, production auth, backend endpoints, or backend gameplay changes.

## Context

The frontend currently has a strategic map read slice that shows map data as panels/lists.

The next step is a minimal visual map:

* use existing strategic map result
* render systems in a 2D plane
* show visibility state/readiness state as simple visual markers
* keep all interactions read-only
* keep design simple and deterministic

This is not the final map UI. It is a visual readiness layer.

Suggested approach:

* SVG-based map or absolutely positioned div grid.
* Prefer SVG for simplicity and deterministic rendering.
* Normalize backend coordinates into viewport coordinates.
* Show system name and visibility status.
* Use simple CSS classes, not canvas/Three.js yet.

## Implementation steps

1. Inspect existing frontend files:

   * `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
   * `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
   * existing strategic map DTO/types
   * `src/VoidEmpires.Frontend/src/styles.css`
2. Add a visual component, for example:

   * `StrategicMap2DView`
   * `StrategicMapSystemNode`
3. Use strategic map system coordinates:

   * `coordinateX`
   * `coordinateY`
   * optionally ignore `coordinateZ` or show it in tooltip/details
4. Normalize coordinates safely:

   * handle empty system list
   * handle one-system maps
   * avoid divide-by-zero
   * add padding/margins
5. Render:

   * each system as a node
   * system name
   * visibility level/status
   * simple counts: planets, fleets, transfers
6. Keep existing list/panel view available below or beside the visual map.
7. Add basic responsive styling.
8. Do not add mutating buttons.
9. Update `ai/current-state.md` to document Phase 9G.

## Files to read first

* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/styles.css
* src/VoidEmpires.Frontend/README.md
* docs/dev/pre-frontend-contract-checkpoint.md
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/styles.css
* ai/current-state.md

May add:

* src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx
* src/VoidEmpires.Frontend/src/components/StrategicMapSystemNode.tsx

## Acceptance criteria

* Frontend renders a read-only 2D strategic map view.
* Systems use backend coordinates.
* Empty and one-system states are handled safely.
* System visibility/status is visible.
* Existing strategic map read slice remains available.
* No gameplay mutation controls are added.
* No backend changes are introduced.
* Backend validation remains green.
* Frontend build passes.
* `ai/current-state.md` documents Phase 9G.

## Constraints

* Do not add Three.js.
* Do not add WebSockets.
* Do not add production auth.
* Do not add backend endpoints.
* Do not add backend gameplay changes.
* Do not add mutating frontend calls.
* Do not add final visual design.
* Keep the visualization simple, deterministic, and read-only.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Also run frontend validation:

```powershell
cd src/VoidEmpires.Frontend
npm install
npm run build
cd ../..
```

Expected result:

* backend clean build
* backend tests passing
* frontend build passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected frontend/docs/current-state files changed.
4. Commit with a clear message, for example:
   `feat(frontend): add strategic map 2d view`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

