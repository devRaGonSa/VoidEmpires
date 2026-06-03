# TASK-14B

---
id: TASK-14B
title: Phase 14B - Planet cockpit docs smoke checklist and state update
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Document the Planet cockpit foundation and the combined Galaxy plus Planet QA criteria for the completed block.

## Context
After Galaxy polish and the first Planet cockpit surface are in place, the repository should record the new manual QA checklist, the intentional non-goals, and the updated current-state documentation so future task work starts from the right baseline.

## Implementation steps

1. Create or update a dedicated Planet cockpit checklist document.
2. Update the frontend smoke checklist and strategic map cockpit checklist as needed for the combined Galaxy plus Planet block.
3. Record the intentional exclusions and the current controlled construction boundary.
4. Update `ai/current-state.md` or equivalent current-state documentation if present.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/strategic-map-cockpit-checklist.md`, if present
- `docs/dev/planet-cockpit-checklist.md`, if present
- `src/VoidEmpires.Frontend/README.md`
- `ai/current-state.md`, if present

## Expected files to modify

- `docs/dev/planet-cockpit-checklist.md`, create or update
- `docs/dev/frontend-foundation-smoke-checklist.md`, if needed
- `docs/dev/strategic-map-cockpit-checklist.md`, if needed
- `src/VoidEmpires.Frontend/README.md`, if needed
- `ai/current-state.md`, if needed

## Acceptance criteria

- Planet cockpit documentation exists and describes the current development gameplay surface clearly.
- Combined manual QA covers Galaxy load, Galaxy overflow, Galaxy Spanish-first copy, Planet load, resources, buildings, queue visibility, Galaxy or Fleet links, collapsed diagnostics, and successful npm build.
- Intentional exclusions such as no 3D, no combat, no espionage, no alliances, no WebSockets, no production auth, no Galaxy mutations, and controlled development-safe construction actions are documented.
- Current-state documentation reflects the new baseline.

## Constraints

- Keep the scope documentation-only.
- Do not imply unsupported gameplay systems are implemented.
- Do not add screenshots, secrets, or private infrastructure details.
- Keep the development-only safety boundaries explicit.

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
- Split the work into additional tasks if limits are exceeded.
