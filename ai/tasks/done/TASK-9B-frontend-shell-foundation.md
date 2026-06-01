# TASK-9B

---

id: TASK-9B
title: Add frontend shell foundation
status: done
type: feature
team: frontend
supporting_teams:
  - architecture
  - backend
  - tests
  - docs
roadmap_item: "Phase 9B - Frontend shell foundation"
priority: high

---

## Goal

Add a minimal frontend foundation/shell for VoidEmpires.

This phase should create the frontend project structure, app shell, routing foundation, API client foundation, and development configuration needed to consume existing dev-only backend endpoints.

This must not add final UI, production auth, gameplay mutation flows, WebSockets, complex 3D rendering, production endpoints, or backend gameplay changes.

## Context

Phase 9A created the pre-frontend contract checkpoint and documented stable dev/backend surfaces.

The frontend should now begin as a controlled prototype that consumes dev contracts:

* strategic map dev endpoint
* fleet UI state dev endpoint
* strategic map action manifest
* fleet action manifest
* visual state endpoints

The first frontend foundation should be conservative and easy to validate.

Preferred approach:

* Use a modern frontend setup inside the repo.
* If no frontend stack exists, create a simple Vite + React + TypeScript app under `src/VoidEmpires.Frontend` or another conventionally clear folder.
* Keep dependencies minimal.
* Keep styling simple.
* Do not introduce a complex UI framework unless already present.
* Do not use final production auth yet.

## Implementation steps

1. Inspect repo structure and docs:

   * `docs/dev/pre-frontend-contract-checkpoint.md`
   * `docs/dev/strategic-map-api-contract.md`
   * `docs/dev/fleet-api-contracts.md`
   * `docs/dev/visual-state-sandbox.md`
   * `AGENTS.md`
2. Add frontend project foundation:

   * package config
   * TypeScript config
   * Vite config if using Vite
   * basic app entrypoint
   * basic shell layout
   * basic route placeholders
3. Add frontend source structure, for example:

   * `src/VoidEmpires.Frontend/package.json`
   * `src/VoidEmpires.Frontend/src/main.tsx`
   * `src/VoidEmpires.Frontend/src/App.tsx`
   * `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
   * `src/VoidEmpires.Frontend/src/config.ts`
   * `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
   * `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
   * `src/VoidEmpires.Frontend/src/styles.css`
4. Add API configuration:

   * `VITE_VOIDEMPIRES_API_BASE_URL`
   * default to same-origin or `http://localhost:5000` only if practical
   * document the expected backend base URL
5. Add a basic shell:

   * title/header
   * navigation placeholders
   * environment/backend base URL display
   * clear badge that the UI consumes Development-only endpoints
6. Add basic frontend validation scripts:

   * `npm run build`
   * optional `npm run typecheck`
7. Do not wire gameplay mutations yet.
8. Update `ai/current-state.md` to document Phase 9B.

## Files to read first

* docs/dev/pre-frontend-contract-checkpoint.md
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* docs/dev/visual-state-sandbox.md
* ai/current-state.md
* AGENTS.md
* package.json if one exists
* README.md if frontend instructions already exist

## Expected files to modify

Expected depending on chosen frontend folder:

* src/VoidEmpires.Frontend/package.json
* src/VoidEmpires.Frontend/index.html
* src/VoidEmpires.Frontend/tsconfig.json
* src/VoidEmpires.Frontend/vite.config.ts
* src/VoidEmpires.Frontend/src/main.tsx
* src/VoidEmpires.Frontend/src/App.tsx
* src/VoidEmpires.Frontend/src/config.ts
* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
* src/VoidEmpires.Frontend/src/styles.css
* ai/current-state.md

May also modify:

* README.md or docs/dev/pre-frontend-contract-checkpoint.md only if useful to document how to run the frontend shell.

## Acceptance criteria

* Frontend project foundation exists.
* App shell renders locally.
* Basic routing/navigation placeholders exist.
* API base URL configuration exists.
* UI clearly indicates it consumes Development-only backend endpoints.
* No gameplay mutation flows are added.
* No production auth is added.
* Backend tests still pass.
* Frontend build/typecheck passes if dependencies are available.
* `ai/current-state.md` documents Phase 9B.

## Constraints

* Do not add production auth.
* Do not add production endpoints.
* Do not add backend gameplay changes.
* Do not add final UI design.
* Do not add WebSockets.
* Do not add complex 3D rendering.
* Do not add gameplay mutation flows.
* Keep dependencies minimal.
* Keep frontend code deterministic and simple.
* Do not remove existing backend docs or tests.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Also run frontend validation if the frontend package is created and npm is available:

```powershell
cd src/VoidEmpires.Frontend
npm install
npm run build
cd ../..
```

If npm is unavailable, document that clearly in the task completion notes, but still keep backend validation green.

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
   `feat(frontend): add shell foundation`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
