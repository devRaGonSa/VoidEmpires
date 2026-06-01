# TASK-9I

---

id: TASK-9I
title: Add frontend visual-state preview links
status: done
type: feature
team: frontend
supporting_teams:
  - backend
  - tests
  - docs
roadmap_item: "Phase 9I - Frontend visual-state preview links"
priority: high

---

## Goal

Add read-only frontend links/panels for inspecting system and planet visual-state dev endpoints.

This phase should connect the frontend visual map readiness work with the existing backend visual state contracts.

It must not add final 3D rendering, WebGL/Three.js, mutating gameplay, production auth, WebSockets, backend endpoints, or backend gameplay changes.

## Context

Backend visual state endpoints already exist:

* `GET /api/dev/solar-systems/{systemId}/visual-state`
* `GET /api/dev/planets/{planetId}/visual-state`

The frontend should allow a developer/user to:

* select a system and fetch its visual state preview
* select a visible planet and fetch its visual state preview
* inspect the payload in a readable way
* understand these are renderer-facing dev contracts, not final rendering

This helps prepare the later 2D/3D visualization layer.

## Implementation steps

1. Inspect existing API client and frontend strategic map detail panel.
2. Add TypeScript DTOs for only the visual-state fields needed by the preview.

   * tolerate extra fields
   * keep types lightweight
3. Add API client methods:

   * `getSystemVisualState(systemId: string)`
   * `getPlanetVisualState(planetId: string)`
4. Add read-only preview UI:

   * "Load system visual state" for selected system
   * "Load planet visual state" for selected planet when planet id exists
   * show loading/error/success states
   * show compact payload summary
   * optionally show raw JSON details in `<details>`
5. Make it clear:

   * this is dev-only
   * this is renderer-facing data
   * this is not final 3D rendering
6. Do not call mutating endpoints.
7. Do not add backend endpoints.
8. Update docs if needed.
9. Update `ai/current-state.md` to document Phase 9I.

## Files to read first

* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/components/StrategicMapSelectionPanel.tsx
* src/VoidEmpires.Frontend/src/styles.css
* docs/dev/visual-state-sandbox.md
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

* src/VoidEmpires.Frontend/src/api/visualStateTypes.ts
* src/VoidEmpires.Frontend/src/components/VisualStatePreviewPanel.tsx

## Acceptance criteria

* Frontend can fetch selected system visual state.
* Frontend can fetch selected planet visual state.
* Loading/error/success states exist.
* Visual-state preview is clearly read-only/dev-only.
* Raw or summarized payload is inspectable.
* No final 3D rendering is added.
* No mutating calls are wired.
* No backend changes are introduced.
* Backend validation remains green.
* Frontend build passes.
* `ai/current-state.md` documents Phase 9I.

## Constraints

* Do not add Three.js.
* Do not add WebGL.
* Do not add WebSockets.
* Do not add gameplay mutations.
* Do not call mutating endpoints.
* Do not add production auth.
* Do not add backend endpoints.
* Do not add backend gameplay changes.
* Keep preview simple and robust to missing fields.

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
   `feat(frontend): add visual-state preview links`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
