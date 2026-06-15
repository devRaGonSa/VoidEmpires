# TASK-35K

---
id: TASK-35K
title: Copy regression hardening Spanish
status: pending
type: tooling
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Expand frontend copy regression checks for the playable loop.

## Context
The copy guard should catch mojibake, risky placeholders, English primary UI fallbacks, and forbidden normal UI wording before they reach future task blocks.

## Implementation steps

1. Review `scripts/check-frontend-copy-regressions.ps1`.
2. Add checks for newly risky strings:
   - raw placeholder angle-bracket command fragments in docs/UI where likely copy-pasted incorrectly;
   - corrupted mojibake sequences such as `Ãƒ`, `Ã‚`, and `Æ’` in scripts/docs/frontend;
   - English fallback labels in primary UI where Spanish should be used;
   - forbidden normal UI wording suggesting instant completion or cheats.
3. Avoid false positives for legitimate technical identifiers.
4. Preserve existing allowed technical terms.
5. Run the check and adjust allowlists narrowly if needed.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1
- Optional: docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Copy guard catches encoding and placeholder regressions.
- Existing code passes.
- False positives are minimized.
- Frontend build passes.

## Constraints

- Do not broadly rewrite UI copy unless required to pass the guard.
- Do not block legitimate technical identifiers.
- Do not perform browser/visual QA.

## Validation

Before completing the task run:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35K message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
