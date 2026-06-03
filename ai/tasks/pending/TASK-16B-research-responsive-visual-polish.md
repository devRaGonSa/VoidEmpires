# TASK-16B-research-responsive-visual-polish

---
id: TASK-16B-research-responsive-visual-polish
title: Research responsive visual polish
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Polish the Research cockpit layout for desktop readability and consistency with Planet and Construction.

## Purpose
The page may be functionally correct but still need layout hardening for spacing, wrapping, overflow and general cockpit feel.

## Current Problem
New pages often work but feel rough until the layout is tuned. Research should look intentional on common desktop widths and still remain usable on smaller screens.

## Context
- The app uses a dark galactic cockpit style.
- Research should match the existing module shell instead of inventing a new visual language.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- shared module layout components
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Implementation Requirements
1. Review Research at common desktop widths around 1200 to 1440px.
2. Prevent horizontal overflow.
3. Ensure long technology names wrap safely.
4. Ensure requirements and cost rows remain readable.
5. Ensure available and blocked cards are visually distinct.
6. Ensure confirmation modal or panel fits.
7. Keep diagnostics collapsed by default.
8. Use existing style tokens and classes where possible.
9. Avoid redesigning the whole app shell.

## UI/UX Requirements
- The page should look like a cockpit, not a table dump.
- Main hierarchy should be:
  - header and context;
  - overview;
  - queue;
  - catalog;
  - actions;
  - diagnostics.
- Avoid excessive vertical emptiness.

## Backend/API Requirements
- No backend change.

## Safety Constraints
- No gameplay changes.
- No new motion that interferes with readability.
- No visual treatment that implies unsupported effects.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- `/research` is visually readable.
- There is no obvious overflow.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Final art or iconography can be handled in a later design block.
- Keep the adjustments incremental and easy to revert.
- Visual polish should not change the core Research semantics.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer CSS refinements over markup rewrites.
