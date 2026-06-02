# TASK-12A

---
id: TASK-12A
title: Phase 12A - Fleet cockpit visual validation docs
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12A"
priority: medium
---

## Goal
Document the Fleet cockpit UI milestone and provide a concrete manual visual validation checklist for the new block.

## Context
Block 11X-12A is intended to deliver a substantial Fleet cockpit UI improvement that should be followed by explicit manual visual QA. The repository documentation needs a clear validation checklist that covers both the new visual review flow and the required non-visual build and test baseline, without leaking secrets or local-only environment details.

## Implementation steps

1. Review the current frontend smoke-check guidance, Fleet API contract docs, and frontend README to understand existing validation instructions.
2. Update the most relevant docs with a Fleet cockpit milestone checklist covering minimal-validation seed setup, backend startup, frontend startup, opening the Flotas page, and the expected visible behaviors of the new cockpit UI.
3. Document that estimate remains visible and read-only, create transfer requires explicit confirmation, cancel transfer requires explicit confirmation when an active transfer exists, and complete, split, or merge remain prototype-only or disabled.
4. Add the non-visual validation baseline with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
5. Keep the documentation free of screenshots, secrets, private addresses, passwords, real connection strings, migrations, or unrelated behavioral changes.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/README.md
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- README.md

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/README.md

## Acceptance criteria

- The documentation includes a manual visual validation checklist for the Fleet cockpit milestone that covers setup, opening Flotas, readable group summary, readable selected-group panel, estimate visibility, create and cancel confirmation requirements, disabled prototype commands, readable feedback area, and reduced raw enum dominance.
- The documentation also includes the non-visual validation baseline with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
- No secrets, screenshots, migrations, or 3D-related guidance are added.
- The documented behavior remains aligned with the frontend prototype boundary and existing backend contracts.

## Constraints

- Keep this task documentation-focused, allowing only very small frontend or docs consistency fixes if strictly necessary.
- Do not add real credentials, private IPs, production authentication guidance, or EF migrations.
- Do not expand the executable frontend command surface beyond estimate, create transfer, and cancel transfer.
- Split follow-up work if the documentation requires broader product-state rewrites beyond this milestone.

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
- If documentation updates start spilling into broader roadmap maintenance, stop and create a smaller follow-up task.
