# Cockpit Copy Guidelines

Use this note when writing or revising visible UI copy for `Galaxy`, `Planet`, `Construction`, `Research`, `Shipyard`, `Fleets`, `Defenses`, and `Ground Army`.

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
- `Abrir cabina`
- `Volver a Galaxia`

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

Account entry should use account/world language such as `cuenta`, `comandante`, `mundo inicial`, or `centro de mando`. Do not present Development seed profiles or local-session/new-game concepts as the normal product path.

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
