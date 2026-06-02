# TASK-13B

---
id: TASK-13B
title: Phase 13B - Fleet technical copy collapse
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12Y-13D"
priority: medium
---

## Goal
Move remaining technical and API-oriented explanatory text out of the primary Fleet gameplay panels and into secondary or collapsed details.

## Context
Some Fleet panels still surface route IDs, action contract names, and endpoint-style language too directly. The goal is to keep useful technical context available while making the primary screen feel like gameplay rather than API documentation.

## Implementation steps

1. Inspect the visible Fleet text for technical or API-oriented phrases.
2. Replace primary gameplay copy with readable Spanish status and guidance text.
3. Move technical details into secondary, collapsed, or debug-specific sections.
4. Keep real error states visible and keep guardrails intact.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet technical detail and helper text rendering

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet technical detail helpers, if needed

## Acceptance criteria

- Primary gameplay panels use readable Spanish guidance.
- Technical/API phrases are kept in secondary or collapsed details.
- Guardrails and actual error states remain visible.
- Split and merge remain disabled or prototype-only.
- Backend contracts are unchanged.

## Constraints

- Do not hide real error states.
- Do not remove important safety warnings.
- Do not change backend contracts.
- Do not enable split or merge execution.

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
