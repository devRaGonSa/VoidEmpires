# Current State

## Phase

The repository is consolidated through the Block 41 product-facing surface pass for the current Development-only product shell.

Block 40 SQL Server baseline work is now in the manual-review phase around the recommended `VoidEmpires_Dev` validation database. Current SQL Server work keeps PostgreSQL as the checked-in default, keeps real credentials outside the repository, and does not apply real database mutations automatically.

Block 41 product-surface work now makes normal frontend navigation and primary cockpit surfaces product-facing by default: the home route, shell, sidebar, continuation banner, confirmation modals, empty/error states, catalog placeholder copy, resource labels, and secondary shell status avoid development/test/prototype language, backend URLs, endpoint details, and localhost references in normal UI. Internal operator panels, diagnostics, materialization controls, action manifests, and backend details remain technical and hidden unless explicitly revealed. This did not add gameplay behavior, final assets, browser screenshot acceptance, SQL Server schema apply, or seed apply.

Block 42 account-registration work now makes `/register` the real player entry for the current playable loop. Registration creates an ASP.NET Core Identity user, account-linked player profile, civilization, home planet ownership, starting resources, production state, and account-backed route context; `/login`, `/api/accounts/me`, and `/api/accounts/logout` support the same account flow. Final gameplay authorization, account recovery/confirmation hardening, production deployment readiness, browser/manual QA, and a manual SQL Server registration run remain deferred.

Block 43 OGame-like UI rework now splits public account entry from authenticated game chrome, makes `Inicio` the current planet overview, aliases `Planeta` to the same overview, shows selected-planet resources with capacity in the top bar, and presents Construction, Research, Shipyard, Defenses, and Ground Army as compact catalog/readiness pages. The copy guard now blocks removed normal-UI terms such as `cabina`, `contexto guardado`, `dar contexto`, `cargar mando`, `siguientes cabinas`, `registrar comandante`, `partida local`, and `nueva partida` on product-surface frontend files. This did not perform browser/manual QA, did not add combat, fleet movement productization, market transactions, alliance mutations, active espionage, SQL Server apply, seed apply, or final asset acceptance.

Latest Block 43 final validation gate: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `779` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `122` transformed modules, `70.65 kB` CSS, and a `199.59 kB` minified / `63.93 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, `check-frontend-copy-regressions.ps1`, and `check-repo-secret-scan.ps1` all succeeded. This was non-visual automated validation only: no browser/manual QA, SQL Server connection, migration apply, generated SQL execution, seed apply, real credential handling, combat, fleet movement productization, market transactions, alliance mutations, or active espionage was performed.

Latest Block 42 automated validation gate: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `779` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `114` transformed modules and a `193.87 kB` minified / `62.21 kB` gzip entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, `check-frontend-copy-regressions.ps1`, and `check-repo-secret-scan.ps1` all succeeded. This was non-visual automated validation only: no browser/manual QA, SQL Server connection, migration apply, generated SQL execution, seed apply, or real credential handling was performed.

## Repository Reality

The AI Platform template has been adapted into a VoidEmpires-specific project workspace with:

- workflow rules in `AGENTS.md`
- planning and orchestration documents under `ai/`
- task lifecycle folders under `ai/tasks/`
- helper scripts under `scripts/`
- the `.NET` solution and projects under `src/` and `tests/`

Current final-database preparation reality:

- checked-in runtime persistence remains PostgreSQL-first by default, but explicit SQL Server provider selection is now supported in runtime and design-time wiring
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs` now accepts a provider selector and chooses `UseSqlServer(...)` only when configuration explicitly requests `sqlserver`
- `src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContextFactory.cs` now reads provider selection from environment variables and defaults to `UseNpgsql(...)` when no SQL Server override is present
- `src/VoidEmpires.Web/Program.cs` reads `ConnectionStrings:DefaultConnection`, only wires persistence-backed services when it is non-empty, and passes through `VoidEmpires:Persistence:Provider`
- repository appsettings remain placeholder-safe, keep `DefaultConnection` empty by default, and do not store real SQL Server credentials
- SQL Server remains a documented future target on user-managed infrastructure, not the active checked-in provider
- no checked-in repository path auto-applies migrations during startup, tests, or helper-script execution
- no real SQL Server migration, update, backup, restore, or seed apply has been performed by the completed Block 38 documentation tasks
- repository secret and copy guards now also check for unsafe connection-string password examples and obvious committed secret patterns while allowing documented placeholders only
- current helper scripts remain manual, validation-only, or explicitly operator-invoked by default rather than hidden SQL Server mutation automation
- `VoidEmpires_Dev` is now documented as the recommended first controlled SQL Server validation target; existing helpers were audited as non-destructive by default, and the current create-database helper remains a manual SSMS reference rather than automatic provisioning
- controlled SQL Server creation preparation now includes the SSMS-oriented `VoidEmpires_Dev` create script, Spanish-first operator checklist, local environment/user-secrets guidance, opt-in read-only `SELECT 1` smoke path, isolated SQL Server baseline migration, guarded idempotent SQL script helper, static generated-SQL safety guard, local app-run guidance after schema preparation, dry-run-first final catalog helper, and strengthened repository secret scan
- the repository does not claim that `VoidEmpires_Dev` already exists, does not commit a resolved SQL Server connection string or real password, and still requires manual operator action for database creation, credential provisioning, schema review/apply, optional smoke validation, and app execution against SQL Server
- the `VoidEmpires_Dev` connection smoke passed manually before the current baseline block; the repository still treats SQL Server smoke as opt-in and does not require it for ordinary `dotnet test`
- `SqlServerInitialBaseline` has been generated under `src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer` after isolating SQL Server design-time migration history; the earlier `TASK-40D` scaffold was rejected because it reused the PostgreSQL-shaped root snapshot
- the idempotent SQL review script has been generated and committed at `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql`; it is guarded by static safety and secret scans and has not been executed
- no SQL Server migration has been applied automatically; schema apply remains a manual SSMS/operator-confirmed step
- no real SQL Server password, resolved connection string, or hardcoded SQL Server credential was committed

Current final-database readiness status:

- provider support: PostgreSQL-first today, with explicit SQL Server selection now available as an external opt-in
- migration status: existing root EF migration history remains PostgreSQL-shaped, while the SQL Server baseline is isolated in its own migration namespace/folder and has a generated idempotent review script
- schema apply status: manual only; no repository script, app startup path, or test path has applied the SQL Server baseline to `VoidEmpires_Dev`
- catalog status: gameplay catalogs remain code-owned, not final relational seed data; final catalog seeding is the next step only after schema exists and currently remains dry-run/deferred for relational apply
- seed status: Development seed profiles remain QA scaffolding, not final production initialization
- current persisted entity inventory: likely final SQL-owned gameplay state currently includes galaxy topology (`Galaxy`, `SolarSystem`, `Star`, `Planet`), player/civilization ownership (`PlayerProfile`, `Civilization`, `PlanetOwnership`), alliance/diplomacy state (`Alliance`, `AlliancePact`, `AllianceMembership`, `DiplomaticContact`), economy/build/research state (`PlanetResourceStockpile`, `PlanetProductionProfile`, `PlanetBuilding`, `PlanetBuildingCapacity`, `PlanetConstructionOrder`, `ResearchProject`, `ResearchOrder`, `PlanetPopulationProfile`), and exploration/fleet progression state (`OrbitalTransfer`, `ExplorationMission`, `ExplorationKnowledge`) plus local stock entities configured in persistence; identity tables remain ASP.NET Core Identity/ops state; final relational ownership for gameplay catalogs and production seed data is still deferred
- current schema naming convention baseline: the more recent explicitly configured tables follow lowercase snake_case names (`planets`, `planet_resource_stockpiles`, `exploration_knowledge`), columns also trend to lowercase snake_case (`planet_id`, `normalized_display_name`, `discovered_at_utc`), and named indexes generally use explicit `ux_` or `ix_` prefixes; however, not every persistence configuration has been normalized to that explicit style yet, so later SQL Server migration-baseline work still needs a full pass over remaining unnamed or convention-driven mappings
- deferred items: manual SSMS schema apply, post-apply verification, final relational catalog ownership/apply, final production seed architecture, and broader explicitly gated SQL Server validation
- safety posture: SQL Server guidance stays documentation-only and manual by default, with external secrets, explicit backups, manual review, and no automatic apply against the user's real server
- latest Block 38 cross-stack validation gate: `dotnet build --no-restore` succeeded with `0` errors and one rerun showing transient `MSB3026` copy-retry warnings while `testhost` held test output DLLs; `dotnet test --no-build` succeeded with `725` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules and a `181.33 kB` minified / `59.14 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, `check-frontend-copy-regressions.ps1`, and the new repository secret scan all passed
- latest Block 38 no-apply closure note: the repository still contains no committed real SQL Server password, no completed Block 38 task applied a real SQL Server change automatically, and manual SQL mutation remains an explicit operator responsibility outside default repo validation
- current completed Block 38 work now includes the documentation-first subset, provider-selection and mapping-audit tasks through `TASK-38I`, the SQL Server migration guidance tasks through `TASK-38N`, and the consolidated final catalog audit in `TASK-38O`; the repository still has not completed SQL Server runtime cutover, SQL Server migration replay readiness, or final relational catalog ownership
- next pending Block 38 implementation categories are the per-catalog ownership tasks (`TASK-38P` through `TASK-38U`) and the final seed architecture tasks (`TASK-38V` through `TASK-38Z`)
- latest Block 38 final validation refresh: `dotnet build --no-restore` succeeded with `16` transient `MSB3026` copy-retry warnings and `0` errors while `testhost` held test output DLLs; `dotnet test --no-build` succeeded with `725` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules, `58.88 kB` CSS, and a `181.33 kB` minified / `59.14 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, and `check-frontend-copy-regressions.ps1` all succeeded; this remained a documentation-first non-cutover validation pass only
- latest Block 39A runbook audit note: SQL Server docs now describe the manual `VoidEmpires_Dev` path: create in SSMS, configure local secrets outside the repo, run the read-only `SELECT 1` smoke, generate and review migration scripts only after the SQL Server baseline exists, apply manually only with operator approval, and run the app against SQL Server only through external provider selection
- latest Block 39N controlled database state validation: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `741` passing tests, `0` failed, and `0` skipped; no SQL Server connection, migration generation, migration apply, seed apply, backup, or restore was performed for this current-state update
- latest Block 39O final validation gate: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `741` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules and a `181.33 kB` minified / `59.13 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, `check-frontend-copy-regressions.ps1`, and `check-repo-secret-scan.ps1` all succeeded; the gate did not connect to SQL Server, did not generate or apply a SQL Server migration, did not apply seeds, and did not add a real SQL Server password
- latest Block 39P closure validation: TASK-39A through TASK-39O were verified in `ai/tasks/done`, `ai/tasks/pending` contains only `.gitkeep`, `dotnet build --no-restore` succeeded with `0` warnings and `0` errors, `dotnet test --no-build` succeeded with `741` passing tests, `0` failed, and `0` skipped, `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules and a `181.33 kB` minified / `59.13 kB` gzip shared entry chunk, and the QA, route lazy-import, frontend copy, and repository secret guards all passed; no real SQL Server password was committed and no SQL Server migration was applied automatically
- latest Block 40 SQL Server baseline state: the accepted SQL Server baseline migration has initial-schema shape and SQL Server metadata, the idempotent SQL script exists for manual review, `scripts/check-sqlserver-generated-script-safety.ps1` and the expanded repository secret scan pass against the committed artifacts, no generated SQL was run, no `dotnet ef database update` was run, schema apply remains manual/user-confirmed, and final catalog seed apply remains the next controlled step after schema exists
- latest Block 40O final validation gate: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `744` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules and a `181.33 kB` minified / `59.13 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, `check-frontend-copy-regressions.ps1`, and `check-repo-secret-scan.ps1` all succeeded; the gate did not connect to SQL Server, did not execute generated SQL, did not run `dotnet ef database update`, did not apply seeds, and did not commit real SQL Server credentials
- latest Block 40P closure validation: `TASK-40A` through `TASK-40O` were verified in `ai/tasks/done`, `ai/tasks/pending` contains only `.gitkeep`, `dotnet build --no-restore` succeeded with `0` warnings and `0` errors, `dotnet test --no-build` succeeded with `744` passing tests, `0` failed, and `0` skipped, `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules and a `181.33 kB` minified / `59.13 kB` gzip shared entry chunk, and the QA, route lazy-import, frontend copy, and repository secret guards all passed; no real SQL Server password was committed, no generated SQL was executed, and no SQL Server migration was applied automatically
- latest final catalog seed ownership audit: checked-in catalog source validation currently covers `15` building rows, `8` research rows, `4` orbital asset rows, `1` defense row, and `7` resource rows, but non-dry-run final catalog apply remains intentionally unsupported because the SQL Server baseline does not yet define dedicated final catalog table ownership, deterministic upsert behavior, or disposable replay validation for those rows
- latest disposable SQL Server replay validation note: `docs/dev/sql-server-disposable-replay-validation.md` now documents the manual disposable-database replay gate for the accepted baseline script, including idempotent second-run verification and post-apply inspection; the gate was not executed, is not automated, does not target `VoidEmpires_Dev`, and does not use `dotnet ef database update`
- latest Block 41 product-surface documentation note: product-facing UI copy no longer exposes development/test/prototype wording by default, while operator/internal tools remain hidden and technical; accepted SQL Server seeded validation status is unchanged, manual visual QA remains deferred, and no gameplay expansion was added
- latest Block 41AU subset closure note: `TASK-41A` through `TASK-41AT` are in `ai/tasks/done`; remaining pending Block 41 work is limited to later closure/follow-up tasks. Backend validation for this closure passed with `dotnet build --no-restore` and `dotnet test --no-build`; no browser/manual QA, migration apply, SQL Server action, or gameplay expansion was performed.
- latest Block 41AV operator-mode note: `docs/dev/operator-mode.md` documents the local-only `?operator=1` and `localStorage["voidempires.operatorMode"] = "1"` reveal mechanisms, while keeping operator tools hidden from normal product flow and explicitly outside production/auth/SQL readiness claims.
- latest Block 41AW SQL Server product-readiness note: product-facing UI cleanup changed no database behavior; the repository still does not record a completed manual SQL Server schema apply or accepted `cockpit-validation` seed run against SQL Server, and real credentials/connection strings remain outside source control.
- latest Block 41AY final validation baseline: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `744` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules, `60.39 kB` CSS, and a `181.48 kB` minified / `59.10 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, `check-frontend-copy-regressions.ps1`, and `check-repo-secret-scan.ps1` all succeeded. This final baseline did not perform browser/manual QA, did not connect to SQL Server, did not apply migrations or generated SQL, did not apply seeds, did not commit secrets, and did not add gameplay behavior.
- latest Block 41AZ closure validation: all remaining Block 41 product-page tasks through Market, Alliance, Ranking, and Espionage are complete; `ai/tasks/pending` contains only `.gitkeep`; `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `744` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `106` transformed modules, `60.39 kB` CSS, and a `181.49 kB` minified / `59.10 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, `check-frontend-copy-regressions.ps1`, and `check-repo-secret-scan.ps1` all succeeded. This closure did not perform browser/manual QA, did not generate final images, did not connect to SQL Server, did not apply migrations or generated SQL, did not apply seeds, did not commit secrets, and did not add gameplay behavior.

## Application Status

The repository contains `VoidEmpires.sln` with:

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Domain`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

