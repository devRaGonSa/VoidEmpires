# TASK-37C

---
id: TASK-37C
title: App shell final layout
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 37A-37BZ - Pre-Product Completion: Full Playable App Shell, UX, Catalogs, Readiness & Finalization Prep v1"
priority: high
---

## Goal
Refactor the global shell so header, sidebar, session context, content, and diagnostics have clear responsibilities.

## Context
This task belongs to the large pre-product completion block. The goal is to move VoidEmpires closer to a coherent playable product shell while explicitly deferring final database/model consolidation, final generated image assets, final visual QA/corrections, production auth hardening, combat, fleet movement, market transactions, and alliance mutations.

## Implementation steps

1. Read every file listed in "Files to read first" before editing.
2. Use ai/orchestrator/component-discovery.md to identify the smallest related component set.
3. Implement only the behavior or documentation required by this task goal.
4. Keep backend state authoritative; do not fake resources, queues, stock, rankings, or readiness.
5. Keep Development-only tools clearly labelled and secondary; raw ids and payloads belong in diagnostics.
6. Update docs, guards, or tests only when the task changes behavior or accepted scope.
7. Run the validation commands listed below before moving the task to done.

## Files to read first

- AGENTS.md
- ai/architecture-index.md
- ai/current-state.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css
- Optional: docs/dev/
- Optional: scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- The task goal is completed: Refactor the global shell so header, sidebar, session context, content, and diagnostics have clear responsibilities.
- All changed files match the Expected files to modify section or the commit message explains why.
- No unrelated files, build artifacts, screenshots, secrets, or local machine paths are committed.
- Visible UI copy remains Spanish-first and avoids mojibake, fake auth, fake resources, and obsolete prototype wording.
- No final DB migrations, final generated images, combat, fleet movement, market transactions, alliance mutations, or production-auth overclaim are introduced.
- Required validation commands pass and results are recorded in the task/commit notes where appropriate.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal and within the task scope
- Prefer small commits
- Preserve lazy loading; do not reintroduce eager cockpit page imports into App.tsx
- Do not fake gameplay state or optimistic-update authoritative backend-owned data
- Do not claim full visual/browser QA unless it was actually performed
- If the change exceeds the task budget, stop and create a follow-up task instead of broadening scope

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-37C message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits per task.
- If the change would exceed these limits, create a follow-up task and stop expanding scope.
