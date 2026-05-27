# TASK-021

---
id: TASK-021
title: Design Identity and Brevo email foundation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: high
---

## Goal
Create a concise design document for VoidEmpires identity and transactional email before implementation.

## Context
VoidEmpires needs user registration and email confirmation. The email provider for user creation and confirmation flows will be Brevo. The repository must document the intended boundaries before code is added. Future implementation tasks must inspect `devRaGonSa/CreateCvApp` for existing Brevo or email patterns, configuration conventions, interfaces, service naming, options classes, templates, or testing approaches, and reuse compatible conventions where appropriate without copying secrets or project-specific content.

## Implementation steps

1. Create `ai/reports/identity-email-foundation.md`.
2. Update `ai/architecture-index.md` if useful to reflect the new identity and email boundaries.
3. Do not create application code.
4. Do not add migrations.
5. Do not modify `DbContext`.
6. Do not add gameplay systems.
7. Capture the user/account, transactional email, Brevo provider, configuration, and testing direction described in the task brief.
8. Ensure the document explicitly tells future implementation tasks to inspect `devRaGonSa/CreateCvApp` before coding Brevo email integration.

## Files to read first

- `ai/repo-context.md`
- `ai/current-state.md`
- `ai/architecture-index.md`
- `README.md`
- `ai/task-template.md`

## Expected files to modify

- `ai/reports/identity-email-foundation.md`
- `ai/architecture-index.md` if needed

## Acceptance criteria

- `ai/reports/identity-email-foundation.md` exists.
- The document covers user/account model direction, transactional email boundary, Brevo provider direction, email flows, configuration, testing strategy, and the reference repository review requirement.
- No secrets or project-specific Brevo data are committed.
- No application code is changed.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- `ai/reports/identity-email-foundation.md` exists and covers all required sections.
- No secrets are committed.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `docs: design identity and Brevo email foundation`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
