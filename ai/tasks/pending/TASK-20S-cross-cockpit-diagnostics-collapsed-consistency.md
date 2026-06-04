# TASK-20S-cross-cockpit-diagnostics-collapsed-consistency

---
id: TASK-20S-cross-cockpit-diagnostics-collapsed-consistency
title: Cross-cockpit diagnostics collapsed consistency
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: medium
---

## Goal
Make diagnostics sections consistent and collapsed across accepted cockpits.

## Purpose
Preserve debug value while reducing clutter so technical details are available on demand instead of mixed into primary gameplay content.

## Current Problem
Diagnostics already exist, but they vary by page in label, location, density, and wording. A consistent diagnostics pattern would reduce clutter while keeping the current developer value.

## Context
- Technical details remain useful during development and QA.
- The goal is not to remove them, but to keep them secondary and easy to find.
- Earlier cockpit work already established patterns that this task can consolidate.

## Files to Inspect First
- all accepted cockpit pages
- `src/VoidEmpires.Frontend/src/styles.css`
- existing `details` and `summary` diagnostics patterns

## Implementation Requirements
1. Standardize the diagnostics label to one chosen pattern, such as:
   - `Diagnostico secundario`
   or
   - `Lectura tecnica`
2. Ensure diagnostics are collapsed by default where that is safe.
3. Move raw ids, payload snippets, and similar technical details into diagnostics where possible.
4. Ensure diagnostics do not break page layout or push key content too far down.
5. Avoid huge payload dumps in the primary visible surface.
6. Keep the summary text short and predictable.

## UI/UX Requirements
- Diagnostics should be discoverable but secondary.
- Spanish-first.
- Technical sections should feel intentionally tucked away, not accidentally hidden.

## Backend/API Requirements
- None.

## Safety Constraints
- No behavior changes.
- No loss of useful QA information.

## Expected Files to Modify
- targeted accepted cockpit pages
- possibly a small shared diagnostics helper
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Diagnostics look and feel more consistent.
- Diagnostics are collapsed by default where appropriate.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Some pages may still retain page-specific diagnostic content if it is genuinely useful.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on diagnostics presentation consistency.
