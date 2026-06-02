# Fleet Controlled Mutation Checklist

Use this checklist for the current non-visual regression pass around the Fleet page estimate -> confirm -> create-transfer flow.

Manual browser review is not required for this block unless a clear frontend regression appears.

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

## Guard checks

1. Try to reuse an estimate after changing the selected group or destination and confirm the frontend rejects it as stale.
2. Refresh fleet UI state and confirm the previous estimate is no longer accepted for create-transfer.
3. While a create-transfer request is already in flight, confirm the frontend blocks duplicate submissions.
4. When the API returns `409`, treat it as stale or already-active fleet state and recalculate the estimate before retrying.

## Recovery notes

1. `minimal-validation` is additive and idempotent. It restores missing baseline rows only.
2. Re-applying it does not delete extra transfers, reset reserved groups, or refund spent resources after create-transfer.
3. If you need the original baseline exactly, switch to a fresh disposable local database and then apply the seed again.