Current frontend cockpit baseline:

- Block 41 product-facing surface work adds `/` as the product home route and keeps `/galaxy` as the map route, with normal UI copy avoiding development/test/prototype wording and backend/localhost details. Operator-only diagnostics and materialization tools remain secondary, explicitly revealed, and technical.

- Block 36 completed the UI information architecture cleanup for the current cockpit suite without changing backend gameplay semantics. The global header no longer presents disconnected mock resource bars as selected-context truth, sidebar labels now better distinguish guarded mutations, readiness, read-only, Development QA, and future work, and obsolete global read-only copy was removed from mutation-capable surfaces.
- Planet, Construction, Research, and Shipyard were decluttered around the active backend context, primary action/read-state, backend-owned resources or queues, handoffs, secondary Development tools, and collapsed diagnostics. Planet now reads as the hub, Construction reads as the focused catalog/queue surface, and Research/Shipyard use the shared page context strip plus one secondary diagnostic path.
- `DevelopmentToolsPanel` is the shared secondary surface for Development QA actions on Planet/Construction. Resource accrual and due-queue materialization stay explicit Development actions, require confirmation, mutate only Development data through backend endpoints, and never run on page load, sidebar navigation, or ordinary card selection.
- `GameModal` remains the shared confirmation foundation for Construction, Research, and Shipyard gameplay mutations, and the Development action confirmation flow now covers the scoped Planet materialization actions. These changes did not add combat, fleet movement, exploration missions, market transactions, alliance mutations, or new gameplay semantics.
- `/register` is now the primary playable account entry route. It posts to `POST /api/accounts/register`, creates the Identity account and initial player world, logs in after successful registration, stores only non-sensitive navigation context, and routes to the generated home planet. `/onboarding` remains only a compatibility alias to registration rather than a separate local/new-game product model.
- Galaxy v1 remains read-only and has been restored as the accepted seeded cockpit baseline: `/galaxy` is now the canonical map route, `/` is the product home route, missing or invalid or failed or empty context now renders explicit Spanish states, seeded focus falls back to owned or visible systems instead of leaving the screen blank, diagnostics now expose a compact state summary when context exists, and cross-cockpit handoffs preserve owning Fleet context without exposing gameplay mutations from Galaxy.
- Planet v1 now exists at `/planet` as a 2D dashboard and context hub with a development-only UI-state read endpoint, Spanish-first presentation helpers, deterministic seeded economy and construction context, readable resources or production or capacity sections, explicit backend materialization controls for planet resource accrual, grouped buildings, queue visibility, guarded construction enqueue, and dashboard handoff cards for Construction, Fleets, Galaxy, and the specialized cockpits.
- Construction v1 now exists at `/construction` as a focused general-infrastructure route for the same owned-planet construction state, with catalog readability, shared modal-based confirmation, Spanish error guidance, guarded real enqueue through the Development backend only, authoritative queue/resource refresh feedback after success, visible accepted-but-not-yet-refreshed lag handling, and secondary handoff cards for Research, Ground Army, Shipyard, and Defenses.
- Research v1 now exists at `/research` as the second accepted controlled persisted mutation cockpit after Construction, with a deterministic seeded QA path that exposes at least one available item (`Ingenieria planetaria`), visible blocked items with meaningful Spanish reasons, category-grouped catalog state, truthful summary counts and recommendation fallback, visible requirements, costs, durations, queue and completed-project summaries, shared modal-based confirmation before enqueue, backend-confirmed immediate resource deduction, backend state refresh after success, specific Spanish error mapping with secondary diagnostics, non-mutating blocked research cards, no automatic completion, and a conservative disabled complete-due placeholder when the backend route is not scoped safely to the cockpit.
- Shipyard v1 now exists at `/shipyard` as a development-safe cockpit foundation upgraded from the earlier placeholder route, with deterministic seeded `Aurelia` context, visible resources, production capability and readiness summaries, categorized orbital asset options, visible queue and local stock reads, shared modal-based confirmation for guarded development enqueue through the scoped orbital production endpoint, explicit success refresh feedback, a conservative disabled complete-due placeholder because the current backend route is still global, cross-navigation back to Planet, Construction, Research, Fleets, and Galaxy, explicit copy that Fleet movement and command execution remain outside this cockpit, and a dedicated Development-only repeated-QA preparation path through `POST /api/dev/shipyard/qa-state/prepare` plus `scripts/dev-qa-prepare-orbital-production-ui-state.ps1`.
- Defenses v1 now exists at `/defenses` as a development-safe cockpit foundation upgraded from the earlier placeholder route, with deterministic seeded `Aurelia` context, defense readiness summaries, a visible `DefenseGrid` structure and option state, readable stockpile or missing-resource guidance, truthful queue and complete-due limitation messaging, collapsed diagnostics, explanatory handoffs toward Construction, Shipyard, Fleets, Planet, and Galaxy, and an explicit read-only posture for this block because there is still no accepted `POST /api/dev/defenses/...` enqueue path.
- Ground Army v1 now exists at `/ground-army` as a development-safe cockpit foundation upgraded from the earlier placeholder route, with deterministic seeded `Aurelia` context, visible readiness and population summary state, visible ground structures and catalog options, truthful available and blocked training comparisons, completed queue-history context, collapsed diagnostics, and explanatory handoffs toward Construction, Defenses, Fleets, Planet, and Galaxy while keeping unsafe mutation confirmation-based, disabled, or handed off rather than combat-scoped.
- Market v1 now exists at `/market` as the accepted development-safe read-only economy cockpit for the current suite, with deterministic seeded `Aurelia` and `Helios Gate` context, Spanish-first economy copy, visible civilization and local reserves, selected-planet production, advisory reference ratios that stay non-executable, secondary trade-signal and future-route context, visibly disabled future market operations, explicit handoffs toward Planet, Construction, Shipyard, Fleets, and Galaxy, and one collapsed diagnostic surface that keeps technical detail secondary while making clear that buying, selling, auctions, player-to-player trading, resource mutation, and trade-route execution remain out of scope.
- The Market visual and read-only polish block `26C-26L` is now complete for the implemented frontend baseline; no transaction gameplay was introduced, and final screenshot-backed acceptance still remains user-driven.
- Espionage v1 now exists at `/espionage` as the accepted read-only intelligence cockpit, with deterministic seeded `Helios Gate` context, grouped intelligence coverage and target catalogs, Spanish-first signal and coverage labels, normalized future-mission cards that remain visibly disabled, collapsed diagnostics, and handoffs toward Galaxy, Planet, Fleets, and Research while keeping spy mission execution, sabotage, infiltration, counter-espionage, combat, and WebSockets out of scope.
- Alliance v1 now exists at `/alliance` as the accepted read-only diplomacy cockpit, with deterministic seeded `Void Seed Civilization` context, visible alliance-status summary, known-contact or readiness catalog, disabled future pact and diplomacy action placeholders, collapsed diagnostics, and handoffs toward Galaxy, Market, and Espionage while keeping alliance creation, pact execution, invitations, role management, messaging, treasury controls, shared visibility, and any diplomacy mutation out of scope.
- Ranking v1 now exists at `/ranking` as the accepted read-only power-index cockpit, with deterministic seeded `Void Seed Civilization` context, visible power summary, explicit category cards, demo-only comparison rows, disabled future leaderboard or season or reward placeholders, collapsed diagnostics, and handoffs toward Galaxy, Market, Espionage, and Alliance while keeping public ladders, matchmaking, rewards, public profiles, persistence, and production-auth changes out of scope.
- Fleets remains the accepted dev-cockpit foundation and now supports simple URL-based context links into Planet, Construction, and Shipyard while keeping destination context optional.
- Query-context helpers now centralize `civilizationId` and `planetId` navigation so cockpit links and the shared sidebar preserve returned or seeded ids across Planet, Construction, Research, Shipyard, Defenses, Fleets, Market, and Galaxy handoffs.
- Local playable-session storage is a non-sensitive URL rebuilding convenience only. It may preserve ids and friendly labels from registration or Development QA flows, but it is not authentication, authorization, ownership, a bearer token, a cookie, a role claim, or production session state.
- Planet is the playable-loop hub for the current Block 36 frontend. It owns the primary return point for local session continuation and handoffs into Construction, Research, Shipyard, Defenses, Fleets, and Galaxy while keeping backend reads authoritative.
- Planet, Construction, Research, Shipyard, Defenses, and Fleets are now session-aware enough to offer local playable-session continuation when query ids are missing, while still revalidating state through backend reads once ids are present.
- Resource display is now normalized for the playable loop through shared resource formatting helpers, keeping primary labels consistent for `Creditos`, `Metal`, `Cristal`, `Gas`, `Energia`, `Deuterio`, and `Poblacion` while preserving backend-sourced quantities and calculations.
- `GameModal` is now the shared frontend confirmation foundation for Construction, Research, and Shipyard review steps. Opening a modal is local UI state only; the explicit primary action plus acknowledgement checkbox remains the only mutation trigger.
- Planet resource economy v1 is explicit and backend-authoritative: visible accrual is materialized only through the Development resource-economy action and then re-read from backend state. Normal page load does not silently mutate resources or run a frontend timer.
- Queue materialization v1 is implemented as a Development-only, scoped backend boundary through `IGameplayQueueMaterializationService`, `GameplayQueueMaterializationService`, and `POST /api/dev/queues/materialize-due`. It processes only due open orders for the requested civilization and optional planet, returns per-queue summaries, and keeps terminal or not-yet-due rows idempotent.
- Construction queue materialization now applies already-paid due construction orders to persisted building state, including construct and upgrade behavior, without charging resources a second time. Missing upgrade targets stay open with notes instead of being force-completed.
- Research queue materialization now applies already-paid due research orders to persisted `ResearchProject` state, creates the project when needed, upgrades it to the target level, and clears the open order by marking it completed.
- Shipyard queue materialization now applies already-paid due orbital production orders to local `OrbitalAssetStock` for the scoped owned planet and marks those orders completed. Planetary asset production, fleet creation, and stock-to-fleet allocation remain outside this materialization boundary.
- Planet exposes the only current frontend queue materialization action, clearly labeled as `Development QA`. It is an explicit user action that calls the scoped Development endpoint, re-reads Planet state afterward, and does not run on page load, navigation, or card selection.
- Development-only queue materialization helpers are now documented and parser-checked: `scripts/dev-qa-materialize-due-queues.ps1` calls the scoped endpoint, while `scripts/dev-qa-prepare-playable-session-state.ps1 -PrintQueueMaterializationCommand` only prints a follow-up command for later manual QA.
- The playable-session diagnostics endpoint and helper are now present: `GET /api/dev/playable-session/diagnostics` and `scripts/dev-qa-get-playable-session-diagnostics.ps1` read resources, construction, research, shipyard, stock, readiness notes, warnings, and limitations without applying seeds, accruing resources, enqueueing, materializing queues, or moving fleets.
- `scripts/dev-qa-playable-loop-guide.ps1` is the single safe guide command for the current local loop. By default it prints backend, playable-session setup, frontend, onboarding, materialization, and diagnostics steps without running hidden mutations.
- The frontend playable-loop states were hardened around missing ids, local-session recovery, backend failures, no available actions, open-order blockers, queue materialization no-ops, and read-only scope. Backend errors remain visible while raw technical payloads stay secondary.
- Frontend diagnostics panels now surface compact summary items and collapsed raw payloads for Planet, Construction, Research, and Shipyard without making diagnostics dominate the main cockpit flow.
- Script and copy guardrails are stronger: PowerShell QA helpers are parser-checked with UTF-8 setup, the frontend copy guard now catches known mojibake sequences, risky id placeholders, English primary UI fallbacks, instant-completion wording, cheat wording, and forbidden normal-UI materialization phrasing.
- Backend regression coverage now protects the read-only boundary: playable-session diagnostics compare resources, queue status counts, building counts, research project counts, and orbital stock, while due-queue materialization has an explicit not-due no-op endpoint test.
- Browser and visual QA for the hardened playable loop remains deferred to `docs/dev/deferred-visual-qa-master-checklist.md`; this block validated backend behavior, frontend build/type safety, scripts, docs, and static guardrails only.
- Module-specific catalog duplication has been reduced by extracting shared planet layout components and route builders.
- The accepted cockpit suite now shares a clearer polish baseline: primary copy is more gameplay-facing, diagnostics stay collapsed or clearly secondary, action hierarchy is more consistent, responsive overflow has been tightened, and sidebar or module-state cues better distinguish implemented versus future modules.
- Block 37 now documents the near-product frontend information architecture, canonical local demo path, deferred visual screenshot checklist, and product readiness decision through `docs/dev/product-completion-audit.md`, `docs/dev/single-product-demo-guide.md`, `docs/dev/deferred-visual-qa-master-checklist.md`, and `docs/dev/product-readiness-report.md`.
- Current readiness decision: the shell is ready for Development-only local product demo after the documented validation commands pass; production readiness, final visual acceptance, final database/model readiness, final art/asset readiness, combat readiness, final movement readiness, market readiness, alliance readiness, and production-auth readiness are not granted.
- Development-only seed profiles now provide the standard QA setup path for Galaxy, Planet, Construction, Research, Ground Army, Shipyard, Fleets, Market, Defenses, Espionage, Alliance, and Ranking without manual SQL.
- `minimal-validation` remains the deterministic shared baseline, `cockpit-validation` is now the first coherent cross-cockpit demo scenario for Galaxy, Planet, Construction, Research, Ground Army, Shipyard, Fleets, Market, Defenses, Espionage, Alliance, and Ranking together, and the current cockpit-specific richer profiles are `shipyard-validation`, `fleet-validation`, `research-validation`, and `planet-full-validation`.
- Seed profiles are additive, deterministic, idempotent, and Development-only. They restore documented baseline rows and minimums but do not destructively clear queues, extra transfers, or other user mutations.
- Development seed profiles are QA/operator fixtures only. Real player entry is the account registration flow at `/register`, which creates account-linked player, civilization, home planet, ownership, starting resources, and production state without relying on `cockpit-validation`.
- Account bootstrap first assigns an existing unowned Terran planet, or creates an account-bootstrap galaxy/system/home planet when needed. Multiple account registrations receive distinct profiles, civilizations, home planets, and ownership rows while sharing the same starting resource and production baseline.
- The real persisted Development enqueue path is now covered for Construction, Research, and Shipyard through direct endpoint tests, negative-path coverage, resource-deduction checks, cross-cockpit read-model regression coverage, and construction-specific frontend/runtime documentation; Fleet coverage for this block remains the read-only post-Shipyard verification path.
- `cockpit-validation` is now verified to preserve manual QA-created Construction, Research, and Shipyard orders created through the supported dev endpoints while still avoiding duplicate seeded history rows.
- The Construction cockpit now has a documented playable-v1 boundary: Development-only, explicit confirmation required before submit, no auto-completion, no optimistic local queue fabrication, backend-owned immediate resource deduction, backend-owned queue refresh after success, and no construction mutation expansion into Planet, Research, Defenses, Shipyard, Fleets, or Galaxy handoffs.
- Construction remains the first accepted controlled persisted mutation cockpit, Research is now the second, Shipyard keeps its separate documented enqueue boundary, and the read-only cockpits remain read-only unless a narrower accepted mutation path was already documented for that cockpit.
- Backend-only QA helpers now exist for baseline capture plus one real Construction, Research, or Shipyard enqueue without manual SQL, followed by Fleet read-state verification: `scripts/dev-qa-baseline.ps1`, `scripts/dev-qa-create-construction-order.ps1`, `scripts/dev-qa-create-research-order.ps1`, `scripts/dev-qa-create-shipyard-production-order.ps1`, and `scripts/dev-qa-fleet-read-state.ps1`.
- The persisted QA PowerShell helpers are now hardened against the real Development DTO shapes: the shared script helpers accept `resourceType + quantity`, `resourceType + amount`, flat stockpile objects, and unknown-shape fallback warnings, and `dev-qa-baseline.ps1` no longer crashes on the outdated direct `.amount` assumption.
- The persisted QA create-order helpers are now aligned with the live backend command contracts: the Construction helper posts backend-compatible enqueue values instead of stringified UI action text, and the Research helper now recognizes `Civilization already has an open research order.` from either parsed JSON errors or the raw HTTP body so the expected reused-Development-database no-op path works in both Windows PowerShell and newer shells.
- The persisted-flow runbook is documented in `docs/dev/persisted-gameplay-flow-checklist.md`, and the Shipyard/Fleet companion runbook is documented in `docs/dev/shipyard-fleet-persisted-qa.md`, including backend start, seed apply, baseline capture, real enqueue commands, expected success/no-op interpretation, Fleet read-state verification, and reseed-preservation checks.
- A lightweight local PowerShell check now exists at `scripts/check-dev-qa-scripts.ps1` to parser-check the persisted QA helpers and validate resource formatting plus known Construction, Research, and Shipyard no-op helper behavior without requiring the backend to be running.
- Richer development seed profiles now reserve deterministic high sequence ranges for their completed queue-history rows, preventing runtime collisions when `cockpit-validation` is applied over reused development databases that already contain manual QA queue activity.
- The development seed apply endpoint now converts persisted-state write conflicts into `409 Conflict` responses with diagnostic details instead of surfacing an unhandled runtime failure.
- The current frontend boundary model is documented in `docs/dev/planet-module-boundaries.md`.
- The frontend route-loading baseline and post-lazy outcome are now documented in `docs/dev/frontend-performance-notes.md`: cockpit pages are route-lazy-loaded from `App.tsx`, the shared shell keeps synchronous navigation infrastructure only, the previous `551.88 kB` entry-chunk warning has been replaced by a `179.32 kB` shared entry chunk plus cockpit-specific async chunks, and the current Vite build no longer emits the old `500 kB` warning.
- The frontend lazy-loading block changed route-loading architecture only. It did not change gameplay rules, backend contracts, accepted cockpit URLs, or the current Spanish-first cockpit acceptance boundaries.
- A lightweight guard now exists at `scripts/check-frontend-route-lazy-imports.ps1` so future changes do not quietly reintroduce direct eager cockpit-page imports in `src/VoidEmpires.Frontend/src/App.tsx`. The broader `scripts/check-dev-qa-scripts.ps1` helper now runs that guard when present.
- The current Research cockpit QA flow and acceptance boundaries are documented in `docs/dev/research-cockpit-checklist.md`.
- The current Shipyard cockpit QA flow and accepted Fleet boundary are documented in `docs/dev/shipyard-cockpit-checklist.md`.
- The current Defenses cockpit QA flow and accepted non-combat boundary are documented in `docs/dev/defenses-cockpit-checklist.md`.
- The current Ground Army cockpit QA flow and accepted non-combat boundary are documented in `docs/dev/ground-army-cockpit-checklist.md`.
- The current Espionage cockpit QA flow and accepted read-only intelligence boundary are documented in `docs/dev/espionage-cockpit-checklist.md`.
- The current Alliance cockpit QA flow, seeded route, read-only behavior, and diplomacy exclusions are documented in `docs/dev/alliance-cockpit-checklist.md`.
- The current seed profile catalog, discovery endpoint, deterministic ids, and QA URLs are documented in `docs/dev/development-seed-profiles.md`.
- The current Alliance v1 backend-safe scope is now documented in `docs/dev/alliance-cockpit-checklist.md`: `Civilization` remains the correct requester identity, the consolidated development-only Alliance UI-state read path stays metadata-only, the seeded `cockpit-validation` baseline includes one deterministic diplomatic contact for repeatable `/alliance` QA, and Alliance v1 must not imply membership authority, invitations, shared visibility, shared sensors, trade, war, espionage, or any mutation flow.
- The current Ranking v1 QA route, seeded checklist, demo-comparison posture, and non-public scope are documented in `docs/dev/ranking-cockpit-checklist.md`.

