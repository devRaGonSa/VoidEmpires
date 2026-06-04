# TASK-26F

---
id: TASK-26F
title: Phase 26F - Market reference ratios copy and safety polish
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: medium
---

## Goal

Clarify that Market reference ratios and prices are advisory only and cannot be executed as trades.

## Current problem

Ratio cards can easily resemble live offers if their labels and visual treatment are too assertive. That would blur the read-only boundary of the cockpit.

## Context from current implementation

- Market already exposes reference values and guidance signals.
- The product decision keeps Market strictly non-transactional.
- The visual language must support that boundary even if the values are useful and deterministic.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts
- src/VoidEmpires.Frontend/src/api/marketTypes.ts
- docs/dev/market-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md

## Implementation requirements

1. Polish the ratio and reference labels so they read as advisory and non-executable.
2. Ensure visible copy includes or aligns with:
   - `Referencia de intercambio`
   - `Ratio orientativo`
   - `No es una oferta activa`
   - `Solo lectura`
3. Review the visual treatment of the reference cards so they do not resemble primary actions or confirmations.
4. Keep the displayed values deterministic and tied to the current read model.
5. If diagnostics need a brief source explanation, keep it secondary and collapsed.

## UI/UX requirements

- Ratio cards should support comparison and interpretation, not action.
- Any explanatory text must prevent confusion without overwhelming the card layout.
- Avoid bright CTA-like accents or labels that feel transactional.

## Backend/API requirements

- No backend changes are expected.
- No new mutation affordances or fake transaction previews.

## Safety constraints

- No exchange execution.
- No resource mutation.
- No language that implies an order can be confirmed from Market.

## Acceptance criteria

- Ratio cards clearly read as advisory and non-executable.
- No control or style suggests the user can complete a trade.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If reference ratios are currently rendered through generic summary components, small styling or helper adjustments may be needed to keep them from inheriting action-like emphasis.

