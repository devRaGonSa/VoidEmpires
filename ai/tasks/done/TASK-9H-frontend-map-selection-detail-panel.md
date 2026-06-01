# TASK-9H

---

id: TASK-9H
title: Add frontend map selection detail panel
status: done
type: feature
team: frontend
supporting_teams:
  - backend
  - tests
  - docs
roadmap_item: "Phase 9H - Frontend map selection detail panel"
priority: high

---

## Goal

Add read-only selection behavior and a detail panel for the frontend strategic map.

Users should be able to select a system or planet from the 2D map/list and inspect relevant metadata without executing gameplay actions.

This phase must not add mutating commands, production auth, backend endpoints, WebSockets, Three.js, or final UI.

## Context

Phase 9G adds a simple 2D map.

Now the frontend should support:

* selecting a system node
* viewing system details
* selecting/inspecting planets within the selected system
* viewing read-only metadata from the strategic map response

The detail panel should make readiness metadata understandable:

* visibility
* planets
* fleet presence
* transfer overlays
* sensor profiles
* detection coverage
* interception readiness
* diplomacy/alliance/pact notes when available

Do not treat readiness metadata as authorization.

## Implementation steps

1. Inspect Phase 9G components.
2. Add selection state:

   * selected system id
   * selected planet id if useful
3. Add detail panel component, for example:

   * `StrategicMapSelectionPanel`
4. System details should include:

   * system name
   * coordinates
   * visibility level/reason
   * owned status
   * planet count
   * fleet presence count
   * transfer overlay count
   * sensor/detection summaries
5. Planet details should include:

   * planet name if visible
   * planet type/status if visible
   * visibility level/reason
   * colonization status
   * command availability as read-only text
6. Fleet/transfer details should remain read-only.
7. Show global notes/readiness metadata:

   * route/fuel notes
   * sensor notes
   * detection notes
   * interception notes
   * diplomacy/alliance/pact notes if present in payload
8. Do not add buttons that call mutating endpoints.
9. Update docs if needed.
10. Update `ai/current-state.md` to document Phase 9H.

## Files to read first

* src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx
* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
* src/VoidEmpires.Frontend/src/styles.css
* docs/dev/pre-frontend-contract-checkpoint.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx
* src/VoidEmpires.Frontend/src/styles.css
* ai/current-state.md

May add:

* src/VoidEmpires.Frontend/src/components/StrategicMapSelectionPanel.tsx
* src/VoidEmpires.Frontend/src/components/ReadinessNotesPanel.tsx

## Acceptance criteria

* User can select a system from the 2D view or list.
* Selected system details render read-only.
* Planet summaries/details render read-only.
* Readiness metadata is visible and clearly non-authoritative.
* No mutating gameplay commands are wired.
* No backend changes are introduced.
* Backend validation remains green.
* Frontend build passes.
* `ai/current-state.md` documents Phase 9H.

## Constraints

* Do not add gameplay mutations.
* Do not call mutating endpoints.
* Do not add production auth.
* Do not add backend endpoints.
* Do not add backend gameplay changes.
* Do not add WebSockets.
* Do not add Three.js.
* Do not add final UI design.
* Keep selection state local and deterministic.

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
   `feat(frontend): add map selection detail panel`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
