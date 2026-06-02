# Planet Visual Blender Prototype

This folder contains a local Blender prototype script that uses `bpy` to explore planet visual variety.

## Boundaries

- This prototype is a reference artifact only.
- It is not connected to the backend, frontend runtime, EF migrations, build pipeline, or production asset loading.
- It must not be imported into `.NET`, frontend, or gameplay runtime code.
- Do not commit generated binary assets from Blender output in this folder.

## Usage In Blender

1. Open Blender and switch to the Scripting workspace.
2. Open `xuniverse_planet_generator_variety.py`.
3. Change the `PLANET_PRESET` value near the top of the script.
4. Run the script with `Alt+P`.

## Preset Families

- `lava`: volcanic and molten surface variants
- `gas`: striped and storm-focused gas giant variants
- `continental`: habitable or semi-habitable land and ocean variants
- `rocky`: barren, mineral, desert, or Mercury-like rocky variants
- `ringed`: planets with visible ring systems, including some gas and oceanic variants

## Relation To Future Runtime Work

This prototype exists to validate visual direction before the project formalizes backend-facing `PlanetVisualState` data and frontend procedural rendering. Future runtime implementations should translate the visual lessons from this script into supported data contracts and rendering systems rather than executing Blender Python inside the application stack.