Current intentional exclusions:

- no 3D or WebGL renderer
- no combat gameplay
- no Ground Army combat, invasion, or assault resolution
- no real interception execution
- no espionage gameplay beyond the current read-only cockpit foundation
- no espionage execution
- no infiltration
- no sabotage
- no counter-espionage execution
- no alliance gameplay authority
- no alliance creation
- no pact execution
- no alliance invitations or applications
- no alliance role management
- no alliance messaging or treasury controls
- no public ranking
- no matchmaking
- no ranking rewards
- no public player ranking profiles
- no auth-backed playable session bootstrap
- no production login-to-civilization resolution
- no production authentication
- no production auth
- no production data
- no final database/model consolidation
- no final generated image or asset integration
- no production-ready active civilization resolver
- no hidden resource accrual on ordinary page load
- no frontend-only resource timer
- no automatic queue materialization on ordinary page load, sidebar navigation, route entry, or card selection
- no queue materialization framed as normal gameplay cheating; the current visible action is Development QA only
- no manual SQL standard path for the accepted persisted QA loop
- no market transactions
- no buying
- no selling
- no player-to-player trading
- no auctions
- no resource mutation from Market
- no trade-route execution from Market
- no Fleet movement, transfer creation, split, merge, or combat execution from Shipyard
- no new Fleet movement, split, merge, transfer creation, transfer cancellation, transfer completion, or stock-to-fleet allocation in the accepted persisted Shipyard/Fleet QA loop
- no combat, interception execution, fleet movement, or shield simulation from Defenses
- no invasion, bombardment, orbital transport command, or fleet movement execution from Ground Army
- no real specialized module execution yet outside the current backend-supported Research and Shipyard enqueue paths plus the accepted Fleets flow; Espionage remains analysis-only
- no real research effects beyond queue and completion state
- no destructive seed reset behavior
- no final screenshot-backed visual QA performed in the Block 32P validation pass
- no final screenshot-backed visual QA performed in the Block 33 validation pass
- no final screenshot-backed visual QA performed in the Block 34 validation pass
- no playable-loop combat, attack, fleet movement, mission creation, or auto-complete action introduced by session navigation
- no queue-progression combat, attack, fleet movement, mission creation, or auto-complete action introduced by materialization

