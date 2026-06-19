# Product Readiness Report

Status date: 2026-06-17

VoidEmpires currently has a coherent Development-only product shell suitable for local demo and QA. It is not production-ready: final database/model consolidation, final image and asset integration, production authentication, combat, final fleet movement, market transactions, alliance mutations, and screenshot-backed browser acceptance remain open.

## Readiness Summary

| Area | Current status | Readiness |
|---|---|---|
| Backend host and project structure | Web, Application, Domain, Infrastructure, and Tests projects exist with established boundaries. | Foundation ready. |
| Development seed baseline | `cockpit-validation` covers the accepted cockpit suite for local demo. | Demo ready. |
| Frontend route shell | Accepted cockpit routes are lazy-loaded and share the current shell/navigation model. | Demo ready. |
| Playable loop | `/onboarding`, `/planet`, `/construction`, `/research`, and `/shipyard` form the current controlled local loop. | Development-only ready. |
| Readiness/advisory routes | Galaxy, Defenses, Ground Army, Fleets, Market, Espionage, Alliance, and Ranking expose accepted read or readiness states. | Development/read-only ready. |
| Validation docs | Demo guide, product audit, deferred visual QA checklist, and cockpit checklists exist. | Documentation ready. |
| Visual/browser acceptance | Checklist exists, but screenshots and human browser pass are not complete. | Not yet accepted. |
| Production access | Registration and email confirmation exist, but cockpit context does not resolve from production auth. | Not product-ready. |

## Ready For Local Demo

- One canonical demo path exists in `docs/dev/single-product-demo-guide.md`.
- The final frontend information architecture is documented in `docs/dev/product-completion-audit.md`.
- The deferred browser screenshot pass is scripted in `docs/dev/deferred-visual-qa-master-checklist.md`.
- Development QA scripts can prepare a playable session, print scoped materialization commands, read diagnostics, and parser-check helper behavior.
- Construction, Research, and Shipyard create real Development database rows only after explicit confirmation and then rely on backend refreshes.
- Planet Development QA tools are secondary and explicitly scoped to Development data.
- Read-only/advisory routes keep their current boundaries and do not add unsupported gameplay execution.

## Development-Only Surface

These flows are usable for local QA and demo, but they are not production features:

| Surface | Development-only behavior |
|---|---|
| `/onboarding` | Creates a local playable start with explicit ids; it is not login. |
| Seed profiles | Additive deterministic setup through Development endpoints. |
| Queue materialization | Explicit scoped helper/action for due Development orders only. |
| Resource accrual | Explicit backend materialization, not an automatic page-load timer. |
| Construction/Research/Shipyard enqueue | Real persisted rows through Development-backed guarded flows. |
| Fleet transfer tools | Controlled Development mutation path tracked by the Fleet checklist, not final movement productization. |
| Diagnostics | Read backend state and raw payload context; they are support surfaces, not primary gameplay. |

## Final DB And Model Needs

- Consolidate final persisted gameplay models before treating the shell as durable product behavior.
- Replace Development seed assumptions with production-owned initialization and migration strategy.
- Decide final queue, resource, stock, fleet, research, market, alliance, and ranking ownership boundaries.
- Keep CI/test paths independent of the real PostgreSQL database unless a future integration-test task explicitly configures that boundary.

## Final DB Phase Prep Plan

This section is summarized separately in `docs/dev/final-db-phase-prep.md`, and the SQL Server-specific testing position for that phase is recorded in `docs/dev/sql-server-test-strategy.md`.

The current persistence-stack audit for that phase is now captured in `docs/dev/final-db-phase-prep.md`, including:

- current EF/Npgsql package and registration facts
- current DbContext and migration layout
- current Development seed and static catalog sources
- current PostgreSQL-specific assumptions in mappings and migrations
- current SQL Server cutover constraints and risks

Deployment posture for that phase:

- the final database target is an external user-managed SQL Server, not a bundled repository database container
- no checked-in `docker-compose` or similar repository deployment template currently provisions SQL Server
- deployment-specific connection strings should be injected through environment variables or platform secret storage, never through committed passwords
- when operators use self-hosted infrastructure such as a NAS or VM-hosted app container, SQL Server certificate settings must still be explicit in the external connection string, for example `Encrypt=True;TrustServerCertificate=True;`

