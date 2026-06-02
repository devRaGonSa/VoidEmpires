# TASK-12U

---
id: TASK-12U
title: Phase 12U - Fleet estimate and order result gameplay card
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12S-12X"
priority: medium
---

## Goal
Make the estimate and order result area feel like an OGame-style command summary before the player confirms a transfer.

## Context
The Fleet flow already supports estimate and create-transfer actions, but the result area can still feel technical instead of like a gameplay confirmation card. This task improves the presentation while preserving the existing guards against stale or duplicate submissions.

## Implementation steps

1. Review the current estimate and create-transfer presentation in the Fleet page.
2. Build a compact result card that shows the command summary fields available from the backend.
3. Add clean fallback copy when duration, cost, or detailed route data are unavailable.
4. Keep the current stale-estimate and duplicate-submit protections intact.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- the typed Fleet client helpers used for estimate and create-transfer actions

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet estimate/result presentation helpers

## Acceptance criteria

- After estimating, the player sees a compact gameplay-style result card.
- The card clearly presents squad, origin, destination, estimate, viability, and warnings when available.
- Fallback copy is clean when the backend does not provide exact values.
- The create-transfer confirmation still follows the estimate flow.
- Stale estimate and duplicate-submit guards remain in place.
- Split and merge remain disabled or prototype-only.

## Constraints

- Do not add polling or WebSockets.
- Do not add backend work unless a tiny mapping issue is discovered.
- Keep the change minimal and readable.

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
