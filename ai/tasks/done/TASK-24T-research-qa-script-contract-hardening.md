# TASK-24T

---
id: TASK-24T
title: Phase 24T - Research QA script contract hardening
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

Harden `dev-qa-create-research-order.ps1` against the real Research UI-state DTO shape.

## Purpose

The Research QA helper should select a safe enqueue target from real backend metadata, explain queue-occupied cases clearly, and print before or after state without relying on brittle DTO assumptions.

## Current problem

The previous helper was written against a specific DTO expectation. This task should verify that the current available-research path, enqueue metadata path, and queue interpretation still match the real endpoint response.

## Files to read first

- `scripts/dev-qa-create-research-order.ps1`
- `scripts/dev-qa-baseline.ps1`
- Research UI-state DTOs and endpoint tests
- `docs/dev/persisted-gameplay-flow-checklist.md`

## Implementation requirements

1. Ensure the script selects available research from the real DTO path.
2. Ensure the enqueue payload uses backend-provided command metadata.
3. If no available research exists because the queue is already occupied, print a useful message explaining that state.
4. Print before or after:
   - queue count
   - available count
   - selected research
   - response success or failure
5. Do not create multiple orders accidentally.
6. Do not run seed unless explicitly requested by a switch.

## Backend/API requirements

- No backend change is expected unless the current response lacks necessary authoritative metadata.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not build payloads from display-only text
- Do not enqueue multiple orders accidentally
- Do not run seed implicitly

## Acceptance criteria

- The Research QA helper matches the real endpoint contract.
- Queue-occupied or no-available-target states are reported clearly.
- Validation passes.

## Validation

Run parser checks for the scripts and backend validation only if backend files are touched.

## Notes / residual risks

- If the helper must distinguish between “no available research because blocked” and “queue already occupied,” prefer messages grounded in backend fields instead of speculative explanations.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on the Research helper and its docs.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer narrow contract hardening over script redesign.
