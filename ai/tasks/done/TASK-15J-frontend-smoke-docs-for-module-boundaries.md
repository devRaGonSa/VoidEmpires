# TASK-15J

---
id: TASK-15J
title: Frontend smoke docs for module boundaries
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Document the new module boundaries and manual QA expectations.

## Current problem
Without documentation, future tasks may reintroduce the same mixing problem by putting everything back into `/planet` or `/construction`.

## Context from current implementation
The repository already has frontend smoke and cockpit checklists. This task should update them and add a boundary-specific guide so the new model is explicit.

## Files to inspect first
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/strategic-map-cockpit-checklist.md`
- `ai/current-state.md`

## Expected files to modify
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/planet-module-boundaries.md` if created

## Implementation requirements
- Add or update a `docs/dev/planet-module-boundaries.md` guide.
- Document the responsibilities of:
  - `/planet`
  - `/construction`
  - `/research`
  - `/ground-army`
  - `/shipyard`
  - `/defenses`
  - `/fleets`
  - `/galaxy`
- Add `Do not mix` rules:
  - Do not put the full construction catalog in `/planet`.
  - Do not put defense, army, shipyard or research full catalogs in `/construction`.
  - Do not add mutations to Galaxy.
  - Do not enable unavailable specialized gameplay from placeholders.
- Update smoke checklists with exact URLs such as:
  - `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
  - `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Include visual QA expectations that:
  - `/planet` is a dashboard;
  - `/construction` is general infrastructure;
  - specialized modules open placeholders;
  - there is no full category duplication.

## UI/UX requirements
- Keep the docs practical and concise.

## Backend/API requirements
- No backend change.

## Safety constraints
- Do not document unsupported features as implemented.

## Acceptance criteria
- Future Codex tasks have clear written boundaries.
- Docs mention intentional placeholders.
- Validation commands pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- This doc becomes a source for the next frontend blocks.
