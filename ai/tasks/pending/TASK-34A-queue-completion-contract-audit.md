# TASK-34A

---
id: TASK-34A
title: Queue completion contract audit
status: pending
type: platform
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Audit current queue/order models and define the safe backend-authoritative completion/materialization contract before changing behavior.

## Context
Construction, Research, and Shipyard can enqueue persisted orders. Block 34 adds explicit, deterministic completion materialization for due orders, so the contract must be documented first to prevent production auth or cheating overclaims.

## Implementation steps

1. Identify persisted order models for Construction, Research, and Shipyard/orbital production.
2. Document order states such as pending/open, completed, cancelled/cleared if any, and due/end time fields.
3. Identify whether completion or materialization services already exist.
4. Document the game state changed on completion:
   - Construction upgrades/applies building or structure level.
   - Research applies technology level or unlock.
   - Shipyard increases backend-backed ship/unit stock or inventory.
   - Resources are not deducted again on completion if enqueue already deducted them, unless current backend semantics require otherwise.
5. Define the v1 materialization policy:
   - process only due orders;
   - be idempotent;
   - scope by civilization and planet where possible;
   - avoid global uncontrolled mutation from page load;
   - expose explicit Development QA endpoint/helper only.
6. Update the listed docs with the contract.
7. Make no behavior changes.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/construction-cockpit-checklist.md
- docs/dev/research-cockpit-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/planet-cockpit-checklist.md

## Acceptance criteria

- Completion semantics are documented before implementation.
- Current order models, states, and due-time fields are identified.
- Safe materialization scope and idempotency policy are documented.
- No production overclaim is introduced.
- No behavior changes are made.

## Constraints

- Do not implement completion logic in this task.
- Do not perform or claim browser/visual QA.
- Do not add production auth, WebSockets, workers, combat, movement, missions, market, alliances, or espionage execution.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34A message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
