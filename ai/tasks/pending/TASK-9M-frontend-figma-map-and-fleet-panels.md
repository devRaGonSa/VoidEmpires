# TASK-9M

---
id: TASK-9M
title: Align strategic map and fleet panels with Figma
status: pending
type: feature
team: frontend
supporting_teams:
  - design
  - docs
roadmap_item: "Phase 9M - Frontend Figma map and fleet panels"
priority: high
---

## Goal

Align the existing strategic map and fleet/readiness panels with the Figma visual language.

This task should update the current 2D map, selection panel, visual-state preview, fleet UI panel, and action manifest panels to use the new UI primitives and Figma-derived tokens.

It must remain read-only and must not add gameplay mutations or backend changes.

## Context

Figma has relevant frames:

- `2:625` Galaxia
  - clean map panel
  - system and slot markers
  - legend and status labels: Libre, Debil, Aliado, Hostil, Escombros
- `2:536` Flotas
  - mission setup panel
  - active fleet cards with progress bars
  - compact rows and readable state
- `2:2` Resumen
  - dashboard cards
  - resource cards
  - activity feed
  - local system mini view
- `2:1635` Design System
  - card, progress, badge, fleet row, button components
- `4:2` Planeta v2 - Selector
  - active planet selector pattern
  - compact top context panel

The implemented frontend currently displays real backend dev data. It should not invent gameplay, but it can use Figma-like structure, cards, badges, legends, and progress bars to present current read-only data more coherently.

## Implementation steps

1. Inspect current frontend pages:
   - `StrategicMapPage.tsx`
   - `FleetsPage.tsx`
   - `StrategicMap2DView.tsx`
   - selection, detail, and visual-state components
2. Refactor to use UI primitives from Phase 9L.
3. Align the strategic map:
   - use dark panel and card composition
   - add a Figma-like map legend
   - improve system nodes with visibility and status classes
   - retain backend-driven coordinates
   - keep empty and one-system states safe
4. Align the selection and detail panel:
   - use cards, badges, progress, and compact rows
   - group data into readable sections
   - keep readiness metadata clearly non-authoritative
5. Align the fleet page:
   - use cards for fleet groups and active transfers
   - use progress bars for transfer progress where available
   - action manifest panels must remain read-only
   - mutating actions can be marked but not executable
6. Align the visual-state preview:
   - use compact readable JSON and details presentation
   - keep dev-only warning
7. Update frontend Figma docs.
8. Update `ai/current-state.md` to document Phase 9M.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSelectionPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/VisualStatePreviewPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/frontend-figma-alignment.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSelectionPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/VisualStatePreviewPanel.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/frontend-figma-alignment.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`

May also add:

- `src/VoidEmpires.Frontend/src/components/ui/UiSectionHeader.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/UiDataRow.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/UiLegend.tsx`

## Acceptance criteria

- Strategic map page visually follows Figma map and card style.
- Fleet page visually follows Figma fleet, card, and progress style.
- Existing read-only API consumption remains intact.
- Action manifests remain read-only.
- No mutating gameplay calls are wired.
- No backend code is changed.
- Backend validation remains green.
- Frontend build passes.
- `ai/current-state.md` documents Phase 9M.

## Constraints

- Do not add gameplay mutations.
- Do not call mutating endpoints.
- Do not add production auth.
- Do not add backend endpoints.
- Do not add backend gameplay changes.
- Do not add WebSockets.
- Do not add Three.js/WebGL.
- Do not add final UI design beyond Figma-aligned foundation.
- Keep UI robust to missing or null data.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Also run frontend validation:

```powershell
cd src/VoidEmpires.Frontend
npm install
npm run build
cd ../..
```

Expected result:

- backend clean build
- backend tests passing
- frontend build passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected frontend, docs, and current-state files changed.
4. Commit with a clear message: `feat(frontend): align map and fleet panels with figma`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
