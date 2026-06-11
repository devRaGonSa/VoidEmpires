# TASK-32B

---
id: TASK-32B
title: Add reusable frontend modal foundation for gameplay confirmations
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Create a reusable, accessible modal component that can support gameplay confirmation flows without changing gameplay behavior yet.

## Context
Construction, Research, and Shipyard already require explicit confirmation in different forms. This task introduces a shared modal primitive so later migration tasks can reuse one safe confirmation pattern instead of duplicating one-off panels.

## Implementation steps

1. Review existing shared components and styles to match the current frontend conventions.
2. Add a reusable gameplay modal component under `src/VoidEmpires.Frontend/src/components`.
3. Support title, description, body content, primary and secondary actions, loading or disabled states, and close behavior that is safe for confirmation dialogs.
4. Add only the minimal supporting styles required in `styles.css`.
5. Keep labels Spanish-first where the component embeds default copy.
6. Verify the component compiles without migrating production pages unless required for type safety.

## Files to read first

- `ai/orchestrator/component-discovery.md`
- `src/VoidEmpires.Frontend/src/components`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/components/GameModal.tsx`
- `src/VoidEmpires.Frontend/src/components/index.ts` or equivalent shared export file if one exists
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- A reusable gameplay modal component exists and compiles.
- The modal supports primary and cancel actions, loading or disabled state, and safe close behavior.
- The styles stay close to the current visual language and avoid a broad redesign.
- No gameplay page behavior changes are introduced in this task.

## Constraints

- Preserve current frontend architecture and lazy-loading patterns.
- Avoid broad CSS changes.
- Keep copy Spanish-first.
- Do not migrate page flows yet unless required to compile.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.
- No eager route imports are introduced.
- No unrelated frontend pages are modified.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(frontend): add gameplay modal foundation`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Defer page migrations to follow-up tasks.
