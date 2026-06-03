# TASK-13W

---
id: TASK-13W
title: Phase 13W - Planet construction action availability and guards
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Surface construction action availability clearly and guard the UI from unsupported or unsafe mutation attempts.

## Context
Before enabling any construction execution flow, the Planet cockpit should show what is available, blocked, or diagnostic-only. This keeps the UI honest and prevents blind mutation buttons against incomplete backend support.

## Implementation steps

1. Review the Planet cockpit action metadata and current backend support for construction-related commands.
2. Render player-readable action availability states such as available, insufficient resources, missing requirement, already queued, unavailable, or diagnostic-only.
3. Add view-model guards so unsupported actions cannot trigger network calls.
4. Keep raw capability or backend keys secondary or collapsed.

## Files to read first

- Planet cockpit types and components from Phases 13Q-13V
- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
- `src/VoidEmpires.Application/Buildings/`
- existing fleet guarded-action patterns in `src/VoidEmpires.Frontend/src/pages/` or components

## Expected files to modify

- Planet cockpit action components
- Planet frontend view-model helpers
- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`, if guarded action wiring is needed

## Acceptance criteria

- Construction actions surface readable Spanish availability states.
- Unsupported actions stay disabled or clearly diagnostic-only.
- The UI cannot fire unsupported construction calls.
- Raw capability keys are secondary and do not dominate primary action labels.
- The task does not yet enable blind mutation buttons.

## Constraints

- Any future mutation must require explicit confirmation.
- Do not invent execution support that the backend does not provide.
- Keep the action safety model deterministic and conservative.
- Preserve readable Spanish labeling.

## Validation

Before completing the task ensure:

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
