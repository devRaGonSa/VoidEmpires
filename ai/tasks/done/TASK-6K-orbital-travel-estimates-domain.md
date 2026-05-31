# TASK-6K

---

id: TASK-6K
title: Add domain-only orbital travel estimates with resource costs
status: pending
type: feature
team: backend
supporting_teams:

* domain
* tests
  roadmap_item: "Phase 6K - Orbital travel estimates domain foundation"
  priority: high

---

## Goal

Extend the existing domain-only orbital travel estimation foundation so it can return a read-only orbital travel estimate containing:

* abstract distance units
* estimated travel duration
* estimated resource costs by ResourceType
* SpaceAssetType-based cost multiplier support

This phase must not charge resources, persist anything, create transfers, modify transfer creation behavior, or add endpoints.

## Context

The repository already contains `OrbitalTravelEstimator` with basic abstract distance and duration estimation.

This task adds a richer read-only estimate model in the domain layer only. It is a foundation for future orbital transfer previews and fuel/resource-cost validation, but it must not change actual transfer creation yet.

The estimator must remain deterministic, simple, testable, and independent from persistence.

Current intended behavior:

* distance is still abstract and currently one unit per planet-to-planet transfer
* duration is still based on `DurationPerAbstractDistanceUnit`
* resource cost is estimated only, not charged
* costs are returned per `ResourceType`
* costs can scale by distance and by `SpaceAssetType`
* the implementation must be conservative and must not introduce route graphs, fuel systems, persistence, endpoints, or migrations

## Implementation steps

1. Read the existing `OrbitalTravelEstimator` and its tests.
2. Inspect existing domain enums/types for `ResourceType` and `SpaceAssetType`.
3. Add immutable domain read models for orbital travel estimates:

   * `OrbitalTravelEstimate`
   * `OrbitalTravelResourceCost`
4. Extend `OrbitalTravelEstimator` with a method that creates a full estimate for:

   * current planet id
   * destination planet id
   * space asset type
5. Keep existing public behavior compatible where possible.
6. Add estimated resource costs using small deterministic constants suitable for a foundation.
7. Add a `SpaceAssetType` cost multiplier mapping inside the estimator or a small nearby domain helper.
8. Add unit tests covering:

   * invalid current planet id
   * invalid destination planet id
   * same current/destination planet
   * positive distance
   * expected duration
   * estimated resource costs are returned
   * costs scale by asset type multiplier
   * costs scale by distance if the estimator supports distance as an input
   * returned costs do not mutate external state
9. Keep the task domain-only.

## Files to read first

* src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimatorTests.cs
* src/VoidEmpires.Domain/Resources/ResourceType.cs
* src/VoidEmpires.Domain/Assets/SpaceAssetType.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

* src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs
* tests/VoidEmpires.Tests/OrbitalTravelEstimatorTests.cs

If `OrbitalTravelEstimate` or `OrbitalTravelResourceCost` are better as separate files according to repository conventions, it is acceptable to add:

* src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimate.cs
* src/VoidEmpires.Domain/Fleets/OrbitalTravelResourceCost.cs

Do not modify application, infrastructure, web endpoints, EF migrations, or persistence files.

## Acceptance criteria

* A read-only orbital travel estimate model exists in the domain layer.
* The estimate includes distance, duration, and resource costs.
* Resource costs are represented by `ResourceType`.
* Resource costs account for `SpaceAssetType` multiplier.
* Existing estimator tests still pass.
* New estimator tests cover the new behavior.
* No resources are charged.
* No orbital transfer creation behavior changes.
* No database migration is added.
* No endpoint is added.
* No UI or sandbox change is added.
* `ai/current-state.md` may remain unchanged unless the repository convention requires updating it after implementation.

## Constraints

* Do not implement Phase 6L.
* Do not add HTTP endpoints.
* Do not add application services.
* Do not modify infrastructure or EF persistence.
* Do not add migrations.
* Do not change real transfer creation behavior.
* Do not introduce route graph logic.
* Do not introduce fuel inventory or resource charging.
* Do not add combat, interception, alliances, espionage, UI, or renderer behavior.
* Keep the change minimal.
* Prefer fewer than 5 modified files.
* Prefer fewer than 200 lines of code.
* Follow existing naming and test style.

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
   `feat(fleets): add orbital travel estimate costs`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
