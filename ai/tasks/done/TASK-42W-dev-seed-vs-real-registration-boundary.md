# TASK-42W-dev-seed-vs-real-registration-boundary

---
id: TASK-42W
title: Dev seed versus real registration boundary
status: done
type: docs
team: platform
supporting_teams: [frontend]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Separate development seed profiles from real registration in docs and current state.

## Context
Cockpit validation remains an internal validation flow. Real player entry is account registration, and normal UI should not mention seed behavior.

## Implementation steps

1. Review current state, development seed docs, and product readiness docs.
2. Clarify that cockpit validation and seed profiles are internal/operator flows.
3. Clarify that registration is the real player entry path.
4. Ensure normal UI copy does not mention seed as a product concept.
5. Keep any operator-only seed docs clearly scoped.

## Files to read first

- ai/current-state.md
- docs/dev/development-seed-profiles.md
- docs/dev/product-readiness-report.md
- docs/dev/product-mode-visibility-contract.md
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- ai/current-state.md
- docs/dev/development-seed-profiles.md
- docs/dev/product-readiness-report.md
- docs/dev/product-mode-visibility-contract.md

## Acceptance criteria

- Current state names registration as real player entry.
- Dev seed/cockpit-validation remains internal.
- Normal UI is not instructed to expose seed language.
- Docs avoid manual/browser QA claims.

## Constraints

- Do not remove internal dev/operator scripts.
- Documentation-focused task.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
