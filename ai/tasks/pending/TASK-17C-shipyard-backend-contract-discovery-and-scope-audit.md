# TASK-17C-shipyard-backend-contract-discovery-and-scope-audit

---
id: TASK-17C-shipyard-backend-contract-discovery-and-scope-audit
title: Shipyard backend contract discovery and scope audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Audit the real backend, domain, application, infrastructure, web, and test surface related to Shipyard so the v1 cockpit only uses safe supported behavior.

## Purpose
Turn Shipyard planning into an evidence-based backlog. This task defines what the current repository already supports for orbital asset production and what must remain out of scope for Shipyard v1.

## Current Problem
`/shipyard` is still a placeholder-style module boundary page. The repository likely already contains pieces for asset requirements, production queues, stock, and orbital group creation, but the current source of truth is the code, not memory or prior chat state. If Shipyard v1 assumes unsupported behavior, it could duplicate Fleet commands, expose unsafe mutations, or mislead later frontend tasks.

## Context
- Galaxy is accepted as read-only.
- Fleets is accepted as the place to inspect and move existing orbital groups.
- Construction and Research already use cockpit-specific read models and dev-only mutations.
- Shipyard must become a development-safe production cockpit, not a combat, movement, or split/merge module.

## Files to Inspect First
- `ai/current-state.md`
- `docs/dev/fleet-api-contracts.md`
- `docs/dev/fleet-controlled-mutation-checklist.md`
- `src/VoidEmpires.Domain/Assets/`
- `src/VoidEmpires.Domain/Fleets/`
- `src/VoidEmpires.Application/Assets/`
- `src/VoidEmpires.Infrastructure/Assets/`
- `src/VoidEmpires.Web/`
- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`

## Implementation Requirements
1. Inventory existing Shipyard-relevant domain concepts, including:
   - asset type and taxonomy models;
   - production requirements and costs;
   - production order and queue state;
   - stock or inventory ownership models;
   - orbital group creation or allocation concepts.
2. Inventory existing application and infrastructure services, including:
   - catalog or lookup services;
   - enqueue production services;
   - process or complete due production services;
   - stock creation or increment services;
   - orbital group creation from stock services.
3. Identify all existing dev-only endpoints that can affect Shipyard behavior.
4. Identify current automated tests that cover asset production, stock, or orbital group handoff.
5. Document the safe Shipyard v1 scope. The scope must explicitly classify each behavior as:
   - available now;
   - available only through dev-only endpoints;
   - read-only only;
   - unsafe or out of scope for this block.
6. Confirm whether Shipyard v1 can safely support:
   - reading available orbital asset production options;
   - showing requirements, costs, and durations;
   - showing local planetary or orbital stock;
   - showing a production queue;
   - enqueueing one safe asset production order;
   - completing due production;
   - linking to Fleets for existing orbital groups.
7. Record the findings in `docs/dev/shipyard-cockpit-checklist.md` or a narrowly scoped Shipyard backend note if that checklist does not yet exist.

## UI/UX Requirements
- No UI implementation is required here.
- The audit must provide enough factual detail that later frontend tasks can render truthful available, blocked, and disabled states in Spanish.
- Diagnostics-oriented findings should clearly separate player-facing labels from backend keys.

## Backend/API Requirements
- Prefer documentation and test coverage only.
- If a small code correction is needed to make the audit truthful, keep it narrow and add tests.
- Do not introduce production endpoints.

## Safety Constraints
- Do not add fleet movement behavior.
- Do not enable split or merge.
- Do not add combat, interception, espionage, alliances, pacts, WebSockets, or auth.
- Do not bypass resource or ownership validation.
- Do not auto-apply migrations to a real database.

## Expected Files to Modify
- `docs/dev/shipyard-cockpit-checklist.md` or a Shipyard-specific backend contract note
- Narrow backend or test files only if a small corrective change is needed to document the real surface

## Acceptance Criteria
- The existing Shipyard-relevant backend surface is documented from real code inspection.
- The safe Shipyard v1 scope is explicit and bounded.
- Later tasks know which endpoints, services, and tests are safe to reuse.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` only if frontend files are touched.

## Notes / Residual Risks
- If backend support is thinner than expected, Shipyard v1 must remain partially or fully read-only.
- The audit must not overclaim orbital group allocation support just because related domain types exist.
- Keep the output usable for both backend and frontend follow-up work.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer documentation and pinpoint tests over broad refactors.
