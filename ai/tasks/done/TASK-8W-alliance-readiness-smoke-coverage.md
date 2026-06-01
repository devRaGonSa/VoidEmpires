# TASK-8W

---

id: TASK-8W
title: Add alliance readiness smoke coverage
status: pending
type: hardening
team: backend
supporting_teams:
  - tests
  - docs
  - application
  - infrastructure
  - web
roadmap_item: "Phase 8W - Alliance readiness smoke coverage"
priority: high

---

## Goal

Add smoke coverage proving alliance readiness metadata is coherent with diplomatic contacts, strategic map, visibility, and dev tooling.

This phase should primarily add tests/docs. It must not introduce full alliance gameplay.

## Context

The current stack has:

* diplomatic contacts
* strategic map diplomacy metadata
* alliance readiness foundation
* alliance readiness metadata in strategic map
* alliance readiness dev endpoint/manifest metadata

The smoke test should confirm alliance metadata is visible to dev tooling but does not alter gameplay state or reveal hidden data.

## Implementation steps

1. Inspect diplomacy/contact tests and strategic map smoke tests.
2. Add or extend smoke coverage:

   * seed requesting civilization
   * seed at least one diplomatic contact
   * seed at least one alliance and membership for requesting civilization
   * optionally seed another civilization in same alliance
   * read diplomatic contacts
   * read alliance readiness
   * read strategic map
   * read action manifest
   * assert alliance metadata appears for requesting civilization
   * assert diplomatic contacts remain distinct from alliance membership
   * assert ally/foreign systems are not revealed through alliance readiness
   * assert visibility remains driven by ownership/exploration knowledge only
   * assert no resources/fleets/transfers/missions/knowledge/diplomatic contacts are mutated by alliance reads
3. Protect current limitations:

   * no shared visibility
   * no alliance permissions
   * no pacts/treaties
   * no trade
   * no war
   * no espionage
   * no combat
   * no UI
4. Update docs only if gaps are found.
5. Update `ai/current-state.md` to document Phase 8W and expected final test baseline.

## Files to read first

* tests/VoidEmpires.Tests/DiplomaticContactQueryServiceTests.cs
* tests/VoidEmpires.Tests/DevDiplomaticContactEndpointTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/*SmokeTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* tests/VoidEmpires.Tests/AllianceReadinessSmokeTests.cs
* docs/dev/strategic-map-api-contract.md only if documentation mismatch is found
* ai/current-state.md

May also touch:

* tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs

## Acceptance criteria

* Smoke coverage validates alliance metadata with diplomacy contacts, strategic map, visibility, and manifest tooling.
* Tests prove alliance readiness does not grant shared visibility.
* Tests prove alliance readiness does not mutate persisted state.
* Tests prove diplomatic contacts remain distinct from alliance membership.
* Tests prove action manifest exposes alliance readiness read action.
* Current limitations are protected.
* Docs updated if necessary.
* `ai/current-state.md` documents Phase 8W.

## Constraints

* Prefer tests/docs over production changes.
* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not add alliance commands or invitations.
* Do not add shared visibility.
* Do not add alliance permissions.
* Do not add pacts/treaties.
* Do not add trade.
* Do not add war.
* Do not add espionage.
* Do not add combat.
* Do not mutate exploration knowledge, resources, fleets, transfers, missions, or diplomatic contacts.
* Keep tests deterministic.

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
3. Verify changed files are expected tests/docs/current-state unless small fixes were necessary.
4. Commit with a clear message, for example:
   `test(diplomacy): add alliance readiness smoke coverage`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

