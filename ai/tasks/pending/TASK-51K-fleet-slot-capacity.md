# TASK-51K

---
id: TASK-51K
title: Fleet slot capacity
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 11
dependencies: ["TASK-51B-fleet-mission-domain-model.md"]
---

## Goal

Add fleet mission slot rules.

Purpose:
Limit simultaneous active missions per civilization.

V1:
- define baseline slots;
- optionally derive additional slots from an existing research technology if logical.

Must count:
- outbound;
- returning;
- other active persisted states.

Completed missions do not consume slots.

Tests required.

## Context

Slot limits span all planets of a civilization and require transactional last-slot arbitration at launch.

Prerequisites: `TASK-51B-fleet-mission-domain-model.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Define baseline slots and only add an existing logical research modifier if justified.
2. Count all asset-holding active phases including preparing/reserved, outbound, processing and recalled returns; only settled terminal missions release slots.
3. Return typed used/max/remaining and a shared predicate. O must recheck atomically; count-then-save alone is unsafe.

## Files to read first

- `src/VoidEmpires.Domain/Research/ResearchCatalog.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferActivityQueries.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `tests/VoidEmpires.Tests/FleetOperationalOverviewServiceTests.cs`

## Expected files to modify

- `src/VoidEmpires.Domain/Fleets/FleetSlotPolicy.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Fleets/FleetSlotQueryService.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Application/Fleets/IFleetSlotQueryService.cs` (new/proposed; reconcile with A audit)
- `tests/VoidEmpires.Tests/FleetSlotPolicyTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Test boundaries, completed/history exclusion, all returning inclusion, civilization isolation and optional research policy.
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
