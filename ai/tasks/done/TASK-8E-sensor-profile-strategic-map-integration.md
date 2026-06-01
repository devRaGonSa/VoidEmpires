# TASK-8E

---

id: TASK-8E
title: Integrate sensor profiles into strategic map metadata
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8E - Sensor profile strategic map integration"
priority: high

---

## Goal

Integrate read-only sensor profile metadata into strategic map/readiness surfaces.

This phase must not change visibility reveal rules, add real sensor range, add scanner mechanics, mutate exploration knowledge, or introduce espionage/combat/interception.

The goal is UI/dev readiness only.

## Context

Phase 8D adds read-only sensor profiles.

Strategic map and dev tooling should be able to expose sensor capability notes so future UI prototypes understand:

* which owned planets or orbital groups provide sensor metadata
* whether a visible/known node has local sensor coverage metadata
* current sensor limitations
* that sensors are not yet used to reveal or detect anything

## Implementation steps

1. Read Phase 8D sensor profile contracts/service.
2. Inspect strategic map DTOs and service:

   * `GetStrategicMapResult`
   * `StrategicMapSystemDto`
   * `StrategicMapPlanetDto`
   * `StrategicMapFleetPresenceDto`
3. Add lightweight sensor metadata to strategic map result where appropriate, for example:

   * top-level `SensorNotes`
   * per-system sensor profile summaries
   * per-planet sensor profile summaries
   * fleet presence sensor metadata if clean
4. Ensure the service uses `ISensorProfileService` or equivalent.
5. Do not use sensor profiles to reveal visibility or alter command validation.
6. Add tests:

   * owned planet strategic map item includes sensor profile metadata
   * orbital group/fleet presence includes sensor metadata if implemented
   * unknown/not-visible nodes do not gain revealed detail from sensors
   * strategic map remains read-only
7. Update `docs/dev/strategic-map-api-contract.md`.
8. Update `ai/current-state.md` to document Phase 8E.

## Files to read first

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Application/StrategicMap/GetSensorProfilesResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/SensorProfileService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/SensorProfileServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

May also modify:

* src/VoidEmpires.Application/StrategicMap/*Sensor*.cs
* tests/VoidEmpires.Tests/SensorProfileServiceTests.cs

## Acceptance criteria

* Strategic map exposes read-only sensor metadata.
* Sensor metadata does not reveal unknown data.
* Sensor metadata does not change visibility or command validation.
* Docs explicitly state sensors are metadata only at this stage.
* Tests cover integration behavior.
* `ai/current-state.md` documents Phase 8E.

## Constraints

* Do not reveal visibility through sensors.
* Do not add real sensor range.
* Do not add scanner mechanics.
* Do not mutate exploration knowledge.
* Do not mutate resources/fleets/missions.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Keep payload lightweight.

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
   `feat(map): surface sensor profile metadata`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
