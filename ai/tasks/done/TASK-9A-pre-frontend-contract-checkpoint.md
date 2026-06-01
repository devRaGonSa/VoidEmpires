# TASK-9A

---

id: TASK-9A
title: Add pre-frontend contract checkpoint
status: pending
type: docs
team: architecture
supporting_teams:
  - backend
  - frontend
  - docs
  - tests
roadmap_item: "Phase 9A - Pre-frontend contract checkpoint"
priority: high

---

## Goal

Create a technical checkpoint documenting the stable backend/dev contracts that are ready for the next frontend foundation phase.

This task should be documentation-first. It must not add final frontend code, production endpoints, gameplay effects, migrations, or large refactors.

The checkpoint should make it clear what a future frontend can safely consume first, what is dev-only, what is still placeholder/readiness metadata, and what must not be treated as gameplay-complete.

## Context

The backend now has a broad set of UI-readiness/dev contracts:

* strategic map read model
* visual state endpoints
* fleet UI state endpoint
* fleet action manifest
* strategic map action manifest
* exploration preview/missions/knowledge endpoints
* sensor profiles
* detection coverage
* interception opportunities
* diplomatic contacts
* alliance readiness
* alliance pact readiness
* docs for fleet and strategic map APIs
* visual sandbox gating rules

Before moving into frontend foundation, create a checkpoint that frontend work can use as a source of truth.

## Implementation steps

1. Create a new document:

   * `docs/dev/pre-frontend-contract-checkpoint.md`
2. Document:

   * current intended frontend entry points
   * which endpoints are Development-only
   * which endpoints are read-only
   * which endpoints mutate dev state
   * strategic map payload readiness
   * fleet UI state payload readiness
   * visual state payload readiness
   * exploration tooling lifecycle
   * sensors/detection/interception readiness limitations
   * diplomacy/alliance/pact readiness limitations
   * current non-goals
3. Include a recommended first frontend slice:

   * shell/layout
   * API client
   * strategic map read
   * fleet UI state read
   * action manifest reads
   * system/planet visual state read
   * no production auth flow yet unless explicitly introduced later
4. Include clear warnings:

   * dev endpoints are not production endpoints
   * readiness metadata is not gameplay authorization
   * no shared visibility from alliance/pact metadata
   * no real sensors/detection/interception/combat yet
   * no final UI/3D rendering yet
5. Update existing docs only if needed:

   * `docs/dev/strategic-map-api-contract.md`
   * `docs/dev/fleet-api-contracts.md`
   * `docs/dev/visual-state-sandbox.md`
6. Add/adjust lightweight docs test only if the repository already has docs validation conventions.
7. Update `ai/current-state.md` to document Phase 9A and final baseline.

## Files to read first

* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* docs/dev/visual-state-sandbox.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* docs/dev/pre-frontend-contract-checkpoint.md
* docs/dev/strategic-map-api-contract.md if endpoint list needs updating
* docs/dev/fleet-api-contracts.md if endpoint list needs updating
* docs/dev/visual-state-sandbox.md if gating/status needs updating
* ai/current-state.md

## Acceptance criteria

* Pre-frontend contract checkpoint document exists.
* It accurately lists current dev/backend surfaces.
* It distinguishes read-only endpoints from dev-mutating endpoints.
* It identifies readiness metadata versus real gameplay.
* It states current limitations and non-goals.
* It recommends a safe first frontend slice.
* `ai/current-state.md` documents Phase 9A.
* Existing test suite remains green.

## Constraints

* Prefer docs/current-state only.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add gameplay effects.
* Do not add migrations.
* Do not refactor existing services unless a docs inconsistency reveals a small required fix.
* Keep the checkpoint practical for frontend implementation.

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
3. Verify changed files are docs/current-state unless small fixes were necessary.
4. Commit with a clear message, for example:
   `docs(frontend): add pre-frontend contract checkpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

