# Frontend Figma Alignment

## Purpose

This document records the current frontend token foundation derived from the Figma concept:

- file key: `v7xlTqIsX2bPdCIEzBAwxs`
- page: `Xuniverse UI v1 - Modern Simple`
- design system frame: `2:1635`

Phase 9K does not implement the final interface. It establishes reusable frontend tokens so later frontend tasks can align individual screens and components without re-encoding palette and spacing decisions.

## Token source

Extracted color source:

- `bg`: `#050814`
- `top`: `#070D1B`
- `side`: `#081023`
- `panel`: `#0B1328`
- `panel2`: `#101B36`
- `line`: `#243A66`
- `text`: `#EAF2FF`
- `muted`: `#8FA6C8`
- `blue`: `#37A8FF`
- `cyan`: `#46EFFF`
- `green`: `#52E6A7`
- `red`: `#FF5C7A`
- `amber`: `#FFC857`
- `purple`: `#9B7CFF`
- `metal`: `#AEB8C6`
- `crystal`: `#7DE7FF`
- `deut`: `#7CFFB2`

## Frontend token mapping

The frontend token layer lives in `src/VoidEmpires.Frontend/src/styles.css`.

Primary semantic mapping:

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

Supporting UI foundation:

- spacing scale through `--ve-space-*`
- radius scale through `--ve-radius-*`
- panel/elevation scale through `--ve-shadow-*`
- reserved shell layout variables through `--ve-layout-*`

## Current layout conventions

Reserved layout targets from the Figma concept:

- desktop frame: `1440 x 960`
- topbar height: `64px`
- sidebar width: `230px`
- main content start: approximately `260px`
- compact resource pills around `142-150px x 28px`

The current prototype does not yet implement the final sidebar or final resource-pill composition. These layout values are stored now so later tasks can consume them consistently.

## Usage rules

- Prefer semantic tokens over raw hex values in component and page styling.
- Keep the current frontend dark, compact, and readable rather than introducing a final visual overhaul in token-only tasks.
- Treat resource colors as domain accents, not generic status colors.
- Keep current development-only warnings visually clear.

## Non-goals for Phase 9K

- no final production UI
- no gameplay mutations
- no auth redesign
- no backend contract changes
- no WebSocket or renderer pipeline work
