# TASK-16Q-research-enqueue-payload-contract-audit

---
id: TASK-16Q-research-enqueue-payload-contract-audit
title: Research enqueue payload contract audit
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: high
---

## Goal
Identify the exact mismatch between the frontend enqueue payload and the backend Research enqueue contract.

## Purpose
The Research cockpit now correctly shows at least one available item and opens confirmation, but the backend still rejects the enqueue request by validation. Before changing any code, we need to know which contract field is wrong and why.

## Current Problem
The UI shows `Ingenieria planetaria` as available and the confirmation panel treats it as ready to submit, but the backend rejects the request with a generic validation failure. This suggests a mismatch in one or more of the following:

- wrong technology id or key;
- wrong target level;
- wrong civilization id;
- wrong planet id;
- wrong endpoint;
- missing command metadata;
- stale capability token or unavailable action state.

## Context
- The read model is no longer the obvious problem.
- The frontend can render the available card and open confirmation.
- The failure happens at submit time, not at display time.
- The task is diagnostic-first and should leave a clear trail for follow-up work.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Infrastructure/Research/`
- `src/VoidEmpires.Web/`
- `tests/VoidEmpires.Tests/`

## Implementation Requirements
1. Trace the exact request emitted by the frontend when confirming `Ingenieria planetaria`.
2. Compare that request to the backend enqueue endpoint contract.
3. Identify the required fields, including:
   - `civilizationId`;
   - `planetId`, if required;
   - technology id/key/type;
   - target level;
   - `requestedAt` or `dueAt`, if relevant;
   - any action or capability id.
4. Determine whether the frontend is accidentally using a display label instead of a stable backend key.
5. Determine whether the backend expects a different route or JSON shape.
6. Add a short developer note in `docs/dev/research-cockpit-checklist.md` or a narrow research API note with the real request contract.
7. If the fix is small and safe, it may be included here with tests.

## UI/UX Requirements
- No visual redesign in this task.
- If a user-facing error message can be improved safely, keep it Spanish and short.
- Do not change card layout or cockpit hierarchy as part of the audit.

## Backend/API Requirements
- Do not weaken validation.
- Do not make the backend accept invalid payloads just to satisfy the UI.
- The backend and the read model must agree on the command contract.

## Safety Constraints
- No production endpoint.
- No hidden mutation behavior outside the normal enqueue path.
- No research effects.
- Do not add speculative payload fields without evidence.

## Expected Files to Modify
- `docs/dev/research-cockpit-checklist.md` or a narrow Research API note.
- Backend or frontend files only if the audit reveals a small, safe fix.
- Tests only if any code changes are made.

## Acceptance Criteria
- The exact root cause of the validation rejection is known.
- The task leaves a clear code or documentation trail.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched.

## Notes / Residual Risks
- Do not hide the problem behind a generic catch-all error.
- If there are multiple candidate mismatches, document them in priority order and identify the most likely primary one.
- Keep the investigation focused on the seeded `minimal-validation` flow.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer documentation plus pinpoint tests over broad refactors.
