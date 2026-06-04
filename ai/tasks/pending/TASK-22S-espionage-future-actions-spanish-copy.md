# TASK-22S

---
id: TASK-22S
title: Phase 22S - Espionage future actions Spanish copy
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: high
---

## Goal

Normalize the disabled future-action cards in Espionage into consistent Spanish copy with an unambiguous read-only boundary.

## Purpose

The page may show future espionage categories as roadmap hints, but they currently use English placeholder wording. This task keeps those cards visible only as future references while making their disabled state clearer and fully Spanish.

## Current problem

Future-action cards still display English text such as:

- `Reconnaissance remains a future placeholder and is not executable from this cockpit.`
- `Infiltration gameplay is not implemented.`
- `Sabotage gameplay is not implemented.`

This wording feels technical, exposes English implementation language, and weakens the read-only contract.

## Context

Espionage v1 must remain read-only. Future actions can be shown only as disabled roadmap hints. They must never look like buttons that the user should expect to execute from this cockpit in the current phase.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- `docs/dev/cockpit-copy-guidelines.md`

## Component discovery

Inspect how other accepted cockpits render disabled actions and limitation messaging. Reuse existing badge, status, or secondary-copy patterns rather than inventing a one-off placeholder style just for Espionage.

## Implementation requirements

1. Translate and normalize all visible future-action labels and support text, covering at minimum:
   - `Reconocimiento activo`
   - `Infiltración`
   - `Sabotaje`
   - `Contraespionaje`
   - `Robo de tecnología`
2. Use consistent disabled messaging such as:
   - `No disponible en esta versión.`
   - `Solo lectura en esta cabina.`
   - `La misión queda visible como referencia futura, pero no se puede ejecutar.`
3. Remove English placeholder terms such as:
   - `gameplay`
   - `placeholder`
   - `implemented`
   - `executable`
4. Keep any buttons or action affordances disabled and visually secondary.
5. Do not wire handlers.
6. Do not call any API.
7. Preserve honest limitation language so the page does not imply that sabotage, infiltration, or theft systems already exist.

## UI/UX requirements

- Disabled future actions must remain visibly secondary.
- Copy must make the read-only boundary obvious in the first glance.
- Labels should be short; nuance belongs in supporting text, not oversized buttons.

## Backend/API requirements

- None.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts` only if the existing disabled-state helpers need reuse

## Safety constraints

- No espionage execution
- No sabotage
- No infiltration
- No counter-espionage
- No theft
- No API mutation

## Acceptance criteria

- All future-action cards are Spanish-first.
- Future-action cards remain visibly disabled and read-only.
- No English placeholder wording remains in those cards.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Later phases may selectively enable one mission family, but this task must not pre-wire that future scope.
- If a shared disabled-state helper uses neutral copy that already fits, prefer reusing it instead of adding Espionage-specific duplication.

## Commit and push

1. Run `git status`.
2. Confirm the changes stay within future-action presentation scope.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep the change presentation-only.
