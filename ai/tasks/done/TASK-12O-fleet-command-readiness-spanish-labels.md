# TASK-12O

---
id: TASK-12O
title: Phase 12O - Fleet command readiness Spanish labels
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12O"
priority: medium
---

## Goal
Translate and simplify the selected squad command readiness area so it looks like game UI rather than raw command metadata.

## Context
The selected squad panel now gives useful command context, but the readiness area still reads too much like an internal contract summary in places. This task should make command readiness and blocked reasons feel like short game labels while keeping the technical details available as secondary metadata when needed.

## Implementation steps

1. Inspect the selected squad panel, command readiness rendering, and any helper text that explains command eligibility or blocking reasons.
2. Replace technical command labels with concise Spanish gameplay labels.
3. Rewrite visible blocked reasons into readable Spanish and keep detailed technical reasons secondary or collapsed where useful.
4. Demote API-oriented bullets and route details from the main selected squad panel, especially where they are currently presented as primary content.
5. Preserve all current command behavior and guardrails, and validate with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- The readiness area uses short Spanish gameplay labels such as `Disponible`, `Lista para ordenar`, `Divisible`, `Fusionable`, and `Sin traslado anulable` where appropriate.
- Technical readiness details remain accessible as secondary information instead of dominating the main panel.
- Readable Spanish blocked reasons such as `Falta elegir destino.`, `Requiere una estimación válida.`, `La escuadra está reservada.`, and `Ya existe un traslado activo.` replace raw technical phrasing in the primary view.
- Complete-due, split, and merge remain disabled or prototype-only.
- No backend behavior changes or new endpoints are introduced.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and label-oriented.
- Do not enable extra commands.
- Do not remove important blocked reasons; rewrite them into readable Spanish.
- Split follow-up work if the readability work requires broader panel redesign.

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
- If readiness labels need deeper state modeling, stop and create a smaller follow-up task first.
