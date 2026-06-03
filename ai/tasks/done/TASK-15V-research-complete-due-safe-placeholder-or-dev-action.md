# TASK-15V-research-complete-due-safe-placeholder-or-dev-action

---
id: TASK-15V-research-complete-due-safe-placeholder-or-dev-action
title: Research complete-due safe placeholder or dev action
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Expose research completion processing only if the backend has a safe Development-only path; otherwise show a clear placeholder.

## Purpose
Due research items can exist, but completion may imply effects. The UI should stay conservative unless the backend path is explicitly safe.

## Current Problem
Completion support can be global, not scoped, or not safe enough for a module cockpit. The user should never be misled about what is actually available.

## Context
- Construction kept complete-due disabled when the backend path was not safe enough.
- Research should use the same conservative standard.

## Files to Inspect First
- Existing Research complete-due services or endpoints
- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`

## Implementation Requirements
1. Inspect existing complete-due support.
2. If a safe Development-only endpoint exists:
   - expose a controlled `Completar investigaciones vencidas` action;
   - require explicit confirmation;
   - show count of due items;
   - refresh state after success.
3. If support is global or not safe:
   - keep the action disabled or placeholder;
   - explain that completion is not available from this cabin in this build.
4. Do not apply real gameplay effects unless the backend already does and tests cover it.
5. Do not create background worker controls.

## UI/UX Requirements
- Spanish-first copy only.
- Completion action must be secondary to enqueue and catalog browsing.
- Disabled placeholder must be clear and not shame the user.

## Backend/API Requirements
- If an endpoint is added or used, add tests.
- Respect Development-only gating.
- Keep any completion pathway deterministic and testable.

## Safety Constraints
- No production completion.
- No hidden unlock effects.
- No background worker toggles.
- No fake progress animation that implies unsupported behavior.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- Backend files and tests only if safe support is added

## Acceptance Criteria
- Complete-due state is understandable.
- If enabled, it is confirmation-based and tested.
- If disabled, it is explicitly explained.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Completion effects should be a future explicit block if not already stable.
- Prefer a clear placeholder over a risky button.
- The cockpit should remain honest about capabilities.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer disabling over adding speculative UI.
