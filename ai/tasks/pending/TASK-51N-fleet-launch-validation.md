# TASK-51N

---
id: TASK-51N
title: Fleet launch validation
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 14
dependencies: ["TASK-51F-fleet-speed-calculation.md", "TASK-51G-galactic-distance-calculation.md", "TASK-51H-travel-time-calculation.md", "TASK-51I-fuel-consumption-calculation.md", "TASK-51J-fleet-cargo-capacity-calculation.md", "TASK-51K-fleet-slot-capacity.md", "TASK-51L-origin-fleet-availability-service.md", "TASK-51M-fleet-launch-request-contract.md"]
---

## Goal

Centralize mission launch validation.

Validate:
- civilization owns origin;
- destination exists;
- ships available;
- quantities valid;
- fuel available;
- cargo resources available;
- cargo capacity sufficient;
- fleet slot available;
- mission type allowed for destination;
- no sending fleet to same planet unless valid special action;
- colonization rules;
- deploy destination ownership;
- transport destination rules.

Return structured player-facing errors.

Tests required.

## Context

Read-only evaluation is shared by authenticated preview and transactional launch. W later connects colonization rules; unsupported handlers fail closed until wired.

Prerequisites: `TASK-51F-fleet-speed-calculation.md`, `TASK-51G-galactic-distance-calculation.md`, `TASK-51H-travel-time-calculation.md`, `TASK-51I-fuel-consumption-calculation.md`, `TASK-51J-fleet-cargo-capacity-calculation.md`, `TASK-51K-fleet-slot-capacity.md`, `TASK-51L-origin-fleet-availability-service.md`, `TASK-51M-fleet-launch-request-contract.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Compose predecessor results into a mutation-free validation/preview snapshot covering every original requirement.
2. Resolve visible destinations and mission eligibility. Prefer owned-destination Transport in v1 unless audit explicitly supports safe foreign delivery; Deploy requires same owner; Colonize requires W; Explore reuses visibility rules.
3. Check cargo Gas plus fuel, every balance, capacity, speed/enums/quantities, no same-planet dispatch, slots and fresh stock. Return stable codes for Spanish copy.
4. Expose re-evaluation inside O transaction with injected time/cancellation. Missing colony policy is a rejection, never fake success.

## Files to read first

- `src/VoidEmpires.Infrastructure/Fleets/OrbitalGroupPlannerService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalFuelReadinessService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalStockGroupService.cs`
- `src/VoidEmpires.Infrastructure/Colonization/PlanetColonizationService.cs`
- `src/VoidEmpires.Web/AccountEndpoints.cs`

## Expected files to modify

- `src/VoidEmpires.Application/Fleets/IFleetMissionLaunchValidator.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Fleets/FleetMissionLaunchValidator.cs` (new/proposed; reconcile with A audit)
- `tests/VoidEmpires.Tests/FleetMissionLaunchValidationTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Table-driven tests cover every launch check, invisible destinations, duplicate types, forged civilization, empty/overflow input and unchanged balances after preview.
- Every original Goal requirement is covered by this slice or a named later integration task; missing prerequisites are explicit, never fake success.
- Relevant deterministic tests/validation pass; no build artifacts or secrets are committed.

## Constraints

- Backend owns state, time, accounting and results in the .NET 8 layered persistent multiplayer universe.
- No combat, attack/espionage execution, loot, market/trade routes or final assets. Keep Spanish-first UI and authenticated shell.
- Never automatically apply migrations, SQL or seeds. Preserve SQL Server support and provider-independent ordinary tests; do not change default provider to match a local setting.
- In-process locks and InMemory-only tests do not prove cross-process SQL correctness; document provider limits and use existing conditional-write/transaction patterns.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build` (focused cases above plus suite; report actual count).
- For storage/services/workers inspect configured integration tests. Current `scripts/run-integration-tests.ps1` is an unadapted placeholder: do not use it as evidence; log `No integration tests configured.` If later adapted, run configured suite and require success.
- Run `git diff --stat` and `git diff --name-only` (also staged equivalents after staging), compare allowlist and budget. Never claim manual SQL/browser QA without execution.

## Commit and push

1. Check `git status`; pull configured upstream per workflow before implementation.
2. Move this task to in-progress, implement/validate, stage intended files and commit a focused change.
3. Move the completed task to done, include lifecycle update in task cycle, and push configured feature branch after validation. Preserve unrelated legacy in-progress tasks.

## Change Budget

- Prefer fewer than 5 implementation files, under 200 changed lines and fewer than 3 commits; count generated changes honestly.
- Stop before exceeding budget and refine/split minimum follow-ups during implementation under AGENTS.md (at most 3 at once). Do not duplicate existing tasks. This planning pass creates only the 104 requested files.
