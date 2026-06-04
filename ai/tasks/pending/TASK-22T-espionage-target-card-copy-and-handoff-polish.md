# TASK-22T

---
id: TASK-22T
title: Phase 22T - Espionage target card copy and handoff polish
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: medium
---

## Goal

Polish Espionage target-card labels, descriptions, and neighboring cockpit handoff copy so targets read naturally in Spanish.

## Purpose

The target catalog already works, but some wording is repetitive, technical, or slightly awkward. This task keeps the current routing intact while making target interpretation and handoff language easier to scan.

## Current problem

Target cards are functional, but some labels and helper text still read awkwardly or too generically, for example:

- `Handoff sugerido`
- `Cobertura`
- repetitive technical counts
- generic `Galaxia` references that do not explain why that handoff is the next useful surface

## Context

Espionage interprets intelligence. `Galaxy`, `Planet`, `Fleets`, and `Research` remain the neighboring surfaces where the user continues broader inspection. The goal is clearer language, not new routing behavior.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`

## Component discovery

Identify where target-card labels, summaries, and route handoff text are assembled. Check whether route labels already come from shared helpers elsewhere so Espionage can reuse the same action language instead of inventing a special dialect.

## Implementation requirements

1. Replace `Handoff sugerido` with clearer Spanish such as:
   - `Cabina sugerida`
   - or `Siguiente lectura`
2. Normalize handoff labels by destination:
   - Galaxy -> `Revisar en Galaxia` or `Volver al mapa`
   - Planet -> `Abrir planeta`
   - Fleets -> `Revisar flotas`
   - Research -> `Ver progreso tecnológico`
3. Improve target-card descriptions with more natural wording, for example:
   - `Los datos incompletos no desbloquean acciones ofensivas.`
   - `La lectura depende de la visibilidad estratégica actual.`
4. Reduce repetition when target cards already show visibility and confidence elsewhere.
5. Avoid exposing raw backend field names, DTO wording, or key-like labels in the visible card body.
6. Keep existing context-preserving links intact unless a link is clearly broken.

## UI/UX requirements

- Target cards should remain scannable.
- Handoff labels should be action-oriented but clearly non-mutating.
- Spanish-first wording should match the accepted cockpit tone used elsewhere.

## Backend/API requirements

- None.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts` only if a shared label helper is the right owner

## Safety constraints

- No route behavior changes beyond label or presentation polish unless a route is actually broken
- No mutations
- No new navigation model

## Acceptance criteria

- Target cards read naturally in Spanish.
- Handoff labels are consistent and context-preserving.
- The visible copy no longer relies on awkward technical wording.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Exact lore tone can still evolve later; this task should focus on clarity first.
- If route label patterns are already shared across cockpits, prefer aligning to those conventions rather than making Espionage unique.

## Commit and push

1. Run `git status`.
2. Verify only target-card or handoff presentation files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep routing logic stable.
