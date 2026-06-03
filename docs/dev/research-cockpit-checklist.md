# Research Cockpit Backend Contract

This note documents the current Research backend surface so later cockpit work can stay dev-safe and avoid inventing gameplay or endpoints.

## Current backend reality

- `src/VoidEmpires.Domain/Research/ResearchType.cs` defines the stable research ids as an enum.
- `src/VoidEmpires.Domain/Research/ResearchCatalog.cs` defines the current catalog entries with base costs and bonus keys.
- The catalog does not currently expose display names, categories, prerequisites, or per-tech duration fields.
- `src/VoidEmpires.Domain/Research/ResearchProject.cs` persists civilization research level state.
- `src/VoidEmpires.Domain/Research/ResearchOrder.cs` persists queue state with civilization id, source planet id, research type, target level, sequence, start/end timestamps, and status.
- `src/VoidEmpires.Application/Research/IResearchQueueService.cs` and `src/VoidEmpires.Infrastructure/Research/ResearchQueueService.cs` implement the dev-safe enqueue flow.
- `src/VoidEmpires.Application/Research/IResearchOrderCompletionService.cs` and `src/VoidEmpires.Infrastructure/Research/ResearchOrderCompletionService.cs` implement due-order completion.
- `src/VoidEmpires.Infrastructure/Research/ResearchUpgradeService.cs` exists as a persistence-backed upgrade service, but it is not exposed by a dev HTTP endpoint.

## What the backend can do today

- Enqueue a research order for a civilization and source planet through `POST /api/dev/research/orders/enqueue`.
- Complete due research orders through `POST /api/dev/research/orders/complete-due`.
- Persist completed research levels in `ResearchProjects`.
- Derive research bonuses through `ResearchBonusCalculator`.

## What the backend cannot do yet

- There is no research catalog read endpoint.
- There is no list endpoint for the current queue.
- There is no completed-items read endpoint.
- There is no current-queue read model endpoint.
- There is no research UI route that should claim to be fully playable on top of the backend alone.

## Safe v1 scope

The first Research cockpit can safely present these pieces:

- catalog rows from the known `ResearchType` values
- visible requirements, costs, and estimated durations
- readiness and availability as read-only guidance
- explicit-confirmation enqueue only when the dev endpoint is available and tested
- explicit-confirmation complete-due only when the dev endpoint is available and tested

The first Research cockpit must keep these boundaries:

- no fake or implied tech effects beyond the existing persisted research levels and bonus calculations
- no hidden unlock logic
- no production-only mutation routes
- no claim that list, current queue, or completed items are available from the backend until a real read endpoint exists

## Known effects already wired in the codebase

- `ResearchProject` levels are consumed by planet economy, construction, and building-capacity calculations.
- `ResearchBonusCalculator` exposes the current bonus values for supported research types.
- `ResearchQueueService` spends stockpile resources when an order is enqueued.
- `ResearchOrderCompletionService` marks due orders completed and upgrades or creates the persisted project record.

## Test coverage status

- Service and domain tests already cover catalog behavior, duration math, enqueue validation, resource spending, and due-order completion.
- No dedicated HTTP endpoint tests for the research dev routes were found during this audit.

## Practical frontend guidance

- Treat Research as a dev-only cockpit with explicit confirmation on mutating actions.
- Keep unsupported queue/history views as placeholders until a read endpoint exists.
- Prefer Spanish-first labels for player-facing text, but keep technical ids available as secondary metadata.

