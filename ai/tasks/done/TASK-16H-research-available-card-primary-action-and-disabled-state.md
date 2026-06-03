# TASK-16H-research-available-card-primary-action-and-disabled-state

---
id: TASK-16H-research-available-card-primary-action-and-disabled-state
title: Research available card primary action and disabled state
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Make available and blocked Research cards visually truthful.

## Purpose
Once the seed exposes an available item, the UI must clearly distinguish executable, blocked, in-queue, and completed states so QA can trust what it is seeing.

## Current Problem
The cockpit currently makes all cards look blocked. When availability is fixed, the CTA states must also be corrected so a blocked item does not look like a primary action and an available item does not look disabled.

## Context
- Construction already established the pattern for truthful disabled and guarded actions.
- Research should follow the same principle.
- Only available items should be able to open the confirmation flow.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Update card rendering so:
   - available items show a clear primary CTA like `Revisar investigacion`;
   - blocked items show a disabled or secondary CTA like `Revisar requisitos` or `Faltan recursos`;
   - in-queue items show `En cola` or `En investigacion`;
   - completed items show `Completada`.
2. Ensure clicking blocked actions does not open confirmation and does not call mutation endpoints.
3. Ensure only available actions can open the confirmation flow.
4. Ensure action state is derived from backend availability plus frontend guardrails.
5. Keep raw action availability details in diagnostics.

## UI/UX Requirements
- Available cards should visually stand out but not dominate the page.
- Blocked cards should remain readable and clearly non-executable.
- Spanish text must be the primary player-facing language.
- Button styles should align with the Planet and Construction cockpit patterns.

## Backend/API Requirements
- No backend change is expected if the availability data is already correct.
- If the UI reveals a mismatch between card state and backend state, the backend issue should be addressed in a separate task.

## Safety Constraints
- Do not mutate from blocked cards.
- Do not infer availability only from frontend cost math if the backend says blocked.
- Do not make disabled states look like executable actions.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- One seeded available research shows a primary CTA.
- Seeded blocked researches show a secondary or disabled state.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual QA should capture one available and one blocked card in the same session.
- If the page supports a compact mobile mode later, the truthfulness of the CTA states must remain intact.
- Keep the action text short enough that it still reads clearly inside a card.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer state and class adjustments over large markup rewrites.
