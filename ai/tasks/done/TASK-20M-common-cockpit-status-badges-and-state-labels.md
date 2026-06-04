# TASK-20M-common-cockpit-status-badges-and-state-labels

---
id: TASK-20M-common-cockpit-status-badges-and-state-labels
title: Common cockpit status badges and state labels
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Unify status badges and state labels across accepted cockpits.

## Purpose
Reduce duplication and inconsistency so the same state reads and looks similar everywhere in the app, especially across the most visible gameplay cockpits.

## Current Problem
Badge and status-chip usage currently varies by page. Some pages say `Solo lectura`, others use `Readiness only`, `Disponible`, `Protegido`, `Secundario`, `Sin mutacion local`, or `No disponible`. The semantics are mostly correct, but the vocabulary and visual treatment are inconsistent.

## Context
- The app now has multiple cockpit pages with overlapping state concepts.
- A small shared helper or CSS consolidation may be enough to reduce duplication without introducing a large design-system refactor.
- Research, Shipyard, Defenses, and Ground Army are the most visible places where inconsistency is easiest to notice.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/components/`
- `src/VoidEmpires.Frontend/src/styles.css`
- presentation helper files under `src/VoidEmpires.Frontend/src/utils/`

## Implementation Requirements
1. Identify repeated badge and state-label patterns across accepted cockpits.
2. Add or extend a shared badge helper or component if that is the safest way to standardize the visible states.
3. Standardize labels for:
   - read-only
   - available
   - blocked
   - in queue
   - completed
   - safe placeholder
   - diagnostics
   - Development-only
   - context preserved
4. Use Spanish primary labels.
5. Preserve visual distinctions between:
   - good or available
   - warning or blocked
   - neutral or read-only
   - secondary or diagnostic
6. Avoid a broad risky refactor; a small shared helper or CSS class consolidation is enough.
7. Update at least the most visible inconsistencies across Research, Shipyard, Defenses, and Ground Army.

## UI/UX Requirements
- The same state should look and read similarly across pages.
- Blocked actions or blocked states must not look primary.
- State language should align with the shared vocabulary from the previous task.

## Backend/API Requirements
- None.

## Safety Constraints
- No gameplay changes.
- No mutation changes.

## Expected Files to Modify
- small shared component or helper file under `src/VoidEmpires.Frontend/src/components/` or `src/VoidEmpires.Frontend/src/utils/`
- targeted cockpit pages
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Core badge vocabulary is more consistent.
- Frontend build passes.
- No obvious visual regressions are introduced in the most visible accepted cockpits.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- A fuller design system can come later.
- This task should stay pragmatic and avoid turning into a whole-component rewrite.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on visible state standardization.
