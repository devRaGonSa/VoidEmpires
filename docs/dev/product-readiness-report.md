# Product Readiness Report

Status date: 2026-07-07

VoidEmpires currently has a coherent Development-only product shell suitable for local demo and QA. Normal route navigation and primary cockpit surfaces now default to product-facing copy: development/test/prototype wording, backend URLs, endpoint details, localhost references, and raw technical diagnostics are not part of the normal first-render UI. It is not production-ready: final database/model consolidation, final image and asset integration, production auth hardening, combat, final fleet movement, market transactions, alliance mutations, and screenshot-backed browser acceptance remain open.

## Readiness Summary

| Area | Current status | Readiness |
|---|---|---|
| Backend host and project structure | Web, Application, Domain, Infrastructure, and Tests projects exist with established boundaries. | Foundation ready. |
| Development seed baseline | `cockpit-validation` covers the accepted cockpit suite for local QA/operator demo only. | Demo ready. |
| Frontend route shell | Accepted cockpit routes are lazy-loaded and share the current shell/navigation model. | Demo ready. |
| Product-facing copy | Normal UI hides development/test language and backend details by default; operator-only tooling remains explicit and secondary. | Product-surface ready for local demo. |
| Account playable loop | `/register`, `/login`, `/planet`, `/construction`, `/research`, and `/shipyard` now use the account entry path and guarded game routes. | Backend/frontend foundation ready; production authorization still deferred. |
| Readiness/advisory routes | Galaxy, Defenses, Ground Army, Fleets, Market, Espionage, Alliance, and Ranking expose accepted read or readiness states. | Development/read-only ready. |
| Validation docs | Demo guide, product audit, deferred visual QA checklist, and cockpit checklists exist. | Documentation ready. |
| Visual/browser acceptance | Checklist exists, but screenshots and human browser pass are not complete. | Not yet accepted. |
| Production access | Registration, login, current session, logout, and initial world bootstrap exist; final gameplay authorization and manual SQL Server verification remain deferred. | Not product-ready. |

## Block 42 Registration Contract Audit

Recorded on 2026-07-07 for `TASK-42A`:

- ASP.NET Core Identity is present behind `POST /api/auth/register` and `GET /api/auth/confirm-email`; it is wired only when persistence is configured and sends confirmation mail through the transactional email abstraction.
- Account registration now creates the Identity user, `PlayerProfile`, initial `Civilization`, home planet, `PlanetOwnership`, starting resources, and production profile through the account bootstrap path.
- `/register` is the primary product entry. `/onboarding` remains as an alias to registration.
- Development seed profiles are not part of real player entry. `cockpit-validation` remains an internal QA fixture for repeatable cockpit validation and must not be presented as normal onboarding.
- Login, `/api/accounts/me`, logout, HTTP-only Identity cookie configuration, guarded frontend routes, and registration-to-home-world navigation are implemented.
- The current registration contract is: register account, create/link player profile, create civilization, assign or generate home planet, create ownership, initialize economy/production state, login, then navigate to the generated home planet route.
- The current multiplayer coexistence contract is: independent registrations create distinct player profiles, civilizations, home planets, and active ownership rows, while using the same starting resource and production baseline.
- Local playable-session storage remains a navigation convenience only and is not authentication, authorization, ownership, role, token, cookie, or account state.
- This audit still does not claim final gameplay authorization, account recovery/confirmation product UX, production deployment hardening, browser QA, or manual SQL Server registration validation.

## Block 42 Final Automated Validation Gate

Recorded on 2026-07-07 for `TASK-42AJ`:

