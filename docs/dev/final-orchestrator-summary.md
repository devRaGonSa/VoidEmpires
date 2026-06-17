# Final Orchestrator Summary

Status date: 2026-06-17

Use this as the quick handoff for future chats after Block 37 near-product closure work.

## Current State

- VoidEmpires has a coherent Development-only local product shell.
- The canonical demo guide is `docs/dev/single-product-demo-guide.md`.
- The current accepted criteria are in `docs/dev/final-acceptance-criteria.md`.
- Remaining risks are in `docs/dev/final-risk-register.md`.
- Product readiness and validation history are in `docs/dev/product-readiness-report.md`.
- The current shell is accepted for local demo only after the documented validation commands pass.

## Accepted Development Shell

- `/onboarding` creates a Development playable start and returns explicit ids.
- Planet is the hub for selected colony context, backend-owned resources, buildings, queues, handoffs, and secondary Development QA tools.
- Construction, Research, and Shipyard are guarded persisted Development mutation surfaces.
- Due-queue materialization and resource accrual remain explicit Development QA actions.
- Galaxy, Defenses, Ground Army, Market, Espionage, Alliance, and Ranking stay read-only or readiness/advisory unless a separate task accepts a narrower mutation path.
- Diagnostics stay secondary; raw ids and payloads belong in diagnostics or docs.

## Exclusions

Do not claim or add these without explicit future tasks and validation:

- production auth, login/logout, active civilization resolution, or final authorization
- final database/model consolidation, migrations, or product seed readiness
- final generated or curated assets
- browser screenshot acceptance or final visual QA
- combat, invasion, interception execution, or defense mitigation
- final fleet movement productization
- market buying, selling, auctions, player trading, or trade-route execution
- alliance creation, invitations, pacts, roles, treasury, messaging, or shared visibility
- public ranking ladders, rewards, matchmaking, or public profiles

## Core Commands

Near-product validation:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```

Local demo guide command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-playable-loop-guide.ps1
```

## Warnings For Future Tasks

- Keep backend reads authoritative for resources, queues, stock, buildings, research, rankings, and readiness.
- Do not fabricate optimistic frontend state for backend-owned gameplay.
- Do not enable hidden mutations from page load, route entry, sidebar navigation, or ordinary card selection.
- Use a fresh disposable Development database or the documented QA preparation helpers when reused local data blocks demo success paths.
- Preserve lazy-loaded cockpit routes and rerun the lazy-import guard after frontend route changes.
- Keep visible UI copy Spanish-first and rerun the copy regression guard after copy or docs changes.
- Do not commit screenshots, generated assets, local build outputs, secrets, real connection strings, or local machine paths.

## Next Likely Phases

1. Final DB/model consolidation and catalog seed validation.
2. Final generated/curated asset manifest and placeholder replacement.
3. Post-DB/post-asset browser screenshot QA and corrections.
4. Production auth and active civilization resolution.
5. Combat, movement productization, market transactions, and alliance mutations as separate backend-owned systems.
