# TASK-23H

---
id: TASK-23H
title: Phase 23H - Market reference prices and ratios
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Show deterministic read-only reference prices or ratios without implementing a real market.

## Purpose

A Market cockpit needs price-like information, but the repository must not silently introduce real dynamic pricing, transaction execution, or player-facing exchange mechanics in this phase.

## Current problem

The current codebase has resources, costs, and production, but no real exchange system. Market still needs a safe reference layer so the cockpit feels useful without misrepresenting gameplay.

## Context

Reference values should be clearly marked as advisory. They can be derived from existing resource relationships or deterministic demo logic, but they must never look like executable offers.

## Files to read first

- Market read-model or service files
- Market view-model and presentation helpers
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `docs/dev/market-cockpit-checklist.md`

## Component discovery

Inspect current cost tables, affordability logic, resource labels, and any existing ratio-like metadata. Prefer deterministic reuse of current resource relationships over adding a dynamic pricing subsystem.

## Dependency analysis

Possible data paths:

- backend read model derives reference rows -> Market DTO -> frontend table or cards
- frontend view model derives advisory ratios from existing deterministic fields -> page presentation

Whichever path is chosen, it must be deterministic and clearly labeled as read-only.

## Implementation requirements

1. Add a read-only reference price or ratio display.
2. Use explicit advisory labels such as:
   - `Referencia de intercambio`
   - `Ratio orientativo`
   - `Precio no ejecutable`
   - `Solo lectura`
3. Support safe examples such as:
   - `Metal <-> Cristal`
   - `Metal <-> Gas`
   - `Cristal <-> Gas`
   - `Creditos <-> recursos`
   only if those rows are deterministic and safe.
4. Make clear that:
   - these are not active offers
   - no trade can be confirmed from this cockpit
5. If backend support does not already provide ratios, derive a deterministic demo ratio in the read model or frontend mapper and label it as a reference only.
6. Add tests if backend code computes or shapes ratios.

## UI/UX requirements

- Avoid implying a live market
- Spanish-first
- Use a table or compact cards that read as advisory data, not interactive listings
- Keep disabled or explanatory copy visible near the ratio display

## Backend/API requirements

- If ratios are backend-derived, keep the logic deterministic and cover it with tests.
- Do not add price mutation or persistence.

## Expected files to modify

- Market page or helper files for ratio presentation
- Market backend read-model files only if ratio shaping is implemented there
- tests if backend derivation is added

## Safety constraints

- No transactions
- No real exchange execution
- No resource mutation
- No simulated order book

## Acceptance criteria

- Reference prices or ratios are visible in Market.
- Labels clearly state that the values are read-only and advisory.
- Build and tests pass if backend is touched.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Run backend validation if backend files are changed:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- Real price formation remains a future system and should not be implied by this v1 reference layer.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is limited to advisory ratio presentation or deterministic read shaping.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a simple deterministic advisory layer over complex formula tuning.
- If realistic pricing logic starts emerging, stop and create a future gameplay task instead.
