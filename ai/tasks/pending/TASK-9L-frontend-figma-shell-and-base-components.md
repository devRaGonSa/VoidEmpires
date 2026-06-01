# TASK-9L

---
id: TASK-9L
title: Align frontend shell and base components with Figma
status: pending
type: feature
team: frontend
supporting_teams:
  - design
  - architecture
  - docs
roadmap_item: "Phase 9L - Frontend Figma shell and base components"
priority: high
---

## Goal

Align the frontend shell and base components with the Figma layout language.

This task should introduce reusable UI primitives and update the app shell, topbar, and sidebar using the Figma-derived tokens.

It must not add gameplay mutations, production auth, backend endpoints, WebSockets, Three.js/WebGL, or final UI.

## Context

Figma layout source:

- Topbar height: `64px`
- Sidebar width: `230px`
- Main content starts around x=260
- Navigation list contains:
  - Resumen
  - Planeta
  - Construccion
  - Investigacion
  - Ejercito Tierra
  - Astillero
  - Defensas
  - Flotas
  - Galaxia
  - Espionaje
  - Alianza
  - Mercado
  - Ranking
- Resource pills in the topbar: Metal, Cristal, Deuterio, Poblacion, Energia
- User label appears on the right, e.g. `RaulG`
- The current app can still expose only implemented pages and routes; non-implemented nav entries must be disabled placeholders, not broken links

This task should create components that future pages can reuse.

Suggested components:

- `AppShell`
- `TopResourceBar`
- `SidebarNav`
- `UiCard`
- `UiBadge`
- `UiButton` or `UiActionChip`
- `UiProgressBar`
- `PrototypeWarning` or `DevEndpointNotice`

## Implementation steps

1. Inspect current shell:
   - `src/VoidEmpires.Frontend/src/App.tsx`
   - `src/VoidEmpires.Frontend/src/styles.css`
2. Add reusable components under `src/VoidEmpires.Frontend/src/components/ui/`.
3. Implement:
   - `AppShell`
   - `SidebarNav`
   - `TopResourceBar`
   - `UiCard`
   - `UiBadge`
   - `UiProgressBar`
   - `DevEndpointNotice`
4. Update `App.tsx` to use the new shell.
5. Keep existing route and page behavior intact.
6. Represent Figma navigation labels.
7. Mark not-yet-implemented sections as disabled or placeholders.
8. Do not add mutating buttons.
9. Update README or frontend Figma alignment docs if useful.
10. Update `ai/current-state.md` to document Phase 9L.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts`
- `src/VoidEmpires.Frontend/README.md`
- `docs/dev/frontend-figma-alignment.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/UiCard.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/UiBadge.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/UiProgressBar.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/DevEndpointNotice.tsx`
- `docs/dev/frontend-figma-alignment.md`
- `ai/current-state.md`

May also modify:

- `src/VoidEmpires.Frontend/README.md`

## Acceptance criteria

- Frontend shell follows Figma topbar and sidebar layout.
- Resource bar exists using Figma resource styling.
- Sidebar uses Figma navigation labels.
- Non-implemented sections are safe placeholders or disabled states.
- Reusable UI primitives exist.
- Existing pages still work.
- No mutating gameplay calls are added.
- No backend code is changed.
- Backend validation remains green.
- Frontend build passes.
- `ai/current-state.md` documents Phase 9L.

## Constraints

- Do not add gameplay mutations.
- Do not call mutating endpoints.
- Do not add production auth.
- Do not add backend endpoints.
- Do not add backend gameplay changes.
- Do not add WebSockets.
- Do not add Three.js/WebGL.
- Keep the shell simple and aligned with Figma, not final production polish.

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
4. Commit with a clear message: `feat(frontend): align shell with figma`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
