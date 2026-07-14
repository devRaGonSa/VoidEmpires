# TASK-54A

---
id: TASK-54A
title: Fleet page decluttering
status: done
type: product
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 54"
priority: high
---

## Goal
Replace the diagnostic-heavy Fleets cockpit with the compact player-facing Block 54 layout.

## Context
The normal page must derive route context, hide technical IDs and diagnostic panels, and retain only the compact hero, fleet summary, local stock creation, stationed fleets, movement composer, and active movements.

## Implementation steps
1. Inspect the existing Fleets page, view models, shared UI components, and route-context pattern.
2. Remove normal-mode diagnostic/navigation/placeholder surfaces.
3. Establish concise responsive sections and empty states for the product flow.

## Files to read first
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/api/fleetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummary.tsx`
- `src/VoidEmpires.Frontend/src/components/LocalOrbitalStockPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetCreationCard.tsx`
- `src/VoidEmpires.Frontend/src/components/StationedFleetList.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetMovementComposer.tsx`
- `src/VoidEmpires.Frontend/src/components/ActiveFleetMovements.tsx`

## Acceptance criteria
- Normal Fleets UI contains only the Block 54 product sections.
- No manual GUID loader, repeated navigation, raw IDs, or diagnostic-heavy copy remains.
- Empty states are concise and controls remain accessible and responsive.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-copy-regressions.ps1`

## Commit and push
Commit separately on the Block 54 branch; push after the complete block validates.

## Change Budget
- Prefer fewer than 5 files and under 200 changed lines.
