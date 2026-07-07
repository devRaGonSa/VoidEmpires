# Cockpit Copy Guidelines

Use this note when writing or revising visible UI copy for `Galaxy`, `Inicio`, `Planet`, `Construction`, `Research`, `Shipyard`, `Fleets`, `Defenses`, and `Ground Army`.

## Block 43 UI Contract

- Public auth pages use account-entry language and must stand outside the authenticated game shell.
- Authenticated `Inicio` is the current planet overview: resources, queues, planet state, and direct gameplay routes.
- `Planeta` should not be a separate competing overview. Keep it only as a compatibility alias or redirect when needed.
- Construction, Research, Shipyard, Defense, and Ground Army pages are action catalogs, not module launch hubs.
- Gameplay copy should feel like a browser strategy game: direct state, costs, requirements, availability, queue pressure, and action outcomes.

## Layer Rules

- Gameplay copy explains state, intent, and next action in player terms.
- Limitation copy explains what the cockpit does not do without sounding like a backend error.
- Diagnostics keep technical details available, but collapsed or visually secondary.
- Docs may use implementation language when exact routes, statuses, or backend behavior matter.

## Preferred Vocabulary

Status terms:

- `Disponible`
- `Bloqueada`
- `En cola`
- `En curso`
- `Completada`
- `Cierre no disponible`

Action terms:

- `Preparar orden`
- `Revisar orden`
- `Confirmar`
- `Volver a Galaxia`
- `Construir`
- `Investigar`
- `Fabricar`
- `Entrenar`

Secondary labels:

- `Diagnostico secundario`
- `Detalle tecnico`
- `Fuera del alcance de esta cabina`

## Preferred Concept Mapping

- `readiness` -> `preparacion`
- `handoff` -> `pasar a` or `continuar en`
- `ownership` -> `control local` or `colonia propia`
- `affordability` -> `coste y reservas locales`
- `complete due` -> `cierre` or `cierre automatico` when the operation is disabled

## Terms To Avoid In Primary UI

Avoid these in heroes, first-viewport summaries, visible action cards, and navigation labels unless the copy is inside diagnostics:

- `cabina`
- `contexto guardado`
- `dar contexto`
- `cargar mando`
- `siguientes cabinas`
- `registrar comandante` inside authenticated gameplay pages
- generic `continuar` as a gameplay CTA
- `endpoint`
- `DTO`
- `payload`
- `affordance`
- `raw`
- `mutation`
- `build`
- `dev route`
- `partida local`
- `sesion local`
- `new local game`
- `Development-safe`
- `seed`
- `cockpit-validation`

Account entry should use account/world language such as `cuenta`, `comandante`, `mundo inicial`, or `centro de mando`. Inside authenticated gameplay, prefer planet/empire action language and avoid registration copy. Do not present Development seed profiles or local-session/new-game concepts as the normal product path.

## Allowed Limitation Patterns

- `Esta accion todavia no esta habilitada desde esta cabina.`
- `El cierre automatico permanece fuera de esta superficie.`
- `La resolucion de combate no forma parte de esta version.`
- `Esta cabina mantiene la lectura visible, pero la confirmacion sigue en el modulo propietario.`

## Writing Guidance

- Prefer one stable term for the same state across cockpits.
- Keep action labels short; put nuance in supporting text, not in the button.
- If a technical term must remain, pair it with player-facing meaning and move the raw detail to diagnostics.
- Treat `pendiente de clasificar` as a fallback of last resort, not as accepted primary copy.

## Related Notes

- Use `docs/dev/cross-cockpit-language-audit.md` to find current problem spots.
- Use `docs/dev/frontend-foundation-smoke-checklist.md` for the shared QA pass that verifies copy hierarchy.
