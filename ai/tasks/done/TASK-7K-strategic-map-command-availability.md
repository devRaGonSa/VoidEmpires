# TASK-7K

---

id: TASK-7K
title: Add strategic map command availability
status: done
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
* docs
  roadmap_item: "Phase 7K - Strategic map command availability"
  priority: high

---

## Goal

Add read-only command availability metadata to the strategic map so future UI prototypes can know which map-level actions are available or blocked.

This phase should not execute commands. It should only expose availability/capability metadata.

## Context

The project already has:

* fleet command availability in fleet overview/UI state
* action manifests
* strategic map read model
* visibility integration

The strategic map should now expose command availability for map nodes, such as:

* can view system
* can view planet detail
* can view fleet state
* can estimate travel from an owned/current group to visible destination
* can create transfer through existing fleet command path
* blocked because unknown/not visible/not owned/no fleet context

This must not replace actual command validation. It is UI-readiness metadata only.

## Implementation steps

1. Inspect fleet command availability DTOs.
2. Inspect strategic map DTOs and visibility fields.
3. Add strategic map command availability DTOs, for example:

   * `StrategicMapCommandAvailabilityDto`
   * `StrategicMapCommandBlockReason`
   * per-system/per-planet command metadata
4. Derive availability from:

   * visibility level
   * ownership
   * whether a planet/system is visible
   * whether known fleet/orbital presence exists
   * existing fleet UI/action manifest conventions where useful
5. Keep this read-only and deterministic.
6. Update strategic map service to populate command availability.
7. Update strategic map action manifest notes if needed.
8. Add tests:

   * owned visible planet has view/detail commands available
   * unknown/not-visible node has commands blocked
   * travel/transfer-related availability is presented as capability metadata, not as permission to bypass command validation
   * command availability does not mutate state
9. Update `docs/dev/strategic-map-api-contract.md`.
10. Update `ai/current-state.md` to document Phase 7K.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* src/VoidEmpires.Application/Fleets/GetDevFleetUiStateResult.cs
* src/VoidEmpires.Application/Fleets/GetFleetOperationalOverviewResult.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Application/StrategicMap/StrategicMapCommandAvailabilityDto.cs if split file is preferred
* src/VoidEmpires.Application/StrategicMap/StrategicMapCommandBlockReason.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs if notes need updates
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs if manifest notes change
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Strategic map exposes command availability metadata.
* Availability reflects visibility and ownership.
* Unknown/not-visible nodes are blocked with explicit reasons.
* Command availability is documented as UI metadata, not authorization.
* No gameplay commands are executed.
* Tests cover available and blocked cases.
* Docs are updated.
* `ai/current-state.md` documents Phase 7K.

## Constraints

* Read-only only.
* Do not add new command execution endpoints.
* Do not bypass existing fleet command validation.
* Do not add persisted exploration/fog-of-war state.
* Do not add sensors/scanners.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add migrations.
* Keep contracts lightweight and deterministic.

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
   `feat(map): add strategic map command availability`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
