# Single Product Demo Guide

This is the canonical local demo path for the current VoidEmpires product shell. It covers backend startup, frontend startup, Development playable-session creation, guarded enqueue flows, due-queue materialization, diagnostics, and expected visible states.

This guide is Development-only. It does not claim production auth, final database/model consolidation, final generated assets, combat, final fleet movement, market transactions, alliance mutations, browser visual QA, or screenshot acceptance.

## Preconditions

- Use a local or disposable Development database.
- Run commands from the repository root.
- Keep `ASPNETCORE_ENVIRONMENT=Development`.
- Keep real connection strings, passwords, API keys, and private hostnames outside source control.
- Use `cockpit-validation` or the helper-generated playable session instead of manual SQL.

## Fast Command Guide

The script below prints the current ordered command sequence without running hidden mutations:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-playable-loop-guide.ps1
```

Use the rest of this document when you need the complete product-demo pass with expected states.

## 1. Start The Backend

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=VoidEmpireDB_Dev;Username=postgres;Password=YOUR_LOCAL_PASSWORD"
dotnet run --project .\src\VoidEmpires.Web
```

Expected state:

- `/health` responds.
- Development endpoints are available.
- No seed or gameplay mutation runs automatically on startup.

## 2. Prepare A Playable Session

Run this in a second terminal:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-playable-session-state.ps1 -ElapsedSeconds "3600" -PrintQueueMaterializationCommand
```

Expected state:

- The helper creates a real Development playable start through the backend.
- It prints `CivilizationId` and `HomePlanetId`; use `HomePlanetId` as `planetId` in frontend routes.
- If elapsed time is supplied, resource accrual is materialized by the backend and then re-read.
- The printed materialization command is only guidance for later manual QA; it is not executed by this helper.

Set local variables from the printed ids when running later helpers:

```powershell
$civilizationId = "PASTE_PRINTED_CIVILIZATION_ID"
$planetId = "PASTE_PRINTED_HOME_PLANET_ID"
```

## 3. Start The Frontend

```powershell
$env:VITE_VOIDEMPIRES_API_BASE_URL = "http://localhost:5142"
npm run dev --prefix src/VoidEmpires.Frontend
```

Expected state:

- The Vite dev server starts locally.
- The shell loads cockpit routes through lazy route imports.
- Local playable memory may help rebuild URLs, but every cockpit still re-reads backend state from visible ids.

## 4. Open The Core Demo Routes

Open these with the ids from step 2:

```text
http://localhost:5173/planet?civilizationId=$civilizationId&planetId=$planetId
http://localhost:5173/construction?civilizationId=$civilizationId&planetId=$planetId
http://localhost:5173/research?civilizationId=$civilizationId&planetId=$planetId
http://localhost:5173/shipyard?civilizationId=$civilizationId&planetId=$planetId
http://localhost:5173/fleets?civilizationId=$civilizationId&planetId=$planetId
http://localhost:5173/galaxy?civilizationId=$civilizationId&planetId=$planetId
```

Expected state:

- Planet is the hub for selected colony context, resources, buildings, queue status, handoffs, and secondary Development QA tools.
- Construction, Research, and Shipyard are focused guarded enqueue surfaces.
- Fleets can inspect relevant state and keeps its controlled Development transfer boundary separate.
- Galaxy remains read-only.
- Diagnostics stay secondary and raw ids do not dominate the first viewport.

## 5. Enqueue One Guarded Action

Choose one of these paths for the demo. Use the frontend path when validating the user flow; use the backend helper path when collecting terminal evidence.

| Surface | Frontend path | Backend helper | Expected success |
|---|---|---|---|
| Construction | Select an available construction action, review it, check the acknowledgement, then submit. | `.\scripts\dev-qa-create-construction-order.ps1 -CivilizationId $civilizationId -PlanetId $planetId` | A real order id is returned, local resources decrease immediately, and the refreshed queue reflects backend state. |
| Research | Select an available research card, review it, check the acknowledgement, then submit. | `.\scripts\dev-qa-create-research-order.ps1 -CivilizationId $civilizationId -PlanetId $planetId` | A real order id is returned, resources decrease immediately, and the refreshed Research state shows the queue or an honest reused-database blocker. |
| Shipyard | Select an available orbital production item, review it, check the acknowledgement, then submit. | `.\scripts\dev-qa-create-shipyard-production-order.ps1 -CivilizationId $civilizationId -PlanetId $planetId` | A real order id is returned, resources decrease immediately, and local orbital stock stays unchanged until due materialization. |

Expected controlled no-op states:

- An occupied Construction queue means no second construction order should be created for that planet.
- An occupied Research queue means no second research order should be created for that civilization.
- An occupied Shipyard production queue means no second orbital production order should be created for that planet.
- These are real persisted Development states, not reset failures.

## 6. Materialize Due Queues Deliberately

Only run this after you have an order that should be due for the selected context:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-materialize-due-queues.ps1 -CivilizationId $civilizationId -PlanetId $planetId -ElapsedSeconds "3600"
```

Expected state:

- The helper warns that it mutates the Development database.
- It processes only due scoped Construction, Research, and Shipyard orders.
- Not-yet-due orders remain open.
- Resources are not charged a second time during materialization.
- After it runs, refresh Planet, Construction, Research, and Shipyard before trusting visible queue, building, research, or stock state.

## 7. Read Diagnostics

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-get-playable-session-diagnostics.ps1 -CivilizationId $civilizationId -PlanetId $planetId
```

Expected state:

- Diagnostics read resources, Construction, Research, Shipyard, orbital stock, readiness notes, warnings, and limitations.
- Diagnostics do not apply seeds, accrue resources, enqueue orders, materialize queues, move fleets, or change ownership.
- Use `-RawJson` only when technical payload inspection is needed.

## Product Shell Boundaries

- Market remains read-only: no buying, selling, auctions, player trading, or route execution.
- Alliance remains read-only: no creation, invitation, pact, role, treasury, or messaging flow.
- Espionage remains read-only: no missions, sabotage, infiltration, or counter-espionage execution.
- Defenses and Ground Army remain readiness surfaces: no combat, invasion, or defense execution.
- Shipyard does not move fleets or allocate stock into fleet groups in this demo.
- Production authentication and active civilization resolution remain deferred; route context is still explicit ids plus local navigation memory.

## Validation For This Guide

Static validation for this documentation task:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```

Related source documents:

- `docs/dev/product-completion-audit.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/development-tools-action-inventory.md`
- `docs/dev/deferred-visual-qa-master-checklist.md`
