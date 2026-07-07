# TASK-41AM

---
id: TASK-41AM
title: Product footer or secondary info
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: medium
---

## Goal
Add or polish secondary info area if needed.

## Context
Any footer or secondary shell information should avoid dev details in primary product UI.

## Implementation steps

1. Inspect current shell/footer/secondary info surfaces.
2. Remove backend URLs, localhost details, provider names, and dev labels from product mode.
3. If useful, include a small product-like version/status area without technical URLs.
4. Keep technical details operator-only.
5. Preserve layout responsiveness.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Secondary info contains no dev details in product mode.
- No backend URLs are visible.
- Layout remains coherent.

## Constraints

- Do not add marketing landing-page content.
- Do not expose provider or localhost details.
- Do not add final images or assets.

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
