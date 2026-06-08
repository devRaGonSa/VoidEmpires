# TASK-31H

---
id: TASK-31H
title: Defense errors and blocked affordance
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Normalize Defenses user-facing blocked and error states so the page is understandable and safe whether it is real-enqueue capable or still read-only.

## Context
Defenses sits in a high-risk domain because the UI could easily imply unsupported gameplay if blocked or unavailable states are vague. This task hardens the primary copy, keeps raw backend details secondary, and ensures blocked cards never look like active primary actions.

## Implementation steps

1. Inventory the current and expected Defenses error or blocked states from backend contracts and UI readiness data.
2. Add Spanish-first user-facing copy for insufficient resources, existing open order, unavailable or missing prerequisite cases, invalid planet context, unsupported endpoint phase, and unexpected backend failures.
3. Ensure raw backend payloads stay in diagnostics rather than primary presentation.
4. Ensure blocked cards and read-only states do not visually imply that production is already playable when it is not.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- scripts/check-frontend-copy-regressions.ps1
- docs/dev/defenses-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/styles.css
- scripts/check-frontend-copy-regressions.ps1
- docs/dev/defenses-cockpit-checklist.md

## Acceptance criteria

- Defenses blocked and error states are understandable in Spanish-first copy.
- Raw backend payloads remain secondary or diagnostic-only.
- Blocked cards do not present a misleading primary action.
- If Defenses is read-only, the primary UI does not imply production is already accepted gameplay.
- `npm run build --prefix src/VoidEmpires.Frontend` and `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not add fake action paths or new mutation semantics

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
