# Fleets Cockpit Checklist

This checklist documents the current Fleet contract boundary that is safe to reference from the orbital production block.
Use `docs/dev/fleet-api-contracts.md` for the full development endpoint inventory and `docs/dev/shipyard-fleet-persisted-qa.md` for the accepted post-Shipyard backend-only runbook.

## Current contract audit

- Real persisted mutations already exist in the broader Fleet development surface for transfer creation, cancellation, completion, split, merge, and stock-to-group allocation.
- The accepted orbital-production block uses Fleet only as read-state verification after a Shipyard enqueue.
- Safe read routes for this block:
  - `GET /api/dev/fleets/ui-state?civilizationId={id}`
  - `GET /api/dev/fleets/overview?civilizationId={id}`
  - `GET /api/dev/fleets/action-manifest`

## Safe block boundary

- Safe now:
  - re-read stationed groups, active-transfer summaries, command-readiness flags, and current-planet resource contexts
  - confirm Shipyard resource spending appears in Fleet `resourceContexts[]`
- Keep read-only for this block:
  - all Fleet interactions initiated from Shipyard, Defenses, or Planet
- Requires future hardening before this block can treat it as safe:
  - `POST /api/dev/fleets/orbital-groups/create-from-stock`, because it consumes stock, creates a new group on every success, does not enforce source-planet ownership itself, and does not require `originPlanetId == currentPlanetId`

## Current read-model fields used by this block

- `groups[]`: stationed or active-transfer fleet state
- `resourceContexts[]`: current-planet `Credits`, `Metal`, `Crystal`, and `Gas` balances
- `actionHints[]`: metadata only, not an approval to execute the route from this block
- `interceptionNotes[]`: read-only explanatory notes only