Current implemented foundations:

- PostgreSQL/EF Core persistence boundary.
- ASP.NET Core Identity persistence foundation.
- Galaxy, player, civilization, ownership, economy, buildings, research, construction queue, research queue, population, asset production, asset stock, orbital group, and orbital transfer persistence models and migrations.
- Optional queue workers for construction, research, asset production, and orbital transfers.
- Development-only HTTP validation endpoints for current backend foundations.
- Planet visual state contracts and persisted visual state service.
- Solar system visual state contracts and persisted system visual state service.
- System visual metadata for renderer preparation: star metadata, coordinates, orbital slots, orbit radii, orbit angles, and scale hints.
- Read-only system visual overlays: stationed orbital group markers and planned/active transfer route overlays.
- Read-only orbital travel estimate previews through the application, infrastructure, and development API layers, including affordability and insufficient-resource details.
- Read-only orbital route profile metadata for travel estimates, classifying abstract distances into deterministic `LocalOrbit`, `InnerSystem`, `OuterSystem`, and `LongRange` bands with placeholder risk and fuel metadata.
- Read-only placeholder orbital fuel readiness previews for travel estimates, deriving estimated fuel units, estimated range, readiness, and not-ready reasons without adding fuel inventory or fuel charging.
- Reusable resource affordability and spend service for persisted planet stockpiles, with atomic multi-resource validation and spending.
- Orbital transfer creation charges estimated travel costs from persisted planet stockpiles before reserving groups and creating transfers.
- Persistent orbital group split foundation for stationed groups, preserving civilization, origin planet, current planet, asset type, and status while decreasing the source group quantity.
- Persistent orbital group merge foundation for compatible stationed groups, increasing the target group quantity and removing the source group.
- Read-only fleet operational overview for civilization-scoped orbital groups, active transfer summaries, and command availability flags.
- High-level fleet lifecycle smoke coverage validates preview, transfer creation and resource charging, active-transfer command rejection, cancellation, split, merge, second transfer completion, and final overview state.
- Route/fuel lifecycle smoke coverage validates that travel estimates include route profile and placeholder fuel readiness, remain read-only, do not require fuel inventory or route graph persistence, and stay coherent with fleet UI state hints through transfer completion.
- Developer-facing fleet API contract documentation under `docs/dev/fleet-api-contracts.md`, covering development gating, request/response payloads, status codes, read-only versus mutating behavior, resource charging/no-refund behavior, restrictions, and a compact fleet lifecycle example.
- Fleet development endpoint response consistency review found the current status-code and response-shape conventions already aligned for safe frontend tooling use, so no endpoint behavior changes were introduced in Phase 6W.
- Visual sandbox development documentation now reports the current Phase 6X validation baseline instead of the stale Phase 6H/6I test count.
- Development-only fleet UI state endpoint aggregates operational group state, active transfer summaries, command availability, current-planet resource contexts, and action hints for future UI prototypes.
- Development-only fleet UI state endpoint exposes route/fuel readiness capability hints for each group, while leaving concrete route profile and fuel readiness previews null until a destination-specific travel estimate is requested.
- Development-only fleet action manifest exposes deterministic, machine-readable metadata for current fleet dev actions, including route, method, mutability, required fields, success status, common error statuses, and notes for route/fuel preview flows.
- Read-only strategic map service consolidates civilization-scoped relevant systems, planet visual/layout summaries, owned ownership markers, fleet presence, active transfer overlays, and route/fuel capability notes without adding gameplay behavior or production UI endpoints.
- Development-only strategic map endpoint at `GET /api/dev/strategic-map?civilizationId={id}` exposes the Phase 7E read model behind existing development gating and persistence checks.
- Strategic map development contract documentation under `docs/dev/strategic-map-api-contract.md` describes request/response fields, gating behavior, side effects, limitations, and relationship to visual/fleet read models.
- Strategic map projections sanitize foreign owned planet visual intensity details until a real visibility/sensor model exists.
- Read-only map visibility service derives civilization-scoped visibility from current persisted ownership and exploration knowledge: owned planets are `Owned`, systems containing owned planets are `Visible`, other planets in ownership-visible systems are `Visible` but not owned, explored systems are `Visible`, explored planets are `Visible`, and unknown systems/planets keep detail fields hidden.
- Strategic map system and planet DTOs now include derived visibility level, reason, and visibility booleans so development clients can distinguish owned, visible, and unknown relevant map nodes without adding persisted fog-of-war.
- Strategic map system and planet DTOs now include read-only command availability metadata for map view/detail and fleet travel/transfer capability hints. These flags are UI-readiness metadata and do not replace existing command validation.
- Strategic map system and planet DTOs now include read-only exploration preview metadata for unknown nodes while blocking preview for already-visible or owned nodes.
- Strategic map command metadata includes `exploration.mission.create` as a UI-readiness hint only when the target is preview-eligible; visible, owned, or revealed targets block it with `ExplorationPreviewUnavailable`, and the create service remains authoritative for validation.
- Strategic map planet DTOs sanitize unknown planets by keeping detail fields null while preserving stable ids, visibility metadata, preview metadata, and command availability.
- Development-only strategic map action manifest at `GET /api/dev/strategic-map/action-manifest` exposes deterministic metadata for current strategic map, visual state, fleet UI state, exploration preview, and related manifest read actions.
- Strategic map action manifest now includes exploration tooling metadata for preview read, mission create, mission complete-due, and knowledge read actions, including routes, methods, mutability, required fields, success statuses, common error statuses, and safety notes.
- Strategic map action manifest includes `exploration.mission.list` metadata for the development-only mission list read, including optional status filtering.
- Development-only exploration preview endpoint at `GET /api/dev/strategic-map/exploration-preview?civilizationId={id}` exposes read-only exploration readiness metadata derived from map visibility.
- Minimal persistent exploration mission foundation exists with `ExplorationMission`, `ExplorationMissionStatus`, EF mapping, and a migration for planned/completed mission lifecycle state. No creation endpoint, completion worker, visibility reveal, sensors, fog-of-war, route graph, pathfinding, combat, interception, or UI behavior has been added.
- Minimal persistent exploration knowledge foundation exists with `ExplorationKnowledge`, `ExplorationKnowledgeSource`, EF mapping, and a migration for civilization-scoped known systems and optional known planets. Mission completion records it, and map visibility consumes it as read-only visibility.
- Development-only exploration mission creation exists at `POST /api/dev/strategic-map/exploration-missions/create`, creating planned missions only for current exploration-preview-eligible unknown targets with deterministic placeholder due times and no resource cost, fleet assignment, completion, visibility reveal, sensors, fog-of-war, route graph, pathfinding, combat, interception, or UI behavior.
- Development-only exploration mission completion exists at `POST /api/dev/strategic-map/exploration-missions/complete-due`, marking due planned missions completed and recording exploration knowledge that read models can expose as visibility without fog-of-war/sensor persistence, rewards, combat, interception, route graph, pathfinding, background worker, or UI behavior.
- Development-only exploration mission list endpoint exists at `GET /api/dev/strategic-map/exploration-missions?civilizationId={id}&status={optional}`, returning read-only, civilization-scoped planned/completed mission rows in deterministic requested/due/id order without creating, completing, cancelling, or mutating missions.
- Development-only exploration knowledge read endpoint exists at `GET /api/dev/strategic-map/exploration-knowledge?civilizationId={id}`, returning ids-only, civilization-scoped exploration knowledge rows in deterministic discovered/system/planet order without mutating missions, knowledge, visibility, resources, fleets, sensors, fog-of-war, route graph, pathfinding, combat, interception, or UI state.
- Exploration mission lifecycle smoke coverage validates preview -> create planned mission -> complete due mission -> record knowledge -> reveal read-model visibility -> strategic map exposure and preview blocking, while ownership remains unassigned, foreign-owned details stay sanitized, and seeded fleet/resource state remains unchanged.
- Exploration tooling readiness smoke coverage validates preview, mission creation, mission list, due completion, knowledge read, map visibility, strategic map reveal, action manifest metadata, resource/fleet non-mutation, and current no-sensors/no-rewards/no-combat/no-route-graph limitations together.
- Read-only sensor profile service derives civilization-scoped placeholder sensor metadata from active owned planets and stationed orbital groups, using deterministic sensor class, range tier, scan strength, and source-kind values without adding persisted sensor state, visibility reveal, scanner mechanics, or gameplay behavior.
- Read-only detection coverage service derives civilization-scoped, conservative local-system coverage metadata from owned planet and stationed scout sensor context, using deterministic source kind, source system/planet ids, coverage class, range tier, confidence, and limitation notes without revealing hidden targets, creating knowledge, or persisting detection state.
- Strategic map metadata now surfaces sensor profile notes and visible local profile summaries for owned planets and stationed orbital groups, while keeping unknown nodes unrevealed and leaving visibility and command validation unchanged.
- Strategic map metadata now also surfaces detection coverage notes plus visible system/planet coverage summaries derived from owned planets and stationed scout groups, while keeping unknown nodes unrevealed and leaving visibility and command validation unchanged.
- Development-only sensor profile read endpoint at `GET /api/dev/strategic-map/sensor-profiles?civilizationId={id}` exposes derived profile rows for tooling, and the strategic map action manifest now includes `sensor.profile.read`.
- Development-only detection coverage read endpoint at `GET /api/dev/strategic-map/detection-coverage?civilizationId={id}` exposes derived coverage rows for tooling, and the strategic map action manifest now includes `detection.coverage.read`.
- Sensor readiness smoke coverage validates sensor profiles, strategic-map sensor metadata, exploration knowledge visibility, unknown target sanitization, manifest metadata, and resource/fleet/knowledge non-mutation together.
- Detection readiness smoke coverage validates sensor profiles, detection coverage, strategic-map detection metadata, exploration-knowledge visibility, action-manifest metadata, unknown-target sanitization, and no-mutation guarantees together.
- Read-only interception opportunity service derives civilization-scoped metadata for active orbital transfers, surfacing own transfers as self-observed/non-hostile items and exposing only conservatively detected foreign transfers with friendly-context readiness blocking when applicable.
- Strategic map transfer overlays and fleet UI active-transfer summaries now surface read-only interception readiness metadata plus top-level notes, while keeping current transfer behavior, visibility rules, and command validation unchanged.
- Development-only interception opportunity endpoint at `GET /api/dev/strategic-map/interception-opportunities?civilizationId={id}` exposes the interception-readiness read model directly for tooling, and manifest metadata now advertises the action from both the strategic-map and fleet manifest surfaces.
- Interception readiness smoke coverage now validates the current path across detection coverage, interception opportunities, strategic map overlays, fleet UI state, and manifest metadata while proving the reads stay conservative and do not mutate gameplay state.
- Minimal alliance readiness foundation exists with `Alliance`, `AllianceMembership`, status/role enums, EF mapping, and a civilization-scoped read-only query service that returns deterministic alliance membership metadata without granting gameplay permissions, shared visibility, diplomacy automation, pacts, trade, espionage, war, combat, chat, or UI behavior.
- Strategic map now surfaces top-level alliance readiness notes plus requesting-civilization alliance membership metadata, while keeping alliance readiness read-only and preserving the current rule that alliances do not add relevance, reveal allied systems/fleets, share visibility, or share sensor/detection/interception data.
- Development-only alliance readiness read endpoint at `GET /api/dev/strategic-map/alliances/readiness?civilizationId={id}` now exposes civilization-scoped alliance metadata directly for tooling, and the strategic map action manifest now includes `alliance.readiness.read`.
- Alliance readiness smoke coverage now validates direct alliance readiness reads, diplomatic contacts, strategic map metadata, manifest discoverability, conservative visibility, and no-mutation guarantees together.
- Minimal alliance pact readiness foundation exists with `AlliancePact`, pact type/status enums, EF mapping, and a civilization-scoped read-only query service that returns deterministic pact metadata for alliances where the requesting civilization has an active membership. It does not add pact gameplay effects, shared visibility, permissions, trade, war, defense execution, espionage, combat, production endpoints, or final UI.
- Strategic map now surfaces top-level alliance pact readiness notes plus requesting-civilization alliance pact metadata, while keeping pact readiness read-only and preserving the current rule that pacts do not add relevance, reveal allied systems or fleets, share visibility, or share sensor, detection, or interception data.
- Development-only alliance pact readiness read endpoint at `GET /api/dev/strategic-map/alliances/pacts/readiness?civilizationId={id}` now exposes civilization-scoped pact metadata directly for tooling, and the strategic map action manifest now includes `alliance.pact.readiness.read`.
- Pre-frontend checkpoint documentation now consolidates the current development-only backend surfaces, read-only versus mutating tooling routes, readiness limitations, and a recommended first frontend slice in `docs/dev/pre-frontend-contract-checkpoint.md`.
- Strategic map readiness smoke coverage validates that strategic map, visual state, fleet UI state, strategic map action manifest, and exploration preview read surfaces remain coherent and do not mutate stockpiles, orbital groups, or transfers.
- Visibility and command readiness smoke coverage validates owned, foreign-owned, and unknown strategic map nodes across visibility and strategic map read models; verifies command availability for visible nodes and blocked commands for unknown nodes; and protects read-only behavior across systems, planets, ownerships, stockpiles, orbital groups, and transfers.
- Static visual sandbox at `/dev/visual-state/index.html`.
- CSS-only pseudo-3D visual sandbox rendering for planet/system preview, overlays, markers, and transfer routes.
- Static sandbox assets are gated behind the same development switch as development APIs.
- `src/VoidEmpires.Frontend` now provides a minimal Vite + React + TypeScript frontend shell with a conservative app layout, route placeholders for strategic map and fleet inspection, backend base URL configuration through `VITE_VOIDEMPIRES_API_BASE_URL`, and explicit development-only warnings.
- The frontend strategic map page now reads `GET /api/dev/strategic-map?civilizationId={id}`, exposes civilization-id driven loading/error/success states, renders system and planet summaries, and surfaces readiness metadata as informational only.
- The frontend strategic map page now also renders a deterministic, read-only SVG 2D map view that normalizes backend system coordinates into a responsive viewport while keeping the existing summary cards below the visual layer.
- The frontend strategic map page now supports deterministic local selection for systems and planets, allowing read-only inspection of visibility, counts, and command/readiness metadata without calling mutating endpoints.
- The frontend strategic map page now also links selected systems and visible planets to the existing visual-state development endpoints, exposing compact renderer-facing summaries plus raw JSON payload inspection without adding rendering or gameplay mutations.
- The frontend fleet page now reads `GET /api/dev/fleets/ui-state`, `GET /api/dev/fleets/action-manifest`, and `GET /api/dev/strategic-map/action-manifest`, rendering fleet summaries and manifest metadata as read-only inspection panels without wiring gameplay mutations.
- Frontend setup, limitations, current visual map behavior, and smoke validation are now documented in `src/VoidEmpires.Frontend/README.md` and `docs/dev/frontend-foundation-smoke-checklist.md`.
- Phase 9K adds a Figma-derived frontend token foundation in `src/VoidEmpires.Frontend/src/styles.css`, including reusable raw palette variables, semantic color tokens, spacing/radius/elevation scales, and reserved shell layout variables aligned to the `Xuniverse UI v1 - Modern Simple` concept without introducing the final UI.
- Frontend Figma alignment guidance is now documented in `src/VoidEmpires.Frontend/README.md` and `docs/dev/frontend-figma-alignment.md` so later UI tasks can reuse the same color and layout vocabulary.
- Phase 9L aligns the frontend shell with the Figma layout language through reusable `components/ui` primitives, a 64px-style top resource bar, a 230px-style sidebar with Figma navigation labels, safe disabled placeholders for non-implemented sections, and an `AppShell` composition that preserves the existing read-only route behavior.

