# TASK-26J

---
id: TASK-26J
title: Phase 26J - Market diagnostics and raw values containment
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: medium
---

## Goal

Ensure Market technical details remain collapsed, secondary, and clearly separated from the primary user-facing economy UI.

## Current problem

Market aggregates read-model and economy data. Without careful containment, raw ids, endpoint-like labels, DTO terminology, or payload-style wording can leak into the primary interface and weaken the cockpit experience.

## Context from current implementation

- The accepted frontend suite already treats diagnostics as collapsed and secondary.
- Market is especially at risk because it combines reserves, references, and signals that may expose more technical naming.
- This task is about presentation containment, not about removing useful debug information entirely.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts
- src/VoidEmpires.Frontend/src/api/marketTypes.ts
- docs/dev/cockpit-copy-guidelines.md
- docs/dev/market-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md

## Implementation requirements

1. Search the Market frontend surface for technical terms such as:
   - `endpoint`
   - `DTO`
   - `payload`
   - `raw`
   - `capability`
   - `route`
   - `mutation`
   - `dev`
   - `id`
2. Move technical values and labels out of primary UI sections when possible.
3. Keep useful debug information available inside collapsed diagnostics only.
4. Use the existing diagnostic pattern or a Spanish label aligned with:
   - `Diagnostico secundario`
5. Avoid removing information that developers still need for inspection; the goal is containment, not blindness.

## UI/UX requirements

- Primary UI should stay readable for gameplay-oriented QA.
- Diagnostics should remain available but visually subordinate.
- Raw values should never be the first thing the user sees on Market.

## Backend/API requirements

- No backend changes are expected.
- No contract changes should be made solely to hide frontend wording.

## Safety constraints

- No behavior change beyond presentation containment.
- No removal of useful diagnostics entirely unless the information is duplicated elsewhere.

## Acceptance criteria

- Technical labels and raw values are kept out of the primary Market UI.
- Diagnostics remain collapsed and useful.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Some technical labels may come from shared presentation helpers; adjustments should avoid degrading other cockpits that intentionally rely on those helpers.

