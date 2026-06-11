# TASK-32A

---
id: TASK-32A
title: Audit playable session contracts for modals onboarding and planet resources
status: pending
type: feature
team: gameplay
supporting_teams: [frontend, backend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Document the current confirmed baseline for gameplay confirmations, onboarding shape, and planet resource economy before any implementation work starts.

## Context
Block 32A-32P touches several connected systems. This first task exists to reduce scope risk by auditing the current confirmation UX in Construction, Research, and Shipyard, the current user or player model, and the current backend resource-economy behavior before behavior changes are introduced.

## Implementation steps

1. Review the current confirmation patterns in `ConstructionPage`, `ResearchPage`, and `ShipyardPage`.
2. Identify whether a reusable modal or dialog component already exists in the frontend.
3. Audit the current backend identity, player, civilization, and planet-ownership model to confirm whether the UI still depends on seeded fixed ids.
4. Audit where planet resources are stored, whether production exists, whether elapsed-time accrual exists, and where deductions already happen.
5. Define the safe implementation scope for this block without overclaiming production authentication.
6. Update the persisted gameplay and cockpit checklist docs with the confirmed findings and explicit deferred items.

## Files to read first

- `ai/architecture-index.md`
- `ai/orchestrator/component-discovery.md`
- `ai/current-state.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Expected files to modify

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/shipyard-cockpit-checklist.md`

## Acceptance criteria

- The current confirmation-flow patterns are documented for Construction, Research, and Shipyard.
- The existence or absence of a reusable modal foundation is documented.
- The current user, player, civilization, and starting-planet model is documented without speculation.
- The current resource storage, deduction, and accrual behavior is documented.
- The safe scope for this block is written down, including the no-production-auth-overclaim constraint.
- No runtime behavior changes are introduced.

## Constraints

- Follow the architecture and conventions of the current repository.
- Keep the task documentation-first with no production behavior changes.
- Do not claim visual QA or production auth behavior that does not exist.
- Keep visual QA explicitly deferred.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- No unrelated source files are modified.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs(tasks): audit playable session contracts`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split follow-up implementation into later tasks rather than widening this audit.