Current foundation chain:

```text
Identity -> PlayerProfile -> Civilization -> PlanetOwnership -> Economy -> Buildings -> Queues -> Assets -> OrbitalGroups -> OrbitalTransfers -> PlanetVisualState -> SystemVisualState -> VisualSandbox -> DevSandboxGating
```

## Visual State Design Note

Accepted current rules:

- `PlanetVisualStateDto` is a read contract, not a persisted gameplay entity.
- `PlanetVisualProfileDto` describes render hints.
- `PlanetVisualProfileCatalog` differentiates visual behavior by `PlanetType`.
- `PlanetVisualIntensityCalculator` derives deterministic normalized intensities from existing game data.
- `PlanetVisualStateService` derives single-planet visual state from persisted game data.
- `SystemVisualStateService` returns ordered planet visual states for a solar system.
- `SystemVisualStateDto` includes identity, coordinates, star metadata, layout hints, orbital group markers, transfer overlays, and planets.
- `OrbitalGroupVisualMarkerDto` and `OrbitalTransferVisualOverlayDto` are renderer-facing read projections, not command models.
- Transfer overlay progress is a read-time visual approximation from departure/arrival timestamps.
- `/dev/visual-state/index.html` is development tooling, not final game UI.
- Static sandbox assets are not served in Production by default.
- Static sandbox assets are served outside Development only when `VoidEmpires:DevEndpoints:Enabled=true`.

