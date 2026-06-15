# TASK-36E

---
id: TASK-36E
title: Shared page header and context strip
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Create or refine reusable page-level components for compact context.

## Context
Pages currently repeat large cards for session, planet, civilization, resources, and page status. Shared compact components should reduce duplication without hidden backend fetches.

## Implementation steps

1. Review existing components and page-level context markup.
2. Add or reuse components such as `PageHeader`, `PageContextStrip`, or `SessionResourceStrip`, using names consistent with current code.
3. Support:
   - page title;
   - concise page purpose;
   - status badge;
   - current planet/civilization summary;
   - optional primary navigation/handoff;
   - optional resource summary.
4. Do not display raw ids in primary context.
5. Keep the component visually compact.
6. Make it reusable by Planet, Construction, Research, and Shipyard.
7. Avoid hidden backend fetches.

## Files to read first

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Shared compact header/context foundation exists.
- Component avoids raw ids in primary UI.
- No page migration is required unless needed for compilation.
- Frontend build passes.

## Constraints

- Do not add hidden backend requests.
- Do not fake resources.
- Keep visual changes compact and aligned with the theme.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36E message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
