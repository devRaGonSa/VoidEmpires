# TASK-22Q

---
id: TASK-22Q
title: Phase 22Q - Espionage visible English copy audit
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: high
---

## Goal

Audit the accepted Espionage cockpit and document every remaining English or mixed-language string that is still visible outside diagnostics.

## Purpose

The Espionage cockpit is already accepted functionally as a read-only intelligence surface, but visual QA still found residual English copy. This task creates a precise inventory so the remaining polish work can be completed safely without drifting into gameplay or backend scope.

## Current problem

Recent visual QA found English or mixed-language phrases still visible in player-facing UI, including:

- `passive signal rows available`
- `No passive signal rows available`
- `sensor profile rows`
- `detection coverage rows`
- `visible transfer trajectories`
- `Reconnaissance remains a future placeholder...`
- `Infiltration gameplay is not implemented.`
- `Sabotage gameplay is not implemented.`

Additional English may still exist in cards, labels, empty states, helper text, future-action placeholders, coverage summaries, or signal diagnostics that have not yet been reviewed systematically.

## Context

`/espionage` is already accepted as a read-only intelligence cockpit. This block must not introduce spy mission execution, sabotage, infiltration, theft, counter-espionage, combat, WebSockets, or any new live gameplay behavior. The work is limited to copy hierarchy, Spanish-first presentation, and minor read-model wording refinements if absolutely necessary.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx`
- `src/VoidEmpires.Frontend/src/utils/espionagePresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/espionageViewModel.ts`
- `src/VoidEmpires.Frontend/src/api/espionageTypes.ts`
- `docs/dev/cockpit-copy-guidelines.md`
- `docs/dev/espionage-cockpit-checklist.md`

## Component discovery

Start from the Espionage page and trace every visible label back to its presentation helper or view-model source. Check whether wording is authored directly in the page, composed by helper functions, or derived from raw DTO fields. Prefer finding the existing owner of each visible phrase instead of duplicating mapping logic.

## Implementation requirements

1. Search the Espionage frontend path for likely English regressions, including:
   - `passive`
   - `signal rows`
   - `sensor profile`
   - `detection coverage`
   - `visible transfer`
   - `Reconnaissance`
   - `Infiltration`
   - `Sabotage`
   - `gameplay`
   - `placeholder`
   - `implemented`
   - `rows`
2. Classify each occurrence as one of:
   - primary UI and must be translated or rewritten
   - secondary UI and should be translated
   - diagnostics-only and acceptable only if collapsed and clearly technical
   - docs/tests and update only if they reflect visible UX language
3. Record the audit results directly in the implementation notes or checklist update so later tasks have a concrete punch list.
4. Add a concise note to `docs/dev/espionage-cockpit-checklist.md` that this copy-normalization pass targets visible English leakage outside diagnostics.
5. Do not change behavior unless a copy fix is completely obvious and low risk while performing the audit.

## UI/UX requirements

- Primary and secondary Espionage UI must be Spanish-first.
- Diagnostics may still expose technical detail, but the wrapper and visible framing should remain Spanish.
- Audit output should make it easy to verify the first viewport and target cards without guesswork.

## Backend/API requirements

- None expected.
- If a raw backend label is the real source of visible English, note it precisely before proposing any backend change.

## Expected files to modify

- `docs/dev/espionage-cockpit-checklist.md`
- Espionage frontend files only if a tiny obvious wording fix is made during the audit

## Safety constraints

- No espionage execution
- No new endpoints
- No mission handlers
- No mutation behavior
- No broad refactor disguised as copy cleanup

## Acceptance criteria

- The visible English or mixed-language copy inventory is documented clearly.
- Each issue is classified by severity and likely source.
- The follow-up implementation path for Tasks `22R` through `22V` is unambiguous.
- Validation passes for any touched files.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
dotnet build --no-restore
dotnet test --no-build
```

Run backend validation only if backend or test files were actually touched.

## Notes / residual risks

- Do not remove useful diagnostics just because they look technical; instead, keep them collapsed and wrapped in Spanish.
- Some English may come from helper-level pluralization or fallback text rather than from the page component itself.

## Commit and push

1. Run `git status`.
2. Confirm only the intended audit-related files changed.
3. Commit with a message that reflects the audit scope.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep this task focused on audit clarity, not on implementing every copy fix.