- `dotnet build --no-restore` succeeded with `0` warnings and `0` errors.
- `dotnet test --no-build` succeeded with `779` passing tests, `0` failed, and `0` skipped.
- `npm run build --prefix src/VoidEmpires.Frontend` succeeded; Vite transformed `114` modules and emitted a `193.87 kB` minified / `62.21 kB` gzip entry chunk.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded, including route lazy-import, copy-regression, repository secret-scan, SQL Server generated-script safety, and QA helper checks.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1` succeeded.
- `git status` was reviewed after validation.

This was a non-visual automated validation pass. It did not perform browser/manual QA, did not connect to SQL Server, did not apply migrations or generated SQL, did not apply seeds, did not add real SQL Server credentials, and did not grant production readiness.

## Block 43 Visual Review Contract

Recorded on 2026-07-07 for `TASK-43A`:

- The current UI must move away from an internal orchestrator/productivity-dashboard feel and toward a browser strategy game surface inspired by OGame-like information density.
- `/login` and `/register` must be standalone public account pages outside the authenticated game shell. They should not render the empire sidebar, resource/status bar, or gameplay navigation as their primary frame.
- `Inicio` becomes the authenticated current planet overview after account resolution. It should summarize the selected/current planet, resources, active queues, and direct gameplay actions.
- `Planeta` should be merged into `Inicio` as the same current-planet overview. If the route remains for compatibility, `/planet` should redirect or alias to the `Inicio` experience instead of duplicating a second overview page.
- Construction, Research, Shipyard, Defense, and Ground Army pages must read as focused action catalogs: dense four-column-capable catalog grids, clear costs, requirements, availability, queue/action state, and a direct primary action where the backend supports it.
- Normal gameplay pages must not present internal workflow concepts as player-facing copy: `cabina`, `contexto guardado`, `dar contexto`, `cargar mando`, `siguientes cabinas`, `registrar comandante` inside gameplay pages, generic `continuar` gameplay CTA, or duplicated module navigation cards.

This contract documents direction only. It does not claim browser/manual QA, screenshot acceptance, final art acceptance, SQL Server acceptance, or new gameplay behavior.

## Block 44 Shell Regression Audit

Recorded on 2026-07-08 for `TASK-44A`:

- Block 43 correctly removed duplicated in-page navigation cards, but the current routing also lets game routes render through `PublicAuthLayout` whenever the app-level account session is not ready. That removes the left game sidebar because the public layout intentionally has no sidebar or resource bar.
- The current account state is loaded independently by `App`, `AppShell`, `AuthRequiredState`, and route pages. Those separate async checks can disagree during loading or refresh, which can produce mixed states such as game content from a ready route component while the shell header still shows anonymous `Entrar` / `Crear cuenta` actions.
- `/`, `/planet`, and all guarded module routes are protected by `AuthRequiredState`, but `App.tsx` can wrap those same routes in the public layout before the guard and shell settle. Direct URLs with `civilizationId` and `planetId` must not bypass the account-backed game-shell decision in normal UI.
- Intended shell matrix: `/login`, `/register`, and `/registro` use standalone public account layout with no sidebar; authenticated `/`, `/planet`, `/construction`, `/research`, `/shipyard`, `/defenses`, `/ground-army`, `/fleets`, `/galaxy`, `/market`, `/alliance`, `/ranking`, and `/espionage` use the game layout with persistent desktop sidebar and top resources where selected-planet data is available; anonymous access to game routes shows a clean account prompt or redirect without game content.
- Restoring the global sidebar must not restore duplicated module-navigation cards inside Construction, Research, Shipyard, Defenses, or Ground Army. Those pages remain focused catalogs.
- Follow-up preservation check: restoring the global sidebar did not re-add duplicated cross-module cards inside the catalog pages. Construction remains backed by the planet construction catalog, Research keeps a technology grid, Shipyard keeps a production grid, Defenses keeps a defense catalog, and Ground Army keeps a land-unit catalog. Planet/Inicio may keep concise hub/activity links, but module pages should rely on the global sidebar for main navigation.

This audit was code and documentation review only. It did not perform browser/manual QA and did not add gameplay behavior.

## Ready For Local Demo

- One canonical demo path exists in `docs/dev/single-product-demo-guide.md`.
- The final frontend information architecture is documented in `docs/dev/product-completion-audit.md`.
- The deferred browser screenshot pass is scripted in `docs/dev/deferred-visual-qa-master-checklist.md`.
- Development QA scripts can prepare seeded/operator validation state, print scoped materialization commands, read diagnostics, and parser-check helper behavior.
- Construction, Research, and Shipyard create real Development database rows only after explicit confirmation and then rely on backend refreshes.
- Planet operator tools are hidden from product mode by default, remain technical when explicitly revealed, and stay scoped to Development data.
- Read-only/advisory routes keep their current boundaries and do not add unsupported gameplay execution.

## Development-Only Surface

These flows are usable for local QA and demo, but they are not production features:

| Surface | Development-only behavior |
|---|---|
| `/onboarding` | Route alias to registration; retained for compatibility, not a separate local start. |
| Seed profiles | Additive deterministic setup through Development endpoints for QA/operator validation, not player registration. |
| Queue materialization | Explicit scoped helper/action for due Development orders only. |
| Resource accrual | Explicit backend materialization, not an automatic page-load timer. |
| Construction/Research/Shipyard enqueue | Real persisted rows through Development-backed guarded flows. |
| Fleet transfer tools | Controlled Development mutation path tracked by the Fleet checklist, not final movement productization. |
| Diagnostics and operator panels | Read backend state, raw payload context, and technical action metadata only after explicit operator reveal; they are support surfaces, not primary gameplay. |

## Block 41 Product-Surface Update

Recorded on 2026-07-07:

- `/` is now a product-facing home route for account entry, continuing an account-backed route context, or entering the galaxy.
- The global shell, sidebar, continuation banner, local session copy, confirmation modals, empty/error states, catalog placeholders, and resource labels have been polished away from development/test/prototype wording in normal UI.
- Internal diagnostics, materialization controls, action manifests, and backend/operator details remain available only as hidden or explicit operator surfaces; they are still technical and are not accepted as production gameplay.
- Safe local access to hidden operator surfaces is documented only in `docs/dev/operator-mode.md`.
- The expanded frontend copy regression guard now fails on forbidden development/test/prototype wording in product-surface page and component copy while preserving narrow exceptions for operator-only code and documentation.
- This update did not add gameplay behavior, final production authorization, final assets, browser screenshot acceptance, SQL Server schema apply, or seed apply.
- Manual visual QA remains deferred to `docs/dev/deferred-visual-qa-master-checklist.md`.
- The final no-visible-development report is recorded in `docs/dev/no-visible-development-report.md`.

## Block 41 Final Validation Baseline

Recorded on 2026-07-07 for `TASK-41AY`:

- `dotnet build --no-restore` succeeded with `0` warnings and `0` errors.
- `dotnet test --no-build` succeeded with `744` passing tests, `0` failed, and `0` skipped.
- `npm run build --prefix src/VoidEmpires.Frontend` succeeded; Vite transformed `106` modules, emitted `60.39 kB` CSS, and kept the shared entry chunk at `181.48 kB` minified / `59.10 kB` gzip.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded, including the route lazy-import guard, copy regression guard, repository secret scan, and SQL Server generated-script safety check.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1` succeeded.
- Tooling note: the .NET commands reported available workload updates; this was informational and did not fail validation.

