# TASK-53B

---
id: TASK-53B
title: Ground Army playable production UI
status: pending
type: product
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 53"
priority: high
---

## Goal
Replace the Ground Army readiness dashboard with a compact quantity-based production module aligned with Shipyard and Defenses.

## Context
The current page contains prototype navigation and readiness/dashboard copy. The player-facing page needs one compact hero, an optional live queue, a compact garrison, a four-column catalog, and a confirmation modal.

## Implementation steps

1. Inspect the Ground Army page and the shared production-card, action-row, modal, and countdown patterns.
2. Implement quantity selection, totals, stable blockers, confirmation, enqueue, and reload behavior.
3. Add focused frontend regression coverage for formatting, expiry, and removed prototype copy.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- `src/VoidEmpires.Frontend/src/components/GroundArmyCatalogCard.tsx`
- `src/VoidEmpires.Frontend/src/components/LiveQueueCountdown.tsx`
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- `src/VoidEmpires.Frontend/src/components/GroundArmyCatalogCard.tsx`
- `src/VoidEmpires.Frontend/src/components/GroundArmyCatalogCard.test.tsx`
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.test.tsx`

## Acceptance criteria

- The page contains only the compact productized sections requested by Block 53.
- Each ground card supports quantity, authoritative totals, blockers, and confirmation.
- The active queue uses `LiveQueueCountdown`, triggers expiry once per order, and cannot remain permanently misleading.
- Removed prototype/readiness copy is guarded by tests.

## Constraints

- Reuse the existing production action-row and modal patterns.
- Keep Home unchanged unless shared timestamp handling naturally applies.
- No Tailwind or new UI framework.

## Validation

- `npm run build`
- Block 53 frontend guard scripts

## Commit and push

Commit this task separately on the Block 53 branch; push after the complete Block 53 cycle validates.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

