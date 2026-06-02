# TASK-11R

---
id: TASK-11R
title: Phase 11R - Development seed reset and recovery documentation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11R"
priority: medium
---

## Goal
Document how to recover or re-run local development validation after controlled mutation changes dev data.

## Context
The new create-transfer path mutates development state, so validation runs may start from a changed fleet state. The repository docs need to explain how to reapply the minimal-validation seed, what that seed does and does not reset, and how to inspect fleet state with API endpoints before re-running checks.

## Implementation steps

1. Inspect `docs/dev/fleet-api-contracts.md`, the frontend README, the root README, and any development seed docs already present.
2. Add or update documentation describing how create-transfer mutates development data and how repeated validations may begin from a changed state.
3. Explain how to safely re-apply the `minimal-validation` seed, including any limitations about what it resets.
4. Document the recommended non-visual validation sequence and how to verify fleet state after mutation using API endpoints.
5. If the seed does not fully reset mutated transfer state, document that limitation clearly and only create a follow-up task if it is truly needed.
6. Keep the documentation free of secrets and placeholder-only for any connection strings.

## Files to read first

- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/README.md
- README.md
- ai/current-state.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/README.md
- README.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Documentation explains that create-transfer mutates development data.
- Documentation explains how to safely re-apply the minimal-validation seed.
- Documentation explains what the seed does and does not reset, or clearly states the limitation if it is not a full reset.
- Documentation explains how to verify fleet state through API endpoints after mutation.
- Documentation includes a recommended non-visual validation sequence.
- No secrets, real connection strings, private IPs, or local-only credentials are introduced.

## Constraints

- Keep the work documentation-focused.
- Do not touch PostgreSQL directly.
- Do not apply EF migrations.
- Do not add frontend code unless docs links need a small consistency fix.
- Use placeholders for connection strings.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` only if frontend files are touched

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single commit for this task.
- If the recovery guidance requires broader seed changes, split the extra work into a follow-up task.
