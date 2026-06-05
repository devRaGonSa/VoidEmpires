# TASK-28Q

---
id: TASK-28Q
title: Visual QA Findings Audit
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: high
---

## Goal
Record the latest manual cockpit visual QA findings and classify each issue as blocking or non-blocking for final visual acceptance.

## Context
Current snapshot shows copy/fallback issues in Ranking, Alliance and Market that prevent final visual acceptance even though pages load correctly. We need explicit traceability before applying fixes.

## Implementation steps

1. Review manual QA screenshots and notes from the latest pasted task scope.
2. Update the language/copy tracking docs with pass/fail status by cockpit.
3. Mark blocking issues explicitly:
   - Ranking visible English leaks.
   - Alliance English leak.
   - Alliance primary UI raw ID/fallback leakage.
   - Repeated Market resource unknown fallback.
4. Keep non-blocking results for espionage and galaxy and link to reviewed evidence in docs.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/cross-cockpit-language-audit.md

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/cross-cockpit-language-audit.md

## Acceptance criteria

- Blockers are listed per cockpit and source copy.
- No behavior change is introduced in this task.
- `npm run build --prefix src/VoidEmpires.Frontend` runs successfully if docs-only task touches frontend docs context.
