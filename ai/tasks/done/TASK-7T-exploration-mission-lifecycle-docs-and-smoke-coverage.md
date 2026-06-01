# TASK-7T

---

id: TASK-7T
title: Add exploration mission lifecycle docs and smoke coverage
status: done
type: hardening
team: backend
supporting_teams:
  - tests
  - docs
  - application
  - infrastructure
  - web
roadmap_item: "Phase 7T - Exploration mission lifecycle docs and smoke coverage"
priority: high

---

## Goal

Add high-level smoke coverage and documentation for the minimal exploration mission lifecycle.

This phase should focus on proving the lifecycle:
preview -> create mission -> complete due mission -> strategic map remains conservative.

It must not add real fog-of-war reveal or complex exploration gameplay.

## Context

The current exploration pipeline should now include:

- exploration preview
- persistent exploration mission
- create mission endpoint/service
- complete due missions service/endpoint
- strategic map remains conservative
- visibility is not revealed yet

This smoke coverage should protect current intentional limitations and prepare the future real visibility reveal phase.

## Implementation steps

1. Inspect exploration preview tests.
2. Inspect mission creation tests.
3. Inspect mission completion tests.
4. Inspect strategic map readiness smoke tests.
5. Add or extend smoke coverage:
   - seed unknown system/planet and owned civilization context
   - preview unknown target as eligible
   - create exploration mission
   - assert mission is persisted as planned
   - complete due mission
   - assert mission completed
   - assert strategic map/visibility still does not reveal target yet
   - assert no fog-of-war/known-system/sensor state exists
   - assert no resource/fleet mutation happens
6. Update docs:
   - lifecycle endpoints
   - side effects
   - limitations
   - explicit "completion does not reveal visibility yet"
7. Update `ai/current-state.md`:
   - Phase 7T
   - baseline expected test count
   - next recommended work should mention real visibility reveal as a future phase
8. Do not add gameplay behavior unless required to fix lifecycle correctness.

## Files to read first

- `tests/VoidEmpires.Tests/StrategicMapReadinessSmokeTests.cs`
- `tests/VoidEmpires.Tests/ExplorationActionPreviewServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionCreateServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationMissionCompletionServiceTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`

May also touch:

- `tests/VoidEmpires.Tests/DevExplorationMissionEndpointTests.cs`
- `tests/VoidEmpires.Tests/StrategicMapReadinessSmokeTests.cs`

## Acceptance criteria

- Smoke coverage validates preview -> create -> complete.
- Completion leaves visibility conservative.
- No known-system/fog-of-war/sensor state is created.
- No resources or fleets are mutated.
- Docs describe endpoints and limitations.
- `ai/current-state.md` documents Phase 7T.
- No new gameplay beyond the minimal lifecycle.

## Constraints

- Do not reveal visibility.
- Do not add real exploration result generation.
- Do not add fog-of-war persistence.
- Do not add sensors/scanners.
- Do not add rewards.
- Do not mutate resources/fleets.
- Do not add combat/interception.
- Do not add espionage or diplomacy.
- Do not add route graph/pathfinding.
- Do not add UI.
- Keep smoke coverage deterministic.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Expected result:

- clean build
- 0 errors
- no new warnings
- all tests passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify changed files are expected tests/docs/current-state unless small fixes were necessary.
4. Commit with a clear message, for example:
   `test(exploration): add mission lifecycle smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