The final DB/model phase should start only after the current Development-only shell boundaries are preserved and the module catalog notes remain the source of seed-shape input:

- `docs/dev/catalog-metadata-readiness.md`
- `docs/dev/building-catalog-final-db-readiness.md`
- `docs/dev/research-catalog-final-db-readiness.md`
- `docs/dev/resource-economy-final-db-readiness.md`
- `docs/dev/ship-catalog-final-db-readiness.md`
- `docs/dev/defense-catalog-final-db-readiness.md`

Recommended entry criteria:

1. Confirm which catalog metadata becomes durable relational seed data versus versioned content rows.
2. Confirm final ownership for resource stockpiles, production profiles, population profiles, construction queues, research queues, orbital production queues, orbital stock, fleet groups, transfers, market state, alliance state, ranking snapshots, and player-to-civilization resolution.
3. Confirm which current Development seed profiles stay QA-only and which deterministic rows become production initialization.
4. Confirm the migration boundary for catalog metadata separately from player-owned gameplay state.
5. Confirm that final generated assets remain nullable references until the asset phase lands.

Recommended implementation sequence for the later final DB task:

1. Add seed metadata contracts or tables for catalog rows without changing owned gameplay state rows.
2. Add seed validation that every catalog row maps to a known domain key and every cost-bearing resource maps to a persisted or explicitly non-spendable resource key.
3. Add migrations in a dedicated task and run them only against disposable/local validation databases, not automatically against a real shared database.
4. Add tests that keep ordinary CI independent of real PostgreSQL, with integration tests enabled only when repository-specific integration configuration exists.
5. Update read models to consume durable metadata only after validation proves frontend fallbacks and backend catalogs stay aligned.
6. Keep production auth, combat, fleet movement productization, market transactions, alliance mutations, and final asset acceptance outside the DB metadata change unless separate tasks explicitly accept those behaviors.

Required validation gates for that phase:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- catalog seed drift tests for building, research, resource, ship/orbital, and defense metadata
- migration generation/replay against a disposable local database when integration tests are configured
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

No integration tests are configured for this documentation-only prep update.

## Final Image And Asset Needs

- Final generated or curated assets are still pending.
- The placeholder-to-final replacement plan is documented in `docs/dev/product-placeholder-asset-contract.md`.
- The post-DB/post-asset correction loop is prepared in `docs/dev/deferred-visual-qa-master-checklist.md`.
- Current UI should not be treated as final art direction or screenshot acceptance.
- Future asset work must preserve readable cockpit information density and avoid hiding backend-owned state behind decorative media.

## Product-Blocking Dependencies

1. Production authentication and active civilization resolution.
2. Final authorization/ownership enforcement for cockpit context.
3. Final database/model consolidation and migration policy.
4. Final visual QA with captured screenshots across all major routes.
5. Combat, invasion, interception, and defense execution design.
6. Fleet movement productization beyond the current Development transfer tooling.
7. Market buying, selling, auctions, player trading, and trade-route execution.
8. Alliance creation, invitations, pacts, roles, treasury, messaging, and shared visibility behavior.
9. Final image/asset integration.

The remaining product risks are tracked in `docs/dev/final-risk-register.md`.

The accepted near-product shell criteria are tracked in `docs/dev/final-acceptance-criteria.md`.

The future-chat handoff summary is tracked in `docs/dev/final-orchestrator-summary.md`.

Block 37 closure is complete when `ai/tasks/pending` contains only `.gitkeep`, every `TASK-37*` task file is in `ai/tasks/done`, the latest closure commit is pushed on `main`, and the deferred phases above remain explicitly outside the accepted scope.

## Block 37 Closure

Recorded on 2026-06-17 for `TASK-37BZ`:

