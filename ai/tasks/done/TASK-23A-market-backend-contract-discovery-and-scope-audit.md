# TASK-23A

---
id: TASK-23A
title: Phase 23A - Market backend contract discovery and scope audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Audit the current backend, domain, application, infrastructure, development endpoints, and existing docs to define the safe read-only scope for `Mercado v1`.

## Purpose

Before a real Market cockpit is built, the repository needs a precise inventory of which economy, resource, stockpile, production, affordability, and logistics concepts already exist and which ones must remain out of scope for this phase.

## Current problem

`/market` is still a future placeholder module. The repository already exposes resources and production across Planet, Construction, Research, Shipyard, Defenses, Ground Army, and Fleets, but there is not yet a dedicated scope note that tells future tasks what belongs in Market and what must remain elsewhere or stay disabled.

## Context

Current accepted state shows that:

- `Galaxy` is the accepted read-only strategic cockpit and must remain read-only.
- `Planet` already exposes local resources and production context.
- `Construction`, `Research`, `Shipyard`, `Defenses`, and `Ground Army` consume or display resource costs.
- `Fleets` already frames logistics and transfer context.
- `ai/current-state.md` still lists `no market` as a current exclusion.
- Future Market work must therefore stay read-only, Development-safe, and honest about limitations.

## Files to read first

- `AGENTS.md`
- `ai/current-state.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/fleet-api-contracts.md`
- `src/VoidEmpires.Domain/`
- `src/VoidEmpires.Domain/Planets/`
- `src/VoidEmpires.Domain/Fleets/`
- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/`

## Component discovery

Inspect the current resource, stockpile, production, cost, affordability, transfer, and route-adjacent components first. Prefer reusing those concepts rather than inventing a parallel market subsystem.

## Dependency analysis

Map the current chain that supplies economy state to the frontend, for example:

- planet and construction read endpoints -> resource or production services -> persisted stockpile models
- shipyard and defenses read models -> cost or affordability services -> planet resource state
- fleet read models -> logistics or reserve context -> persisted orbital transfer and group state
- development seed profiles -> deterministic economy data -> dev-only cockpit endpoints

Document which components call which services so later tasks know where a safe Market read model can be assembled.

## Implementation requirements

1. Inventory the current economy concepts already implemented in the repository:
   - resource types
   - planet stockpiles
   - civilization-level reserves if present
   - production per hour or similar flow metrics
   - build or queue costs
   - affordability helpers
   - logistics or reserve signals from Fleets
   - any domain names that mention market, trade, exchange, route, listing, or commerce
2. Identify whether an existing service or endpoint already supports a Market read model.
3. Identify what must remain in `Planet`, `Construction`, `Shipyard`, `Fleets`, and `Galaxy` rather than being duplicated in Market.
4. Define the safe v1 scope explicitly:
   - read civilization context
   - read selected planet context if provided
   - show resource reserves
   - show production or economy flow if currently derivable
   - show deterministic reference ratios or prices as advisory only
   - show future trade-route or listing placeholders as disabled
   - no buying
   - no selling
   - no auctions
   - no player-to-player trading
   - no resource transfers
5. Record the findings in `docs/dev/market-cockpit-checklist.md` or an equivalent dedicated backend contract note if that checklist does not exist yet.
6. If documentation alone is insufficient and a tiny backend display metadata change is required to support the audit, cover it with tests.

## UI/UX requirements

- This task does not need to ship a cockpit UI.
- The audit must define language that allows Market to communicate economy insight without implying that active transactions exist.
- Any discovered raw enums, DTO names, capability keys, or request names should be marked as diagnostics-only material for later tasks.

## Backend/API requirements

- Prefer docs and tests only.
- Reuse existing Development-only read surfaces where possible.
- Do not add a production market endpoint in this task.
- If code changes are unavoidable, keep them read-only and add or update tests.

## Expected files to modify

- `docs/dev/market-cockpit-checklist.md` if created in this task
- a backend contract note under `docs/dev/` if a checklist is not the right target
- backend tests only if tiny audit-supporting code changes are required

## Safety constraints

- No market mutations
- No transaction execution
- No buy or sell orders
- No auctions
- No player-to-player trading
- No resource mutation
- No fleet mutation
- No WebSockets

## Acceptance criteria

- The repository has an explicit written scope for `Mercado v1`.
- Existing economy and logistics-adjacent concepts are inventoried with references to the real components that expose them.
- Safe boundaries clearly distinguish read-only economy analysis from active trade gameplay.
- Later backend and frontend tasks can point to this audit instead of guessing the scope.
- Validation passes.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if frontend docs or typed contract files are touched.

If integration coverage is checked during implementation and no integration test harness is configured, record:

`No integration tests configured.`

## Notes / residual risks

- The audit may confirm that no dedicated market backend exists yet; if so, the correct outcome is a documented reuse plan based on current resources and production read-state, not a rushed transaction subsystem.
- If current economy support is thinner than expected, later tasks must present that limitation honestly instead of inventing fake commerce behavior.

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm only intended docs or test files changed.
4. Commit with a clear message.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code or docs in a single implementation task.
- If the audit reveals a broader contract gap, create a follow-up task instead of expanding this one into endpoint implementation.
