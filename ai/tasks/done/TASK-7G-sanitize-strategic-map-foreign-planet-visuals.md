# TASK-7G

---
id: TASK-7G
title: Sanitize foreign planet visuals in strategic map
status: done
type: bugfix
team: backend
supporting_teams:

* application
* infrastructure
* tests
  roadmap_item: "Phase 7G - Strategic map visibility hardening"
  priority: high

---

## Goal

Prevent the strategic map read model from exposing other civilizations' detailed planet visual/development signals when a planet appears in a relevant system but is not owned by the requesting civilization.

## Context

Phase 7E sanitizes ownership identity for planets not owned by the requester, but the strategic map planet DTO still carries colonization and development intensity values copied from `PlanetVisualStateDto`. Until a real visibility/sensor model exists, foreign owned planets should not reveal requester-inaccessible development detail.

## Implementation steps

1. Inspect `StrategicMapService`.
2. Sanitize non-requester owned planet fields so other civilizations' ownership/development detail is not exposed.
3. Preserve requester-owned planet visual summaries.
4. Preserve neutral/unowned planet summaries where no civilization ownership exists.
5. Add or update tests for foreign owned planet sanitization.

## Files to read first

* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* ai/current-state.md
* AGENTS.md

## Expected files to modify

* src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* ai/current-state.md

## Acceptance criteria

* Foreign owned planets do not expose another civilization id.
* Foreign owned planets do not expose detailed development/intensity values.
* Requester-owned planet summaries remain unchanged.
* Tests cover the sanitization behavior.
* Validation succeeds.

## Constraints

* Do not add a full visibility, sensor, alliance, or espionage model.
* Do not add migrations.
* Keep the change read-only.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message.
5. Push the branch to the remote.
