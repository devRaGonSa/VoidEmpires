# TASK-46A

---
id: TASK-46A
title: OGame module polish contract
status: done
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Document the corrected OGame-like module contract after visual review.

## Context
Construction, Research, Shipyard and Defenses need a stricter authenticated module layout: compact title, optional active queue, and one compact catalog grid.

## Implementation steps

1. Document the module contract for the four core modules.
2. Record forbidden module UI copy and panels.
3. Keep this task documentation-only.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- src/VoidEmpires.Frontend/src/components/PlanetModuleLayout.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx

## Expected files to modify

- ai/current-state.md

## Acceptance criteria

- Construction, Research, Shipyard and Defenses contract records compact page title, active queue only when active, one single catalog container, and compact 4-card desktop grid.
- Categories may influence ordering but must not create separate large containers.
- Forbidden module UI is recorded: Continuar mundo, Crear cuenta inside authenticated module pages, Limpiar seleccion inside authenticated module pages, Infraestructura dashboard, Edificios actuales as a separate module block, Tecnologias completadas as a main block, Progreso cientifico as a dashboard wrapper, Contexto conservado, Vista normalizada, Construccion v1 / Investigacion v1 / Astillero v1 / Defensas v1, Revisar bloqueo, and block-review modal for normal blocked items.
- No behavior change.

## Constraints

- Follow repository conventions.
- Do not modify unrelated files.
- Keep the change minimal.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
