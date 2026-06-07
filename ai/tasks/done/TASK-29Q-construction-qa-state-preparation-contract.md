# TASK-29Q

---
id: TASK-29Q
title: Construction QA state preparation contract
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 29Q-29T"
priority: medium
---

## Goal
Define and document the safe Development-only contract for preparing a reused Development database state so Construction manual QA can run without manual SQL.

## Context
Manual QA on `/construction` succeeds only when the owned target planet is clear of blocking open orders and has sufficient resources for at least one action. Seed reapply is additive and idempotent, so repeated runs on shared databases can leave active construction rows.

## Implementation steps

1. Audit current construction storage and checks (`PlanetConstructionOrder`, `PlanetConstructionQueueService`, queue completion path, construction UI state provider, persistence registrations).
2. Decide and document a contract that only clears/neutralizes open construction orders for one scoped target planet/civilization.
3. Add explicit contract notes to `docs/dev/construction-cockpit-checklist.md` and `docs/dev/persisted-gameplay-flow-checklist.md`:
   - default seeded target identifiers
   - preserved historical state behavior
   - resource minimums and intended resulting queue state
4. Keep all behavior Development-only with no production changes.

## Contract decision

- Open-construction storage: only rows in `PlanetConstructionOrder` with
  `Status = Pending` or `Status = Active` are considered blocking.
- Blocking criterion: a scoped `(civilizationId, planetId)` pair with status `Pending`/`Active`.
- Scope and mutation:
  - If the scoped planet is not actively owned by the scoped civilization, preparation fails.
  - Existing blocking open rows are neutralized by marking status `Cancelled` (safe, idempotent, deterministic history-preserving).
  - Completed/`Pending`? only open rows above are changed; non-open rows remain untouched.
- Resource contract:
  - If stockpile exists, top up to minimums `1000` credits, `1000` metal, `1000` crystal, `1000` gas.
  - If stockpile is missing, no stockpile mutation occurs and the response notes that explicitly.
- Default scope: `civilizationId = 00000000-0000-0000-0000-000000000001`, `planetId = 40000000-0000-0000-0000-000000000001`.
- Historical/completed and unrelated rows are preserved and may remain visible.

## Safety rule summary

- Preparation is a Development-only utility, called explicitly.
- It is non-destructive outside development scope and does not touch any non-target planet/civilization.
- It is safe to run repeatedly (idempotent once open orders are gone).
- It does not run on page load and is not invoked by seed scripts.

## Files to read first

- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Application/Development/IDevelopmentSeedService.cs`
- `src/VoidEmpires.Infrastructure/Buildings/PlanetConstructionQueueService.cs`
- `src/VoidEmpires.Domain/Buildings/PlanetConstructionOrder.cs`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Expected files to modify

- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Acceptance criteria

- A safe scoped contract is documented.
- The contract is explicit that no production behavior changes are introduced.
- Contracted values support repeated manual QA on reused Development databases.

## Validation

- Documentation review and cross-file consistency check.
- Command-path sanity check with existing persisted QA runbook flow.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
