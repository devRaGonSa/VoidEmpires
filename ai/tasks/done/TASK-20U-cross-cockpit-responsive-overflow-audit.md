# TASK-20U-cross-cockpit-responsive-overflow-audit

---
id: TASK-20U-cross-cockpit-responsive-overflow-audit
title: Cross-cockpit responsive overflow audit
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: medium
---

## Goal
Audit and fix obvious responsive or overflow issues across accepted cockpits.

## Purpose
Improve readability and reduce layout friction at common desktop widths so the accepted demo stays usable without a full redesign.

## Current Problem
Cockpit pages are dense, and some panels or cards may overflow, look cramped, or create awkward whitespace at common desktop widths.

## Context
- Previous module blocks fixed local overflow issues in isolation.
- The app now needs one cross-cockpit pass that improves shared layout behavior.
- This should remain a pragmatic audit, not a broad visual redesign.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/styles.css`
- all accepted cockpit pages
- shared layout component files

## Implementation Requirements
1. Review common widths around:
   - `1280px`
   - `1366px`
   - `1440px`
2. Fix obvious issues such as:
   - horizontal overflow
   - clipped badges
   - cards with too much inline text
   - buttons that do not wrap well
   - grid columns that become too narrow
   - empty vertical gaps created by shared layout rules
3. Keep scope pragmatic and focused on obvious regressions.
4. Prefer reusable CSS class improvements over many one-off page hacks.
5. Do not redesign the whole layout system.

## UI/UX Requirements
- Accepted cockpits should remain readable at common desktop widths and typical zoom ranges.
- Main content should avoid horizontal scrollbars unless truly unavoidable.

## Backend/API Requirements
- None.

## Safety Constraints
- No gameplay changes.
- No route or behavior changes unrelated to layout.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/styles.css`
- targeted accepted cockpit pages or shared layout components

## Acceptance Criteria
- Obvious overflow issues are reduced.
- Frontend build passes.
- No broad layout regression is introduced.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual visual QA remains the final check.
- Some page-specific edge cases may remain for future work.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on pragmatic layout fixes.
