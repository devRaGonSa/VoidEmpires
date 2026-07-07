# Final Acceptance Criteria Before DB And Asset Phases

Status date: 2026-07-07

These criteria define the accepted near-product state for the current Development-only shell after Block 42 account registration and initial world bootstrap work. They do not grant production readiness, final database/model readiness, final generated asset readiness, browser screenshot acceptance, final authorization readiness, combat readiness, final movement readiness, market readiness, or alliance readiness.

## Accepted Near-Product State

The current shell is accepted for local Development demo when all of these are true:

1. `docs/dev/single-product-demo-guide.md` is the canonical demo path.
2. The demo runs against a local or disposable Development database.
3. Development seed/profile helpers are explicit and do not run automatically on startup.
4. `/register` creates an account-backed playable start, logs in through the Identity cookie flow, and returns explicit generated world ids; `/onboarding` remains only a compatibility alias to registration.
5. Cockpit routes use backend-backed current account state plus visible `civilizationId` and `planetId` context, with local browser memory only as navigation assistance.
6. Planet remains the hub for selected colony context, backend-owned resources, buildings, queues, handoffs, and secondary Development QA tools.
7. Construction, Research, and Shipyard create real Development database orders only after explicit confirmation and backend refresh.
8. Due-queue materialization and resource accrual remain explicit Development QA actions.
9. Galaxy, Defenses, Ground Army, Market, Espionage, Alliance, and Ranking remain read-only or readiness/advisory surfaces unless a separate accepted mutation path exists.
10. Diagnostics remain secondary and do not replace primary gameplay state.
11. Lazy cockpit route loading remains protected by the route import guard.
12. Spanish-first visible copy remains protected by the copy regression guard.
13. No build artifacts, screenshots, generated assets, secrets, or local machine paths are committed.

## Required Validation For This State

Before claiming the accepted near-product Development shell:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```

## Explicit Non-Acceptance

The current state is not accepted for:

- final authorization/ownership enforcement for product traffic beyond the current guarded account route foundation
- account recovery/confirmation product UX, production deployment hardening, or multi-civilization account selection
- final database/model consolidation or product migration readiness
- final generated or curated image/asset acceptance
- screenshot-backed browser/visual acceptance
- combat, invasion, interception execution, or defense mitigation
- final fleet movement productization
- market buying, selling, auctions, player trading, or trade-route execution
- alliance creation, invitations, pacts, roles, treasury, messaging, or shared visibility
- public ranking ladders, rewards, matchmaking, or public profiles

## Handoff Rule

Any future task that claims one of the non-accepted areas must update this document, `docs/dev/product-readiness-report.md`, and `docs/dev/final-risk-register.md` with the new validation evidence and residual risks.
