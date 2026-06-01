# TASK-7X

---

id: TASK-7X
title: Add exploration reveal lifecycle docs and smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:
  - tests
  - docs
  - application
  - infrastructure
  - web
roadmap_item: "Phase 7X - Exploration reveal lifecycle docs and smoke coverage"
priority: high

---

## Goal

Add high-level smoke coverage and documentation for the first real exploration reveal lifecycle.

The lifecycle should now prove:
preview -> create mission -> complete due mission -> knowledge recorded -> map visibility uses knowledge -> strategic map shows revealed target conservatively.

This phase should primarily add tests/docs unless small fixes are needed.

## Context

Phases 7U, 7V, and 7W add:

- exploration knowledge persistence
- mission completion records knowledge
- map visibility consumes knowledge

This task locks the full lifecycle and documents current rules.

## Implementation steps

1. Inspect existing exploration mission lifecycle smoke tests.
2. Extend or add a new smoke test:
   - seed owned civilization context and an unknown target system/planet
   - preview target as eligible
   - create mission
   - complete due mission
   - assert knowledge rows exist
   - assert map visibility now marks target as known/visible according to Phase 7W rule
   - assert strategic map includes target system/planet
   - assert ownership is not incorrectly assigned
   - assert foreign ownership remains hidden/sanitized
   - assert exploration preview is now blocked for revealed target
   - assert resources/fleets are not mutated
3. Update `docs/dev/strategic-map-api-contract.md`:
   - document knowledge/reveal lifecycle
   - document current visibility rules
   - document side effects of completion: records knowledge only, no rewards/fleet changes
4. Update `ai/current-state.md`:
   - Phase 7X
   - current baseline expected test count
   - next recommended work
5. Do not add new gameplay behavior unless needed to make the lifecycle coherent.

## Files to read first

- `tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs`
- `tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationActionPreviewServiceTests.cs`
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `tests/VoidEmpires.Tests/ExplorationMissionLifecycleSmokeTests.cs`
- `tests/VoidEmpires.Tests/StrategicMapReadinessSmokeTests.cs` if needed
- `docs/dev/strategic-map-api-contract.md`
- `ai/current-state.md`

May also touch:

- `tests/VoidEmpires.Tests/MapVisibilityServiceTests.cs`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/ExplorationActionPreviewServiceTests.cs`

## Acceptance criteria

- Smoke coverage validates full reveal lifecycle.
- Knowledge is recorded after mission completion.
- Visibility consumes knowledge.
- Strategic map shows revealed target conservatively.
- Exploration preview is blocked after reveal.
- Foreign ownership is not leaked.
- No resources/fleets/rewards are mutated.
- Docs describe reveal rules and limitations.
- `ai/current-state.md` documents Phase 7X.

## Constraints

- Do not add final UI/frontend code.
- Do not add production endpoints.
- Do not add sensors/scanners.
- Do not add rewards.
- Do not mutate resources/fleets.
- Do not add espionage, diplomacy, combat, or interception.
- Do not add route graph/pathfinding.
- Keep smoke coverage deterministic.
- Keep reveal conservative and documented.

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
   `test(exploration): add reveal lifecycle coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
