# TASK-23N

---
id: TASK-23N
title: Phase 23N - Market cockpit docs and smoke checklist
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - qa
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Document Market cockpit behavior, seeded QA URLs, manual smoke steps, and explicit exclusions.

## Purpose

The new cockpit needs repeatable QA guidance so future work does not accidentally claim active transactions or resource mutation before those systems are intentionally introduced.

## Current problem

Market has no cockpit-specific checklist yet. Without one, QA and future orchestration work will rely on memory, and scope drift becomes more likely.

## Context

Existing accepted cockpits already have checklist docs and smoke guidance. Market should gain equivalent documentation as part of the foundation block.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/fleet-api-contracts.md`
- `ai/current-state.md`

## Component discovery

Inspect the current checklist structure, seeded URL format, and screenshot-QA patterns used by accepted cockpits. Prefer matching those docs instead of creating a new documentation style.

## Dependency analysis

Expected doc flow:

- seed profile docs -> how to prepare QA state
- cockpit checklist -> what to inspect on `/market`
- frontend smoke checklist -> where Market fits in the broader accepted demo pass
- current state -> continuity note about scope and exclusions

## Implementation requirements

1. Create `docs/dev/market-cockpit-checklist.md`.
2. Include the seeded QA URL:
   - `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
3. Include checks for:
   - page loads
   - civilization and planet context visible
   - resource summary visible
   - production or economy summary visible if supported
   - reference prices or ratios visible if implemented
   - future Market actions disabled
   - handoff links to Planet, Construction, Shipyard, Fleets, and Galaxy
   - diagnostics collapsed
   - no buying
   - no selling
   - no resource mutation
   - no trade-route execution
4. Update `docs/dev/frontend-foundation-smoke-checklist.md`.
5. Update `docs/dev/development-seed-profiles.md` with Market expectations.
6. Update `docs/dev/planet-module-boundaries.md` only if Market boundary notes need an explicit addition.

## UI/UX requirements

- Docs should be practical and screenshot-friendly
- Language should stay Spanish-first where describing visible copy expectations
- Exclusions should be unmistakable

## Backend/API requirements

- No backend change is expected

## Expected files to modify

- `docs/dev/market-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md` only if needed

## Safety constraints

- Do not document unsupported market operations as implemented
- Do not imply hidden transaction capabilities

## Acceptance criteria

- A user can QA Market from docs without guesswork.
- Market scope and exclusions are documented clearly.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

If integration checks are reviewed and no configured integration suite exists, record:

`No integration tests configured.`

## Notes / residual risks

- The docs must clearly state that active transactions remain excluded even if advisory ratios and future-action placeholders are visible.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to intended docs plus any tightly related helper notes.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer updating existing checklist structures over creating parallel documentation.
- If broader documentation drift is found, create focused follow-up tasks instead of expanding this one.
