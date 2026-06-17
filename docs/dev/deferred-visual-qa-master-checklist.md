# Deferred Visual QA Master Checklist

This checklist prepares a later browser pass for the current playable Development loop.
It does not claim that visual QA, screenshots, or manual browser verification have been performed.

Use it with:

- `docs/dev/single-product-demo-guide.md`
- `docs/dev/product-completion-audit.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/product-readiness-report.md`

## Deferred Status

- Status: pending human browser execution.
- Screenshots: pending capture.
- Visual acceptance: not yet claimed.
- Scope: `/onboarding`, `/galaxy`, `/planet`, `/construction`, `/research`, `/shipyard`, `/defenses`, `/ground-army`, `/fleets`, `/market`, `/espionage`, `/alliance`, and `/ranking`.
- Non-scope: production auth, real combat, fleet movement from Shipyard or Planet, exploration missions, WebGL/3D acceptance, and hidden auto-completion.

## Screenshot-Derived Decluttering Checks

These checks come from the user's observed overloaded Planet and Construction screens. This implementation block cleans up the documented issues, but full browser QA remains user-driven until a later manual pass captures and reviews screenshots.

- Global header: confirm it does not show disconnected mock resource bars or static empire values as if they were the selected backend context.
- Sidebar: confirm playable mutation-capable routes, read-only routes, readiness routes, Development QA helpers, and future/disabled work are grouped or labeled distinctly.
- Mutating pages: confirm Construction, Research, Shipyard, guarded Fleet transfer actions, and Development materialization controls do not use obsolete primary `solo lectura` copy.
- Planet hub: confirm primary actions and handoffs are visible before diagnostics, raw ids, endpoint metadata, or deep technical notes.
- Construction catalog: confirm available and blocked construction options appear without excessive first-viewport scrolling, duplicate planet-summary noise, or debug-first panels.
- Development tools: confirm QA-only resource/materialization actions are collapsed, secondary, or visually separated from normal gameplay actions.
- Development actions: confirm any action that mutates Development data opens an explicit review or confirmation modal/flow before the backend request.
- Diagnostics: confirm diagnostics remain collapsed by default or clearly secondary, and never dominate the primary workflow.
- Resource labels: confirm visible resource terms stay coherent as `Creditos`, `Metal`, `Cristal`, `Gas`, with `Energia`, `Deuterio`, and `Poblacion` treated as distinct context terms when shown.
- Readiness pages: confirm Defensas, Flotas, Mercado, Alianzas, Ranking, Espionaje, and Ground Army keep honest read-only/readiness scope without primary-looking unavailable actions.

## Ordered Browser Plan

1. Start the backend:
   - `dotnet run --project .\src\VoidEmpires.Web`
