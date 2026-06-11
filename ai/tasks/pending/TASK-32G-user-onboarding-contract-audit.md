# TASK-32G

---
id: TASK-32G
title: Audit safe onboarding contract for playable session creation
status: pending
type: feature
team: gameplay
supporting_teams: [backend, frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Define the safest supported onboarding scope for creating a playable player, civilization, and starting planet without overstating the repository's authentication story.

## Context
The current gameplay UI may still rely on seeded ids and development-only entry paths. Before adding a new onboarding flow, the repository needs a written decision on whether a real auth-backed path exists or whether the correct solution is a clearly development-only playable-start creation flow.

## Implementation steps

1. Audit the current backend models and endpoints related to users, players, accounts, civilizations, and planet ownership.
2. Confirm whether authentication middleware and a real user lifecycle already exist and are suitable for reuse.
3. Identify whether any current seed or development-only user or civilization setup already exists.
4. Define the safe onboarding scope for this block and the expected returned identifiers and starting state.
5. Update the persisted gameplay checklist with the supported scope and explicit limitations.

## Files to read first

- `ai/architecture-index.md`
- `ai/orchestrator/component-discovery.md`
- `ai/orchestrator/di-analysis.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application`

## Expected files to modify

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `ai/current-state.md` only if the audit reveals a mismatch that must be noted immediately

## Acceptance criteria

- The repository's current onboarding-related model is documented clearly.
- The task defines whether this block should use real auth-backed onboarding or a development-only playable-start flow.
- The expected onboarding result payload is defined, including ids, starting resources, and limitations.
- No runtime behavior changes are introduced.

## Constraints

- Do not overclaim production authentication capabilities.
- Do not add login, email verification, or password flows unless they already exist and are being reused safely.
- Keep the output implementation-ready for follow-up tasks.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- No unrelated runtime files are modified.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs(tasks): audit onboarding contract scope`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Push implementation details into later tasks rather than broadening the audit.
