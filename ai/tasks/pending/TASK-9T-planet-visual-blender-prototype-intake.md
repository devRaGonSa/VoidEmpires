# TASK-9T

---
id: TASK-9T
title: Phase 9T - Planet visual Blender prototype intake
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Phase 9T - Planet visual Blender prototype intake"
priority: high
---

## Goal

Bring the local Blender planet generator prototype into the repository in a safe, non-runtime prototype location, with documentation and clear boundaries.

## Context

A local untracked Blender prototype exists at the repository root: `xuniverse_planet_generator_variety.py`.

It is a `bpy` script intended to validate planet visual variety before the project moves toward future `PlanetVisualState` data and frontend procedural rendering.

This task should preserve the prototype as a reference artifact only. It must not become part of the .NET or frontend runtime.

## Implementation steps

1. Move or copy `xuniverse_planet_generator_variety.py` into `prototypes/planet-visuals/blender/xuniverse_planet_generator_variety.py`.
2. Add `prototypes/planet-visuals/blender/README.md` explaining:
   - this is a Blender prototype using `bpy`
   - it is not connected to backend, frontend runtime, migrations, or build
   - how to use it in Blender: open the script, change `PLANET_PRESET`, run with `Alt+P`
   - the available preset families at a high level: lava, gas, continental, rocky, ringed
   - how it relates to future `PlanetVisualState` and frontend procedural rendering
3. Keep the prototype out of production build paths.
4. Do not import Python into .NET or frontend code.
5. Do not add generated binary assets.
6. Do not modify gameplay code.

## Files to read first

- `xuniverse_planet_generator_variety.py`
- `ai/architecture-index.md`
- `README.md`

## Expected files to modify

- `xuniverse_planet_generator_variety.py`
- `prototypes/planet-visuals/blender/xuniverse_planet_generator_variety.py`
- `prototypes/planet-visuals/blender/README.md`

## Acceptance criteria

- The Blender prototype is stored in the repository under a dedicated prototype folder.
- The README clearly states the prototype boundaries and Blender usage.
- The prototype is not wired into runtime code or build steps.
- No binary assets or gameplay changes are introduced.
- Validation commands pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

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
