# TASK-24V

---
id: TASK-24V
title: Phase 24V - Docs update for runtime script output
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: high
---

## Goal

Update docs with corrected script usage and expected runtime output.

## Purpose

The persisted-flow runbook should reflect the hardened scripts, the current DTO-backed output, and the real failure modes the user may now see at runtime.

## Current problem

The docs currently describe the helpers at a high level, but they do not yet explain how to interpret runtime output when queues are occupied, when resource formatting falls back, or when a user accidentally pastes command output back into PowerShell.

## Files to read first

- `docs/dev/persisted-gameplay-flow-checklist.md`
- `scripts/dev-qa-baseline.ps1`
- `scripts/dev-qa-create-construction-order.ps1`
- `scripts/dev-qa-create-research-order.ps1`

## Implementation requirements

1. Update `docs/dev/persisted-gameplay-flow-checklist.md`.
2. Include:
   - how to start the backend
   - how to run the baseline script
   - how to run the Construction script
   - how to run the Research script
   - what to do if the queue is already occupied
   - what to do if resources are not printed because the DTO changed
   - a warning not to paste output back into PowerShell
3. Keep the commands copy-pasteable.

## Backend/API requirements

- None.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not include destructive commands
- Do not include secrets
- Do not normalize away real failures

## Acceptance criteria

- The docs reflect corrected runtime script behavior.
- The main user confusion points are addressed explicitly.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Output examples should stay concise enough to remain readable even if the underlying JSON is verbose.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to the intended docs and helper notes.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer one authoritative runbook update over scattered doc edits.
