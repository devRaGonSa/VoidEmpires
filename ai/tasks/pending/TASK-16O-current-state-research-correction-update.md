# TASK-16O-current-state-research-correction-update

---
id: TASK-16O-current-state-research-correction-update
title: Current state Research correction update
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: medium
---

## Goal
Update `ai/current-state.md` to reflect that Research v1 required a QA correction and now has a first usable flow.

## Purpose
`ai/current-state.md` is the continuity document for future chats. It must tell the truth about the corrected Research cockpit state so later work does not assume the old broken or overly optimistic state.

## Current Problem
The current state may already describe Research v1 as a completed cockpit foundation. That is too broad if the seeded QA path does not actually expose an available item or if the completion path remains intentionally disabled.

## Context
- This task should be honest about the corrected acceptance boundary.
- It should preserve the earlier module boundaries and exclusions.
- It should record the new QA correction phase without overclaiming final gameplay support.

## Files to Inspect First
- `ai/current-state.md`
- `docs/dev/research-cockpit-checklist.md`
- `ai/tasks/done/`

## Implementation Requirements
1. Update the phase line or current frontend cockpit baseline to include:
   - `Phase 16P - Research cockpit QA correction and first usable research flow`
   - or equivalent wording.
2. Record that:
   - Research now shows at least one available seeded item;
   - blocked items remain visible with meaningful reasons;
   - the confirmation/enqueue path is usable only when safe;
   - complete-due remains disabled or placeholder unless proven safe;
   - no real technology effects are applied yet;
   - no combat, interception, espionage, 3D, or production auth was introduced.
3. Preserve the previous state about Galaxy, Fleets, Planet, Construction, and module boundaries.
4. Keep the test count accurate after validation.

## UI/UX Requirements
- `ai/current-state.md` must help future orchestration decisions.
- The wording should be concise, stable, and honest.

## Backend/API Requirements
- No backend change is expected.

## Safety Constraints
- Do not claim final research tree support.
- Do not claim complete-due is available unless it really is.
- Do not erase important caveats about dev-only behavior.

## Expected Files to Modify
- `ai/current-state.md`
- Possibly one small cross-reference in a Research doc if needed

## Acceptance Criteria
- The current state is accurate and honest.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- This block closes a visual defect, not the full Research system.
- The current-state note should make it obvious that the cockpit is usable for QA but still conservative in scope.
- Keep the phase wording aligned with the actual repo history.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a narrow continuity update.
