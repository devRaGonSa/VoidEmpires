# TASK-13L

---
id: TASK-13L
title: Phase 13L - Strategic map layout overflow and responsive hardening
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Fix Galaxy layout overflow, clipping, and badge wrapping issues so the cockpit remains readable at common desktop widths.

## Context
Visual QA showed right-rail badges, advisory labels, and technical cards leaking outside their containers. The strategic map layout should feel stable and deliberate without redesigning the shell or reducing the map's visual priority.

## Implementation steps

1. Audit Galaxy layout structure, card sizing, badge rows, and right-rail panel behavior.
2. Harden CSS so badges wrap, shrink, or stack safely instead of overflowing horizontally.
3. Prevent card overlap and clipping at common desktop widths around `1200-1440px`.
4. Keep the current dark galactic cockpit direction and avoid shell-wide redesign work.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSystemCard.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- relevant strategic map UI components, if needed

## Acceptance criteria

- No common Galaxy cockpit card or badge visibly overflows its container at desktop widths around `1200-1440px`.
- Right-rail advisory and technical sections remain readable.
- Cards no longer visually overlap.
- The map remains the primary visual element.
- The app shell is not redesigned.

## Constraints

- Keep the current shell structure.
- Do not add 3D.
- Do not change backend behavior.
- Keep the current dark galactic cockpit direction.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA on `/galaxy` confirms no horizontal clipping or overflow at common desktop widths

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