This was a non-visual automated validation pass. It did not perform browser or manual QA, did not connect to SQL Server, did not apply migrations or generated SQL, did not apply seeds, did not add real SQL Server credentials, and did not add gameplay behavior.

## Final DB And Model Needs

- Consolidate final persisted gameplay models before treating the shell as durable product behavior.
- Keep Development seed assumptions out of player registration, and replace any remaining final product initialization assumptions with production-owned initialization and migration strategy.
- Decide final queue, resource, stock, fleet, research, market, alliance, and ranking ownership boundaries.
- Keep ordinary CI/test paths independent of any real operator-managed database; the checked-in provider is still PostgreSQL/Npgsql today, and future SQL Server validation must remain explicitly gated.
- Block 41 product-surface copy work did not change the accepted SQL Server posture: `VoidEmpires_Dev` remains a manual/operator-managed validation target, the idempotent SQL review script is not auto-applied, and no real SQL password or resolved connection string belongs in the repository.
- As of the Block 41 product-surface pass, the repository still does not record a completed manual SQL Server schema apply or accepted `cockpit-validation` seed run against SQL Server. Those remain operator-controlled validation steps outside normal `dotnet test`.
- As of the Block 42 account flow, SQL Server registration behavior is documented as using Identity tables plus account world bootstrap after manual schema apply, but no manual SQL Server registration run is recorded.

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

1. Production authorization hardening and active civilization resolution beyond the current one-initial-civilization account flow.
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

Decision not granted: production readiness, final visual acceptance, final database/model readiness, final art/asset readiness, combat readiness, final movement readiness, market readiness, alliance readiness, or production authorization hardening.