Current intentional limitations:

- no Three.js/Babylon.js implementation
- no real WebGL renderer
- no meshes, shaders, textures, or binary render assets from the backend
- no persisted visual customization model
- no route graph/pathfinding model or persisted fuel inventory/refueling model
- no combat/interception overlay model
- no final game UI layout

## Fleet and Transfer Design Notes

Accepted current rules:

- `OrbitalGroup` represents grouped orbital assets.
- Origin and current planet are intentionally separated.
- Local crew/operator capacity is validated during production, not during parking/stationing.
- `OrbitalTransfer` persists transfer intent, timing, status, origin, and destination.
- Creating a transfer reserves the orbital group.
- Completing a due transfer moves the group to the destination and marks the transfer completed.
- Orbital travel estimates are preview-only read models. They calculate distance, duration, and estimated resource costs without creating transfers, reserving groups, charging resources, mutating stockpiles, or persisting estimates.
- Orbital travel estimates report whether the current planet stockpile can afford estimated costs and identify insufficient resources without spending balances.
- Orbital route profiles are read-only metadata derived from abstract distance units. Current bands are intentionally coarse placeholders: distance `1` is `LocalOrbit`, `2-3` is `InnerSystem`, `4-6` is `OuterSystem`, and `7+` is `LongRange`. Current profiles are supported and use a placeholder fuel multiplier of `1.0`; they do not introduce pathfinding, route graphs, fuel inventory, combat, interception, alliances, or espionage.
- Orbital fuel readiness is a placeholder read model derived from asset type, group quantity, abstract distance, and route profile. It reports estimated fuel units required, estimated range units available, readiness, and not-ready reasons, but it does not add persisted fuel state, refueling, spending, or transfer-creation behavior.
- Creating an orbital transfer charges the estimated travel costs from the current planet stockpile before reserving the orbital group and creating the transfer.
- Cancelling an orbital transfer is explicit, persistent, and only available before completion. Phase 6R cancellation marks the transfer cancelled, releases the reserved orbital group back to stationed status at its current persisted planet, and does not refund charged resources.
- An orbital group with an active transfer cannot be split, merged as a source or target, assigned a second transfer, or used for a new travel estimate preview. Active transfer means a transfer that is neither completed nor cancelled. Completion and cancellation remain the valid lifecycle operations for the active transfer itself.
- Splitting an orbital group is available only for stationed groups owned by the requesting civilization. The split quantity must be positive and lower than the source quantity.
- Merging orbital groups requires different stationed groups owned by the requesting civilization, sharing the same current planet and asset type. The target group quantity increases and the source group is removed.
- Fleet operational overview is read-only. It consolidates orbital group state, active transfer timing/status, and command availability without creating transfers, cancelling transfers, completing transfers, splitting, merging, charging resources, or mutating persisted state.
- Fleet lifecycle smoke tests are xUnit tests over EF in-memory services; the repository integration-test script remains a placeholder and reports no configured integration tests.
- Fleet UI state is read-only development tooling for future UI prototypes. It aggregates existing overview data, resource context for group current planets, and action hints without mutating persisted state.
- Fleet UI state route/fuel readiness hints intentionally do not invent a destination. The travel estimate endpoint remains the source of concrete route profile and fuel readiness previews because it requires `destinationPlanetId`.
- Fleet action manifest is read-only development tooling for future UI prototypes. It lists available dev fleet actions and contracts, including route/fuel preview guidance, but does not replace command validation.
- The current sandbox renders markers and transfer route lines as visual indicators only.
- Strategic map read model is read-only backend preparation for future map UI. Relevance includes owned planets, active transfer origin/destination planets, and exploration-known systems for the requesting civilization. Ownership, fleet details, and detailed planet visual intensity signals from other civilizations are not exposed by this read model.
- Map visibility is a derived read-only projection, not persisted fog-of-war. Phase 7I does not add exploration missions, sensors, known-system persistence, espionage, diplomacy, route graphs, pathfinding, combat, interception, or UI. Until a real knowledge model exists, unknown systems and planets are returned with identifiers and `Unknown` visibility while names, coordinates, star type, planet type, size, colonization status, and orbital slot are hidden.
- Strategic map visibility integration remains annotation-only. Phase 7J preserves the existing strategic-map relevance scope and can annotate already-relevant active-transfer destinations as `Unknown`, but it does not make the strategic map endpoint enumerate every unknown persisted system.
- Strategic map command availability is deterministic and read-only. Phase 7K derives availability from visibility and current requesting-civilization fleet context, blocks unknown/not-visible nodes explicitly, and keeps transfer creation routed through existing fleet command validation.
- Phase 7L adds integrated smoke coverage for the current visibility and command readiness contract without adding gameplay behavior or persistence.
- Phase 7M corrected strategic map travel/transfer command availability to use requesting-civilization fleet context from the returned map rather than only the local system being projected.
- Strategic map exploration preview is deterministic and read-only. Unknown nodes can show `exploration.preview` as available; already-visible and owned nodes are blocked as `AlreadyVisible` or `AlreadyOwned`. It does not create exploration missions, sensors, persisted fog-of-war, scanners, espionage, diplomacy, combat, interception, route graph, or pathfinding state.
- Strategic map action manifest is read-only development tooling for future UI prototypes. It lists strategic map, exploration preview, visual-state, fleet UI state, and manifest read actions with method, route, required fields, success status, common error statuses, and notes.
- Strategic map readiness smoke coverage protects the current limitation that map/readiness contracts do not expose mesh, texture, binary, shader, route graph, pathfinding, combat, or interception payload fields.
- Phase 7Q adds only the persistent exploration mission data model: requesting civilization, target system, optional target planet, requested/due/completed timestamps, and planned/completed status. Exploration preview remains read-only and does not create missions or reveal map visibility.
- Phase 7R adds a minimal creation service and dev-only endpoint for planned exploration missions. System-level placeholder missions are due after 30 minutes, planet-level placeholder missions after 45 minutes, and creation is allowed only when the existing exploration preview marks the target as eligible.
- Phase 7S adds a minimal completion service and dev-only endpoint that completes due planned missions by timestamp. Completion currently closes mission lifecycle state only and intentionally does not reveal visibility or create knowledge/fog-of-war persistence.
- Phase 7T adds lifecycle smoke coverage and documentation for the current preview -> create -> complete flow. The expected behavior remains conservative: completed exploration missions do not reveal targets, create known-system/fog-of-war/sensor state, grant rewards, or mutate fleet/resource state.
- Phase 7U adds only the exploration knowledge persistence foundation: civilization, system, optional planet, discovery source, optional source mission, discovery timestamp, and database uniqueness indexes for system-level and planet-level knowledge. It does not change visibility, mission completion, sensors, scanners, fog-of-war behavior, or UI.
- Phase 7V records exploration knowledge when due planned exploration missions complete. System-target missions record system knowledge; planet-target missions record both system and planet knowledge. Map visibility and strategic map reads do not consume this knowledge until Phase 7W.
- Phase 7W integrates exploration knowledge into map visibility and strategic map relevance. Ownership still has priority; explored systems and planets use existing `Visible` visibility with `ExploredSystem` and `ExploredPlanet` reasons; system-level knowledge does not reveal every planet detail.
- Phase 7X hardens the full exploration reveal lifecycle with smoke coverage and documentation. The validated path proves knowledge recording, visibility consumption, conservative strategic-map exposure, blocked preview after reveal, no ownership leakage, and no resource/fleet/reward mutation.
- Phase 7Y hardens strategic map projection sanitization so unknown planets in explored systems do not leak names, planet types, sizes, colonization status, orbital layout, visual scale, or intensity details.
- Phase 7Z adds the exploration knowledge query service and development-only read endpoint so tooling can inspect currently recorded knowledge rows for a civilization. The endpoint stays ids-only and read-only, with deterministic ordering and standard dev-route gating.
- Phase 8A expands strategic map tooling metadata for exploration preview read, mission create, mission complete-due, and knowledge read actions, and adds `exploration.mission.create` command hints to strategic map systems and planets without changing gameplay behavior.
- Phase 8B adds the exploration mission query service and development-only mission list endpoint with optional status filtering, plus manifest/docs coverage for the new read action.
- Phase 8C adds consolidated smoke coverage for the exploration dev tooling lifecycle across preview, mission create/list/complete, knowledge read, visibility, strategic map reveal, manifest metadata, and non-mutation guarantees.
- Phase 8D adds the read-only sensor profile service foundation for future exploration, detection, espionage, and interception systems. Profiles are derived placeholders only, scoped by civilization, and do not persist sensor state or reveal visibility.
- Phase 8E integrates those placeholder sensor summaries into the strategic map read model as metadata-only notes, per-system/per-planet summaries for visible nodes, and fleet-presence summaries where a stationed group has a derived profile.
- Phase 8F adds the development-only sensor profile read endpoint and manifest metadata so tooling can inspect sensor readiness directly without adding production endpoints or gameplay effects.
- Phase 8G hardens the sensor readiness path with smoke coverage across sensor profiles, strategic map metadata, exploration knowledge visibility, manifest metadata, and no-mutation guarantees.
- Phase 8I integrates detection coverage into strategic-map readiness metadata with top-level notes plus visible system/planet summaries, while preserving current visibility and command behavior.
- Phase 8J adds a development-only detection coverage read endpoint plus strategic-map action manifest metadata so tooling can inspect detection readiness directly without adding gameplay behavior.
- Phase 8K hardens the detection readiness path with integrated smoke coverage across sensor profiles, detection coverage, strategic-map metadata, visibility, manifest metadata, and no-mutation guarantees.
- Phase 8L adds the first interception readiness read model. It remains read-only, civilization-scoped, and conservative: own transfers appear only as self-observed metadata, foreign transfers surface only when both endpoints are already visible and current local-system detection coverage exists, friendly interceptor context is derived from stationed requester fleets, and no interception execution, combat, persistence, or visibility reveal is added.
- Phase 8M integrates interception readiness into the existing strategic map and fleet UI read surfaces. Current transfer overlays and active-transfer summaries can now expose self-observed or blocked readiness notes, top-level interception notes explain that execution is unsupported, and hidden foreign transfers remain omitted from these UI/readiness surfaces.
- Phase 8N adds a development-only interception opportunity read endpoint plus manifest metadata for direct tooling discovery. The endpoint follows current development gating and persistence rules, returns read-only readiness rows only, and does not execute interception or reveal hidden transfer state.
- Phase 8O adds end-to-end smoke coverage for the interception readiness stack. The validated path proves self-observed own-transfer metadata, conservative omission of hidden foreign transfer data, manifest discoverability, and no mutation of transfers, fleets, resources, knowledge, or missions.
- Phase 8T adds a minimal alliance readiness foundation: persisted alliance and alliance membership metadata, conservative validation, a deterministic civilization-scoped read query, and focused tests. It intentionally does not add invitations, permissions, shared visibility, pacts, trade, espionage, war, combat, chat, production endpoints, or final UI.
- Phase 8U integrates that alliance readiness foundation into the strategic map as top-level notes plus requesting-civilization membership metadata only. Diplomatic contacts remain separate metadata, and alliance readiness still does not change relevance, visibility, authorization, or allied data exposure.
- Phase 8V adds a development-only alliance readiness read endpoint plus strategic-map action manifest metadata so tooling can inspect alliance readiness directly without adding production endpoints, gameplay effects, shared visibility, permissions, or allied data exposure.
- Phase 8W hardens the full alliance readiness path with smoke coverage across the query service, diplomatic contacts, strategic map, manifest metadata, conservative visibility, and no-mutation guarantees.
- Phase 8X adds the minimal alliance pact readiness domain foundation: persisted pact metadata between alliances, conservative validation, a deterministic civilization-scoped read query for active alliance participants, and focused domain/persistence/query tests. It intentionally does not add pact gameplay effects, visibility sharing, permissions, trade execution, war, defense automation, espionage, combat, production endpoints, or final UI.
- Phase 8Y integrates that alliance pact readiness foundation into the strategic map as top-level notes plus requesting-civilization pact metadata only. Pact readiness still does not change strategic-map relevance, visibility, authorization, allied data exposure, trade behavior, war state, defense behavior, or combat behavior.
- Phase 8Z adds a development-only alliance pact readiness read endpoint plus strategic-map action manifest metadata so tooling can inspect pact readiness directly without adding production endpoints, shared visibility, permissions, trade behavior, war state, defense behavior, espionage, combat, or final UI.
- Phase 9A adds a documentation-first checkpoint for frontend foundation work, consolidating stable dev/backend contracts, current read-only versus mutating surfaces, readiness limitations, non-goals, and a recommended safe first frontend slice without adding production endpoints or gameplay behavior.
- Phase 9B adds the first frontend project foundation under `src/VoidEmpires.Frontend` with Vite, React, TypeScript, route placeholders for the strategic map and fleet prototype views, a shared backend API base URL configuration, and a visible reminder that the shell consumes development-only backend contracts.
- Phase 9C adds the first frontend strategic map read slice: typed strategic map response handling, a read-only query flow keyed by civilization id, map/system summary rendering, and conservative display of readiness metadata without introducing command execution, auth, backend changes, or 3D rendering.
- Phase 9D adds the first frontend fleet inspection slice: typed fleet UI-state and manifest handling, a read-only fleet query flow keyed by civilization id, grouped fleet summaries, resource/interception notes, and manifest panels that clearly mark mutating actions without executing them.
- Phase 9E documents how to install, run, build, and smoke-check the frontend prototype, updates the pre-frontend contract checkpoint with the current frontend foundation status, and keeps the prototype limitations explicit.
- Phase 9G adds the first visual readiness layer for the frontend strategic map: a simple 2D SVG projection that uses backend coordinates, handles empty and single-system maps safely, shows visibility state directly on map nodes, and preserves the existing list/card inspection view.
- Phase 9H adds a read-only selection detail layer for the frontend strategic map so users can choose a system from the map or list, inspect planet metadata, and review readiness notes without executing gameplay actions.
- Phase 9I connects the frontend strategic map selection flow to the existing visual-state development endpoints with read-only system and planet preview loaders, compact payload summaries, and raw JSON inspection for renderer-facing contracts.
- Phase 9J hardens the current frontend visual map slice with updated README guidance, an explicit smoke checklist for 2D map and visual-state preview behavior, and refreshed frontend checkpoint documentation that keeps the UI framed as development-only readiness tooling.
- Phase 9M aligns the current strategic map and fleet prototype screens with the established Figma token layer: the strategic map now uses a staged map panel, compact legend, grouped selection/detail cards, and cleaner visual-state previews, while the fleet page now uses compact fleet cards, active-transfer progress bars, clearer resource-context framing, and explicit read-only manifest presentation without wiring any mutating actions.
- Phase 9N finalizes the current frontend baseline by tracking `src/VoidEmpires.Frontend/package-lock.json`, keeping `node_modules` and `dist` untracked, and updating the README/checkpoint docs so install, build, smoke, and read-only UI expectations stay deterministic after the Figma alignment work.

