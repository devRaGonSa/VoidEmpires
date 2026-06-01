# TASK-8A

---

id: TASK-8A
title: Add exploration tooling manifest and UI metadata
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - web
  - tests
  - docs
roadmap_item: "Phase 8A - Exploration tooling manifest and UI metadata"
priority: high

---

## Goal

Expose exploration mission and knowledge tooling metadata through the strategic map action manifest and strategic map read metadata so future UI prototypes can discover exploration-related actions without hardcoding routes.

This task should add metadata only. It must not add final UI and must not change gameplay behavior.

## Context

The strategic map action manifest already describes strategic map, visual state, fleet UI state, exploration preview, and related actions.

Now that exploration missions and knowledge exist, the manifest should include:

* exploration preview read
* exploration mission create
* exploration mission complete due
* exploration knowledge read
* exploration mission list read, if TASK-8B is already implemented later; if not, leave a note for the future or update in TASK-8B

Strategic map systems/planets already include command availability. The command metadata should clearly indicate:

* preview available for unknown targets
* create mission is available only for preview-eligible targets
* knowledge read is a top-level tooling action
* completion is a dev operation, not normal UI gameplay yet

## Implementation steps

1. Inspect `DevStrategicMapActionManifestService`.
2. Inspect strategic map command availability DTOs.
3. Add action manifest entries:

   * `exploration.preview.read`
   * `exploration.mission.create`
   * `exploration.mission.completeDue`
   * `exploration.knowledge.read`
4. For each action include:

   * method
   * route
   * read-only/mutating flag
   * required fields
   * success status
   * common error statuses
   * notes
5. Update strategic map command availability if useful:

   * unknown target can show `exploration.mission.create` capability metadata
   * visible/owned/revealed target should block create with explicit reason
   * keep it as UI metadata only; actual create service remains source of truth
6. Add tests:

   * manifest contains exploration preview/create/complete/knowledge actions
   * mutability flags are correct
   * required fields are correct
   * strategic map command availability includes exploration mission create hint where appropriate, if implemented
   * command availability does not mutate state
7. Update docs.
8. Update `ai/current-state.md` to document Phase 8A.

## Files to read first

* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Application/StrategicMap/GetDevStrategicMapActionManifestResult.cs
* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs if command hints are extended
* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs if new block reason/action metadata is required
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs if command hints are extended
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Strategic map action manifest includes exploration tooling actions.
* Mutating and read-only flags are accurate.
* Required fields and routes are documented in manifest.
* Strategic map command metadata remains UI-readiness only.
* Actual services remain authoritative for validation.
* Tests cover manifest and any command metadata changes.
* Docs are updated.
* `ai/current-state.md` documents Phase 8A.

## Constraints

* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add new gameplay behavior.
* Do not add sensors/scanners.
* Do not mutate resources/fleets.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Keep metadata deterministic.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Expected result:

* clean build
* 0 errors
* no new warnings
* all tests passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `feat(exploration): expose tooling manifest metadata`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
