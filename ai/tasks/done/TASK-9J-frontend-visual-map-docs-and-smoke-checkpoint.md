# TASK-9J

---

id: TASK-9J
title: Add frontend visual map docs and smoke checkpoint
status: done
type: hardening
team: frontend
supporting_teams:
  - backend
  - docs
  - tests
roadmap_item: "Phase 9J - Frontend visual map docs and smoke checkpoint"
priority: high

---

## Goal

Add documentation and smoke validation checkpoint for the frontend visual map readiness slice.

This task should confirm that the frontend 2D map, selection details, and visual-state preview links are buildable, documented, and explicitly constrained as dev/readiness UI.

It must not add new gameplay behavior, production auth, backend endpoints, WebSockets, final 3D rendering, or complex UI refactors.

## Context

The frontend now has:

* shell/routing/API client
* strategic map read slice
* fleet UI state read slice
* action manifest read-only panels
* 2D strategic map visualization
* selection/detail panel
* visual-state preview links

This checkpoint should make it easy to validate the current frontend work before later phases add more UI polish or rendering sophistication.

## Implementation steps

1. Update frontend README:

   * describe 2D map view
   * describe selection/detail behavior
   * describe visual-state preview links
   * list required dev endpoints
   * document that no mutating gameplay calls are wired
2. Update or add smoke checklist:

   * frontend build passes
   * strategic map page loads
   * civilization id can be entered
   * systems render in 2D map
   * system selection updates detail panel
   * visual-state preview links load when backend is running
   * fleet/manifests page still loads
3. Update `docs/dev/pre-frontend-contract-checkpoint.md`:

   * mark visual map readiness slice as present
   * document current non-goals
4. Add small code/doc hardening if necessary:

   * empty states
   * clearer prototype warning labels
   * no mutating buttons
5. Do not add new backend code unless fixing a tiny docs mismatch.
6. Update `ai/current-state.md` to document Phase 9J and final baseline.

## Files to read first

* src/VoidEmpires.Frontend/README.md
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
* src/VoidEmpires.Frontend/src/App.tsx
* docs/dev/pre-frontend-contract-checkpoint.md
* docs/dev/visual-state-sandbox.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Frontend/README.md
* docs/dev/pre-frontend-contract-checkpoint.md
* ai/current-state.md

May also modify:

* docs/dev/frontend-foundation-smoke-checklist.md
* src/VoidEmpires.Frontend/src/App.tsx
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

* Frontend visual map readiness is documented.
* Smoke checklist covers 2D map, selection panel, visual-state preview, fleet page, and manifests.
* Docs clearly state this is dev/readiness UI.
* Docs clearly state there are no mutating gameplay calls.
* Backend validation remains green.
* Frontend build passes.
* `ai/current-state.md` documents Phase 9J and final baseline.

## Constraints

* Prefer docs/checklist/hardening only.
* Do not add gameplay mutations.
* Do not call mutating endpoints.
* Do not add production auth.
* Do not add backend endpoints.
* Do not add backend gameplay changes.
* Do not add WebSockets.
* Do not add Three.js/WebGL.
* Do not add final visual design.
* Keep docs practical.

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
3. Verify changed files are expected frontend/docs/current-state unless small fixes were necessary.
4. Commit with a clear message, for example:
   `docs(frontend): add visual map smoke checkpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
