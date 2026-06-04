# TASK-24S

---
id: TASK-24S
title: Phase 24S - Construction QA script contract hardening
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: high
---

## Goal

Harden `dev-qa-create-construction-order.ps1` against the real Construction UI-state DTO shape.

## Purpose

The Construction QA helper should build its selection and payload from authoritative backend fields, print useful before or after state, and fail clearly when no safe action is available.

## Current problem

The previous block assumed a stable DTO shape. This task should verify that the script’s available-action lookup, queue counting, and resource printing still match the real endpoint payload.

## Files to read first

- `scripts/dev-qa-create-construction-order.ps1`
- `scripts/dev-qa-baseline.ps1`
- Construction UI-state DTOs and endpoint tests
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Implementation requirements

1. Inspect how the script selects the available Construction action.
2. Ensure it uses the real path to available actions.
3. Ensure the payload is built from backend-provided command fields, not display text.
4. If no available action exists, print a useful message and exit with a controlled failure.
5. Print before or after:
   - resources if available
   - queue count
   - selected action label or key
   - response success or failure
6. Do not create multiple orders accidentally.
7. Do not run the seed unless explicitly requested by a switch.

## Backend/API requirements

- No backend change is expected unless the current response is clearly missing a required authoritative field.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not build payloads from display-only text
- Do not enqueue multiple orders accidentally
- Do not run seed implicitly

## Acceptance criteria

- The Construction QA helper matches the real endpoint contract.
- It prints a useful controlled message when no safe enqueue target exists.
- Validation passes.

## Validation

Run parser checks for the scripts and backend validation only if backend files are touched.

## Notes / residual risks

- If the endpoint exposes only a display label and not an authoritative action key, that would be a backend contract bug and should be handled carefully rather than papered over.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on the Construction helper and its docs.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer narrow contract hardening over script redesign.
