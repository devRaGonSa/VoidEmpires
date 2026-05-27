# Initial Galaxy Domain Model

VoidEmpires needs a deterministic galaxy structure that can support long-term progression, colonisation, fleet travel, visibility, wars, alliances, and a future 3D solar-system map. This document defines the first durable model without introducing player ownership, economy, fleets, combat, research, construction, or alliances.

## Core Concepts

### Galaxy

A `Galaxy` represents a large playable region or universe shard. It has a stable identifier, a display name, and many solar systems. Future scaling can split users, background work, or map queries by galaxy.

### SolarSystem

A `SolarSystem` belongs to one galaxy and has deterministic integer coordinates. It contains exactly one star and zero or more planets in the initial model. The system is future-ready for known/unknown visibility, fleet movement, scanning, and 3D rendering.

### Star

A `Star` belongs to one solar system. It has a name and a simple `StarType`. Star attributes are intentionally small at this stage; richer stellar effects can be added later.

### Planet

A `Planet` belongs to one solar system. It has an orbital slot, name, type, size, and colonisation status. Planets can exist without player ownership. Economy, buildings, troops, defences, population, and owner/civilisation links are intentionally excluded from this foundation.

### Coordinates

`GalaxyCoordinates` uses integer `X`, `Y`, and `Z` values. Persistence-critical identity must not depend on floating point values. A solar system is uniquely addressed by `GalaxyId + X + Y + Z`.

## Initial Invariants

- Galaxy names must not be empty.
- Solar system names must not be empty.
- Star names must not be empty.
- Planet names must not be empty.
- Planet size must be positive.
- Planet orbital slot must be positive.
- Solar system coordinates are value-based and deterministic.
- Solar system coordinates must be unique inside a galaxy at persistence level.
- Planet orbital slots must be unique inside a solar system at persistence level.
- Each solar system should have exactly one star in the initial model.
- Planets can exist in an uncolonized state.

## Future Considerations

- Known system visibility and fog-of-war.
- 3D system-map rendering.
- Fleet travel and mission arrival resolution.
- Colonisation and starting planet assignment.
- Planet ownership by player/civilisation.
- System generation rules.
- Multi-galaxy or universe-shard balancing.
- Strategic actions such as raid, occupy, devastate, or annex-like outcomes.
