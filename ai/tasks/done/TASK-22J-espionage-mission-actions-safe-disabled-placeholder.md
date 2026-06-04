# TASK-22J

---
id: TASK-22J
title: Phase 22J - Espionage mission actions safe disabled placeholder
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: medium
---

## Goal

Show future espionage mission actions as safe disabled placeholders rather than executable controls.

## Purpose

Espionage should acknowledge the roadmap shape of the module without misleading players into thinking sabotage, infiltration, theft, or counter-espionage already work.

## Current problem

The page needs to explain the future boundary clearly. If it omits actions entirely, the module can feel unfinished; if it presents them too strongly, it can imply unsupported gameplay.

## Context

Other accepted cockpits already use disabled or handed-off action panels to communicate safe boundaries. Espionage should follow that pattern and keep unsupported operations unmistakably inactive.

## Files to read first

- Espionage page component
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- shared action-button or card styles
- `docs/dev/cockpit-copy-guidelines.md`

## Component discovery

Review disabled action treatment in `Defenses`, `Ground Army`, `Shipyard`, and `Research`. Match the shared hierarchy where unsupported actions are secondary and explanatory.

## Implementation requirements

1. Add a section such as `Misiones futuras` or `Acciones de inteligencia`.
2. Show disabled placeholders for actions such as:
   - `Reconocimiento activo`
   - `Infiltracion`
   - `Sabotaje`
   - `Contraespionaje`
   - `Robo de tecnologia`
3. Each placeholder must clearly communicate:
   - `No disponible en esta version.`
   - `Solo lectura en esta cabina.`
4. Do not wire click handlers, mutation calls, or optimistic UI.
5. Do not present disabled actions as the brightest or most primary controls on the page.
6. Prefer grouped or summarized placeholders if too many cards would crowd the viewport.

## UI/UX requirements

- Useful for roadmap clarity
- Not misleading
- Spanish-first
- Secondary or disabled visual treatment

## Backend/API requirements

- None
- No new endpoints

## Expected files to modify

- Espionage page component
- presentation helpers if action labels or disabled reasons are centralized
- styles if a disabled action pattern needs small cockpit-specific support

## Safety constraints

- No mission execution
- No sabotage
- No theft
- No counter-espionage
- No backend calls from disabled controls

## Acceptance criteria

- Future mission actions are visible but unmistakably disabled.
- The page communicates the roadmap boundary without suggesting current availability.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If the disabled section becomes too visually heavy, reduce it to a compact summary with one or two representative cards.
- Keep terminology aligned with the taxonomy task to avoid duplicate phrasing.

## Commit and push

1. Run `git status`.
2. Confirm only intended frontend files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep this as a presentation-only task.
