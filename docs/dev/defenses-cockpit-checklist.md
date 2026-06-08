# Defenses Cockpit Checklist

Defenses cockpit v1 is a development-only readiness cockpit.
It is no longer a placeholder, but it still must not behave like combat, interception, fleet command, or shield simulation.

Use `docs/dev/development-seed-profiles.md` for seed setup and `docs/dev/planet-module-boundaries.md` to keep `/defenses` separate from `/construction`, `/shipyard`, `/ground-army`, and `/fleets`.

## Seeded route

- Apply `cockpit-validation`.
- Open `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- Expected planet: `Aurelia`.

## What this cockpit is for

- show defense readiness on the selected owned planet
- show deployed defensive structures and defensive construction options
- explain affordability, missing resources, and safe handoff paths
- show defense queue context or clearly document when completion stays unavailable
- keep diagnostics secondary and collapsed

## What this cockpit must not do

- execute combat
- execute interception
- launch fleet movement
- resolve bombardment, invasion, or planetary damage
- expose 3D or WebGL rendering
- pretend complete-due or other unsafe mutations are supported when they are not

## Manual QA checks

1. Confirm the page loads through `/defenses`, not a placeholder shell.
2. Confirm the header context shows `Aurelia` and owned-planet readiness data.
3. Confirm the readiness summary is visible in the first viewport and reflects the richer `cockpit-validation` stockpile.
4. Confirm deployed defensive structure state is visible, including the seeded `DefenseGrid`.
5. Confirm defensive option cards are visible and explain readiness or construction handoff clearly.
6. Confirm at least one available or ready defense option is visible in the seeded baseline.
7. Confirm blocked-state language is clear when supported. The default baseline may not show a blocked comparison because the current defensive catalog exposes only one real structure type.
8. Confirm queue or completion messaging is truthful. If no defense queue is active, the page must say so clearly. If complete-due remains unavailable, the page must present that as a limitation, not as a hidden action.
9. Confirm handoff links or action copy toward `Construccion`, `Astillero`, `Flotas`, `Planeta`, and `Galaxia` are visible and preserve context.
10. Confirm diagnostics stay collapsed or clearly secondary after load.
11. Confirm no button from this cockpit launches combat, interception, or fleet movement.
12. Confirm the page remains free of 3D, WebGL, and tactical battle presentation.

## Expected seeded result

- `Aurelia` is the controlled planet.
- one visible `DefenseGrid` structure appears in the defensive inventory
- one deterministic defense option for `DefenseGrid` remains visible
- stockpile and missing-resource guidance are readable
- complete-due remains explicitly unavailable in this build
- defensive actions stay construction-scoped or handoff-only

## Scope boundary

- `DefenseGrid` is the only currently meaningful seeded defensive structure.
- `ResearchType.Shielding` and visual military-intensity tags remain metadata only.
- Fleet endpoints own movement, transfer, split, merge, and related mutation flows. Defenses must not call them.
- There is still no shield hit-point model, interception execution, bombardment logic, invasion-defense resolution, or defense automation.

## Current contract audit

- Real persisted mutation now:
  - none accepted directly from `Defenses`
- Read-only now:
  - `GET /api/dev/defenses/ui-state` aggregates owned-planet stockpile, `DefenseGrid` structure state, defense construction-option readiness, and defense queue visibility
  - the defense read model is derived from `GET /api/dev/planets/ui-state`, so it inherits construction-backed readiness rather than owning a separate defense queue service
- Future backend work required:
  - defense-specific enqueue, scoped completion, or automation routes
  - combat, interception, bombardment, or shield-resolution behavior

## Final statement

Defenses v1 is a truthful readiness cockpit.
It can show structures, options, affordability, queue context, diagnostics, and safe handoffs, but anything resembling combat or active defense execution remains out of scope.
