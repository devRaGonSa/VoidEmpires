# TASK-34B

---
id: TASK-34B
title: Shared queue materialization service
status: pending
type: backend
team: gameplay
supporting_teams: [platform]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Add or identify a shared backend service boundary for materializing due gameplay orders across supported domains.

## Context
Construction, Research, and Shipyard completion should share a clear orchestration boundary so endpoint and QA tooling do not duplicate completion logic.

## Implementation steps

1. Review current application and infrastructure service conventions.
2. Reuse an existing completion/materialization service if one fits; otherwise add a service/facade such as `GameplayQueueMaterializationService` or `DueOrderMaterializationService` using project naming conventions.
3. Expose a clear method accepting:
   - `civilizationId`;
   - optional `planetId`;
   - `nowUtc`;
   - `includeConstruction`;
   - `includeResearch`;
   - `includeShipyard`.
4. Return a summary including:
   - `succeeded`;
   - processed Construction count;
   - processed Research count;
   - processed Shipyard count;
   - skipped/not-due counts;
   - notes.
5. Ensure the boundary is scoped and does not process unrelated civilizations when scoped.
6. Keep implementation idempotent at the orchestration layer and leave domain-specific completion to later tasks if needed.
7. Make no frontend changes.

## Files to read first

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/

## Expected files to modify

- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Web/
- tests/VoidEmpires.Tests/

## Acceptance criteria

- Shared backend boundary exists or an existing service is clearly reused.
- Request and response summary contracts are explicit.
- Scoped calls do not process unrelated civilizations.
- Backend build and tests pass.

## Constraints

- Do not add frontend UI.
- Do not expose production cheating behavior.
- Do not add background workers unless already part of existing safe architecture.
- Keep logic backend-authoritative and deterministic.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34B message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If a larger service split is required, create follow-up tasks.
