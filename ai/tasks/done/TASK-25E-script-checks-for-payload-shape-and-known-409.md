# TASK-25E

---
id: TASK-25E
title: Phase 25E - Script checks for payload shape and known 409
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 25A-25H - Persisted QA create scripts command payload alignment"
priority: medium
---

## Goal

Strengthen script validation to catch payload-shape issues before live runtime use.

## Purpose

The script check helper should cover the most important local helper logic so obvious payload mistakes or known-response handling regressions are easier to catch before manual QA.

## Implementation requirements

1. Update `scripts/check-dev-qa-scripts.ps1`.
2. Add lightweight checks for:
   - the Construction payload builder not using the display label as `action`
   - payload-summary formatting helper behavior
   - Research `409` handling helper behavior if extracted
3. Do not add heavy dependencies.
4. Run parser and helper checks.

## Acceptance criteria

- `check-dev-qa-scripts.ps1` covers the new helper logic where feasible.
- The check remains lightweight and local.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```
