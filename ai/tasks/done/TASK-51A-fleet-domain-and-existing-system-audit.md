# TASK-51A

---
id: TASK-51A
title: Fleet domain and existing system audit
status: done
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 1
dependencies: []
---

## Goal

Audit all existing fleet, orbital movement and ownership code before modifying the architecture.

Inspect:
- OrbitalGroup
- OrbitalTransfer
- OrbitalAssetStock
- SpaceAssetType
- asset production
- planet ownership
- galaxy/solar-system coordinates
- fleet UI state services
- fleet endpoints
- existing movement foundations
- EF mappings and migrations
- current dev/test routes

Document:
- what can be reused;
- what should be extended;
- what is obsolete;
- what invariants already exist;
- where movement state currently lives.

Do not change gameplay in this task.

Validation:
dotnet build --no-restore
dotnet test --no-build

## Context

Architecture and execution-order gate only. Plan base: Block54 commit 472c6d29102eaea7211c6a33e2c67aed58d4ec8c, eight commits ahead of fetched origin/main 4d8c5bf362ada4580a911b1c2dc0fcf3fa8f5d40. Actual 2026-10-03 validation: main 807 passing tests; plan base 814 passing tests; both zero failures/skips and zero build warnings/errors. Re-run after synchronization; historical 799 is stale.

Planning verification also passed the frontend build and the repository QA, lazy-import, copy, secret and generated-SQL safety guards. No integration tests configured. No database apply, seed, browser/manual QA or gameplay implementation was performed. The explicit request authorizes all 104 planning files together; implementation budgets apply independently to later task slices.

Prerequisites: None; required audit gate.

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Trace catalog/production -> OrbitalAssetStock -> OrbitalGroup -> OrbitalTransfer -> completion/cancel -> fleet UI, coordinates and ownership. Inspect linked tests, SQL mappings/migrations and DI incrementally.
2. Document reuse, extension, obsolete behavior and invariants. Groups are single-type; allocation removes stock; arrival relocates group; cancellation instantly releases it. Choose one mission authority with legacy adapters/legs before B. BL/BM verify compatibility later, not redesign architecture.
3. Record placeholder travel (distance 1, one hour, Credits+Gas independent of quantity), StorageCapacity/OperatingRange metadata, missing concurrency guards, unique planet ownership, default Npgsql versus real SQL Server development, opt-in transfer worker, knowledge-only exploration and missing colony bootstrap.
4. Trace account claims -> PlayerProfile -> Civilization. Legacy dev fleet endpoints accept supplied IDs; a planet-filtered read calls global completion; destination projection lists all planet names. New routes must authorize before refresh and filter by visibility.
5. Decide transport destination/overflow conservation, prepaid return fuel, fleet/colony slots, colony ship consumption/escort return, ownership-loss recovery and recalled-returning active status. No combat.
6. Record scheduling by execution_order and dependencies; suffix order is A..Z,AA..AZ,BA..BZ,CA..CZ with Y before X for bootstrap. Existing done51A/B/C are unrelated: use full filenames. Runner only discovers markdown and invokes Codex; metadata is guidance, not claimed scheduler support. Resolve order before dependent work without changing automation in this audit.
7. Write component/dependency map, main versus unmerged54 evidence and stale-document caveats. No gameplay, SQL apply or seed mutation.

## Files to read first

- `ai/current-state.md`
- `src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalStockGroupService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/OrbitalTransferConfiguration.cs`
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`

## Expected files to modify

- `docs/dev/fleet-mission-engine-v1.md` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Every requested audit area has evidence paths and a reuse/extend/obsolete decision.
- Single-authority design and dependency route exist for all remaining tasks; no completed unrelated task is reopened.
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

- Audited synchronized HEAD `15df5e771ff9337e1ffc9731a3178d25cbe9805d`, containing merged Block 54/main `bcf2b530d7d5fa24c9c8a3f8a820cec0e4778557`; the planning context above remains historical.
- Delivered `docs/dev/fleet-mission-engine-v1.md`: source/DI map, one mission authority, inventory/cargo ownership, legacy drain/cutover, product decisions and B-CZ integration route. No gameplay implementation.
- Fresh `dotnet restore`, `dotnet build --no-restore` (0 warnings/errors) and `dotnet test --no-build`: 814 passed, 0 failed, 0 skipped. Restore/test permission failures were resolved by successful reruns with required permissions, without code changes.
- SQL smoke explicitly disabled; its early-return test is counted as passing. No SQL or relational/concurrency validation claimed. No integration tests configured.
- Repository secret scan passed. Diff stat/name/scope checks passed: only the audit and this exact task lifecycle, within the change budget.
- No migrations/SQL/seed applied, no browser/manual QA, no PR. Other 103 Block 51 tasks remain pending and unchanged; no Block 52 created. Stop after this task.
