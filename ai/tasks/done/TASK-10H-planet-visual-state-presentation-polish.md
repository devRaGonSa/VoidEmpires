# Phase 10H - Planet visual state presentation polish

## Summary

Expose seeded PlanetVisualState data in a more readable development presentation without introducing 3D rendering or gameplay changes.

## Goal

Add or improve a development-only frontend presentation for PlanetVisualState so seeded planet visual metadata is readable and inspectable instead of raw JSON-only.

## Scope

- Frontend-focused.
- Add or improve a development-only visual-state panel where appropriate:
  - it can be integrated into Galaxia selected-planet details, or a compact PlanetVisualState section if already present.
- Display:
  - planet name;
  - planet type label;
  - `visualSeed`;
  - `surfaceProfile`;
  - `lightDistributionMode`;
  - `platformMode`;
  - `atmosphereProfile`;
  - `cloudProfile`;
  - `supportsNightLights`;
  - `supportsSurfacePlatforms`;
  - `supportsOrbitalMegastructureHints`;
  - intensities as percentages or bars:
    - `colonizationIntensity`
    - `urbanIntensity`
    - `industrialIntensity`
    - `terraformingIntensity`
    - `militaryIntensity`
    - `orbitalPresenceIntensity`
- For now, do not render actual 3D planets.
- Do not add Three.js/WebGL.
- Do not change the backend endpoint unless absolutely necessary.
- If the frontend does not currently fetch PlanetVisualState, add a safe development-only fetch triggered by selected planet or explicit button.
- Handle `404` and `503` gracefully.

## Files to Read First

- Strategic Map frontend page and selected-planet detail components
- any existing PlanetVisualState frontend code or API helpers
- `src/VoidEmpires.Frontend/src/api` types related to visual state
- helpers created by `TASK-10E`

## Expected Files to Modify

- frontend visual-state page, panel, or selected-planet detail components only
- shared frontend formatting helpers from `TASK-10E` if a small extension is needed

## Constraints

- Do not commit secrets.
- Do not hardcode database passwords, local private IPs, or real connection strings.
- Do not touch PostgreSQL.
- Do not apply EF migrations.
- Do not add gameplay mutations.
- Do not add WebSockets.
- Do not add Three.js/WebGL.
- Do not introduce production authentication.
- Keep dev/prototype boundaries clear.
- Keep changes incremental and reviewable.
- Preserve the current successful validation baseline.

## Validation

Run:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Manual Validation

- Selecting or inspecting `Aurelia` can show visual-state data.
- Values are readable and not raw JSON-only.
- No 3D rendering is introduced.
