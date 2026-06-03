# TASK-16F-minimal-validation-seed-research-available-item-fix

---
id: TASK-16F-minimal-validation-seed-research-available-item-fix
title: Minimal validation seed research available item fix
status: obsolete
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Ensure the `minimal-validation` seed actually produces at least one available Research item for Aurelia.

## Purpose
The Research cockpit cannot be manually QAed if every technology is blocked. We need one safe, deterministic available item and several blocked ones so the UI can prove the full flow.

## Current Problem
The visual QA result contradicts the previous block: the seed was expected to give one available item, but the live state shows none. That blocks validation of confirmation, enqueue, queue refresh, and blocked-versus-available contrast.

## Context
- `DevelopmentSeedService` already seeds Aurelia, resources, buildings, and other QA scaffolding.
- The fix should preserve the existing Galaxy, Fleet, Planet, and Construction seed expectations.
- The intended result is not a fake production-like tech tree; it is a deterministic QA seed with just enough room to test the cockpit.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs` or equivalent
- Research catalog and availability service/domain files
- Any tests covering seeded UI-state counts

## Implementation Requirements
1. Adjust the `minimal-validation` seed or research readiness data so that:
   - one research item is available;
   - several research items remain blocked;
   - resources are sufficient for the available item;
   - prerequisites are satisfied for the available item;
   - queue state allows enqueueing;
   - the result remains deterministic.
2. Keep the seed conservative; do not overfund it beyond QA needs.
3. Ensure the available item is simple and early-game if the catalog supports that.
4. Update tests to assert the intended seeded distribution.
5. If the relevant resource scope is planet-based, ensure Aurelia has enough in that scope.
6. If the relevant scope is civilization-wide, seed that scope instead.

## UI/UX Requirements
- No frontend changes are required in this task.
- The backend seed must support a visible green or available state in the cockpit.
- The resulting UI should still show blocked items for contrast.

## Backend/API Requirements
- Tests must prove the backend state, not only the rendered frontend.
- Prefer a minimal seed adjustment over introducing new gameplay logic.
- Do not add migrations unless the current model absolutely requires them.

## Safety Constraints
- Do not create production seed behavior.
- Do not commit secrets.
- Do not silently alter other cockpit seeds.
- Do not apply migrations automatically.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs` or the nearest Research endpoint test file

## Acceptance Criteria
- `minimal-validation` returns at least one available Research action.
- Tests cover the expected available and blocked distribution.
- Build and tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched.

## Notes / Residual Risks
- This task was satisfied by the deterministic stockpile restore added while completing `TASK-16E-research-availability-root-cause-audit`.
- The chosen available Research should be simple and early-game, for example `Ingenieria planetaria` or `Extraccion de recursos`, depending on the actual catalog rules.
- If the catalog or resource model changed since the last block, update the seed against the current contract rather than against the old expectation.
- Keep the fix deterministic so browser QA can repeat it exactly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer seed and test adjustments over new systems.
