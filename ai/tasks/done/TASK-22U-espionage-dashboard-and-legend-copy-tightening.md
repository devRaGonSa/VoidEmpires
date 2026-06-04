# TASK-22U

---
id: TASK-22U
title: Phase 22U - Espionage dashboard and legend copy tightening
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: medium
---

## Goal

Tighten the Espionage hero, dashboard summary, and intelligence legend copy so the page explains confidence levels clearly without reading like debug output.

## Purpose

The top of the cockpit already communicates the right concepts, but some phrases are longer or more technical than necessary. This task improves readability and tone while preserving the current read-only intelligence meaning.

## Current problem

The hero and explanation sections work, but parts of the page still risk sounding too operational or technical. The cockpit should clarify what each intelligence level means without drifting into raw read-model language or overexplaining.

## Context

The page currently includes:

- a summary of coverage
- intelligence level cards or legend entries
- recommended focus
- target catalog

Those sections already establish the cockpit structure, so this task should refine copy rather than redesign the layout.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `docs/dev/cockpit-copy-guidelines.md`

## Component discovery

Trace the visible wording for hero copy, section headings, legend labels, and supporting paragraphs. Reuse any existing presentation helper that already maps visibility or confidence language instead of duplicating strings inside JSX.

## Implementation requirements

1. Review and polish visible copy in:
   - the hero
   - `Resumen de cobertura`
   - `Cómo interpretar la inteligencia`
   - the recommended focus section
2. Preserve the current meaning for confidence or visibility levels such as:
   - confirmed
   - visible or observed
   - partial contact
   - orbital signal
   - unconfirmed
3. Prefer compact Spanish wording such as:
   - `Confirmado`
   - `Lectura estable`
   - `Datos incompletos`
   - `Señal pasiva`
   - `Sin evidencia estable`
4. Avoid long paragraphs when a short heading plus one supporting sentence is enough.
5. Keep the safety statement that incomplete data does not unlock offensive actions.
6. Do not introduce invented certainty scores or false precision.

## UI/UX requirements

- Spanish-first
- Compact and readable explanatory copy
- Clear visual hierarchy between title, supporting note, and diagnostics
- No debug-tone phrasing in the first viewport

## Backend/API requirements

- None.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`

## Safety constraints

- No gameplay changes
- No new intelligence model
- No fabricated percentages or hidden logic

## Acceptance criteria

- Dashboard and legend copy are clearer and shorter.
- The read-only intelligence boundary remains explicit.
- The first viewport feels Spanish-first and gameplay-facing.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Final lore tone can be revisited later, but this task should prioritize clarity and consistency with other accepted cockpits.
- If the current legend relies on shared status wording, align to those shared helpers instead of hand-authoring competing labels.

## Commit and push

1. Run `git status`.
2. Confirm only dashboard or legend presentation files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep layout churn minimal.
