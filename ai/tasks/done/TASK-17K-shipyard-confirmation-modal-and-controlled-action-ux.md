# TASK-17K-shipyard-confirmation-modal-and-controlled-action-ux

---
id: TASK-17K-shipyard-confirmation-modal-and-controlled-action-ux
title: Shipyard confirmation modal and controlled action UX
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Introduce the controlled confirmation UX pattern for Shipyard actions so production cannot be triggered accidentally or opaquely.

## Purpose
Keep Shipyard aligned with the safer interaction standards already established in Construction and Research.

## Current Problem
Shipyard v1 should feel playable, but its main action is still a real mutation when enabled. A direct one-click enqueue would be risky, especially while the backend surface is still being audited. The user needs a confirmation flow that shows what will happen before any request is sent.

## Context
- Controlled confirmation is already part of the accepted mutation pattern in other development cockpits.
- Shipyard actions may remain disabled if backend support is not safe.
- This task focuses on the UX shell and state management, not on backend mutation enablement itself.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Shipyard view-model and presentation helper files
- `src/VoidEmpires.Frontend/src/styles.css`
- Similar confirmation handling in Construction or Research pages

## Implementation Requirements
1. Add controlled action state for the primary production review flow.
2. When an available production card is selected, open a confirmation surface instead of immediately submitting.
3. The confirmation UI should be able to show:
   - planet;
   - asset;
   - quantity;
   - cost;
   - duration;
   - readiness or requirement summary;
   - confirm action;
   - cancel action.
4. If the selected option is blocked or unsupported, the confirmation should either not open or should open in a clearly non-submitting diagnostic mode.
5. Prevent double submission while a later mutation request is pending.
6. Keep raw request details secondary.

## UI/UX Requirements
- Spanish-first button labels and headings.
- The primary action label should read naturally, for example `Revisar produccion`.
- Confirmation copy should be explicit and calm rather than alarmist.
- Cancel should always be non-mutating and obvious.

## Backend/API Requirements
- No backend change is required in this UX task alone.
- The confirmation state should be compatible with a later dev-only enqueue endpoint.

## Safety Constraints
- Do not submit any mutation from this task unless the dedicated enqueue task is also implemented.
- No optimistic queue changes.
- No silent fallback to direct submit.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/utils/` Shipyard interaction or display helpers if needed

## Acceptance Criteria
- Available Shipyard actions use a controlled confirmation flow.
- Blocked actions do not mutate and remain truthful.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- This task should not guess submit payload details; it only prepares the UX surface.
- Keep the modal or panel reusable for enqueue and possible complete-due confirmation if that later becomes safe.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Keep the work focused on interaction state and copy, not a larger visual redesign.
