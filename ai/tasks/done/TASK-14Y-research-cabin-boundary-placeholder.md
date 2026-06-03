# TASK-14Y

---
id: TASK-14Y
title: Research cabin boundary placeholder
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Create or refine the Research cabin as its own boundary so research-related content is not mixed into `/construction`.

## Current problem
The construction catalog still includes a research laboratory entry alongside general construction. That blurs the difference between building a lab and performing research.

## Context from current implementation
The backend already has research foundations from earlier phases, but this block must not wire full research gameplay unless it is already safe and intentionally exposed. The focus here is UI boundary clarity.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `ai/current-state.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx` or the equivalent route page
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Implement or refine `/research`.
- Show:
  - planet context;
  - `Investigación` title;
  - explanation that technologies and research queues belong here;
  - current placeholder or dev-safe status;
  - a note about laboratory/building dependency if useful;
  - a clear message if no safe endpoint is wired.
- If useful, show a compact related-infrastructure item for `Laboratorio de investigación`, but only as a handoff or contextual link.
- Do not show the full construction catalog.
- Do not execute research queue mutations.
- Preserve query parameters when navigating.

## UI/UX requirements
- Spanish-first.
- Clear module-specific identity.
- Useful placeholder, not an empty page.
- No raw DTO names.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- No research enqueue.
- No research completion.
- No rewards or tech effects.
- No production modifiers.

## Acceptance criteria
- `/research` exists and is useful.
- Research content is not shown as a full construction category in `/construction`.
- Navigation from Planet/Construction to Research preserves context.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- A later block can implement Research v1 using existing backend foundations.
