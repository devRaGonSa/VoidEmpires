# TASK-51R

---
id: TASK-51R
title: Fleet arrival materialization service
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 18
dependencies: ["TASK-51B-fleet-mission-domain-model.md", "TASK-51O-fleet-launch-transaction.md"]
---

## Goal

Create central mission arrival processor.

Given a due outbound mission:
- claim it idempotently;
- execute mission-specific arrival behavior;
- transition state;
- create return leg when required;
- never execute twice.

Must be safe under concurrent normal reads/workers.

Tests required.

## Context

Existing transfer completion and worker are integration seams, not permission for a second independent mission processor.

Prerequisites: `TASK-51B-fleet-mission-domain-model.md`, `TASK-51O-fleet-launch-transaction.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Select bounded due outbound phases with stable ordering, UTC now and explicit civilization/request scope or global-worker mode.
2. Atomically claim phase, execute effects and persist next phase in one transaction. Crash/retry cannot leave an unrecoverable Processing claim.
3. Provide focused handlers for S/T/V/X; unsupported handlers fail safely. Use scheduled phase timestamps so delayed reads do not arbitrarily restart journey duration.
4. Repeated read/worker calls have zero additional effects; AC/AD/BL consolidate legacy integration.

## Files to read first

- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferCompletionService.cs`
- `src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs`
- `src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `tests/VoidEmpires.Tests/OrbitalTransferCompletionServiceTests.cs`

## Expected files to modify

- `src/VoidEmpires.Application/Fleets/IFleetMissionArrivalService.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Fleets/FleetMissionArrivalService.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `tests/VoidEmpires.Tests/FleetMissionArrivalServiceTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Test not due/exact due, duplicate/concurrent claim, handler rollback, unsupported mission and delayed return schedule.
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
