# TASK-51O

---
id: TASK-51O
title: Fleet launch transaction
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 15
dependencies: ["TASK-51B-fleet-mission-domain-model.md", "TASK-51C-fleet-mission-composition-model.md", "TASK-51D-fleet-cargo-model.md", "TASK-51K-fleet-slot-capacity.md", "TASK-51L-origin-fleet-availability-service.md", "TASK-51M-fleet-launch-request-contract.md", "TASK-51N-fleet-launch-validation.md"]
---

## Goal

Implement atomic fleet launch.

Within one logical transaction:
- validate;
- deduct/reserve ships;
- deduct fuel;
- deduct transported cargo;
- create mission;
- create composition rows;
- create cargo state;
- set status Outbound.

If persistence fails:
- do not partially deduct assets/resources.

Tests required.

## Context

Existing spending helpers save internally; use the audited fleet transaction boundary and construction conditional-claim patterns.

Prerequisites: `TASK-51B-fleet-mission-domain-model.md`, `TASK-51C-fleet-mission-composition-model.md`, `TASK-51D-fleet-cargo-model.md`, `TASK-51K-fleet-slot-capacity.md`, `TASK-51L-origin-fleet-availability-service.md`, `TASK-51M-fleet-launch-request-contract.md`, `TASK-51N-fleet-launch-validation.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Within one unit of work revalidate ownership/destination/resources/slots, reserve ships, deduct cargo/prepaid fuel, persist children and transition Outbound.
2. Claim M request key with database uniqueness and canonical payload comparison in the same transaction. Same operation returns existing result; changed payload spends nothing.
3. Use provider-appropriate conditional/serializable protection and fresh-state retry. Serialize last-slot claim as well as stock/resources; P/Q focus hardening without leaving a knowingly unsafe success path.
4. Wire DI once. Any save/commit/cancellation failure rolls back every deduction and claim; independent contexts see no partial mission.

## Files to read first

- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferPersistenceService.cs`
- `src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `tests/VoidEmpires.Tests/OrbitalTransferPersistenceServiceTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Fleets/FleetMissionLaunchService.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/FleetMissionConfiguration.cs` (new/proposed; reconcile with A audit)
- `tests/VoidEmpires.Tests/FleetMissionLaunchServiceTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Test success, duplicate operation, conflicting payload, failed-save rollback, last-slot race and cancellation without requiring SQL Server.
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
