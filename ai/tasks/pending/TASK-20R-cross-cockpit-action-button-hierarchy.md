# TASK-20R-cross-cockpit-action-button-hierarchy

---
id: TASK-20R-cross-cockpit-action-button-hierarchy
title: Cross-cockpit action button hierarchy
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Make primary, secondary, disabled, and handoff buttons consistent across accepted cockpits.

## Purpose
Clarify which controls perform a real action, which merely navigate, and which are intentionally unavailable so the demo feels deliberate instead of visually noisy.

## Current Problem
Buttons currently vary across pages with labels and emphasis such as `Revisar orden`, `Revisar investigacion`, `Revisar produccion`, `Abrir Construccion`, `Preparacion terrestre no disponible`, and `Completar vencidas no disponible`. Some unavailable actions still visually compete with available actions.

## Context
- Research, Shipyard, Defenses, and Ground Army already expose controlled or disabled actions.
- The current block should standardize the visual hierarchy without rewiring behavior unless a broken inconsistency is found.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/styles.css`
- accepted cockpit pages
- shared button or common component files if they already exist

## Implementation Requirements
1. Audit primary action buttons across all accepted cockpits.
2. Standardize the treatment of:
   - primary available action
   - secondary handoff action
   - disabled unavailable action
   - danger or cancel action if present
   - read-only navigation link
3. Ensure disabled or unavailable actions are not styled like bright primary actions.
4. Ensure handoff actions do not imply immediate mutation.
5. Use consistent copy where appropriate:
   - `Revisar orden`
   - `Confirmar`
   - `Cancelar`
   - `Abrir [Cabina]`
   - `No disponible en esta version`
6. Do not rewire action behavior unless a visible behavior bug is discovered.

## UI/UX Requirements
- Visual hierarchy must be clear.
- Users should be able to distinguish mutation, navigation, and unavailable controls quickly.
- Spanish-first copy.

## Backend/API Requirements
- None.

## Safety Constraints
- No new mutation paths.
- No optimistic actions.
- No combat or gameplay expansion.

## Expected Files to Modify
- targeted accepted cockpit pages
- shared button or common component files if present
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Button hierarchy is more consistent across accepted cockpits.
- Available, handoff, and disabled controls are visually distinguishable.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- A deeper component extraction can happen later.
- This task should stay focused on consistency and emphasis.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on hierarchy rather than whole-page redesign.
