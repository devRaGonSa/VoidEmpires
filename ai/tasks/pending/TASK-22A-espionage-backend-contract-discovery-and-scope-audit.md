# TASK-22A

---
id: TASK-22A
title: Phase 22A - Espionage backend contract discovery and scope audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Audit the current backend, domain, application, infrastructure, development endpoints, and existing docs to define the safe read-only scope for `Espionaje v1`.

## Purpose

Before a real Espionage cockpit is built, the repository needs a precise inventory of which intelligence, visibility, and observation concepts already exist and which ones must remain out of scope for this phase.

## Current problem

`/espionage` is still a placeholder module. The repository already exposes strategic visibility, observed systems, partial knowledge, fleet markers, transfer overlays, and read-only strategic diagnostics, but there is not yet a dedicated Espionage scope note that tells future tasks what belongs in this cockpit and what must stay in Galaxy or remain disabled.

## Context

Current accepted state shows that:

- `Galaxy` is the accepted read-only strategic cockpit and already exposes visibility-oriented information.
- `Planet`, `Construction`, `Research`, `Shipyard`, `Fleets`, `Defenses`, and `Ground Army` have cockpit-specific read or guarded-action boundaries.
- `ai/current-state.md` explicitly lists `no espionage gameplay` and `no espionage execution` as current exclusions.
- Future Espionage work must therefore stay read-only, Development-safe, and honest about uncertainty.

## Files to read first

- `ai/current-state.md`
- `docs/dev/strategic-map-cockpit-checklist.md`
- `docs/dev/planet-module-boundaries.md`
- `src/VoidEmpires.Domain/`
- `src/VoidEmpires.Domain/Planets/`
- `src/VoidEmpires.Domain/Fleets/`
- `src/VoidEmpires.Application/`
- `src/VoidEmpires.Infrastructure/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/`

## Component discovery

Inspect the current ownership, exploration, visibility, sensor, detection, interception-readiness, fleet, transfer, and strategic-map read-model components first. Prefer reusing those concepts rather than inventing a parallel espionage domain.

## Dependency analysis

Map the current chain that supplies strategic intelligence to the frontend, for example:

- `DevEndpointMappings` -> strategic map read endpoint -> application/infrastructure read services
- seeded development data -> strategic visibility/exploration knowledge -> read DTOs
- fleet/transfer persistence -> strategic overlays and detection summaries

Document which components call which services so later tasks know where a safe Espionage read model can be assembled.

## Implementation requirements

1. Inventory the current read-only intelligence concepts already implemented in the repository:
   - owned systems and planets
   - visible systems and planets
   - partial-contact or unknown nodes
   - exploration knowledge
   - fleet markers
   - transfer visibility
   - sensor profile notes
   - detection coverage notes
   - interception-readiness notes
   - any existing domain names that mention espionage, intelligence, observation, detection, reconnaissance, or contact
2. Identify whether a dedicated backend service or read model already exists that can support `/espionage`.
3. Identify which data should remain in `Galaxy` because it is map-first, and which data can be reframed in `Espionaje` because it is analysis-first.
4. Define the safe v1 scope explicitly:
   - selected civilization context
   - known, visible, owned, observed, and partial targets
   - confidence and coverage framing if derivable
   - passive signals if already available from existing read models or seed data
   - disabled future mission placeholders
   - no active mission creation or execution
5. Record the findings in `docs/dev/espionage-cockpit-checklist.md` or an equivalent dedicated contract note if that checklist does not exist yet.
6. If documentation alone is insufficient and a tiny backend display metadata change is required to support the audit, cover it with tests.

## UI/UX requirements

- This task does not need to ship a cockpit UI.
- The audit must define language that allows Espionage to communicate uncertainty without implying that active spying exists.
- Any discovered raw enums, DTO names, capability keys, or request names should be marked as diagnostics-only material for later tasks.

## Backend/API requirements

- Prefer docs and tests only.
- Reuse existing Development-only strategic read surfaces where possible.
- Do not add a production espionage endpoint in this task.
- If code changes are unavoidable, keep them read-only and add or update tests.

## Expected files to modify

- `docs/dev/espionage-cockpit-checklist.md` if created in this task
- a backend contract note under `docs/dev/` if a checklist is not the right target
- backend tests only if tiny audit-supporting code changes are required

## Safety constraints

- No espionage execution
- No spy mission creation
- No sabotage
- No infiltration
- No counter-espionage execution
- No resource or technology theft
- No combat or interception execution
- No fleet mutation
- No Galaxy mutation
- No WebSockets

## Acceptance criteria

- The repository has an explicit written scope for `Espionaje v1`.
- Existing intelligence and visibility concepts are inventoried with references to the real components that expose them.
- Safe boundaries clearly distinguish read-only intelligence analysis from active mission gameplay.
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

- The audit may confirm that no dedicated espionage backend exists yet; if so, the correct outcome is a documented reuse plan based on strategic visibility and read-only state, not a rushed new gameplay subsystem.
- If current partial-intelligence support is thinner than expected, later tasks must present that limitation honestly instead of inventing false precision.

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
