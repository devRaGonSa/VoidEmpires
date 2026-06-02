# TASK-12T

---
id: TASK-12T
title: Phase 12T - Fleet planet identity final cleanup
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12S-12X"
priority: medium
---

## Goal
Make planet and squad labels feel like game objects instead of database or debug labels.

## Context
The current Fleet UI still exposes IDs too prominently in some labels and selectors. The screen should prefer readable planet and squad names while keeping IDs available as muted metadata for development and disambiguation.

## Implementation steps

1. Find every Fleet label where planet, squad, group, civilization, or tactical IDs are shown too prominently.
2. Rework the primary labels so names come first and IDs are demoted to secondary metadata.
3. Update select labels so they read like playable game objects with enough disambiguation for development.
4. Keep IDs available in technical or collapsed areas when needed, but avoid repeating them in the main labels.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- the Fleet label helpers and select presentation components

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet label mapping helpers, if needed

## Acceptance criteria

- Primary labels prefer names such as planet or squad names.
- IDs are shown as secondary metadata, not as the main label.
- Select labels remain understandable and still disambiguate dev data.
- No backend DTO changes are made unless a tiny frontend mapping helper cannot solve it.
- No new endpoints are added.
- Split and merge remain disabled or prototype-only.

## Constraints

- Keep the frontend mapping small and local.
- Do not remove IDs from technical or dev areas.
- Do not change backend DTOs unless absolutely necessary.
- Keep the change minimal.

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
