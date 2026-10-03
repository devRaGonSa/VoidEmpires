# TASK-51B

---
id: TASK-51B
title: Fleet mission domain model
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 2
dependencies: ["TASK-51A-fleet-domain-and-existing-system-audit.md"]
---

## Goal

Introduce or consolidate the persistent mission model.

Requirements:
Create/reuse a domain representation capable of storing:
- MissionId
- CivilizationId
- OriginPlanetId
- DestinationPlanetId
- MissionType
- Status
- OutboundDepartureUtc
- OutboundArrivalUtc
- ReturnDepartureUtc nullable
- ReturnArrivalUtc nullable
- CreatedAtUtc
- CompletedAtUtc nullable
- RecalledAtUtc nullable
- deterministic mission metadata as necessary

Statuses should support at least:
- Preparing
- Outbound
- AtDestination / Processing if needed
- Returning
- Completed
- Recalled
- Cancelled / Failed if architecture requires it

Do not add combat-specific fields.

Add domain tests.

## Context

Consolidate the current transfer lifecycle through the audit-approved mission aggregate; do not introduce competing movement authority.

Prerequisites: `TASK-51A-fleet-domain-and-existing-system-audit.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Implement all requested fields/statuses and UTC invariants; keep enum declarations with the focused aggregate if appropriate. Return is an internal leg.
2. Map using existing EF conventions, navigation discovery and a conditional transition/version seam for later atomic claims; no migration generation yet.
3. Define legal/idempotent transitions. Recalled intent cannot free ships/slots before timed return settlement.

## Files to read first

- `src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalTransferStatus.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/OrbitalTransferConfiguration.cs`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `tests/VoidEmpires.Tests/OrbitalTransferTests.cs`

## Expected files to modify

- `src/VoidEmpires.Domain/Fleets/FleetMission.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/FleetMissionConfiguration.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs`
- `tests/VoidEmpires.Tests/FleetMissionTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Test IDs/enums/UTC/time ordering, allowed transitions, terminal immutability and repeated/out-of-order transitions.
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
