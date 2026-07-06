# TASK-41AE

---
id: TASK-41AE
title: Product error states
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Polish error states into product language.

## Context
Normal UI should communicate failures clearly without exposing stack traces, endpoint names, or technical payloads.

## Implementation steps

1. Search frontend pages, components, API helpers, and utilities for error rendering.
2. Change primary error copy to product language such as "No se pudo cargar" or "No se pudo completar la acción".
3. Move technical details to operator diagnostics or secondary collapsed details.
4. Ensure stack traces, endpoint names, raw payloads, and provider names do not appear in product mode.
5. Preserve real failure handling and retry behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Product mode uses player-facing error messages.
- Technical error details are operator-only or secondary.
- Existing error handling remains truthful.

## Constraints

- Do not hide real failures as success or empty state.
- Do not fake backend success.
- Do not remove useful operator diagnostics.

## Validation

Before completing the task ensure:

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
