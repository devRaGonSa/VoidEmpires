# TASK-30G

---
id: TASK-30G-research-validation-errors-spanish-copy
title: Normalize research validation errors into Spanish copy
status: done
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Map backend validation failures into clear Spanish UI messages while keeping diagnostics intact.

## Context
Error output must be comprehensible for players but still preserve raw backend detail for debugging and auditability.

## Implementation steps

1. Inspect backend validation payloads produced by research enqueue.
2. Add an error translation layer in Research UI state handling for:
   - civilization already has open research order
   - insufficient resources
   - unavailable research
   - invalid civilization
   - invalid source planet
   - invalid research type
   - prerequisite missing
   - already researched / level unavailable
   - unexpected backend failure
3. Keep full backend error payload in collapsed diagnostics.
4. Add/adjust regression guard checks for copy regressions.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `scripts/check-frontend-copy-regressions.ps1`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Acceptance criteria

- Known failures show user-friendly Spanish copy.
- Raw backend failure details remain accessible and unhidden.
- No silent fallthrough on known validation states.
- copy regression guard passes.

## Constraints

- Do not change backend validation logic in this task.
- Preserve source-of-truth behavior.
- Keep no mutation side effects on failure.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `fix(frontend): map research enqueue errors to Spanish messages`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
