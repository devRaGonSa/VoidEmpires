# TASK-6V

---

id: TASK-6V
title: Document dev fleet API contracts
status: pending
type: documentation
team: backend
supporting_teams:

* docs
* web
* tests
  roadmap_item: "Phase 6V - Dev fleet API contract documentation"
  priority: medium

---

## Goal

Create developer-facing documentation for the current Development-only fleet API contracts.

This documentation should make the current fleet backend usable by future UI/frontend work without requiring developers to inspect every endpoint test.

## Context

The project now has multiple Development-only fleet endpoints for orbital group and transfer workflows.

Before adding UI or gameplay complexity, the current contracts should be documented:

* route
* HTTP method
* environment/gating behavior
* request payload
* success response
* common error responses
* notes about read-only vs mutating behavior
* notes about resource charging/no refunds
* lifecycle expectations

This task should not change gameplay behavior.

## Implementation steps

1. Inspect current dev endpoint mappings.
2. Identify all current fleet-related dev endpoints, including:

   * orbital transfer create
   * orbital transfer complete
   * orbital transfer cancel
   * orbital transfer lookup/read
   * orbital travel estimate preview
   * orbital group split
   * orbital group merge
   * fleet operational overview
3. Create or update documentation under `docs/dev/`.
4. Suggested file:

   * `docs/dev/fleet-api-contracts.md`
5. For each endpoint document:

   * method and route
   * Development-only gating rule
   * request fields
   * response fields
   * status codes
   * side effects
   * known restrictions
6. Include a compact lifecycle example:

   * estimate
   * create transfer
   * overview
   * cancel
   * split
   * merge
   * create transfer again
   * complete
   * overview
7. Add references to the existing visual sandbox/dev docs if useful.
8. Update `ai/current-state.md` to document Phase 6V.

## Files to read first

* src/VoidEmpires.Web/DevEndpointMappings.cs
* src/VoidEmpires.Web/DevOrbital*.cs
* src/VoidEmpires.Web/DevFleet*.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* docs/dev/
* ai/current-state.md
* AGENTS.md

## Expected files to modify

* docs/dev/fleet-api-contracts.md
* ai/current-state.md

If an existing docs file is clearly a better location, use it and keep the change focused.

## Acceptance criteria

* Fleet dev API contracts are documented.
* Documentation lists all current fleet-related dev endpoints.
* Documentation clearly distinguishes read-only and mutating endpoints.
* Documentation records dev endpoint gating behavior.
* Documentation includes lifecycle example.
* `ai/current-state.md` documents Phase 6V.
* No gameplay behavior changes are introduced.

## Constraints

* Documentation only unless a tiny docs link update is needed.
* Do not modify endpoint behavior in this task.
* Do not add tests unless documentation conventions require docs checks.
* Do not add UI.
* Do not add combat, interception, route graph logic, fuel inventory, alliances, or espionage.

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
3. Verify only expected documentation/state files changed.
4. Commit with a clear message, for example:
   `docs(dev): document fleet api contracts`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
