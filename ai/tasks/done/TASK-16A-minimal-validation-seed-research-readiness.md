# TASK-16A-minimal-validation-seed-research-readiness

---
id: TASK-16A-minimal-validation-seed-research-readiness
title: Minimal validation seed research readiness
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Ensure the minimal-validation seed supports meaningful Research cockpit QA.

## Purpose
The Research page cannot be manually validated if the seed does not provide at least one useful available item, one blocked item and a predictable context.

## Current Problem
DevelopmentSeedService already supports other cockpit QA flows. Research needs the minimum required seed support without turning into fake gameplay.

## Context
- The seed must stay deterministic.
- The seed should support manual QA, not production-like progression inflation.
- Keep Galaxy read-only and do not create hidden completion effects.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Research tests, if present
- Research catalog or domain seed assumptions

## Implementation Requirements
1. Inspect whether research data is catalog-driven or persisted.
2. Ensure minimal-validation supports:
   - one civilization;
   - selected planet Aurelia;
   - at least one available research item;
   - at least one blocked research item;
   - at least one item with a clear prerequisite or resource blocker if supported;
   - empty or readable initial research queue;
   - enough resources for one safe research enqueue if enqueue is enabled.
3. Keep the seed deterministic.
4. Do not overfund resources beyond QA needs.
5. Update tests to assert research readiness if backend data is involved.
6. Document seed ids in the Research docs or checklist.

## UI/UX Requirements
- The seeded Research page should show both available and blocked examples.
- The seed should make manual review fast, not noisy.

## Backend/API Requirements
- Add or update tests if seed changes.
- Avoid migrations unless required by existing model changes.
- Do not introduce any production-only assumptions.

## Safety Constraints
- No fake production data.
- Do not create hidden completed research effects.
- Do not broaden seed scope into combat or fleet behavior.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Possibly a research-specific test or doc file

## Acceptance Criteria
- `minimal-validation` supports Research cockpit manual QA.
- Tests pass.
- `npm run build --prefix src/VoidEmpires.Frontend` passes if frontend docs or types change.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend touched.

## Notes / Residual Risks
- If Research catalog is static, the seed may only need resources and context, not catalog rows.
- Keep the seed small and explicit.
- Favor readability over breadth.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer deterministic seed adjustments only.
