# Fleet Controlled Mutation Checklist

Use this checklist for the current non-visual regression pass around the Fleet page estimate -> confirm -> create-transfer -> confirm -> cancel-transfer or complete-due flow.

Use this checklist as the non-visual mutation baseline for Fleet cockpit v1.

For final manual QA, pair it with the Fleet cockpit v1 visual review section in `docs/dev/frontend-foundation-smoke-checklist.md`.

## Required validation

1. Run `dotnet build --no-restore` from the repository root.
2. Run `dotnet test --no-build` from the repository root.
3. Run `npm run build --prefix src/VoidEmpires.Frontend`.

## Local API preparation

1. Start `VoidEmpires.Web` with development endpoints enabled and a disposable local database configured.
2. Apply `POST /api/dev/seeds/apply` with `{"profile":"minimal-validation"}`.
3. Read `GET /api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001`.
4. Copy a currently stationed scout-group id that can still request travel estimates.

## Regression sequence

1. Submit `POST /api/dev/fleets/orbital-travel/estimate` for the stationed group and destination planet `40000000-0000-0000-0000-000000000002`.
2. Confirm the estimate remains read-only and returns route, duration, cost, affordability, and fuel-readiness metadata.
3. Submit `POST /api/dev/fleets/orbital-transfers/create` only with the latest matching estimate and an explicit UTC `requestedAtUtc`.
4. Confirm the result identifies the created transfer and group, and that success clearly indicates development state mutated.
5. Read `GET /api/dev/fleets/ui-state` again and confirm the refreshed state reflects the planned transfer or reserved group change.
6. If validating the cancel path, submit `POST /api/dev/fleets/orbital-transfers/cancel` only for the currently visible active transfer and only after explicit confirmation.
7. Confirm the cancel result identifies the cancelled transfer or group, clearly reports `estado actualizado`, and keeps the no-refund rule visible.
8. Read `GET /api/dev/fleets/ui-state` again and confirm the cancelled transfer no longer appears active and the group is back in a stationed or available state.
9. If validating the complete-due path instead, use a visible due transfer from `ui-state` or a seeded due-transfer scenario before submitting `POST /api/dev/fleets/orbital-transfers/complete-due` with an explicit UTC `nowUtc`.
10. Confirm the complete-due result reports completed transfer and group ids, clearly reports `estado actualizado`, and removes the resolved due transfer from the active-transfer views after refresh.

## Visual QA focus

For the final manual browser pass, confirm:

1. The main Fleet action column stays readable in this order: escuadra, destino, estimacion, confirmacion, resultado.
2. `create transfer`, `cancel transfer`, and `complete due` are the only executable mutation actions and all remain behind explicit confirmation.
3. `split` and `merge` stay visible only as prototype-only or disabled controls.
4. Friendly ship or planet labels stay visually primary while compact ids remain secondary development metadata.
5. The order panel reads as a simple gameplay flow: escuadra, destino, calcular, revisar, confirmar.
6. The active-transfer panel stays readable with route, status, timestamps, progress, due-state cues, and only the currently available controlled actions.
7. Success, warning, and network failure feedback stay readable without raw JSON, dominant enum numbers, or `NetworkError` wording taking over the panel.
8. Empty states explain unloaded data, no active transfers, no cancellable transfers, and no due transfers without falling back to raw API wording.

## Guard checks

1. Try to reuse an estimate after changing the selected group or destination and confirm the frontend rejects it as stale.
2. Refresh fleet UI state and confirm the previous estimate is no longer accepted for create-transfer.
3. While a create-transfer request is already in flight, confirm the frontend blocks duplicate submissions.
4. When the API returns `409`, treat it as stale or already-active fleet state and recalculate the estimate before retrying.
5. After a successful cancel refresh, confirm the frontend clears the prepared cancel context and does not let the same transfer be re-submitted automatically.
6. After a successful complete-due refresh, confirm the frontend clears the prepared due context and does not leave the resolved transfer visible as still actionable.
7. When cancel or complete-due returns `404`, `409`, or a network failure, confirm the frontend leaves the previous fleet UI state intact until an explicit refresh.

## Recovery notes

1. `minimal-validation` is additive and idempotent. It restores missing baseline rows only.
2. Re-applying it does not delete extra transfers, reset reserved groups, or refund spent resources after create-transfer.
3. Re-applying it also does not remove already-cancelled transfer rows or reconstruct spent travel resources after a cancel-transfer cycle.
4. If you need the original baseline exactly, switch to a fresh disposable local database and then apply the seed again.

## Technical validation baseline

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```
