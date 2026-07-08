# Core Module Visual Contract

Block 45 defines the player-facing contract for the core module pages:

- Construccion
- Investigacion
- Astillero
- Defensas

Each page must stay focused on the OGame-style module loop. The normal player UI shows only:

1. a compact page title;
2. the active queue, or a compact empty queue state;
3. the module catalog grid.

Resources belong in the authenticated game shell top resource bar. Module cards may show costs, durations, requirements, current level or quantity, and available actions, but each module must not duplicate a separate page-level resources panel.

## Forbidden Player-Facing UI

The normal player UI for these modules must not expose internal orchestration, manual context, or duplicated explanation panels. The following copy and controls are forbidden in normal module UI:

- Seleccion disponible
- Mundo guardado
- Uso local
- Lectura segura
- Cargar contexto cientifico
- Cargar contexto defensivo
- Entrada de vista
- Abrir vista
- Abrir defensas
- Id de civilizacion
- Id de planeta
- raw GUIDs
- dashboard defensivo
- que entra aqui

Operator-only or development-only diagnostics may keep technical context only when they are hidden from normal player UI and do not reintroduce manual context loading into the four core modules.
