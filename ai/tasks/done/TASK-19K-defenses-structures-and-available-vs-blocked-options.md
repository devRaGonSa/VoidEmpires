# TASK-19K-defenses-structures-and-available-vs-blocked-options

---
id: TASK-19K-defenses-structures-and-available-vs-blocked-options
title: Defenses structures and available vs blocked options
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Render defensive structures and defense-related preparation options in a way that clearly separates what is currently available, blocked, already built, or only reachable through another module.

## Purpose
Make the Defenses cockpit actionable and understandable without pretending every item can be executed directly from this route.

## Current Problem
Defenses v1 needs meaningful catalog and structure cards, but those cards must communicate availability, blockers, and handoff ownership clearly. A flat mixed list would confuse the player about what can be done now.

## Context
- Construction, Research, and Shipyard already separate available versus blocked actions and surface meaningful reasons.
- Defenses may need to show both current structures and future preparations in the same cockpit.
- Some options may belong to Construction rather than a direct Defenses mutation path.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses presentation helpers
- Defenses view-model helpers
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add a structures and options section that can show:
   - existing defensive structures
   - available defense-related preparations
   - blocked defense-related preparations
2. Each card should show as much truthful state as the model supports, including:
   - defense name
   - category
   - current level or state if available
   - target level or action if applicable
   - cost
   - duration
   - requirements
   - status
   - primary action or blocked reason
3. Available preparations should expose player-safe action framing such as:
   - `Revisar defensa`
   - `Revisar orden defensiva`
4. Blocked cards should expose concise Spanish reasons such as:
   - `Faltan recursos`
   - `Requisito pendiente`
   - `No disponible en esta build`
5. If an item belongs to Construction instead of direct Defenses mutation, show a clear handoff message such as `Gestionar construccion desde Construccion`.
6. Avoid raw enum names anywhere in the main cards.

## UI/UX Requirements
- Cards should be compact, scannable, and clearly grouped.
- Blocked cards should look secondary and informative, not broken.
- Spanish copy must stay consistent with earlier taxonomy work.

## Backend/API Requirements
- No backend change expected unless the read model lacks fields that are already safely derivable.

## Safety Constraints
- No combat behavior.
- No direct mutation unless later confirmation work explicitly enables a safe path.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses view-model or presentation helper files
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Defensive catalog and readiness cards render meaningfully.
- Available, blocked, and handoff states are clearly distinguishable.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- The cockpit may still be mostly read-only in v1, but the cards should make that an intentional design choice rather than an empty limitation.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Focus on card state clarity over visual polish.
