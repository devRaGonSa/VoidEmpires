# TASK-11K

---
id: TASK-11K
title: Phase 11K - Fleet command controlled execution docs and validation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11K"
priority: medium
---

## Goal
Document the controlled execution prototype boundary and the non-visual validation flow for the new frontend read-only estimate path.

## Context
The Block 11H-11K work introduces a narrow read-only command execution path while keeping every mutation command disabled from the UI. The repository docs need to make that boundary explicit so future tasks preserve the current prototype posture and validation evidence stays non-visual for this block.

## Implementation steps

1. Review the current fleet API contract docs and any frontend validation notes related to Fleet page development.
2. Update `docs/dev/fleet-api-contracts.md` or add a focused companion document if that keeps the guidance clearer and smaller.
3. Document that the frontend may execute only the read-only estimate flow, while mutation commands remain prototype-only and disabled from the UI even though development endpoints still exist.
4. Record the non-visual validation steps for backend build, backend tests, frontend build, and optional seed plus API checks, while explicitly noting that manual visual validation is deferred for this block.
5. Keep the documentation free of secrets, real credentials, private IPs, or backend behavior changes.

## Files to read first

- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/README.md
- README.md
- ai/current-state.md

## Expected files to modify

- docs/dev/fleet-api-contracts.md
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/README.md
- README.md

## Acceptance criteria

- Documentation clearly states that only the read-only estimate flow may execute from the frontend prototype UI.
- Documentation clearly states that mutation commands remain disabled or prototype-only in the UI.
- The non-visual validation flow is documented with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
- Manual visual validation is explicitly deferred unless a clear frontend regression appears.
- No secrets, migrations, or backend behavior changes are introduced.

## Constraints

- Keep this task documentation-focused.
- Only allow small frontend or docs consistency fixes if strictly necessary.
- Do not apply EF migrations or change backend behavior.
- Do not include real connection strings, passwords, or local-only credentials.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

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
- If documentation updates start requiring broader product-state rewrites, split the extra work into a follow-up task.
