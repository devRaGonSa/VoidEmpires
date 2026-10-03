# TASK-51D

---
id: TASK-51D
title: Fleet cargo model
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 4
dependencies: ["TASK-51B-fleet-mission-domain-model.md", "TASK-51C-fleet-mission-composition-model.md"]
---

## Goal

Add mission cargo for supported resource transport.

Support:
- Credits
- Metal
- Crystal
- Gas

Cargo invariants:
- no negative amounts;
- no cargo greater than available resources;
- no cargo greater than fleet cargo capacity;
- resources reserved/deducted exactly once.

Do not add market functionality.

Tests required.

## Context

Model cargo here; O/Q own atomic deduction. Credits/Metal/Crystal/Gas already exist in stockpiles.

Prerequisites: `TASK-51B-fleet-mission-domain-model.md`, `TASK-51C-fleet-mission-composition-model.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Add nonnegative decimal cargo with precision/units matching stockpile mappings and checked totals.
2. Provide pure validation against supplied available resources/capacity; J/N/O supply authoritative context. Include combined cargo Gas and reserved fuel affordability.
3. Persist loaded/delivered/remaining semantics so delivery and recall cannot credit one amount twice; no constructor storage/debits.

## Files to read first

- `src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs`
- `src/VoidEmpires.Domain/Economy/ResourceType.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetResourceStockpileConfiguration.cs`
- `tests/VoidEmpires.Tests/PlanetResourceStockpileDomainTests.cs`

## Expected files to modify

- `src/VoidEmpires.Domain/Fleets/FleetMissionCargo.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Domain/Fleets/FleetMission.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/FleetMissionCargoConfiguration.cs` (new/proposed; reconcile with A audit)
- `tests/VoidEmpires.Tests/FleetMissionCargoTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Test zero cargo, each negative amount, exact resource/capacity boundaries, one over, precision and cargo Gas plus fuel.
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
