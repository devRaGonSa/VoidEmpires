# TASK-26C

---
id: TASK-26C
title: Phase 26C - Market visual readiness audit
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: high
---

## Goal

Audit the current Market cockpit implementation before visual QA and identify UI, copy, layout, and diagnostics risks that could block acceptance.

## Current problem

The Market cockpit is technically implemented and documented as read-only, but it has not yet passed focused visual review. The team needs a clear list of what is already present, what reads well, and what could mislead users into thinking Market executes transactions.

## Context from current implementation

- Market already exists as a Development-safe read-only economy cockpit.
- The accepted product boundary keeps Market non-transactional and non-destructive.
- The next block is visual and copy-focused, so later tasks depend on this audit to keep changes narrow and intentional.

## Files to read first

- ai/current-state.md
- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts

## Expected files to modify

- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts

## Implementation requirements

1. Inspect the Market page and supporting view-model and presentation helpers.
2. Confirm whether the main sections are present and visually coherent:
   - hero or header
   - economy summary
   - reserves and production
   - reference prices or ratios
   - trade signals and future routes
   - disabled future operations
   - handoffs
   - collapsed diagnostics
3. Identify raw technical labels, English copy, placeholders, overly prominent diagnostics, or action-like language that suggests Market can buy, sell, or execute routes.
4. Record the likely UI, copy, spacing, and hierarchy risks in `docs/dev/market-cockpit-checklist.md` as the visual QA target list for the rest of the block.
5. Keep any code changes in this task minimal and audit-driven only if a tiny correction is needed to document the actual current state.

## UI/UX requirements

- Focus on section presence, visual hierarchy, language tone, and read-only clarity.
- Ensure diagnostics remain secondary and do not dominate the first screen.
- Prefer the established cockpit page patterns already used across the accepted suite.

## Backend/API requirements

- No backend changes are expected.
- If the audit reveals a frontend assumption that misrepresents the current Market API, document it precisely for later tasks rather than widening backend scope here.

## Safety constraints

- No market mutations.
- No enabling of buy, sell, export, import, or route creation.
- No broad functional changes unless trivial and directly required by the audit.

## Acceptance criteria

- The Market cockpit has a documented visual QA target list.
- The audit clearly identifies copy, layout, and read-only-boundary risks.
- Later tasks can implement polish without rediscovering the same issues.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- Some issues may only be fully visible in the browser, so this audit should distinguish code-level observations from later user-driven visual confirmation.

