# TASK-51C

---
id: TASK-51C
title: Fleet mission composition model
status: done
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 3
dependencies: ["TASK-51B-fleet-mission-domain-model.md"]
---

## Goal

Persist the ships attached to a mission.

Requirements:
Mission composition must support several ship types simultaneously.

Example:
- 10 ScoutCraft
- 5 EscortCraft
- 2 CargoCraft

Use a normalized child entity or equivalent robust representation.

Must record:
- mission id
- ship type
- quantity

Quantity must be > 0.

Do not use frontend-only serialized composition as authoritative storage.

Tests required.

## Context

OrbitalGroup has one SpaceAssetType. Mixed mission composition needs normalized rows and one holder of each ship.

Prerequisites: `TASK-51B-fleet-mission-domain-model.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Add mission-owned positive integral quantities of known types; consistently merge or reject duplicate types.
2. Map mission/type uniqueness, parent association and quantity constraints. Persist composition independently of frontend JSON.
3. Keep group adaptation at the audited seam; no stock allocation rewrite or duplicate stationary representation.

## Files to read first

- `src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs`
- `src/VoidEmpires.Domain/Assets/SpaceAssetType.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/OrbitalGroupConfiguration.cs`
- `tests/VoidEmpires.Tests/OrbitalGroupTests.cs`

## Expected files to modify

- `src/VoidEmpires.Domain/Fleets/FleetMissionShip.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Domain/Fleets/FleetMission.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/FleetMissionShipConfiguration.cs` (new/proposed; reconcile with A audit)
- `tests/VoidEmpires.Tests/FleetMissionCompositionTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Round-trip 10 ScoutCraft/5 EscortCraft/2 CargoCraft; test zero/negative/duplicate/unknown types and overflow.
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

## Completion evidence (2026-10-06)

- Started from clean synchronized `d9993fad7297677bf9a1f6027089eb7c3bc35e79`, with exact A/B prerequisites done and 102 Block 51 tasks pending.
- Added FleetMissionShip with nonempty MissionId, known SpaceAssetType and positive int Quantity, private scalar setters and internal quantity mutation. FleetMission owns a private list exposed through a cached read-only wrapper.
- AddShips is Preparing-only; duplicate types merge into the existing row. Checked quantity and version calculations precede mutation. Successful additions/merges increment StateVersion through the same helper as lifecycle changes; failures leave composition, phase and version unchanged. Empty-composition launch behavior from B remains unchanged.
- EF maps composite primary key (MissionId, AssetType), required scalar fields and a required one-to-many FK to FleetMission with cascade deletion and field access. No extra DbSet, parent mapping changes or OrbitalGroup adapter.
- No CHECK-constraint convention exists in the current configuration files. Positivity remains enforced by the domain plus required EF mapping; final relational constraints belong to TASK-51BH. No provider-specific SQL introduced.
- Fresh restore/build passed with 0 warnings/errors. Focused composition tests: 21 passed, 0 failed, 0 skipped. Full suite: 928 passed, 0 failed, 0 skipped, including unchanged B lifecycle tests. Explicit Include round-trip preserved 10 ScoutCraft/5 EscortCraft/2 CargoCraft; tracked merge and cascade also passed in EF InMemory, without claims about relational constraints or concurrency.
- SQL smoke explicitly disabled; its early return is not live SQL coverage. No integration tests configured. The integration hook remains an unadapted placeholder. Repository secret scan passed.
- Diff/scope checks passed: four expected implementation files (70 changed production lines and 181 test lines), plus this lifecycle record. Full coverage follows the user's earlier explicit authorization to ignore the line limit; no scope expansion or follow-up tasks.
- No migration generation/application, SQL execution, seed mutation, browser/manual QA or PR. All other 101 Block 51 tasks remain pending and unchanged; no Block 52. Stop after this task's commit/push.
