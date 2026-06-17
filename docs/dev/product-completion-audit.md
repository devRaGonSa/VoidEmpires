# Product Completion Audit

This audit captures the near-final product shell before the final database/model consolidation, generated asset pass, production authentication hardening, combat, fleet movement productization, market transactions, and alliance mutations.

It is documentation-only. It does not claim browser visual QA, screenshots, production readiness, or final gameplay completeness.

## Current Playable Surface

| Route | Current role | Mutation boundary | Near-final status |
|---|---|---|---|
| `/onboarding` | Development-only playable start that creates a local test civilization and routes to Planet with explicit ids. | `POST /api/dev/players/starting-civilization`; not production auth. | Usable for local demo entry, not login. |
| `/galaxy` and `/` | Strategic map and system/planet read surface. | Read-only. | Accepted map cockpit baseline. |
| `/planet` | Main colony hub with resources, buildings, queue context, handoffs, and Development QA tools. | Construction enqueue via guarded confirmation; resource and due-queue materialization only from secondary Development tools. | Playable hub baseline. |
| `/construction` | Focused construction catalog and queue route using the Planet implementation variant. | Guarded construction enqueue only; backend refresh required after submit. | First controlled persisted mutation cockpit. |
| `/research` | Research catalog, queue, completed projects, and readiness route. | Guarded research enqueue only; no automatic completion. | Second controlled persisted mutation cockpit. |
| `/shipyard` | Orbital production catalog, queue, stock, and Fleet handoff route. | Guarded orbital production enqueue only; due processing and stock-to-fleet allocation remain deferred. | Controlled orbital production baseline. |
| `/fleets` | Fleet inspection, route estimate, transfer readiness, and controlled Development transfer panels. | Development-only transfer create/cancel/complete-due is tracked by the Fleet checklist; final movement productization remains deferred. | Useful Development cockpit, not final movement gameplay. |
| `/defenses` | Defense readiness, structure context, queue visibility, and handoffs. | No defense-specific mutation. | Readiness cockpit baseline. |
| `/ground-army` | Ground readiness, structures, training comparisons, queue history, and handoffs. | No combat, invasion, or direct training mutation. | Readiness cockpit baseline. |
| `/market` | Economy read surface with reserves, production, advisory ratios, signals, and handoffs. | No buying, selling, auctions, trading, or route execution. | Read-only economy baseline. |
| `/espionage` | Intelligence read surface with target coverage and passive signals. | No missions, sabotage, infiltration, or counter-espionage execution. | Read-only intelligence baseline. |
| `/alliance` | Diplomacy read surface with identity, contacts, and future pact/action placeholders. | No alliance creation, invitations, pact execution, roles, treasury, or messaging. | Read-only diplomacy baseline. |
| `/ranking` | Power-index read surface with category scores and demo comparisons. | No public leaderboard, matchmaking, rewards, or profiles. | Read-only ranking baseline. |

## Shared Product Contracts

- Routes stay lazy-loaded through `App.tsx`; cockpit pages must not return to eager imports.
- Query-string `civilizationId`, `planetId`, and `systemId` remain the current navigation context.
- Local playable-session storage is a convenience for rebuilding Development URLs, not authentication or authorization.
- `GameModal` is the shared confirmation surface for Construction, Research, Shipyard, and Planet Development tool mutations.
- `DevelopmentToolsPanel` and `DevDiagnosticsPanel` keep QA actions and raw technical details secondary.
- Visible state after a mutation must come from a backend read, not from optimistic local queue, stock, resource, ranking, or readiness fabrication.
- `cockpit-validation` is the current broad local demo seed. Richer route-specific seeds remain available for focused QA.

## Product Gaps Before Final DB/Assets Scope

1. Final database/model consolidation is still pending.
2. Final generated image and asset integration is still pending.
3. Production authentication and active civilization resolution are still pending.
4. Browser visual QA and screenshot acceptance are still user-driven and deferred.
5. Combat, invasion, interception resolution, and active defense execution are out of scope.
6. Fleet movement productization remains separate from the current Development transfer cockpit.
7. Market transactions and trade-route execution are out of scope.
8. Alliance and diplomacy mutations are out of scope.
9. Global complete-due endpoints must remain outside normal cockpit actions unless a future task makes them cockpit-scoped.

## Validation Baseline For This Audit

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- Full non-visual product shell validation remains the later closure bundle: frontend build, route lazy-import guard, copy guard, QA script guard, backend build, and tests.
- Manual browser validation remains tracked in `docs/dev/deferred-visual-qa-master-checklist.md`.
