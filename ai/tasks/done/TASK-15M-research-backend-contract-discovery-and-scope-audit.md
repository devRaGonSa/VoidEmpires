# TASK-15M-research-backend-contract-discovery-and-scope-audit

---
id: TASK-15M-research-backend-contract-discovery-and-scope-audit
title: Research backend contract discovery and scope audit
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Audit the current Research backend, domain, application, infrastructure and web surface to define the exact safe scope for a Research cockpit v1.

## Purpose
The frontend already has a separated Research cabin. Before any UI work can safely depend on it, we need a precise map of what the backend actually supports today and what must remain placeholder-only.

## Current Problem
The repo history suggests research queue foundations and development endpoints may exist, but the current source of truth is the repository itself. If we guess at the contract, we risk exposing fake gameplay, unsafe mutation paths, or UI that claims support the backend does not actually have.

## Context
- Research should be development-safe and explicit-confirmation-based.
- The result of this task is an audit and scope definition, not a gameplay implementation.
- Any later Research cockpit task must know which contracts are real, which are dev-only, and which must stay disabled.

## Files to Inspect First
- `ai/current-state.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `src/VoidEmpires.Domain/Research/`
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Infrastructure/Research/`
- `src/VoidEmpires.Web/`
- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`

## Implementation Requirements
1. Discover all existing Research-related domain types.
2. Identify whether a research catalog exists.
3. Identify whether technologies have stable ids, categories, costs, durations, prerequisites and display names.
4. Identify whether a research queue exists and what fields it exposes.
5. Identify whether Development-only endpoints already exist for list, enqueue, complete-due, current queue and completed items.
6. Identify whether those endpoints are already covered by tests.
7. Define the safe v1 scope:
   - read catalog and readiness;
   - show requirements, costs and durations;
   - enqueue only if dev-safe and tested;
   - complete-due only if dev-safe and tested, otherwise placeholder;
   - no actual tech effects beyond persisted queue/completion state unless already present.
8. Produce a concise documentation note if no existing doc covers the Research backend contract.

## UI/UX Requirements
- None directly.
- Any documentation produced should use terms that make the later frontend tasks easy to implement in Spanish.
- Keep the scope language explicit about what the player can and cannot do.

## Backend/API Requirements
- Prefer no code changes unless the audit reveals missing docs or tests only.
- If code or API shape changes are required to support the audit, add tests.
- Do not invent endpoints.
- Do not wire production routes.
- Do not create research effects.

## Safety Constraints
- Do not mutate seed data unless the audit identifies a missing QA prerequisite and a later task handles it.
- Do not add hidden unlock logic.
- Do not make assumptions about tech effects, combat bonuses, fleet unlocks or economy changes.
- Keep production auth out of scope.

## Expected Files to Modify
- `ai/current-state.md` only if a small scope note is needed.
- `docs/dev/research-cockpit-checklist.md` or another narrow dev doc if no suitable Research contract note exists.
- Test files only if an uncovered backend contract needs to be described by tests.

## Acceptance Criteria
- Research backend state is clearly documented.
- Safe Research cockpit v1 scope is defined.
- Known exclusions are explicitly listed.
- Future tasks know which endpoints and services to use.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` only if frontend docs or code are touched.

## Notes / Residual Risks
- If backend Research support is thinner than expected, later tasks must keep enqueue and complete actions disabled rather than forcing unsupported behavior.
- This task should leave a clear paper trail for the next task in the block.
- If the audit finds multiple viable backend paths, prefer the safest dev-only one and document the alternatives explicitly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single documentation commit if possible.
