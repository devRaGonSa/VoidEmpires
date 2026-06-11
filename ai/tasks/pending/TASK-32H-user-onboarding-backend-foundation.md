# TASK-32H

---
id: TASK-32H
title: Add backend foundation for playable start creation
status: pending
type: feature
team: gameplay
supporting_teams: [backend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Implement the safest supported backend endpoint and application flow for creating a playable player or civilization start state.

## Context
After the onboarding contract audit, the backend needs a concrete way to create a playable start. If real auth-backed onboarding is not already safely available, this task should introduce a clearly development-only endpoint for start creation instead of pretending the application has a production-ready account flow.

## Implementation steps

1. Implement the backend command, handler, endpoint mapping, and persistence updates required by the contract from TASK-32G.
2. Support `displayName`, `civilizationName`, and optional `homePlanetName` in the request.
3. Return success status, relevant ids, starting resource snapshot, and limitations in the response.
4. Prevent duplicate or conflicting start creation where reasonable, or return a safe no-op or conflict result.
5. Ensure existing seeded validation data remains unaffected unless explicitly targeted.
6. Add tests covering creation, conflict or idempotency, invalid names, seed isolation, and development-only behavior if applicable.

## Files to read first

- `ai/orchestrator/component-discovery.md`
- `ai/orchestrator/di-analysis.md`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

## Expected files to modify

- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Infrastructure/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- A supported backend flow can create a playable start state.
- The response includes the ids and starting snapshot required by the frontend.
- Duplicate or conflicting requests are handled safely.
- The existing seeded validation civilization is not unintentionally modified.
- Automated tests cover the new behavior and pass.

## Constraints

- Keep the backend as the source of truth.
- Do not overclaim production auth if the flow is development-only.
- Do not widen this task into session management or multiplayer behavior.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(dev): add playable start creation backend`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If command, persistence, and tests exceed budget, create follow-up tasks before continuing.
