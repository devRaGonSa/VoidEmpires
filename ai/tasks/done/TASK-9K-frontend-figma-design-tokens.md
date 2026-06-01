# TASK-9K

---
id: TASK-9K
title: Add frontend Figma design tokens
status: done
type: feature
team: frontend
supporting_teams:
  - design
  - architecture
  - docs
roadmap_item: "Phase 9K - Frontend Figma design tokens"
priority: high
---

## Goal

Add a frontend design-token foundation aligned with the Xuniverse Figma UI Concept v1.

This task should convert the currently extracted Figma tokens into reusable frontend CSS variables and constants plus documentation.

It must not redesign the full UI yet, add gameplay mutations, add production auth, add backend endpoints, add WebSockets, add Three.js/WebGL, or add final UI.

## Context

The frontend exists and currently builds. It already has:

- shell/routing/API client
- strategic map read slice
- fleet UI state read-only panels
- action manifest panels
- 2D strategic map view
- selection/detail panel
- visual-state preview links

However, the previous frontend work was functional/readiness-first and not explicitly aligned with Figma. This task begins explicit Figma alignment.

Figma visual source:

- File key: `v7xlTqIsX2bPdCIEzBAwxs`
- Page: `Xuniverse UI v1 - Modern Simple`
- Design System frame: `2:1635`

Extracted Figma tokens:

- bg: `#050814`
- top: `#070D1B`
- side: `#081023`
- panel: `#0B1328`
- panel2: `#101B36`
- line: `#243A66`
- text: `#EAF2FF`
- muted: `#8FA6C8`
- blue: `#37A8FF`
- cyan: `#46EFFF`
- green: `#52E6A7`
- red: `#FF5C7A`
- amber: `#FFC857`
- purple: `#9B7CFF`
- metal: `#AEB8C6`
- crystal: `#7DE7FF`
- deut: `#7CFFB2`

Extracted Figma layout conventions:

- 1440 x 960 desktop design
- 64px topbar
- 230px left sidebar
- Main content starts around x=260
- Resource pills are compact 142-150px x 28px
- Cards are dark, subtle, compact, with muted subtitles
- Overall style: modern, dark, simple, readable, galactic

## Implementation steps

1. Inspect current frontend:
   - `src/VoidEmpires.Frontend/src/styles.css`
   - `src/VoidEmpires.Frontend/src/App.tsx`
   - `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
   - `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
2. Add or organize CSS variables for Figma tokens.
3. Add semantic tokens, for example:
   - `--ve-color-bg`
   - `--ve-color-topbar`
   - `--ve-color-sidebar`
   - `--ve-color-panel`
   - `--ve-color-panel-2`
   - `--ve-color-border`
   - `--ve-color-text`
   - `--ve-color-muted`
   - `--ve-color-accent-blue`
   - `--ve-color-accent-cyan`
   - `--ve-color-success`
   - `--ve-color-danger`
   - `--ve-color-warning`
   - `--ve-color-purple`
   - `--ve-color-resource-metal`
   - `--ve-color-resource-crystal`
   - `--ve-color-resource-deuterium`
4. Add spacing, radius, and elevation variables aligned with Figma patterns.
5. Add a small frontend design token documentation section:
   - either in `src/VoidEmpires.Frontend/README.md`
   - or `docs/dev/frontend-figma-alignment.md`
6. Keep current functionality intact.
7. Update `ai/current-state.md` to document Phase 9K.

## Files to read first

- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/README.md`
- `docs/dev/pre-frontend-contract-checkpoint.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/README.md`
- `docs/dev/frontend-figma-alignment.md`
- `ai/current-state.md`

May also modify:

- `docs/dev/pre-frontend-contract-checkpoint.md`

## Acceptance criteria

- Figma-derived color tokens exist in frontend CSS.
- Semantic CSS variables are used or ready to use.
- Figma layout conventions are documented.
- Existing frontend build remains green.
- Existing backend build and tests remain green.
- No gameplay mutation calls are added.
- No backend code is changed.
- `ai/current-state.md` documents Phase 9K.

## Constraints

- Do not add final UI.
- Do not add gameplay mutations.
- Do not call mutating endpoints.
- Do not add production auth.
- Do not add backend endpoints.
- Do not add backend gameplay changes.
- Do not add WebSockets.
- Do not add Three.js/WebGL.
- Keep this task focused on tokens, docs, and foundation.

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
4. Commit with a clear message: `feat(frontend): add figma design tokens`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
