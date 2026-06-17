# Final Risk Register

Status date: 2026-06-17

This register tracks the main product risks that remain after the current Development-only product shell. It does not grant production readiness, visual acceptance, final database/model readiness, final asset readiness, combat readiness, final movement readiness, market readiness, alliance readiness, or production-auth readiness.

## Risk Summary

| Risk | Current status | Impact | Mitigation / next decision |
|---|---|---|---|
| Production auth gap | Registration and email confirmation exist, but cockpits still use explicit Development ids and local navigation memory. | Users could misread the local playable start as account-backed access if the boundary is weakened. | Implement login/logout, active civilization resolution, and server-side authorization before any production cockpit claim. |
| Final DB/model gap | Current persistent gameplay rows support the Development loop, while catalog metadata and final ownership boundaries are still being prepared. | Schema drift or seed assumptions could make demo behavior look more final than it is. | Follow `docs/dev/product-readiness-report.md` final DB phase prep before adding migrations or product seed data. |
| Catalog/asset drift | Placeholder asset keys and frontend labels are documented separately from future final catalog metadata. | Final images or labels could drift from backend keys, availability, or state. | Require manifest/key drift validation before replacing `PlaceholderAsset` output. |
| Visual acceptance gap | Browser screenshot QA is documented but not complete. | Layout, overflow, hierarchy, and mobile regressions can remain hidden after non-visual validation. | Execute `docs/dev/deferred-visual-qa-master-checklist.md` after DB/assets are stable and record corrections as follow-up tasks. |
| Gameplay overclaim risk | Defenses, Ground Army, Market, Espionage, Alliance, and Ranking are readiness/read-only surfaces. | UI copy or imagery could imply executable systems that do not exist. | Keep disabled/future actions secondary and fail QA if copy claims combat, transactions, missions, alliance mutations, public ladders, or rewards. |
| Fleet movement productization gap | Fleet tooling supports controlled Development transfer paths and previews, but final route graph/fuel/pathfinding/product rules remain undecided. | Movement behavior can be inconsistent if promoted without final rules and authorization. | Decide route graph, fuel, refueling, pathfinding, arrival, cancellation, and audit contracts before product movement acceptance. |
| Economy transaction gap | Market shows advisory economy state only. | Resource balances could be compromised if transaction UI is enabled without authoritative backend settlement. | Keep buy/sell/auction/player trade/route execution disabled until transaction services, validation, and tests exist. |
| Alliance mutation gap | Alliance cockpit reads identity/contact/readiness state only. | Permissions, shared visibility, treasury, and messaging can become unsafe if mutation UI appears first. | Implement alliance creation, invitations, roles, pacts, treasury, messaging, and authorization as explicit backend-owned workflows. |
| Combat dependency gap | Combat, invasion, interception execution, defense mitigation, and battle audit rules remain deferred. | Military readiness surfaces could be mistaken for resolved outcomes. | Design deterministic combat inputs, resolution, persistence, replay/audit, and visibility effects before executable combat UI. |
| Development seed reuse risk | Seed profiles are additive and preserve manual QA mutations. | Reused local databases can show blocked queues or unexpected resource state during demo. | Use documented QA preparation helpers or a fresh disposable Development database for clean demonstrations. |
| Repository hygiene regression | Ignored local outputs and generated frontend build artifacts exist in local workspaces after validation. | A later task could accidentally stage outputs, screenshots, or local-only files. | Keep `git diff --name-only`, artifact scans, and copy guard in closure tasks before committing. |

## Review Triggers

Update this register when any future task:

1. Adds a final DB migration or product seed path.
2. Replaces placeholders with generated or authored assets.
3. Claims browser/screenshot acceptance.
4. Introduces production auth or active civilization resolution.
5. Enables combat, movement productization, market transactions, or alliance mutations.
6. Changes seed profile behavior or QA reset/preparation semantics.

## Current Validation

Static validation for this documentation task:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```

No browser, screenshot, DB migration, final asset generation, production-auth integration, combat, final movement, market transaction, or alliance mutation validation was performed for this register.
