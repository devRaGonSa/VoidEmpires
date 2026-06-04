# TASK-26K

---
id: TASK-26K
title: Phase 26K - Market docs and smoke checklist finalization
status: done
type: platform
team: platform
supporting_teams:
  - docs
  - frontend
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: medium
---

## Goal

Finalize the Market QA documentation after the visual and read-only polish work is complete.

## Current problem

The Market cockpit needs explicit visual-check documentation so future sessions and user-driven QA know what to look for and what must remain intentionally disabled.

## Context from current implementation

- Market already has cockpit documentation and a shared frontend smoke checklist.
- This block does not add gameplay systems, so the docs must emphasize read-only acceptance and user-driven visual confirmation.
- The Market cockpit uses a deterministic seeded URL that should be recorded exactly.

## Files to read first

- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/development-seed-profiles.md
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- ai/current-state.md

## Expected files to modify

- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/development-seed-profiles.md
- ai/current-state.md

## Implementation requirements

1. Update `docs/dev/market-cockpit-checklist.md`.
2. Update `docs/dev/frontend-foundation-smoke-checklist.md`.
3. Include the exact URL:
   - `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
4. Document the expected visual checks:
   - summary visible
   - reserves visible
   - ratios advisory
   - signals and future routes disabled
   - operations disabled
   - handoffs visible
   - diagnostics collapsed
   - no buy, sell, or resource mutation
5. Explicitly state that visual QA is user-driven and that the cockpit remains read-only.

## UI/UX requirements

- Documentation should mirror the final polished UI language.
- Checks should be practical and easy to follow during manual review.

## Backend/API requirements

- No backend changes are expected.
- Do not describe unsupported mutations as future commands unless already intentionally shown as disabled UI placeholders.

## Safety constraints

- No destructive guidance.
- No manual SQL.
- No instructions that imply Market can perform transactions.

## Acceptance criteria

- Market docs and smoke checklist clearly describe the accepted read-only visual baseline.
- The exact URL and expected checks are documented.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Visual QA remains user-driven, so the docs should distinguish documented expectations from screenshot-based confirmation.

