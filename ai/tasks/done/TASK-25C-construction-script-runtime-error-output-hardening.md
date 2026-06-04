# TASK-25C

---
id: TASK-25C
title: Phase 25C - Construction script runtime error output hardening
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 25A-25H - Persisted QA create scripts command payload alignment"
priority: medium
---

## Goal

Make Construction QA script failures easier to interpret.

## Purpose

If the Construction helper hits another contract mismatch or backend validation failure, the output should be immediately diagnosable without requiring a large raw ASP.NET exception trace.

## Implementation requirements

1. On HTTP 400, print:
   - endpoint
   - selected option label
   - selected backend action value
   - sanitized payload summary
   - response body
2. Do not dump huge stack traces unless a verbose switch is enabled.
3. If the backend says queue, resource, or ownership is invalid, print a Spanish summary.
4. Keep a non-success exit code for real failures.

## Acceptance criteria

- Future contract mismatches are diagnosable from script output alone.
- Real failures still return a non-success exit code.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```
