# TASK-12N

---
id: TASK-12N
title: Phase 12N - Planet and squad identity presentation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12N"
priority: medium
---

## Goal
Improve planet and squad identity presentation so names are primary and technical IDs are clearly secondary metadata.

## Context
Manual review still shows too many combined name-and-ID labels in the Fleet cockpit, which makes the screen feel more technical than playable. This task should make names primary, keep IDs available for development, and reduce visual noise in list cards, selects, status panels, and feedback areas.

## Implementation steps

1. Inspect every Fleet cockpit place where planet or squad IDs appear, including squad cards, the selected squad panel, order selectors, active transfer data, resource contexts, feedback panels, and technical sections.
2. Rework labels so friendly names appear first and technical IDs are pushed into muted secondary metadata.
3. Improve option labels and disambiguation so the UI stays readable without repeating full IDs or overloading the primary line.
4. Keep technical identifiers accessible for development, but avoid showing them as the dominant visual content.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- Planet and squad names are visually primary throughout the Fleet cockpit.
- Technical IDs are shown as secondary metadata, not as the main readable label.
- Labels such as `Aurelia`, `Corona de éter`, `Alcance de cenizas`, `Nave exploradora`, `Nave de carga`, and `Nave de escolta` are prominent while IDs are muted.
- Select options keep enough disambiguation to remain usable without producing visual noise.
- No backend DTO changes or new endpoints are introduced unless a tiny frontend mapping helper is clearly justified.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and metadata-oriented.
- Do not remove IDs entirely from development-facing areas.
- Do not add new endpoints or change backend contracts.
- Split follow-up work if identity cleanup requires broader layout changes.

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
- Prefer a single commit for this task.
- If identity presentation changes grow too broad, stop and create a follow-up task first.
