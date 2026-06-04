# TASK-23J

---
id: TASK-23J
title: Phase 23J - Market future actions disabled placeholders
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Show future market actions as disabled, safe placeholders instead of executable controls.

## Purpose

Market naturally suggests buying, selling, listing, auctioning, importing, and exporting. This task should make those future directions visible without accidentally turning them into real gameplay.

## Current problem

Without explicit disabled placeholders, players may not understand the intended future scope of the cockpit. But if the controls look too active or are wired incorrectly, the page could mislead users into expecting real transactions.

## Context

Espionage future actions and the safe placeholder patterns in other accepted cockpits already show how roadmap-facing but disabled actions can be framed honestly.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- current action button styles in `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/cockpit-copy-guidelines.md`

## Component discovery

Inspect how accepted cockpit pages render disabled future actions, non-primary buttons, and explanation text for unsupported operations. Prefer matching those conventions.

## Dependency analysis

Expected placeholder flow:

- Market future-action metadata or local config -> disabled action cards
- shared status or badge helpers -> non-primary visual treatment
- no API client or mutation handler attachment

## Implementation requirements

1. Add a section such as:
   - `Operaciones futuras`
   - `Acciones de mercado`
2. Show disabled placeholders for actions such as:
   - `Comprar recursos`
   - `Vender recursos`
   - `Crear oferta`
   - `Crear ruta comercial`
   - `Exportar recursos`
   - `Importar recursos`
3. Each placeholder should clearly communicate:
   - `No disponible en esta version.`
   - `Solo lectura en esta cabina.`
   - `La operacion queda visible como referencia futura, pero no se puede ejecutar.`
4. Do not wire mutation handlers.
5. Do not call any endpoint.
6. Do not style disabled actions as primary bright buttons.
7. Prefer secondary or explicitly disabled styling.

## UI/UX requirements

- Useful for roadmap clarity
- Not misleading
- Spanish-first
- Clearly secondary to the live read-only Market content

## Backend/API requirements

- None

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- shared helper files only if a small disabled-action presentation helper is needed

## Safety constraints

- No buying
- No selling
- No offers
- No transfers
- No trade routes

## Acceptance criteria

- Future Market actions are visible but unmistakably disabled.
- No network call or mutation path is introduced for those controls.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Future blocks may later activate selected operations, but this task must keep the current cockpit strictly read-only.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm only intended Market presentation files changed.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer reuse of existing disabled-card styling over new action component complexity.
- If shared action styling requires broader work, leave a focused follow-up task instead.
