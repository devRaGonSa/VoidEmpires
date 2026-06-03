# TASK-13Q

---
id: TASK-13Q
title: Phase 13Q - Planet cockpit API discovery and read model
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Establish the Planet cockpit read model by reusing existing backend data first and adding a development-only cockpit read endpoint only if necessary.

## Context
The next gameplay surface is Planet or Construction. The repository already has persisted planet, ownership, economy, building, construction, research, and asset-production foundations. The first Planet cockpit task should discover the current backend surface area before adding any new HTTP contract.

## Implementation steps

1. Inspect existing frontend API clients and backend dev endpoints for planet, economy, building, and construction data.
2. Reuse existing read endpoints where practical for the first cockpit slice.
3. If no single adequate read model exists, add a development-only Planet cockpit read endpoint that follows existing dev-route conventions.
4. Add backend tests only if backend changes are introduced, including dev-only visibility and current persistence error patterns.

## Files to read first

- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Web/Dev*Planet*.cs`, if present
- `src/VoidEmpires.Application/Buildings/`
- `src/VoidEmpires.Infrastructure/Buildings/`
- `tests/VoidEmpires.Tests/`

## Expected files to modify

- `src/VoidEmpires.Web/*Planet*` endpoint files, if needed
- `src/VoidEmpires.Application/*Planet*` or building-query contracts, if needed
- `src/VoidEmpires.Infrastructure/*Planet*` or building-query services, if needed
- focused tests for the new read model, if backend changes are added

## Acceptance criteria

- The task identifies whether the current backend already supports the Planet cockpit read slice.
- The resulting read model can support selected planet identity, ownership, type, colonization state, stockpiles, production hints, buildings, queue items, available actions, and compact diagnostics.
- Any new HTTP route stays development-only and follows current response conventions.
- Backend tests cover dev-only gating and relevant success or failure patterns when backend changes are introduced.

## Constraints

- Prefer reusing existing backend read contracts.
- Avoid new persistence or migrations unless strictly required.
- Keep the read model deterministic and seed-friendly.
- Do not add production auth or unrelated gameplay systems.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
