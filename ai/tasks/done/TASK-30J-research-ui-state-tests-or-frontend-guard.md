# TASK-30J

---
id: TASK-30J-research-ui-state-tests-or-frontend-guard
title: Add regression protection for research enqueue UX
status: done
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: ""
priority: medium
---

## Goal
Create lightweight regression protection for Research enqueue safety and user-flow signals.

## Context
Frontend tests are not currently established; this task must avoid introducing a heavy test framework.

## Implementation steps

1. Detect whether frontend has existing test runner; if present, add targeted tests.
2. If no framework exists, add documented/static guard notes in docs and/or task artifacts.
3. Validate at minimum:
   - `enqueueResearchOrder` type/API exists (via compile-time check)
   - confirmation copy is present in page code
   - no direct mutation without confirmation path in component logic
4. Keep regressions easy to execute by manual QA until framework is available.

5. If any guard file is added, keep it lightweight and colocated.

## Files to read first

- `ai/task-template.md`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `package.json` (frontend root if present)
- `scripts/check-frontend-copy-regressions.ps1`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx` (if required for compile-time/guards)
- `docs/dev/frontend-foundation-smoke-checklist.md` (documented static guard list)
- Optional light guard file under `ai/tasks/` if needed for process

## Acceptance criteria

- Some form of regression guard exists for critical Research mutation behavior.
- No heavy framework is introduced.
- Build continues to pass.

## Constraints

- No mutation from guard execution itself.
- Keep tasks small and reviewable.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

1. Run `git status`.
2. Stage intended files.
3. Commit with message: `test(frontend): add lightweight research UI guards`.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits for this task.
