# Ground Army Cockpit Checklist

Ground Army v1 is a terrestrial readiness cockpit.
It must not expand into invasion, assault, raid, occupation, bombardment, fleet mutation, orbital transport, or combat resolution.

Use this checklist with `docs/dev/planet-module-boundaries.md`, `docs/dev/development-seed-profiles.md`, and `docs/dev/frontend-foundation-smoke-checklist.md`.

## Current cockpit

- Route: `/ground-army`
- Seeded QA URL: `/ground-army?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Backend read model: `GET /api/dev/ground-army/ui-state?civilizationId=...&planetId=...`
- Scope: readiness, structures, garrison visibility, guarded training preparation, queue history, and handoffs only

## Seeded QA baseline

Apply `cockpit-validation` before manual review.

Expected seeded `Aurelia` result:

- the selected planet is controlled by the requesting civilization
- stockpile and population context are visible
- one visible `Barracks` structure appears in the readiness inventory
- one deterministic `PatrolGroup` option is available
- blocked comparisons remain visible for higher-scope or missing-prerequisite options
- one completed planetary training history row is visible
- complete-due remains unavailable or clearly disabled

The seeded baseline must stay truthful:

- no combat state
- no invasion state
- no troop transfer state
- no fleet movement action
- no fake complete-due execution path

## Manual QA steps

1. Reapply `cockpit-validation` twice if the local database has already been used for QA.
2. Open the seeded Ground Army URL.
3. Confirm `Aurelia` context is visible in the first viewport.
4. Confirm resources, population or manpower context, and readiness summary render without raw payloads dominating the page.
5. Confirm structures and the garrison or readiness inventory are visible.
6. Confirm at least one available option and at least one blocked option are visible together.
7. Confirm blocked states keep readable Spanish guidance.
8. Confirm queue history is visible and readable when seeded.
9. Confirm complete-due stays disabled or clearly unsupported in this build.
10. Confirm handoffs to `Construccion`, `Defensas`, `Flotas`, `Planeta`, and `Galaxia` preserve `civilizationId` and `planetId`.
11. Confirm diagnostics stay collapsed or clearly secondary by default.
12. Confirm the page does not imply combat, invasion, 3D, or fleet movement support.

## Visual review

- The first viewport should read like a specialized readiness cockpit, not a placeholder or raw dev console.
- Context, summary, structures, catalog, queue, and handoffs should appear before deep diagnostics.
- Available actions should read as guarded preparation only.
- Blocked actions should stay visually secondary and should not imply hidden combat systems.
- Development-only messaging should stay explicit but compact.

## Boundaries

Ground Army may show:

- selected-planet context
- terrestrial military structures
- readiness and capacity summaries
- ground unit labels and current stock
- available and blocked training options
- queue history and truthful limitations
- explanatory handoffs to neighboring modules

Ground Army must not execute or imply:

- invasion
- assault
- raid
- occupation
- bombardment
- defense-vs-army combat
- fleet movement
- orbital logistics command
- galaxy mutation

## Controlled action note

The current cockpit can explain and confirm Development-only training readiness, but the generic planetary asset enqueue primitive remains the underlying mutation surface.
Ground Army still does not own a cockpit-specific complete-due endpoint, and it must not call the global processing route directly from the main page.
