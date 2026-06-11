# TASK-32F

---
id: TASK-32F
title: Document gameplay modal guardrails and add lightweight protections
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: medium
---

## Goal
Capture the new gameplay confirmation pattern in docs and add lightweight guardrails so future cockpit work does not regress into unsafe mutation flows.

## Context
Once the shared modal is in use, the repository should document the intended interaction contract: review does not mutate, open does not mutate, explicit confirm mutates, success refreshes from backend, and failure preserves useful local state with Spanish feedback.

## Implementation steps

1. Document the gameplay modal usage pattern and non-goals in the frontend foundation smoke checklist.
2. Review existing script guardrails for opportunities to cover confirmation copy and anti-auto-complete language.
3. Add only lightweight static checks that fit the repository's current scripting approach.
4. Keep the guardrails focused on durable, low-noise signals.
5. Validate that the lazy-loading and copy-regression scripts still pass.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `scripts/check-frontend-route-lazy-imports.ps1`
- `scripts/check-frontend-copy-regressions.ps1`
- `src/VoidEmpires.Frontend/src/components/GameModal.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`

## Expected files to modify

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `scripts/check-frontend-copy-regressions.ps1`
- `scripts/check-frontend-route-lazy-imports.ps1` only if a minimal related guard is justified

## Acceptance criteria

- The gameplay modal usage pattern is documented clearly for future tasks.
- Lightweight safeguards exist for the most likely confirmation-flow regressions when feasible.
- Existing guard scripts continue to pass.
- The task does not introduce an over-engineered testing framework.

## Constraints

- Keep the solution script-first and lightweight.
- Avoid brittle string matching that will create frequent false positives.
- Do not widen the task into end-to-end UI automation.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeds.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs(frontend): add gameplay modal guardrails`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Stop if stronger guarantees would require a separate test harness.
