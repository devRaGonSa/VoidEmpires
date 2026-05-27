# VoidEmpires Roadmap

This roadmap defines the high-level delivery direction for the VoidEmpires project before production code exists.

The statuses below describe planning intent, not implementation completion.

| Phase | Status | Goal | Expected outcome |
|---|---|---|---|
| Phase 0 - Repository and AI Platform setup | in-progress | Replace inherited template state with VoidEmpires-specific repository guidance and workflow assets. | The repository context, roadmap, architecture index, and bootstrap planning documents reflect the actual project direction. |
| Phase 1 - Technical foundation | planned | Create the initial `.NET` solution and establish the first modular application structure. | A buildable solution exists with clear project boundaries, baseline validation, and the first composition roots. |
| Phase 2 - Core domain foundation | planned | Model the core game concepts that describe empires, sectors, fleets, resources, and progression rules. | The domain language and application contracts are explicit enough to support iterative gameplay implementation. |
| Phase 3 - Gameplay systems | planned | Implement the first playable strategic loops around expansion, economy, research, military posture, and conflict resolution. | The project has coherent server-side gameplay rules that represent the core VoidEmpires pillars. |
| Phase 4 - Player experience surfaces | planned | Introduce the first user-facing application surfaces for interacting with the game and operational data. | Players and administrators have initial interfaces or APIs to create, inspect, and advance game state safely. |
| Phase 5 - Persistence and operational workflows | planned | Add durable storage, background processing, and operational safeguards around game state changes. | The platform can persist progression, execute asynchronous work, and recover from common operational failures. |
| Phase 6 - Multiplayer and live-state coordination | planned | Evolve the platform from isolated gameplay flows toward concurrent and potentially live multi-player interactions. | The architecture supports authoritative state transitions, concurrency control, and session or world coordination. |
| Phase 7 - Content, balancing, and live operations | planned | Enable controlled game-content evolution, balancing support, admin tooling, and telemetry-informed iteration. | The team can adjust content and monitor game health without destabilizing the core platform. |
| Phase 8 - Quality, security, and release readiness | planned | Harden the product through testing, observability, security controls, and deployment preparation. | The project is operationally reviewable, testable, and ready for staged releases. |
| Phase 9 - Scalability and post-launch evolution | planned | Prepare the product for larger player populations, richer features, and long-term maintainability. | VoidEmpires can scale its services, data model, and operational practices without losing architectural clarity. |

## Roadmap Notes

- Earlier phases should produce durable architecture and workflow assets before later feature work expands the surface area.
- Gameplay and product assumptions should be refined incrementally through explicit follow-up tasks rather than silent drift.
- If future decisions materially change the target game shape, update this roadmap and `ai/repo-context.md` together.
