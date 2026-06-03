# TASK-17L-shipyard-controlled-production-enqueue-flow

---
id: TASK-17L-shipyard-controlled-production-enqueue-flow
title: Shipyard controlled production enqueue flow
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Enable controlled Shipyard production enqueue only if the backend already supports it safely or can be extended with a development-only endpoint plus tests.

## Purpose
Make Shipyard v1 actually playable in development while preserving explicit confirmation, validation, and safe module boundaries.

## Current Problem
The core Shipyard action is to enqueue orbital asset production. That action must not exist unless the backend contract is real, tested, and clearly development-only. If safe support does not exist, the UI should remain disabled and explain the limitation in Spanish rather than pretending.

## Context
- Controlled confirmation patterns already exist in related cockpits.
- Asset production services, dev endpoints, and resource spending logic may already exist in the repo.
- Shipyard must not mutate Fleets, add movement, or bypass resource validation.

## Files to Inspect First
- Existing asset production enqueue endpoint or service, if present
- Relevant dev endpoint tests for asset production
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Shipyard API client files
- Production queue and resource-spend services

## Implementation Requirements
1. Inspect whether a safe asset production enqueue endpoint already exists.
2. If it exists and is covered well enough, wire the frontend to that endpoint.
3. If it does not exist but the underlying service does and a development-only endpoint is safe to add, add that endpoint plus tests.
4. If neither path is safe, keep the action disabled and explain why in Spanish.
5. Available production action must open confirmation before submit.
6. The confirmation must show:
   - planet;
   - asset;
   - quantity;
   - cost;
   - duration;
   - readiness or requirements summary;
   - `Confirmar produccion`;
   - `Cancelar`.
7. Cancel must never mutate.
8. Confirm must call only the approved dev-safe endpoint.
9. After success, refresh Shipyard state so queue and availability become visible.
10. On failure, show a specific Spanish error rather than a generic technical blob.
11. Add tests for invalid ids, ownership failure, insufficient resources, invalid asset type, and one seeded success path if backend support is enabled.

## UI/UX Requirements
- The primary action should read like a review-then-confirm flow.
- Success copy should be concise, such as `Produccion enviada a la cola.`
- Errors should stay in Spanish and should not expose raw payloads as the primary message.

## Backend/API Requirements
- Any mutation endpoint must be development-only.
- Do not add auth requirements for this dev cockpit.
- Reuse existing validation and service layers rather than creating a parallel production path.

## Safety Constraints
- No production auth.
- No combat, movement, split, or merge behavior.
- No optimistic local queue insertion.
- No mutation against unsupported or global unsafe endpoints.

## Expected Files to Modify
- `src/VoidEmpires.Web/` Shipyard or asset dev endpoint files if needed
- `src/VoidEmpires.Application/Assets/` or `src/VoidEmpires.Infrastructure/Assets/` only if safe endpoint wiring requires it
- `tests/VoidEmpires.Tests/` relevant dev endpoint and service tests
- `src/VoidEmpires.Frontend/src/api/` Shipyard client files
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`

## Acceptance Criteria
- One seeded available production can be confirmed and enqueued safely, or the action remains truthfully disabled if backend support is unavailable.
- Build, test, and frontend build pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- This is the core Shipyard v1 mutation task and should stay tightly scoped.
- If the backend can enqueue but cannot yet expose a clean post-submit read state, note that clearly and keep follow-ups narrow.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- If enabling enqueue safely requires larger API work, split follow-up tasks instead of broadening this one silently.
