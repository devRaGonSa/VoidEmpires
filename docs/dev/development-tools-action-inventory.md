# Development Tools Action Inventory

This inventory audits the current Development-only and development-backed UI actions that can change local validation data.

It is not a product feature list. It records the safety contract for the current cockpit shell while final database consolidation, final assets, production authentication, combat, fleet movement hardening, market transactions, and alliance mutations remain deferred.

## Required UI Contract

- Development QA actions stay secondary or collapsed by default.
- Mutating actions must be visibly labelled as Development-only, Development QA, guarded gameplay confirmation, or a dedicated controlled mutation.
- `GameModal` is the standard confirmation surface for Planet Development tools plus Construction, Research, and Shipyard submit flows.
- Mutating submits must wait for an explicit user action; page load, navigation, diagnostics expansion, or card selection must not mutate backend state.
- After a successful mutation, the cockpit must re-read backend state before presenting queues, reserves, stock, or readiness as updated.
- Raw ids, payloads, endpoint names, and technical failures belong in diagnostics or docs, not the primary cockpit path.

## Current Inventory

| Surface | UI action | Backend effect | Placement | Confirmation | Refresh rule | Status |
|---|---|---|---|---|---|---|
| `/planet` Development tools | `Aplicar 15 min`, `Aplicar 30 min`, `Aplicar 1 h` | Applies scoped resource production materialization for the selected owned planet. | Secondary `DevelopmentToolsPanel`, collapsed by default. | `GameModal` with `Development-only` scope before submit. | Re-read Planet UI state before showing reserve deltas. | Accepted Development QA action. |
| `/planet` Development tools | `Actualizar colas vencidas` | Materializes only due Construction, Research, and Shipyard queues for the selected civilization and planet. | Secondary `DevelopmentToolsPanel`, collapsed by default. | `GameModal` with `Development-only` scope before submit. | Re-read Planet UI state before showing queue summaries. | Accepted Development QA action. |
| `/construction` and Planet construction route | `Enviar orden` | Enqueues one construction order for the selected owned planet. | Gameplay cockpit confirmation, not Development tools. | `GameModal` with guarded gameplay scope plus acknowledgement checkbox. | Re-read Planet UI state before final visible queue/resource feedback. | Accepted development-backed gameplay mutation. |
| `/research` | `Confirmar investigacion` | Enqueues one research order for the selected civilization and source planet. | Gameplay cockpit confirmation, not Development tools. | `GameModal` with guarded gameplay scope plus acknowledgement checkbox. | Re-read Research UI state before final visible queue/catalog feedback. | Accepted development-backed gameplay mutation. |
| `/shipyard` | `Confirmar` for available orbital production | Enqueues one orbital production order for the selected owned planet. | Gameplay cockpit confirmation, not Development tools. | `GameModal` with guarded gameplay scope plus acknowledgement checkbox. | Re-read Shipyard UI state before final visible queue/resource/stock feedback. | Accepted development-backed gameplay mutation. |
| `/fleets` | Travel estimate | Calculates route, cost, and readiness preview. | Main Fleet command panel. | No modal required because it is a read-only preview. | No authoritative state change is expected. | Accepted read-only command preview. |
| `/fleets` | Create transfer, cancel transfer, complete due transfer | Mutates Fleet transfer rows in the Development database. | Dedicated Fleet controlled-mutation panel, not Development tools. | In-panel explicit confirmation and checkbox per `fleet-controlled-mutation-checklist.md`; do not promote to generic QA tools. | Re-read Fleet UI state after success before treating transfer state as current. | Tracked by the dedicated Fleet checklist. |

## Scripted QA Helpers

| Helper | Backend effect | Safety contract | Status |
|---|---|---|---|
| `scripts/dev-qa-register-test-user.ps1` | Calls `POST /api/accounts/register` to create a local QA account plus its initial player world through the normal account bootstrap path. | Generates unique local email/name/password values by default, accepts `BaseUrl`, does not require committed credentials, does not apply seeds, and never prints a caller-supplied password. | Accepted local QA helper. |

## Explicit Non-Actions

- Market buying, selling, auctions, player trading, and route execution remain unavailable.
- Alliance creation, invitations, pacts, roles, treasury, and messaging remain unavailable.
- Espionage missions, sabotage, infiltration, and counter-espionage execution remain unavailable.
- Combat, interception resolution, fleet split/merge production use, and stock-to-fleet allocation remain outside the current product shell.
- Global complete-due endpoints that are not cockpit-scoped must not appear as normal cockpit actions.

## Validation Notes

- Static copy guard for this inventory task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- This audit did not perform browser, screenshot, or final visual QA.
- Use the cockpit-specific checklists for manual validation of the visible flows.
