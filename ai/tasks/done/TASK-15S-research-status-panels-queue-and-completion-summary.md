# TASK-15S-research-status-panels-queue-and-completion-summary

---
id: TASK-15S-research-status-panels-queue-and-completion-summary
title: Research status panels queue and completion summary
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Design the Research cockpit status panels so the queue, completion state and catalog summary are easy to scan.

## Purpose
Players and developers need a quick summary of what is active, what is done, what is blocked and what can happen next.

## Current Problem
The page needs a concise overview to avoid forcing users to inspect every technology card before they can understand the state of the research system.

## Context
- This task is about visibility and summarization, not mutation.
- It should complement the detailed catalog and queue views from earlier tasks.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`

## Implementation Requirements
1. Add a top-level status summary area.
2. Include counts or summaries for:
   - available items;
   - blocked items;
   - active queue;
   - completed items;
   - recommended research.
3. Make the queue panel readable and stable.
4. Clarify whether the queue is empty, active, due or blocked.
5. Ensure the completion summary does not claim unsupported gameplay effects.
6. Keep the language concise and easy to scan.

## UI/UX Requirements
- The page should not read like a raw data table.
- The main hierarchy should feel like:
  - header and context;
  - overview;
  - queue;
  - catalog;
  - actions;
  - diagnostics.
- Long labels should wrap safely.
- Status summaries should use Spanish-first copy.

## Backend/API Requirements
- No backend change expected.
- If completion state is not available, the UI should say so explicitly rather than pretending.

## Safety Constraints
- Do not invent completion effects.
- Do not auto-complete queue items.
- Do not show unsupported counts as authoritative if the backend cannot provide them.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css` if the panel needs support

## Acceptance Criteria
- The cockpit has a readable summary of research state.
- The queue is visible and understandable.
- The completion summary is accurate or explicitly limited.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Summary counts should stay honest even if the backend is partial.
- Do not collapse the UI into a single dashboard card unless the data is truly minimal.
- Keep the panel useful for manual QA as well as normal play.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer layout adjustments over structural rewrites.
