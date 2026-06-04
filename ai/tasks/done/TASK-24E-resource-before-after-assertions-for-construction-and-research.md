# TASK-24E

---
id: TASK-24E
title: Phase 24E - Resource before and after assertions for Construction and Research
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
  - docs
roadmap_item: "Block 24A-24P - Real persisted gameplay flow QA for Construction and Research"
priority: high
---

## Goal

Determine, test, and document the real resource behavior before and after Construction and Research enqueue.

## Purpose

A persisted queue row alone is not enough. The repository needs to know whether each flow deducts resources immediately, reserves them, only validates affordability, or follows a mixed rule, then ensure docs and UI wording reflect the truth.

## Current problem

Different cockpits already display affordability and resource context, but the exact mutation behavior may differ between Construction and Research. If docs or copy imply the wrong rule, backend-only QA will be misleading.

## Context

Construction and Research both consume or validate resources, but the actual domain behavior may not be identical. This task should verify the real rule rather than changing behavior speculatively.

## Files to read first

- Construction and Research command services
- resource or stockpile domain services
- existing affordability or resource tests
- Planet UI state services
- Construction and Research read-model services
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`

## Component discovery

Inspect current resource spend, affordability, reservation, and queue write behavior first. Prefer grounding every claim in an existing service or persisted-state observation.

## Dependency analysis

Expected reasoning flow:

- pre-mutation read-state -> command service -> resource validation or spend behavior -> post-mutation read-state
- backend tests -> docs and possibly UI copy alignment

## Implementation requirements

1. Determine the actual resource behavior for Construction enqueue:
   - deducted immediately
   - reserved
   - validated only
   - mixed
2. Determine the actual resource behavior for Research enqueue.
3. Add tests that assert the real behavior rather than assumed behavior.
4. Do not change business rules unless tests reveal a clear defect.
5. Update docs so the actual rule is stated explicitly.
6. If existing UI wording materially implies the wrong behavior, add a small Spanish copy correction.
7. Keep any copy change narrow and grounded in the tested contract.

## Backend/API requirements

- Backend tests are required.
- No schema changes.
- Do not add new resource models or reservation systems in this task.

## Frontend/UI requirements

- Only adjust copy if current wording is misleading.
- Keep primary UI Spanish-first.
- Example wording should match the actual backend rule rather than generic economy language.

## Expected files to modify

- backend tests around Construction and Research resource behavior
- relevant docs under `docs/dev/`
- frontend copy files only if a narrow correction is needed

## Safety constraints

- No hidden resource creation
- No manual SQL
- No destructive reset
- No speculative business-rule rewrite

## Acceptance criteria

- Resource behavior is tested and documented for both flows.
- UI wording does not contradict backend behavior.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Frontend build is required only if UI copy changes.

## Notes / residual risks

- It is acceptable if Construction and Research use different resource timing rules, as long as the difference is intentional, tested, and documented clearly.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm changed files match the intended tests, docs, and narrow copy scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer test and doc alignment over logic changes.
- If a real business-rule bug is found, fix only the narrow defect and avoid broad economy refactors.
