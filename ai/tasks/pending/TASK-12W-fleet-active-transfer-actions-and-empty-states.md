# TASK-12W

---
id: TASK-12W
title: Phase 12W - Fleet active transfer actions and empty states
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12S-12X"
priority: medium
---

## Goal
Improve the active-transfer panel so cancel and complete-due actions are represented clearly as controlled actions with useful empty states.

## Context
The Fleet screen now has more than one controlled mutation. The active-transfer area should communicate which transfers can be cancelled, which are due, which are already resolved, and what to show when no fleet data is available.

## Implementation steps

1. Review the current active-transfer panel and the state it receives.
2. Add compact playable display states for active, planned, due, completed, and missing-data scenarios.
3. Show cancel and complete-due actions only when they are actually available.
4. Add readable empty states for no active transfers, no cancellable transfers, no due transfers, and unloaded data.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- the Fleet active-transfer rendering helpers

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet active-transfer presentation helpers

## Acceptance criteria

- The active-transfer panel is easier to read and act on.
- Cancel is shown only when a transfer is cancellable.
- Complete-due is shown only when transfers are due or completable.
- Empty states explain what is missing instead of showing raw API noise.
- The UI refreshes after cancel or complete-due success.
- Split and merge remain disabled or prototype-only.

## Constraints

- Do not add polling or WebSockets.
- Do not change backend unless a small mapping mismatch blocks correct display.
- Avoid raw API terms in the main display.
- Keep technical details collapsed or secondary.

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
