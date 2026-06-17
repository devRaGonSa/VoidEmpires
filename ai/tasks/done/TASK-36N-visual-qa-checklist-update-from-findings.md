# TASK-36N

---
id: TASK-36N
title: Visual QA checklist update from findings
status: done
type: docs
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: medium
---

## Goal
Update the deferred visual QA checklist with findings from the user's screenshots.

## Context
The user manually observed overloaded Planet and Construction screens. The deferred visual QA checklist should capture these exact issues for later user-driven browser verification.

## Implementation steps

1. Update `docs/dev/deferred-visual-qa-master-checklist.md` or `docs/dev/frontend-foundation-smoke-checklist.md`.
2. Include explicit checks for:
   - no obsolete `solo lectura` copy on mutating pages;
   - header not showing disconnected mock resources;
   - sidebar grouping playable/read-only correctly;
   - Planet hub has primary actions visible;
   - Construction catalog appears without excessive scrolling/noise;
   - Dev tools collapsed/secondary;
   - Development actions have modals;
   - diagnostics collapsed;
   - resource labels coherent.
3. State that this block performs implementation cleanup but full browser QA remains user-driven unless the user later provides screenshots.
4. Make no gameplay behavior changes.

## Files to read first

- docs/dev/deferred-visual-qa-master-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- docs/dev/deferred-visual-qa-master-checklist.md
- Optional: docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Checklist reflects real observed issues.
- No visual QA overclaim is introduced.
- Validation passes.

## Constraints

- Do not perform or claim full browser QA.
- Do not alter gameplay behavior.
- Keep checklist Spanish-first where it references UI copy.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36N message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
