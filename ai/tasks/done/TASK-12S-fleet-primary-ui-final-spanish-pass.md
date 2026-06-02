# TASK-12S

---
id: TASK-12S
title: Phase 12S - Fleet primary UI final Spanish pass
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12S-12X"
priority: medium
---

## Goal
Remove the remaining visible English from the primary Fleet UI and the adjacent development shell areas shown on the Fleet screen.

## Context
The Fleet cockpit already has a working gameplay layout, but some visible labels in the main screen, shell chrome, and secondary panels still read in English. This task keeps backend contracts intact and focuses on translating the user-facing surface that players see first.

## Implementation steps

1. Inspect the Fleet page, summary panels, selected-group panel, and shell/header/sidebar components used on `/fleets`.
2. Translate visible English copy in primary and semi-primary areas into Spanish.
3. Keep internal keys, route names, and technical action labels in collapsed or secondary sections only.
4. Verify the Fleet screen still reads as a Spanish-first gameplay surface without enabling split or merge.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- the shell/sidebar/header components used by `/fleets`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- the Fleet shell/sidebar/header components used by `/fleets`
- Fleet action/manifest presentation helpers

## Acceptance criteria

- The primary Fleet UI is Spanish-first.
- Visible English examples from the latest QA notes are translated.
- Technical route names and action keys remain confined to secondary or collapsed sections.
- Split and merge remain disabled or prototype-only.
- Backend contracts are unchanged.
- Validation commands pass.

## Constraints

- Follow the existing frontend conventions.
- Do not change backend contracts.
- Do not enable split or merge execution.
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
