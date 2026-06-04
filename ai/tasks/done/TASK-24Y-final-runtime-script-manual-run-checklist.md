# TASK-24Y

---
id: TASK-24Y
title: Phase 24Y - Final runtime script manual run checklist
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: medium
---

## Goal

Add a final concise manual runtime checklist for the user.

## Purpose

After the scripts are hardened, the runbook should end with a short practical checklist the user can execute in order without rereading the whole document.

## Current problem

The runbook contains detailed explanation, but it does not yet have a compact final sequence that the user can follow to verify the scripts after the fix.

## Files to read first

- `docs/dev/persisted-gameplay-flow-checklist.md`
- final script commands from Tasks 24R through 24U

## Implementation requirements

1. Add a concise section to `docs/dev/persisted-gameplay-flow-checklist.md` that includes:
   - backend command
   - baseline command
   - Construction order command
   - Research order command
   - reapply `cockpit-validation`
   - baseline again
2. Include expected success and failure outputs.
3. Include a note that the scripts create real Development database rows.
4. Include a note that repeated runs may find the queue occupied and should report it clearly.

## Backend/API requirements

- None.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not include destructive cleanup steps
- Do not imply that repeated runs should delete prior data

## Acceptance criteria

- The runbook ends with a concise, practical runtime checklist.
- The checklist is safe and copy-pasteable.
- Validation passes.

## Validation

Use docs-only validation plus normal repo build or test checks if other files are touched.

## Notes / residual risks

- The short checklist should point back to the fuller sections only when nuance is truly needed.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to the intended runtime checklist doc scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a concise addition to the existing runbook over a separate new document.
