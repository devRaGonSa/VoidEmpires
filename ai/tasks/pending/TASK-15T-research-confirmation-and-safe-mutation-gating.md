# TASK-15T-research-confirmation-and-safe-mutation-gating

---
id: TASK-15T-research-confirmation-and-safe-mutation-gating
title: Research confirmation and safe mutation gating
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Add explicit confirmation and safe gating for Research enqueue actions only when the backend supports a safe development path.

## Purpose
Research mutations can affect progression state, so the UI must never make them feel automatic or hidden.

## Current Problem
If the page shows action buttons without confirmation or without a safe backend route, it can mislead the user into thinking unsupported mutation is available.

## Context
- The research action must remain explicit-confirmation-based.
- If safe backend support is missing, the control must remain disabled and clearly explained.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- Existing Construction mutation confirmation patterns

## Implementation Requirements
1. Inspect the available Research mutation support.
2. If a safe Development-only action exists, expose it with explicit confirmation.
3. If not, keep the action disabled and explain why in Spanish.
4. Confirmation flow should clearly state what will happen before mutation occurs.
5. Do not expose mutation through a silent card click.
6. Refresh the Research UI state after a successful mutation only if the backend confirms success.

## UI/UX Requirements
- Primary button labels should be Spanish and action-oriented.
- The confirmation title should be clear and direct.
- Unsupported actions should look intentionally disabled, not broken.
- Technical payloads belong in diagnostics only.

## Backend/API Requirements
- If an endpoint is added or used, it must be Development-only and tested.
- Invalid request, not found and conflict cases should be covered where relevant.
- No production endpoint.

## Safety Constraints
- No hidden tech effects.
- No direct mutation from blocked cards.
- No optimistic local queue unless the backend confirms it.
- Do not enable unsafe background behavior.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- Relevant tests if a backend route is added or changed

## Acceptance Criteria
- Available research can be confirmed only if safe backend support exists.
- Blocked research cannot mutate.
- The UI explains disabled actions clearly.
- Build and tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If backend support is incomplete, disabling the action is the correct outcome.
- The safer choice is better than a speculative mutation surface.
- Confirmations should not be skippable by default.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer one safe mutation path or none at all.