## Dev Surface Gating Note

The same switch controls development APIs and static sandbox files.

Accepted current rules:

- Development environment serves dev APIs and `/dev/visual-state/*` static files.
- Production environment does not serve `/dev/visual-state/*` by default.
- Production serves those files and development API routes only when `VoidEmpires:DevEndpoints:Enabled=true`.
- The visual sandbox and assets remain development tooling.

## Validation Status

Run from repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validated baseline after Block 36A-36O:

- backend: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors
- tests: `dotnet test --no-build` succeeded with `719` passing tests, `0` failed, and `0` skipped
- frontend: `npm run build --prefix src/VoidEmpires.Frontend` succeeded
- frontend bundle baseline: current Vite output emits `103` transformed modules, one `180.28 kB` minified shared JS entry chunk (`58.77 kB` gzip), one `54.21 kB` CSS asset (`8.22 kB` gzip), and cockpit-specific async chunks for Onboarding, Galaxy, Planet, Construction, Research, Shipyard, Fleets, Defenses, Ground Army, Espionage, Alliance, Market, Ranking, and the module placeholder route
- persisted QA scripts: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded
- frontend lazy-import guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded
- frontend copy regression guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded
- validation note: `check-dev-qa-scripts.ps1` also invoked the frontend lazy-import and copy-regression guards successfully before its PowerShell parser, resource-format, payload, Shipyard, Fleet, and known no-op helper checks passed
- tooling note: `dotnet build --no-restore` and `dotnet test --no-build` reported available .NET workload updates; this is informational and did not fail validation
- visual QA note: no browser, screenshot, or manual visual QA was performed during this Block 36 validation pass; the deferred master checklist remains in `docs/dev/deferred-visual-qa-master-checklist.md`
- Block 37BM no-gameplay-regression validation: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `720` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `105` transformed modules and a `181.33 kB` minified / `59.14 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, and `check-frontend-copy-regressions.ps1` succeeded.
- Block 37BM scope note: this was a non-visual validation pass for resource, queue, fake frontend mutation, lazy-route, copy, and local-session guardrails; no browser screenshots, final database/model acceptance, final asset acceptance, combat readiness, final movement readiness, market transaction readiness, alliance mutation readiness, or production-auth readiness were claimed.
- Block 37BO final validation pass: `dotnet build --no-restore` succeeded with `0` warnings and `0` errors; `dotnet test --no-build` succeeded with `720` passing tests, `0` failed, and `0` skipped; `npm run build --prefix src/VoidEmpires.Frontend` succeeded with `105` transformed modules, cockpit lazy chunks, and a `181.33 kB` minified / `59.14 kB` gzip shared entry chunk; `check-dev-qa-scripts.ps1`, `check-frontend-route-lazy-imports.ps1`, and `check-frontend-copy-regressions.ps1` succeeded.
- Block 37BO scope note: .NET workload update notices were informational only, no old Vite `500 kB` chunk warning appeared, and no browser screenshots, final database/model acceptance, final asset acceptance, combat readiness, final movement readiness, market transaction readiness, alliance mutation readiness, or production-auth readiness were claimed.
- Block 37BP task-closure note: completed or superseded frontend shell and cockpit task files `TASK-37C` through `TASK-37Z` were moved from pending to done, leaving only final closure/prep `TASK-37BQ` through `TASK-37BZ` in pending for this block.
- Block 37BQ near-product closure note: recent pushed closure commits cover `TASK-37B`, `TASK-37BE` through `TASK-37BP`, with the latest full validation still at `720` passing tests, `0` failed, `0` skipped; deferred items remain final database/model consolidation, final generated assets, final browser/screenshot QA, production auth, combat, final movement productization, market transactions, and alliance mutations.
- Block 37BR final DB phase prep note: `docs/dev/product-readiness-report.md` now records the final DB/model phase entry criteria, ownership decisions, seed/migration sequence, and validation gates while keeping the current shell Development-only and without adding migrations or schema changes.
- Block 37BS final assets phase prep note: `docs/dev/product-placeholder-asset-contract.md` now records the placeholder-to-final asset replacement criteria, asset-set sequence, manifest validation gates, lazy-loading constraints, and screenshot QA dependency without adding generated asset files.
- Block 37BT final corrections phase prep note: `docs/dev/deferred-visual-qa-master-checklist.md` now records the post-DB/post-asset correction entry criteria, screenshot review loop, finding classes, validation gates, and acceptance boundaries without performing browser or screenshot QA.
- Block 37BU README/demo update note: `README.md` now points readers to the canonical near-product local demo guide and summarizes the Development-only caveats, explicit-id route context, guarded mutation boundary, read-only route boundaries, and deferred product dependencies.
- Block 37BV repository cleanliness note: tracked-file scans found no committed build artifacts, screenshots, captures, archives, temp files, or tracked frontend output; machine-local dev-doc links were normalized, an old completed-task private-style connection string example was redacted to placeholders, and full validation passed with `720` tests plus frontend build and guard scripts.
- Block 37BW risk-register note: `docs/dev/final-risk-register.md` now tracks remaining product risks for production auth, final DB/model, catalog/assets, visual acceptance, gameplay overclaims, movement, market, alliance, combat, seed reuse, and repository hygiene without adding gameplay or production behavior.
- Block 37BX acceptance-criteria note: `docs/dev/final-acceptance-criteria.md` now defines the accepted near-product Development shell state, required validation, explicit non-acceptance list, and future handoff rule before final DB/assets and product systems.
- Block 37BY orchestrator-summary note: `docs/dev/final-orchestrator-summary.md` now provides future chats with the current Development-only shell state, accepted boundaries, exclusions, core commands, warnings, and likely next phases.
- Block 37BZ final closure note: pending Block 37 work is closed with `ai/tasks/pending` reduced to `.gitkeep`; all `TASK-37*` files are expected in `ai/tasks/done` after this task is moved, `main` is pushed, the latest full validation baseline remains the `720` passing-test / `105` transformed-module Block 37BO-BV baseline, and final DB/model consolidation, final assets, browser screenshot acceptance, production auth, combat, final movement, market transactions, and alliance mutations remain deferred.
- Block 37BJ documentation update: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded; no browser, screenshot, frontend build, backend build, or backend test pass was performed for this docs-only update.

Previous validated baseline after Block 33A-33O:

- backend: `dotnet build --no-restore` succeeded with `8` transient `MSB3026` copy-retry warnings and `0` errors while `testhost` still held test output DLLs
- tests: `dotnet test --no-build` succeeded with `707` passing tests, `0` failed, and `0` skipped
- frontend: `npm run build --prefix src/VoidEmpires.Frontend` succeeded
- frontend bundle baseline: current Vite output emits `100` transformed modules, one `180.59 kB` minified shared JS entry chunk (`58.88 kB` gzip), one `51.59 kB` CSS asset (`7.88 kB` gzip), and cockpit-specific async chunks for Onboarding, Galaxy, Planet, Construction, Research, Shipyard, Fleets, Defenses, Ground Army, Espionage, Alliance, Market, Ranking, and the module placeholder route
- frontend lazy-import guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeded
- frontend copy regression guard: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeded
- persisted QA scripts: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` succeeded
- validation note: `check-dev-qa-scripts.ps1` also invoked the frontend lazy-import and copy-regression guards successfully before its PowerShell parser, resource-format, payload, Shipyard, Fleet, and known no-op helper checks passed
- tooling note: both `dotnet build --no-restore` and `dotnet test --no-build` reported available .NET workload updates; this is informational and did not fail validation
- technical regression note: the standard non-visual Alliance follow-up pass kept both `check-frontend-route-lazy-imports.ps1` and `check-dev-qa-scripts.ps1` green without reopening any accepted cockpit route behavior
- frontend note: the old Vite `500 kB` chunk-size warning is no longer present after the route-lazy-loading pass, but accepted cockpit route QA remains required because the block changes loading architecture rather than gameplay behavior
- build note: this Block 33 validation run did emit the previously documented transient `MSB3026` copy-retry warnings, but the build still completed successfully
- orbital-preparation note: the repeated Shipyard QA preparation command is `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-orbital-production-ui-state.ps1`
- Manual visual QA for the accepted cross-cockpit demo flow remains a documented seeded-browser pass through `docs/dev/frontend-foundation-smoke-checklist.md` and the cockpit-specific checklists; it was explicitly not performed during this Block 33 non-visual validation pass, so final screenshot-style acceptance is still user-driven.
- Market visual and read-only polish is now implemented and documented through the seeded browser checklists; final screenshot-backed acceptance still remains user-driven, and the block did not expand Market into transaction gameplay or production behavior.
- Alliance read-only diplomacy validation is now implemented and documented through the seeded browser checklists; final screenshot-backed acceptance still remains user-driven, and the block did not expand Alliance into executable diplomacy gameplay.
- Block `28Q-28Z` visual QA corrections are now represented in checklist/docs: copy/fallback issues in Ranking, Alliance, and Market were corrected in code; final screenshot-backed acceptance for this corrective cycle remains pending manual validation by the user.
- state note: the Ranking cockpit is now part of the standard lazy-loaded read-only route suite, and Galaxy or Market or Espionage or Alliance remain read-only after the Ranking rollout.
- closure note: Block `28A-28P` is now closed, `ai/tasks/pending` is reduced to `.gitkeep`, and the accepted cockpit suite keeps `/ranking` inside the standard non-visual regression bundle without changing the non-public ranking boundary.
- closure note: Block `32A-32P` is now closed, `ai/tasks/pending` is reduced to `.gitkeep`, and the accepted playable-session foundation remains Development-only with explicit id handoffs rather than production-auth session resolution.

