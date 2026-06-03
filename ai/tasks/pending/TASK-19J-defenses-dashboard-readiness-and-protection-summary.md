# TASK-19J-defenses-dashboard-readiness-and-protection-summary

---
id: TASK-19J-defenses-dashboard-readiness-and-protection-summary
title: Defenses dashboard readiness and protection summary
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: medium
---

## Goal
Create the top-level Defenses dashboard section that summarizes current protection state, readiness, structures, constraints, and the recommended next step.

## Purpose
Ensure the cockpit opens with a readable overview rather than a raw catalog, helping the player understand defensive posture before looking at option cards or queue details.

## Current Problem
Without an overview layer, Defenses would feel like a list of technical items. The cockpit needs a top summary that explains what is already protecting the planet, what is missing, and whether the next action is available, blocked, or handled by another module.

## Context
- Planet, Research, and Shipyard all benefited from summary sections that frame the page before detailed lists.
- The seeded `Aurelia` context should support an understandable defensive state even if the backend only exposes limited protection data.
- The page must remain truthful about missing systems such as combat or complete-due.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses view-model helpers from `TASK-19H`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add a top dashboard section for the Defenses cockpit.
2. The summary should aim to show:
   - planet name and local context
   - current protection or readiness posture
   - visible defensive structures count or grouping
   - queue presence or absence
   - resource pressure or affordability summary if available
   - recommended next step
3. Provide a conservative recommended next-step rule, for example:
   - reinforce available protection
   - resolve missing requirements
   - hand off to Construction
   - review queue
   - wait because no safe action exists in this build
4. Keep the copy aligned with the real backend limitations.
5. Reuse existing card and panel patterns rather than introducing a brand-new cockpit layout system.

## UI/UX Requirements
- The first screenful should explain the current defensive posture clearly.
- Spanish copy should make unavailable capabilities understandable, not apologetic.
- The summary should feel cockpit-like and intentional, not like debug telemetry.

## Backend/API Requirements
- No backend change expected unless the read model lacks a minimal readiness summary and that gap can be added safely.

## Safety Constraints
- No combat status claims.
- No fake precision about battle effectiveness.
- No mutation behavior introduced in this task.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses view-model helper files
- `src/VoidEmpires.Frontend/src/styles.css` if light styling support is needed

## Acceptance Criteria
- The Defenses cockpit opens with a readable readiness dashboard.
- The page communicates current protection state and next-step guidance without implying unsupported combat behavior.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If the backend can only expose structural and queue metadata, the dashboard should summarize those honestly instead of inventing combat readiness numbers.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the work centered on the summary layer.
