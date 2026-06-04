# TASK-24K

---
id: TASK-24K
title: Phase 24K - Persisted flow docs for backend-only QA
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - backend
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Create a backend-only QA guide for real persisted Construction and Research flows.

## Purpose

The user asked when to start testing with real Construction and Research data. The repo needs one practical guide that explains how to run the full persisted QA flow through PowerShell and dev endpoints, without requiring visual QA.

## Current problem

Existing cockpit checklists focus heavily on the cockpit behavior itself. This block needs an explicit backend-only persisted-flow guide centered on commands, expected state transitions, and safety warnings.

## Context

Visual QA remains separate. This guide should focus on:

- starting the backend
- applying the seed
- capturing baseline state
- creating real orders
- fetching state again
- confirming persistence and safe seed reapply behavior

## Files to read first

- `docs/dev/`
- `scripts/`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Component discovery

Inspect current doc structure, seed instructions, PowerShell examples, and any current command snippets that can be reused instead of duplicated.

## Dependency analysis

Expected documentation flow:

- how to start backend
- how to apply `cockpit-validation`
- how to capture baseline read-state
- how to create Construction and Research orders
- how to re-read state and confirm persistence
- how to reapply the seed and confirm safety

## Implementation requirements

1. Create or update:
   - `docs/dev/persisted-gameplay-flow-checklist.md`
2. Include:
   - how to start the backend
   - how to apply `cockpit-validation` twice
   - how to capture the baseline
   - how to create a Construction order
   - how to create a Research order
   - how to fetch state again
   - what success looks like
   - what failure looks like
   - how to confirm seed reapply preserved manual orders
3. Include exact PowerShell commands or references to scripts created in this block.
4. Include warnings that:
   - these commands create real rows in the Development database
   - they must not be run against production
   - no manual SQL is required
5. Keep the doc practical, copy-pasteable, and backend-focused.

## Backend/API requirements

- None expected.
- The doc should reflect current endpoints accurately if prior tasks adjusted any route or result details.

## Frontend/UI requirements

- None required.
- If cockpit pages are mentioned, keep them secondary to the backend-only QA goal.

## Expected files to modify

- `docs/dev/persisted-gameplay-flow-checklist.md`
- related cockpit checklists only if a narrow cross-reference note is useful
- seed profile docs only if a small linkage note is needed

## Safety constraints

- Docs must not encourage destructive reset
- Docs must not include secrets
- Docs must not present production behavior as part of the normal flow

## Acceptance criteria

- A user can follow the guide without visual QA.
- The command flow is practical and clear.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If some command outputs are inherently verbose, the doc should summarize the key fields users need to confirm rather than pasting huge raw payload examples.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm changed files are limited to intended docs and supporting scripts.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer one strong central checklist over multiple overlapping mini-guides.
- If broader doc cleanup is needed, split it into focused follow-up tasks.
