# TASK-22X

---
id: TASK-22X
title: Phase 22X - Espionage copy regression search, test, or script
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - qa
  - tests
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: medium
---

## Goal

Add lightweight regression protection so the observed English Espionage copy does not quietly return in future work.

## Purpose

Manual visual QA remains important, but a repeatable search or helper-level check can catch obvious regressions earlier and reduce future cleanup work.

## Current problem

The English strings found in Espionage are the kind of regression that can reappear when someone reuses raw helper output or fallback text. The repository may not already have a frontend unit-test stack for this area, so the safest protection must be chosen based on existing tooling.

## Context

This repository may not have frontend unit tests for the cockpit pages. The task must not introduce a heavy new framework just to guard a handful of strings. Use the lightest credible protection that fits existing patterns.

## Files to read first

- `tests/VoidEmpires.Tests/`
- `src/VoidEmpires.Frontend/package.json`
- frontend tooling configuration files
- `docs/dev/frontend-foundation-smoke-checklist.md`
- Espionage frontend files and helpers

## Component discovery

Inspect whether presentation helpers already have a natural test seam. If they do, prefer a small helper-level test. If not, document and automate a repeatable search command instead of forcing a fragile testing setup into the repo.

## Implementation requirements

1. Choose the safest available protection option:
   - If frontend tests already exist, add a small presentation-helper test.
   - If a lightweight existing .NET or script-based check can guard source text safely, use that pattern.
   - If no suitable test path exists, add a docs-backed regression search command as the minimum safeguard.
2. At minimum, document a repeatable search such as:
   - `Select-String -Path src\VoidEmpires.Frontend\src\**\*.tsx,src\VoidEmpires.Frontend\src\**\*.ts -Pattern "passive signal rows|sensor profile rows|detection coverage rows|gameplay is not implemented|future placeholder"`
3. Prefer helper-level tests if they are simple and aligned with existing tooling.
4. Do not introduce a new heavy test framework.
5. Keep the protection aimed at visible copy issues, not generalized linting.

## UI/UX requirements

- The protection should target player-facing copy regressions.
- False positives should stay low enough that the check remains trustworthy.

## Backend/API requirements

- None unless the safest existing test seam lives on the .NET side and still validates presentation helpers or generated copy meaningfully.

## Expected files to modify

- existing frontend test files if they exist
- a lightweight script or documentation file if no test seam exists
- `docs/dev/frontend-foundation-smoke-checklist.md` if the search command is documented there

## Safety constraints

- No behavior changes
- No heavy testing stack
- No broad linting or CI redesign

## Acceptance criteria

- There is a repeatable safeguard against the observed English-copy regression.
- The chosen safeguard matches existing repository tooling.
- Validation passes for any tests or scripts added.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
dotnet build --no-restore
dotnet test --no-build
```

Run backend validation only if .NET tests or supporting scripts require it.

## Notes / residual risks

- Manual visual QA is still the final acceptance gate for wording and hierarchy.
- If the only safe option is a documented search command, be explicit that it is a lightweight guard rather than full UI test coverage.

## Commit and push

1. Run `git status`.
2. Confirm the regression protection stays lightweight.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Avoid introducing new framework debt for a narrow copy guard.