Current validated cockpit QA seed baseline:

- `POST /api/dev/seeds/apply` supports `minimal-validation`, `cockpit-validation`, `shipyard-validation`, `fleet-validation`, `research-validation`, and `planet-full-validation`.
- `GET /api/dev/seeds/profiles` exposes the current profile catalog for Development-only discovery.
- Standard manual QA should start from the documented seed profiles rather than ad hoc SQL.
- Seeded cockpit QA and real account registration are separate paths. `/register` is the product entry, `/onboarding` is only a compatibility alias to registration, and normal account navigation resolves the current world from backend-backed account state rather than from `cockpit-validation` ids.
- SQL Server uses the same Identity tables and account-bootstrap workflow after the baseline schema is manually applied, but the repository still does not claim a completed manual SQL Server registration run.
- The documented canonical seeded Galaxy QA path is `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`, while `/?...` remains a compatibility alias.
- `cockpit-validation` now restores a non-empty, focusable, read-only Galaxy baseline alongside the accepted Planet, Construction, Research, Ground Army, Shipyard, Fleets, Market, Defenses, Espionage, Alliance, and Ranking cockpit flows.
- Reapplying richer seed profiles after manual QA queue activity is now supported without colliding on persisted queue `Sequence` uniqueness.
- The real persisted Construction enqueue path is now covered through backend tests, backend-only helper scripts, and the central persisted-flow runbook.
- The real persisted Construction enqueue path is now also reflected in the frontend `/construction` cockpit with explicit confirmation, backend-confirmed Spanish success/failure copy, authoritative refresh behavior, secondary diagnostics for raw backend detail, and user-driven visual QA still pending final manual execution.
- The real persisted Research enqueue path is now covered through backend tests, backend-only helper scripts, and the central persisted-flow runbook.
- The real persisted Research enqueue path is now also reflected in the frontend `/research` cockpit with Development-only scope, explicit confirmation, backend-confirmed immediate resource deduction, authoritative refresh after success, disabled automatic completion, and final visual QA still user-driven through the seeded browser checklists.
- Research enqueue UX is technically accepted for the current Development-only persisted boundary.
- Manual QA has already validated the open-order no-op path on `/research`, including the guarded confirmation flow surfacing the honest backend rejection when a reused Development database already contains an open research order.
- Manual success-path QA for `/research` was blocked on the reused Development database because additive seed reapply preserves the existing open research order instead of clearing it.
- A Development-only research QA preparation endpoint and helper script now exist so repeated manual success-path QA can clear only the targeted civilization blocker and re-top only the targeted source-planet stockpile without changing production gameplay behavior.
- Visual/manual QA of the actual `/research` success enqueue remains user-driven after running `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-research-ui-state.ps1`.
- The real persisted Shipyard enqueue path is now covered through backend tests, the backend-only Shipyard helper script, and the Shipyard/Fleet companion runbook.
- Fleet read-state after Shipyard enqueue is now covered through backend tests, the backend-only Fleet read helper script, and the Shipyard/Fleet companion runbook.
- Fleets and Planet remain the accepted neighboring read-state surfaces for this block: Fleets is used only for post-Shipyard readiness verification, and Planet remains a summary/handoff cockpit rather than a new orbital or military mutation surface.
- Current verified resource rule: both Construction and Research deduct the full visible cost immediately when enqueue succeeds.
- Current verified resource rule: Shipyard enqueue also deducts the full visible cost immediately when enqueue succeeds, while local orbital stock stays unchanged until due processing.
- `cockpit-validation` now preserves manual Construction and Research orders created during QA while keeping the deterministic read-model baseline intact.
- `cockpit-validation` now also preserves real manual Shipyard production orders created during QA, avoids duplicating seeded completed Shipyard history on reapply, and keeps Shipyard UI-state readable on reused Development databases.
- Stock-to-fleet allocation remains intentionally excluded from the accepted persisted Shipyard/Fleet QA loop because `POST /api/dev/fleets/orbital-groups/create-from-stock` is a real non-idempotent mutation and is not yet hardened for reused-database automation.
- `cockpit-validation` now also supports meaningful Market QA through seeded reserves, selected-planet production, advisory ratios, visible trade signals, disabled future actions, and deterministic `/market` routing without introducing transaction gameplay.
- `cockpit-validation` now also seeds meaningful Defenses readiness through a visible `DefenseGrid` on `Aurelia` while keeping defense queue completion and combat behavior out of scope.
- `cockpit-validation` now also seeds meaningful Ground Army readiness through a visible `Barracks`, one deterministic available `PatrolGroup` path, blocked comparison options, and completed planetary training history on `Aurelia` while keeping combat, invasion, and complete-due execution out of scope.
- `cockpit-validation` now also seeds meaningful Espionage readiness through shared `Helios Gate` visibility, owned `Aurelia`, visible comparison targets, at least one transfer-derived passive signal, Spanish-first visible copy, disabled future mission cards, and collapsed diagnostics while keeping missions, sabotage, infiltration, counter-espionage, combat, WebSockets, and production auth out of scope.
- `cockpit-validation` now also seeds meaningful Alliance readiness through `Void Seed Civilization`, `Aurelia` homeworld context, one deterministic diplomatic contact row, disabled future pact and diplomacy placeholders, and deterministic `/alliance` routing while keeping alliance, pact, invitation, role, treasury, and messaging gameplay out of scope.
- `cockpit-validation` now also seeds meaningful Ranking readiness through deterministic `/ranking` routing, a non-zero power summary, category breakdowns, demo comparison rows, disabled future placeholders, and visible handoffs while keeping public ladders, rewards, matchmaking, and profiles out of scope.
- For repeated `/construction` manual QA on reused Development databases, run `.\scripts\dev-qa-prepare-construction-ui-state.ps1` to clear scoped blocking orders and top-up scoped resources before repeating enqueue operations, then use either `.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed` or the frontend `/construction` confirmation flow.
- For repeated `/research` manual QA on reused Development databases, run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-research-ui-state.ps1` before retrying either the backend helper or the frontend `/research` confirmation flow.
- For repeated `/shipyard` manual QA on reused Development databases, run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-orbital-production-ui-state.ps1` before retrying either the backend helper or the frontend `/shipyard` confirmation flow.
- Visual QA for modal behavior, registration/login navigation, and cross-cockpit navigation remains documented but deferred to a user-driven browser pass; the current repository validation is non-visual.

Recent expected coverage includes orbital groups, orbital transfers, workers, visual state services/endpoints, system layout hints, markers, transfer overlays, static sandbox asset serving, overlay sandbox hooks, static sandbox gating behavior, fleet UI state service, fleet action manifest service, the strategic map read model, the strategic map development endpoint, the map visibility read model, exploration preview readiness, the minimal exploration mission lifecycle, the current Planet or Construction cockpit readability baseline, the minimal-validation Research seed readiness path, the development Research UI-state endpoint baseline, the full seeded Research enqueue smoke path through queue refresh, the development Shipyard UI-state endpoint baseline, the scoped Shipyard enqueue endpoint path, and the strengthened minimal-validation Shipyard seed expectations.

## Recommended Next Work

1. Deepen movement only after deciding whether route graphs, pathfinding, persisted fuel inventory, or refueling are required.
2. Consider whether exploration knowledge should later evolve into a richer fog-of-war model with separate known/scanned/sensored states.
3. Add combat/interception foundations only after fleet movement and visibility contracts stabilize.
4. Start a real renderer spike only after the visual state contract remains stable.
5. Keep `XUniversePlanet Generator Variator` as an external/local prototype reference until the renderer/prototype phase needs it.

## Constraints

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid production authorization hardening, account recovery/confirmation productization, deployment, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- avoid full UI/3D implementation until the required read contracts and development validation endpoints exist
- keep private operational configuration out of version control
