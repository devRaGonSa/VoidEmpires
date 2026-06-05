# Cross-Cockpit Language Audit

Use this note as the source of truth for copy cleanup across `Galaxy`, `Planet`, `Construction`, `Research`, `Shipyard`, `Fleets`, `Defenses`, and `Ground Army`.

## Rules

- Primary gameplay copy should explain command intent, colony state, queues, routes, reserves, training, defense, or next action.
- Secondary limitation copy should explain the gameplay boundary first and the development boundary second.
- Raw errors, endpoint notes, payload wording, and contract detail belong in collapsed diagnostics.
- Exact routes, HTTP statuses, seed notes, and implementation vocabulary belong in docs, not in cockpit copy.

Preferred replacements:

- `preparacion` over `readiness`
- `detalle tecnico` or `diagnosticos` over `payload`, `DTO`, or `endpoint`
- `pasar a Construccion` or `continuar en Flotas` over `handoff`
- `coste y reservas locales` over `affordability`
- `control local` or `colonia propia` over `ownership`
- `no disponible por ahora` or `fuera del alcance de esta cabina` over repeated `No disponible en esta build`

## Findings

### Galaxy

- Keep visible: `Cabina estrategica de Galaxia`, `Superficie de solo lectura`, `Sin mutaciones jugables`.
- Demote or rewrite later: `payloads tecnicos`, endpoint-status notes in visible rules, `Contratos para renderer`, `Payload crudo del sistema`, `Payload crudo del planeta`.
- Reference: `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`.

### Research

- Keep visible: `Investigacion`, `Estado de investigacion`, catalog and queue language, guarded confirmation language.
- Demote or rewrite later: `DTOs crudos`, `Mutacion dev protegida`, `ruta dev segura`, `No hay tecnologias disponibles en esta build`.
- References: `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`, `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`.

### Shipyard

- Keep visible: `Astillero`, `Produccion orbital`, `Reservas locales`, fleet handoff copy.
- Demote or rewrite later: repeated `readiness` labels in visible panels, `El endpoint de desarrollo no esta expuesto en este entorno`, `Esta accion no esta disponible en esta build`, `Notas del endpoint`, `Dev only`.
- References: `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`, `src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts`.

### Defenses

- Keep visible: `Proteccion planetaria`, `Postura actual y siguiente paso`, construction handoff, non-combat limits.
- Demote or rewrite later: `readiness defensivo`, `limites reales de esta build`, `Scope real de affordability`, `affordance secundaria`, `ownership`, `Sin mutacion local`.
- References: `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`, `src/VoidEmpires.Frontend/src/utils/defensePresentation.ts`, `src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts`.

### Ground Army

- Keep visible: `Dashboard terrestre`, `Guarnicion`, `Entrenamiento`, `Ordenes y cierre seguro`.
- Demote or rewrite later: `Boundary Ground Army`, `Readiness only`, `Readiness catalog`, repeated `readiness` headings, `Ground Army` in visible navigation copy.
- References: `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`, `src/VoidEmpires.Frontend/src/utils/groundArmyPresentation.ts`, `src/VoidEmpires.Frontend/src/utils/groundArmyViewModel.ts`.

### Shared fallback labels

- Avoid in primary seeded flows: `pendiente de clasificar`, `Requisito pendiente de clasificar`, `Categoria pendiente de clasificar`, `Estado pendiente de clasificar`, `Accion ... pendiente de clasificar`.
- These are acceptable as safety nets, but they read like unfinished content and should surface only as secondary guidance or diagnostics.
- References: `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`, `src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts`, `src/VoidEmpires.Frontend/src/utils/groundArmyPresentation.ts`, `src/VoidEmpires.Frontend/src/utils/defensePresentation.ts`.

### Final Visual QA Audit Snapshot (Ranking/Alliance/Market)

- Ruta revisada: `/ranking` -> resultado: carga correcta, cabina visible, **bloqueante por copy visible** (`Void Seed Civilization | current`, `Delta`, texto técnico de referencia).
- Ruta revisada: `/alliance` -> resultado: carga correcta, cabina visible, **bloqueante por copia inglesa y fallback técnico visible** (`Alliance cockpit remains read-only in this phase`, `Lectura diplomatica pendiente de clasificar | 0000000`).
- Ruta revisada: `/market` -> resultado: carga correcta, cabina visible, **bloqueante por copy repetido** (`Recurso no clasificado`) en estado primario.
- Ruta revisada: `/espionage` -> resultado: aceptable en capturas revisadas.
- Ruta revisada: `/galaxy` -> resultado: aceptable en capturas revisadas.
- Estados de bloqueo para cierre visual final:
  - Inglés visible en Ranking.
  - Inglés visible en Alliance.
  - Fallback con ID/fuerte técnico en UI primaria de Alliance.
  - Repetición de `Recurso no clasificado` en Market.
- Estado de frontera del bloque:
  - Correcciones de texto/fallback están pendientes de ejecutar en este bloque.
  - La aceptación visual final debe seguir siendo validada por el usuario con captura de pantalla.

## Follow-Up Priorities

1. Remove mixed English or architecture labels from first-viewport headings.
2. Replace repeated `No disponible en esta build` copy where the real limitation is already known.
3. Move endpoint or payload references out of visible action panels and into collapsed diagnostics.
4. Replace `pendiente de clasificar` fallbacks where seeded routes can guarantee a safer label.

## Non-Goals

- No gameplay changes.
- No backend changes.
- No broad rewrite in one pass.
- No deletion of safety detail; the goal is relocation and hierarchy.
