# Planet Module Boundaries

This guide documents the current frontend boundary model for the planet-related cockpits.

## Route responsibilities

- `/galaxy`: read-only strategic overview and route selection.
- `/planet`: the planet dashboard and context hub.
- `/construction`: general civil, economic, and infrastructure construction only.
- `/research`: specialized research cockpit foundation with guarded enqueue and read-model state.
- `/ground-army`: specialized ground army placeholder.
- `/shipyard`: specialized shipyard cockpit foundation with guarded orbital enqueue, queue and stock reads, and a disabled complete-due placeholder.
- `/defenses`: specialized defenses cockpit foundation with readiness, structures, option state, and no combat execution.
- `/fleets`: fleet command cockpit and route context hub.

## Do Not Mix

- Do not put the full construction catalog in `/planet`.
- Do not put defense, army, shipyard, or research full catalogs in `/construction`.
- Do not add mutations to `/galaxy`.
- Do not enable unavailable specialized gameplay from placeholder or readiness-only cabinas.
- Do not treat `/shipyard` as a fleet movement, transfer, split, merge, or combat surface.
- Do not treat `/defenses` as a combat, interception, fleet-command, or shield-simulation surface.

## Current placeholder model

- `Cabina preparada` or `Proximamente` means the surface exists as a route but still acts as a placeholder.
- `Solo lectura` means the page can explain context and navigation but must not execute gameplay mutations.
- `Activa` is reserved for routes that can already perform their intended read or controlled action.
- `Investigacion v1` now sits between placeholder and full gameplay: it supports read-model browsing plus guarded development enqueue, but complete-due remains intentionally disabled in the cockpit.
- `Astillero v1` now also sits between placeholder and full gameplay: it supports read-model browsing plus guarded development enqueue, but Fleet handoff stays explanatory only and complete-due remains intentionally disabled in the cockpit.
- `Defensas v1` now sits between placeholder and full gameplay: it supports readiness, structure visibility, defensive option visibility, queue context, and handoff guidance, but combat and complete-due execution remain intentionally unavailable in the cockpit.

## Seeded QA URLs

- Planet dashboard: `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Construction cockpit: `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Research cockpit: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Ground army placeholder: `/ground-army?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Shipyard cockpit: `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Defenses cockpit: `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
