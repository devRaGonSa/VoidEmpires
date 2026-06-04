# TASK-23F

---
id: TASK-23F
title: Phase 23F - Market dashboard economy summary
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Create the top-level Market dashboard summary that frames reserves, production, trade potential, reference prices, and a recommended focus.

## Purpose

The Market cockpit needs an opening layer that helps the player understand the current economy state quickly before they inspect detailed reserve, ratio, or route sections.

## Current problem

Even after the route loads real data, the Market cockpit would still feel unfinished without a meaningful summary that explains what the economy currently looks like and what deserves attention.

## Context

Accepted cockpits use a clear top-level summary or hero section to orient the player. Market should do the same while staying read-only and avoiding active-trade language.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- `src/VoidEmpires.Frontend/src/components/CockpitHero.tsx`
- Market view-model and label helpers
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Component discovery

Inspect how accepted cockpits build summary cards, recommendation panels, and hero notes. Prefer reusing `CockpitHero`, existing summary card styling, and current badge or status helper patterns.

## Dependency analysis

Expected rendering chain:

- Market UI state -> Market view model
- view model summary -> hero or dashboard section
- shared helpers -> labels, signals, and recommendation copy

The summary should be driven by the normalized Market view model rather than scattered inline logic.

## Implementation requirements

1. Create a top-level dashboard section that summarizes:
   - reserve posture
   - production posture
   - trade potential or economy readiness
   - reference-price availability
   - a recommended focus or next place to inspect
2. Use current backend facts where available and remain honest where data is missing.
3. Support recommendation copy such as:
   - review local reserves
   - inspect production pressure
   - compare reference ratios
   - hand off to another cockpit when Market cannot act directly
4. Reuse shared summary, badge, and hero components instead of inventing one-off layout patterns.
5. Keep Market framed as an economy-reading cockpit, not an order-entry console.

## UI/UX requirements

- The summary should feel like a real cockpit opening state
- Spanish-first
- Clear hierarchy for the most important economy insight
- Avoid oversized diagnostics or visually dominant disabled actions in the hero area

## Backend/API requirements

- No backend change is expected unless the read model lacks a small summary field that is already derivable and safely testable.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- Market view-model or helper files
- shared presentational components only if a tiny extension is required
- `src/VoidEmpires.Frontend/src/styles.css` if section styling needs a narrow addition

## Safety constraints

- No transaction buttons
- No resource mutation
- No fake live-market claims
- No secondary diagnostics crowding out primary gameplay-facing copy

## Acceptance criteria

- Market has a readable top-level dashboard summary.
- The summary helps the player understand current economy posture without requiring deep inspection first.
- The section remains truthful about read-only limitations.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Recommendation logic should remain conservative; if there is not enough data to confidently recommend a focus, the page should say so rather than overstate certainty.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on the Market dashboard surface.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer small view-model additions over large new component trees.
- If the summary starts depending on many new backend-derived metrics, split the extra analytics into a follow-up task.