2. In a separate terminal, print the playable loop guide:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-playable-loop-guide.ps1`
   - Confirm the guide only prints the sequence by default and does not enqueue or materialize anything.
3. Start the frontend:
   - `npm run dev --prefix src\VoidEmpires.Frontend`
4. Open `/onboarding`.
5. Create a fresh Development playable session through the UI.
6. Verify the success state exposes the returned Planet, Construction, Research, and Shipyard links without presenting this as production login.
7. Open the returned Planet link.
8. Verify the local playable-session banner or continuation card is available when ids are missing on a cockpit route.
9. Verify the Planet hub first:
   - colony identity, ownership, resources, production, buildings, construction queue, and handoff links are visible
   - diagnostics remain secondary
   - no queue materialization runs on page load
10. Run explicit resource materialization only from the visible Development QA affordance or helper, then re-read Planet.
11. Verify resource feedback distinguishes changed balances from a no-op materialization.
12. Open Construction from the Planet hub.
13. Enqueue one available construction action only through the guarded confirmation flow.
14. Verify the result comes from a backend refresh:
   - queue is updated from the read model
   - resources are reduced by the visible cost
   - accepted-but-not-visible state does not fabricate a queue row
15. Open Research with the same ids.
16. Enqueue one available research item only through the guarded confirmation flow.
17. Verify the refreshed Research state:
   - active queue row is visible when backend returns it
   - blocked research remains non-mutating
   - complete-due remains unavailable from the normal cockpit
18. Open Shipyard with the same ids.
19. Enqueue one available orbital production item only through the guarded confirmation flow.
20. Verify the refreshed Shipyard state:
   - queue and resource deltas come from backend state
   - local orbital stock does not change until due-queue materialization runs
   - Shipyard still does not move or command fleets
21. Materialize due queues explicitly with the printed scoped helper command, after orders are due:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-materialize-due-queues.ps1 -CivilizationId {created-civilization-id} -PlanetId {created-planet-id} -ElapsedSeconds 3600`
22. Re-open or refresh Planet, Construction, Research, and Shipyard.
23. Verify completed construction, research progress, and orbital stock are visible only after backend-confirmed materialization and follow-up reads.
24. Run read-only diagnostics:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-get-playable-session-diagnostics.ps1 -CivilizationId {created-civilization-id} -PlanetId {created-planet-id}`
25. Open Defenses with the same ids.
26. Verify Defenses stays read-only or construction-handoff scoped:
   - no defense-specific combat, interception, or instant completion action appears executable
   - blocked and unavailable options remain visually secondary
27. Open Fleets with the same ids.
28. Verify Fleets read-state remains honest:
   - visible groups, transfers, estimates, and resource contexts are readable
   - no movement, cancellation, complete-due, split, merge, or stock promotion runs without explicit guarded Fleet actions
29. Open Galaxy with the same civilization and any known planet/system context.
30. Verify Galaxy remains read-only and map-first, with selected-system and planet detail adjacent to the map.
31. Open Ground Army, Market, Espionage, Alliance, and Ranking.
32. Verify those routes keep their accepted boundaries:
   - Ground Army is readiness only and does not execute invasion or combat
   - Market is advisory and does not buy, sell, auction, trade, or execute routes
   - Espionage is intelligence-readiness only and does not run missions
   - Alliance is diplomacy-readiness only and does not create alliances, pacts, invitations, roles, treasury changes, or messages
   - Ranking is a power-index read and does not claim public ladder, reward, matchmaking, or profile readiness
33. Return to Planet and verify the hub still preserves the same `civilizationId` and `planetId`.
34. Confirm no route turns combat, movement, exploration missions, or materialization into normal always-on gameplay copy.

## Screenshot Naming Convention

Use stable names so captures can be compared across passes without relying on local machine paths:

- `01-onboarding-entry.png`
- `02-onboarding-success.png`
- `03-planet-hub-initial.png`
- continue with two-digit route order plus short state, for example `08-research-post-enqueue.png`
- use `desktop` or `mobile` suffix only when the same state is captured at multiple viewport sizes

Do not commit screenshots in this task. Capture files belong to the later human/browser QA artifact location selected for that pass.

## Screenshot Capture List

Capture these screenshots during the later browser pass. Each capture should show the primary workflow first and keep diagnostics collapsed or visibly secondary unless the row says otherwise.

- `/onboarding` before submit.
- `/onboarding` success state with returned cockpit links.
- `/galaxy` map-first read-only view with selected system and planet context.
- `/planet` initial loaded hub for the created playable session.
- `/planet` local-session continuation state with ids absent.
- `/planet` resource materialization feedback, including a no-op case if no due work changed.
- `/construction` available action review before confirmation.
- `/construction` post-enqueue refreshed queue and resource state.
- `/research` available research review before confirmation.
- `/research` post-enqueue queue and blocked-card comparison.
- `/shipyard` available production review before confirmation.
- `/shipyard` post-enqueue queue, resource delta, and unchanged stock state.
- `/planet` after due-queue materialization.
- `/research` after due-queue materialization.
- `/shipyard` after due-queue materialization with updated orbital stock when produced.
- `/defenses` read-only readiness and handoff state.
- `/ground-army` readiness state with available or blocked training comparison.
- `/fleets` read-state with same planet context.
- `/fleets` travel estimate result, if the dedicated Fleet checklist is included in the pass.
- `/fleets` guarded transfer confirmation and refreshed state, only if the dedicated Fleet controlled-mutation pass is intentionally executed.
- `/market` advisory economy state with disabled transaction actions.
- `/espionage` intelligence coverage and disabled mission actions.
- `/alliance` diplomacy identity/contact state with disabled diplomacy actions.
- `/ranking` power-index category and comparison state.
- A diagnostics panel screenshot showing raw details secondary, not dominant.

## Failure Conditions

Treat the later pass as failed if any of these appear:

- a cockpit opens to a blank shell or never leaves a generic loading state
- a route drops the active `civilizationId` or `planetId` unexpectedly
- backend errors are hidden or replaced with fake success
- a page fabricates resources, buildings, research, queues, stock, or fleet groups before backend confirmation
- materialization runs from page load, route entry, sidebar navigation, or ordinary card selection
- Defenses presents real combat/interception execution
- Ground Army presents invasion or combat execution
- Market presents buying, selling, auctions, player trading, or route execution
- Espionage presents mission, sabotage, infiltration, or counter-espionage execution
- Alliance presents creation, invitation, pact, role, treasury, or messaging execution
- Ranking presents public ladder, reward, matchmaking, or profile readiness as implemented
- Shipyard or Planet presents fleet movement as executable
- Fleets presents movement, cancel, complete-due, split, merge, or stock promotion without explicit guarded user action
- diagnostics dominate the primary viewport instead of staying secondary

## Validation Before Running The Future Browser Pass

Run these from the repository root before a human starts the deferred browser pass:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```
