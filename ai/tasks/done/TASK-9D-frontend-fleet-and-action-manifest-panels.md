# TASK-9D

---

id: TASK-9D
title: Add frontend fleet and action manifest panels
status: done
type: feature
team: frontend
supporting_teams:
  - backend
  - tests
  - docs
roadmap_item: "Phase 9D - Frontend fleet and action manifest panels"
priority: high

---

## Goal

Add frontend read-only panels for fleet UI state and backend action manifests.

This phase should make the frontend useful for inspecting available backend/dev actions without executing gameplay commands.

It must not add gameplay mutations, final UI, production auth, WebSockets, 3D rendering, or backend changes.

## Context

The backend exposes:

* `GET /api/dev/fleets/ui-state?civilizationId={id}`
* `GET /api/dev/fleets/action-manifest`
* `GET /api/dev/strategic-map/action-manifest`

The frontend shell should now show:

* fleet group summaries
* active transfer summaries
* route/fuel/interception readiness notes if present
* available dev action manifests as read-only documentation panels

No frontend command execution should be implemented in this phase.

## Implementation steps

1. Inspect frontend shell and API client.
2. Add TypeScript DTOs for:

   * fleet UI state response
   * fleet action manifest
   * strategic map action manifest
3. Add API client methods:

   * `getFleetUiState(civilizationId: string)`
   * `getFleetActionManifest()`
   * `getStrategicMapActionManifest()`
4. Add frontend panels:

   * Fleet page with civilization id input or shared state from shell
   * Fleet group summary
   * Active transfer summary
   * Route/fuel/interception readiness metadata
   * Strategic map action manifest panel
   * Fleet action manifest panel
5. Display action manifest entries as read-only:

   * action key
   * method
   * route
   * read-only/mutating flag
   * required fields
   * notes
6. Add clear warnings:

   * manifest entries are documentation/readiness metadata
   * mutating actions are not wired in frontend yet
   * dev endpoints are not production APIs
7. Do not implement buttons that call mutating endpoints.
8. Update docs if needed.
9. Update `ai/current-state.md` to document Phase 9D.

## Files to read first

* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/App.tsx
* docs/dev/fleet-api-contracts.md
* docs/dev/strategic-map-api-contract.md
* docs/dev/pre-frontend-contract-checkpoint.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
* src/VoidEmpires.Frontend/src/styles.css
* docs/dev/pre-frontend-contract-checkpoint.md if frontend usage docs need updating
* ai/current-state.md

May add:

* src/VoidEmpires.Frontend/src/api/fleetTypes.ts
* src/VoidEmpires.Frontend/src/api/actionManifestTypes.ts
* src/VoidEmpires.Frontend/src/components/ActionManifestPanel.tsx
* src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx

## Acceptance criteria

* Frontend can call fleet UI state dev endpoint.
* Frontend can call fleet action manifest endpoint.
* Frontend can call strategic map action manifest endpoint.
* Fleet page shows loading/error/success states.
* Fleet summaries are rendered read-only.
* Action manifests are rendered read-only.
* Mutating actions are clearly marked but not executable.
* No gameplay mutation commands are wired.
* Backend validation remains green.
* Frontend build/typecheck passes where environment supports it.
* `ai/current-state.md` documents Phase 9D.

## Constraints

* Do not add gameplay mutations.
* Do not add production auth.
* Do not add backend endpoints.
* Do not add backend gameplay changes.
* Do not add WebSockets.
* Do not add final UI design.
* Do not add 3D rendering.
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
   `feat(frontend): add fleet and manifest panels`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