- `ai/tasks/pending` contains only `.gitkeep`.
- All `TASK-37*` task files are now expected to be in `ai/tasks/done` after this closure task is moved.
- The current accepted state remains a Development-only near-product shell for local demo and QA.
- The latest full validation baseline remains the `TASK-37BO` pass: backend build succeeded with `0` warnings and `0` errors, backend tests succeeded with `720` passing tests, frontend build succeeded with `105` transformed modules, and the development QA, lazy-route, and copy-regression guards succeeded.
- The repository cleanliness pass from `TASK-37BV` found no tracked build artifacts, screenshots, captures, archives, temp files, logs, or non-placeholder secret examples.
- This closure does not add or accept final database/model consolidation, final generated assets, browser screenshot acceptance, production authentication, combat, final fleet movement productization, market transactions, or alliance mutations.

## Validation Needs

Before using the shell for a local demo:

```powershell
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```

Before claiming broader non-visual readiness:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

Before claiming visual acceptance:

- Complete the browser pass in `docs/dev/deferred-visual-qa-master-checklist.md`.
- Capture every required screenshot.
- Record failures and corrections in follow-up tasks instead of treating documentation as proof of visual quality.

## Latest No-Gameplay Regression Validation

Recorded on 2026-06-17 for `TASK-37BM`:

- `dotnet build --no-restore` succeeded with `0` warnings and `0` errors.
- `dotnet test --no-build` succeeded with `720` passing tests, `0` failed, and `0` skipped.
- `npm run build --prefix src/VoidEmpires.Frontend` succeeded; Vite transformed `105` modules and kept the shared entry chunk at `181.33 kB` minified / `59.14 kB` gzip.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded, including the route lazy-import guard and copy regression guard.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded.

This was a non-visual validation pass. It did not perform browser screenshot acceptance, final database/model readiness, final asset readiness, combat readiness, final movement readiness, market transaction readiness, alliance mutation readiness, or production-auth readiness.

## Latest Full Validation Pass

Recorded on 2026-06-17 for `TASK-37BO`:

- `dotnet build --no-restore` succeeded with `0` warnings and `0` errors.
- `dotnet test --no-build` succeeded with `720` passing tests, `0` failed, and `0` skipped.
- `npm run build --prefix src/VoidEmpires.Frontend` succeeded; Vite transformed `105` modules, emitted cockpit lazy chunks, kept the shared entry chunk at `181.33 kB` minified / `59.14 kB` gzip, and did not emit the old `500 kB` chunk warning.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded, including the route lazy-import guard and copy regression guard.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded.
- Tooling note: the .NET commands reported available workload updates; this was informational and did not fail validation.

This was still a non-visual validation pass. It did not perform browser screenshot acceptance or grant production readiness, final database/model readiness, final asset readiness, combat readiness, final movement readiness, market transaction readiness, alliance mutation readiness, or production-auth readiness.

## Repository Cleanliness Check

Recorded on 2026-06-17 for `TASK-37BV`:

- Tracked-file scans found no committed `bin`, `obj`, `node_modules`, `dist`, screenshot, capture, image, video, archive, temp, or log artifacts.
- The local ignored workspace still contains expected ignored outputs such as `.vs/`, `ai/worker.lock`, `bin/`, `obj/`, `src/VoidEmpires.Frontend/dist/`, and `src/VoidEmpires.Frontend/node_modules/`; these are not tracked.
- Machine-local `D:/Proyectos/VoidEmpires` markdown links in dev checklists were converted to repository-relative links.
- An old completed task note containing a concrete private-style PostgreSQL connection string example was redacted to placeholder values.
- Follow-up secret scan hits were limited to placeholder examples such as `YOUR_LOCAL_PASSWORD` and `LOCAL_PASSWORD`.
- `dotnet build --no-restore` succeeded with `0` warnings and `0` errors.
- `dotnet test --no-build` succeeded with `720` passing tests, `0` failed, and `0` skipped.
- `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `105` transformed modules and the shared entry chunk at `181.33 kB` minified / `59.14 kB` gzip.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded, including the route lazy-import and copy-regression guards.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded.

## Current Readiness Decision

Decision: ready for Development-only local product-shell demo after the validation commands above pass.

Decision not granted: production readiness, final visual acceptance, final database/model readiness, final art/asset readiness, combat readiness, final movement readiness, market readiness, alliance readiness, or production-auth readiness.
