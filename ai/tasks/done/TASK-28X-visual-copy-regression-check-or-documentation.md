# TASK-28X

---
id: TASK-28X
title: Visual Copy Regression Check or Documentation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Goal
Add a lightweight guard or documented checklist to catch recurring Spanish/placeholder regressions.

## Context
Blocking regressions have already recurred across Ranking, Alliance and Market. We need a repeatable local check or a documented manual gate.

## Implementation steps

1. Add `scripts/check-frontend-copy-regressions.ps1` to scan `src/VoidEmpires.Frontend/src` for banned phrases.
2. Exclude docs paths to avoid false positives.
3. Wire or document the check in existing QA script docs.
4. Re-run QA scripts and frontend build.

## Files to read first

- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Either a new regression script exists or a clear checklist gate documents the checks.
- New check is included in validation flow.
- Existing checks continue to pass.
