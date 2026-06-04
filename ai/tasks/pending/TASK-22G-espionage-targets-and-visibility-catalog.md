# TASK-22G

---
id: TASK-22G
title: Phase 22G - Espionage targets and visibility catalog
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Render known, visible, and partial targets as a readable grouped intelligence catalog.

## Purpose

Espionage should turn existing map intelligence into an analysis catalog that tells the player what is known, what is partially known, and what requires further observation.

## Current problem

Galaxy already shows map-oriented intelligence, but Espionage needs a target-focused view. Without a grouped catalog, the cockpit will not feel distinct or useful.

## Context

The accepted `Galaxy` cockpit already exposes relevant systems, visible planets, partial knowledge, fleet markers, and transfer overlays. Espionage must reframe that information without duplicating map interaction or exposing offensive controls.

## Files to read first

- Espionage page component
- Espionage view-model or presentation helpers
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Component discovery

Review how other cockpits render grouped cards, list sections, badges, and compact supporting metadata. Reuse existing card density and status treatment where possible.

## Implementation requirements

1. Group targets into sections such as:
   - `Sistema propio`
   - `Sistema observado`
   - `Contacto parcial`
   - `Senal orbital`
   - `Sin confirmar`
2. Each target card should show, where supported:
   - target name
   - target type
   - parent system
   - visibility or confidence state
   - known planet type
   - ownership or control if known
   - signal or fleet-marker hints if available
   - recommended handoff destination
3. For uncertain targets, use clear copy such as:
   - `Lectura parcial`
   - `Datos incompletos`
   - `Contacto sin confirmar`
4. Hide raw GUIDs and raw backend identifiers from the primary card body.
5. Do not expose offensive buttons or mission launch controls.

## UI/UX requirements

- Cards must be compact, scannable, and consistent with the current cockpit visual language
- Uncertainty must be obvious through both text and status styling
- Spanish-first copy
- Grouping must help the player reason about the target list at a glance

## Backend/API requirements

- No backend change expected unless the read model lacks minimum target data
- If backend changes become necessary, keep them read-only and covered by tests in the owning backend task

## Expected files to modify

- Espionage page component
- Espionage view-model or grouping helper files
- styles supporting grouped card layouts

## Safety constraints

- No mutation
- No spy mission creation
- No misleading target certainty

## Acceptance criteria

- The target catalog is readable and meaningfully grouped.
- Known, observed, and uncertain targets are visually distinct.
- The primary UI never surfaces raw ids as the main target label.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If current backend support cannot yet produce all five categories, render the supported groups truthfully and document the gap rather than synthesizing fake groups.
- Later mission flows may reuse target identifiers, but this task should keep them strictly read-only.

## Commit and push

1. Run `git status`.
2. Confirm only intended frontend files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep grouping logic reusable and avoid bloating the page component.
