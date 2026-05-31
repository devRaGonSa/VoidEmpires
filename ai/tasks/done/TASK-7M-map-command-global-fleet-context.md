# TASK-7M

---
id: TASK-7M
title: Use map-level fleet context for strategic map travel availability
status: done
type: bugfix
team: backend
supporting_teams:

* infrastructure
* tests
  roadmap_item: "Phase 7M - Map command availability correction"
  priority: high

---

## Goal

Correct strategic map travel and transfer command availability so visible destination planets can use requesting-civilization fleet context from anywhere in the returned strategic map, not only fleet presence in the same system.

## Context

Phase 7K added read-only command availability metadata. The current implementation derives planet travel/transfer availability from fleet presence local to the system being projected. That can incorrectly block a visible destination in another system even when the map includes a requesting-civilization fleet elsewhere.

This is metadata only. It must not execute commands or bypass the existing fleet command validation path.

## Implementation steps

1. Inspect `StrategicMapService` command availability derivation.
2. Inspect strategic map service tests around transfer destinations and command availability.
3. Derive fleet context at the map result level before creating per-system DTOs.
4. Use that map-level context for planet travel/transfer command availability.
5. Add or update tests proving a visible destination in a different system can expose travel/transfer capability metadata when a requesting-civilization fleet exists elsewhere.
6. Keep unknown/not-visible destination commands blocked.

## Files to read first

* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* src/VoidEmpires.Application/StrategicMap/GetStrategicMapResult.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* ai/current-state.md

## Expected files to modify

* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* ai/current-state.md

## Acceptance criteria

* Travel/transfer command availability uses requesting-civilization fleet context from the returned map, not just the planet's local system.
* Visible destination planets in another returned system can show travel/transfer capability metadata when fleet context exists.
* Unknown/not-visible destination planets remain blocked.
* No gameplay commands are executed.
* Tests pass.

## Constraints

* Read-only only.
* Do not add command execution endpoints.
* Do not bypass fleet command validation.
* Do not add migrations or persistence state.
* Keep the change focused.

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
   `fix(map): use map fleet context for travel availability`
5. Push the branch to the remote.
