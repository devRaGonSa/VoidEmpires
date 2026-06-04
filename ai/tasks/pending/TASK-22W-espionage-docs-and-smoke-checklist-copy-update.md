# TASK-22W

---
id: TASK-22W
title: Phase 22W - Espionage docs and smoke checklist copy update
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - qa
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: medium
---

## Goal

Update the Espionage documentation and shared smoke checklist so future QA catches visible English copy regressions immediately.

## Purpose

Once the cockpit copy is normalized, the docs should make that expectation explicit. This task prevents the same issue from quietly returning in later UI polish or feature work.

## Current problem

The documentation already covers the accepted Espionage cockpit and shared frontend smoke flow, but it does not yet spell out the final Spanish-copy expectations for signals, future missions, diagnostics, and visible English leakage.

## Context

`docs/dev/espionage-cockpit-checklist.md` and `docs/dev/frontend-foundation-smoke-checklist.md` already include Espionage in the accepted cockpit suite. They should now reflect the final copy-polish baseline rather than only functional readiness.

## Files to read first

- `docs/dev/espionage-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/cockpit-copy-guidelines.md`

## Component discovery

Inspect the existing checklist style used by the other accepted cockpits. Prefer short, copy-pasteable QA bullets that someone can use during seeded browser verification without rereading implementation details.

## Implementation requirements

1. Update the Espionage checklist to state explicitly that:
   - no visible English copy should remain in primary or secondary UI
   - future mission cards stay disabled in Spanish
   - signal and coverage labels are Spanish-first
   - diagnostics remain collapsed by default
   - no active espionage is available
2. Update the shared frontend smoke checklist to include:
   - Espionage primary copy should be Spanish
   - visible English technical strings outside diagnostics should fail visual QA
3. Keep the docs concise and operational.
4. Preserve the current accepted seeded route and read-only boundary wording.
5. Do not document unsupported mission execution or roadmap promises as if they already exist.

## UI/UX requirements

- QA docs should be specific enough to catch this exact regression quickly.
- Checklist wording should remain concise and screenshot-QA friendly.

## Backend/API requirements

- None.

## Expected files to modify

- `docs/dev/espionage-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md` only if it references the old copy expectation and truly needs alignment

## Safety constraints

- Do not document unsupported active spy missions
- Do not expand the feature scope through docs wording

## Acceptance criteria

- Espionage QA docs reflect the Spanish-copy baseline clearly.
- Shared smoke guidance includes the copy-regression check.
- Validation passes if any code or broader docs require it.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Run the full validation only if code or test files were touched alongside docs; otherwise keep the validation proportional.

## Notes / residual risks

- Avoid overgrowing the docs. The goal is a crisp future regression tripwire, not a narrative rewrite.
- If an existing checklist bullet already implies the same rule, refine it rather than adding duplicate prose.

## Commit and push

1. Run `git status`.
2. Confirm only the intended docs changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep the docs short and actionable.
