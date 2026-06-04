# TASK-24L

---
id: TASK-24L
title: Phase 24L - Frontend refresh after real order feedback
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Ensure Construction and Research frontend refresh and success feedback remain accurate after real persisted order creation.

## Purpose

Even though this block is not visual QA, the accepted cockpit paths should still reflect real persisted orders correctly after the backend confirms them. If current behavior is already correct, this task should verify or document that; if not, it should fix only the minimal issue.

## Current problem

Real persisted QA may reveal stale queue panels, stale resource summaries, or weak success feedback after Construction or Research orders are created. That would make backend-only QA harder to trust and would leave the cockpit technically inconsistent.

## Context

Construction and Research already have confirmation and feedback flows. This task should focus on whether those flows correctly refresh read-state after real order creation, not on redesigning the pages.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- Construction and Research API client files
- presentation helpers
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`

## Component discovery

Inspect the current post-submit refresh flow, loading or success feedback behavior, and any existing patterns from accepted cockpits that already re-read backend state after mutation.

## Dependency analysis

Expected behavior chain:

- user or QA action -> enqueue call -> backend success
- frontend success handler -> re-fetch read-state
- updated queue or resource panels -> visible success feedback grounded in backend confirmation

## Implementation requirements

1. Verify Construction behavior:
   - after a successful create, the page refreshes read-state
   - it shows success feedback
   - queue state reflects the persisted order if the backend returned it or the follow-up read supplies it
2. Verify Research behavior:
   - after a successful enqueue, the page refreshes read-state
   - it shows success feedback
   - queue state reflects the persisted order
3. If current behavior is already correct, satisfy this task with a verification note or test-friendly documentation update.
4. If current behavior is not correct, fix it minimally.
5. Do not add optimistic state that contradicts the backend.
6. Do not redesign the UI.

## Backend/API requirements

- None expected.
- If a tiny backend metadata improvement is truly required for grounded success feedback, keep it minimal and tested.

## Frontend/UI requirements

- Primary copy remains Spanish
- Success feedback should indicate that the backend confirmed the order
- Technical ids and raw payload details remain secondary or diagnostic only

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- related frontend API or helper files only if needed
- docs only if the behavior is verified rather than changed

## Safety constraints

- No bypass
- No fake queue entries
- No optimistic mutation state that outpaces the backend
- No visual redesign

## Acceptance criteria

- Frontend refresh behavior is verified or fixed for both flows.
- Success feedback remains grounded in real persisted backend state.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.
- Backend tests pass if backend code is touched.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Run backend validation only if backend files are touched:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- This task should stay narrow. If frontend flows are already correct but undocumented, documenting that verification is preferable to speculative UI changes.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on refresh behavior or narrow verification notes.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer minimal refresh or feedback fixes over page restructuring.
- If a broader UX issue is found, create a focused follow-up task instead of expanding this one.
