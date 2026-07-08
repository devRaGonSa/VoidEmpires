# TASK-45A

---
id: TASK-45A
title: Core module visual cleanup contract
status: done
type: frontend
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Document the corrected contract for core module pages.

## Context
Core game module pages must present only player-facing queue and catalog content. Resources belong in the global top resource bar, not duplicated inside each module.

## Implementation steps

1. Document that Construccion, Investigacion, Astillero and Defensas show only a compact page title, active queue or compact empty queue, and catalog grid.
2. Document forbidden player-facing UI: Seleccion disponible, Mundo guardado, Uso local, Lectura segura, Cargar contexto cientifico, Cargar contexto defensivo, Entrada de vista, Abrir vista, Abrir defensas, Id de civilizacion, Id de planeta, raw GUIDs, dashboard defensivo, que entra aqui.
3. Make no behavior changes.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/core-module-visual-contract.md

## Expected files to modify

- docs/core-module-visual-contract.md

## Acceptance criteria

- The corrected module visual contract is documented.
- Resources are documented as global top bar content.
- Forbidden player-facing UI is explicitly listed.
- No runtime behavior changes are made.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- dotnet build --no-restore
- dotnet test --no-build

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
