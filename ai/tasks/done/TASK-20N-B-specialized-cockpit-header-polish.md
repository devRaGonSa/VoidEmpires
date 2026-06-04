# TASK-20N-B-specialized-cockpit-header-polish

---
id: TASK-20N-B-specialized-cockpit-header-polish
title: Specialized cockpit header polish follow-up
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Extend the header-hierarchy polish to the specialized accepted cockpits after the shared shell header pattern is established.

## Context
`TASK-20N` established a shared hero pattern for the shell-defining cockpits, but applying the same pass to Research, Shipyard, Defenses, and Ground Army in the same change would exceed the repository's preferred file-count budget.

## Implementation steps

1. Reuse the shared cockpit hero pattern already introduced for the shell-defining pages.
2. Apply it to `Research`, `Shipyard`, `Defenses`, and `Ground Army`.
3. Keep each module name, purpose, and key safety limitation prominent while demoting repetitive development wording.

## Files to read first

- `src/VoidEmpires.Frontend/src/components/CockpitHero.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`

## Acceptance criteria

- Specialized cockpit headers feel less repetitive.
- Development-only wording remains visible but secondary.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Keep the change limited to header hierarchy and copy placement.
- Do not modify unrelated behavior or gameplay flows.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
