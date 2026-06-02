# TASK-13C

---
id: TASK-13C
title: Phase 13C - Fleet order flow final visual polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12Y-13D"
priority: medium
---

## Goal
Make the Fleet order flow visually clearer and more OGame-like without adding complexity.

## Context
The order flow works, but the current step hierarchy and result presentation can still be visually clearer. This task focuses on presentation and compactness rather than new behavior.

## Implementation steps

1. Review the order sequence, selectors, estimate card, confirmation state, and result feedback.
2. Improve the visual distinction between current, available, completed, and pending steps.
3. Make the estimate result card compact, readable, and easy to scan.
4. Keep the controlled mutation flows explicit and easy to understand.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- The five-step order flow is easier to understand visually.
- The estimate result card is compact and readable.
- Confirmations remain explicit and guarded.
- Existing create, cancel, and complete-due flows stay intact.
- Split and merge remain disabled or prototype-only.

## Constraints

- Do not add modals unless one already exists and can be reused simply.
- Do not add new commands.
- Do not add polling or WebSockets.
- Do not change backend.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

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
