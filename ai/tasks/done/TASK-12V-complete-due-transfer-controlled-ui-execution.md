# TASK-12V

---
id: TASK-12V
title: Phase 12V - Complete due transfer controlled UI execution
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12S-12X"
priority: medium
---

## Goal
Enable controlled frontend or development execution for completing due orbital transfers, with explicit prototype guardrails.

## Context
The Fleet screen needs a controlled way to complete due transfers when the current UI or API data says they are ready. The action must remain explicit, guarded, and clearly marked as development-only if that is how the backend models it.

## Implementation steps

1. Inspect the existing transfer actions and typed client contracts used by the Fleet UI.
2. Add the controlled complete-due action only for transfers that are due or ready.
3. Show explicit confirmation, loading, success, and error states.
4. Refresh the Fleet UI state after success without pretending a failed request succeeded.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- the typed client helpers for transfer completion and fleet refresh

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet action helper types or client wrappers, if needed

## Acceptance criteria

- The UI exposes a clearly guarded complete-due action for ready transfers.
- The action shows explicit confirmation and clear feedback states.
- If no due transfer exists, the UI shows a disabled or explanatory state.
- Success refreshes the Fleet state.
- Network failures do not mutate local state as if the action succeeded.
- Split and merge remain disabled or prototype-only.

## Constraints

- Do not add polling or WebSockets.
- Do not add combat or interception.
- Do not apply EF migrations.
- Keep the action controlled and explicit.

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
