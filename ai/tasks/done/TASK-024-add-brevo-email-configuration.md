# TASK-024

---
id: TASK-024
title: Add Brevo email configuration contract
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: high
---

## Goal
Add safe configuration conventions for Brevo transactional email without committing secrets.

## Context
Brevo will send user creation and email confirmation messages. The repository needs configuration sections and documentation but must not include API keys or SMTP secrets.

## Implementation steps

1. Update `src/VoidEmpires.Web/appsettings.json`.
2. Update `src/VoidEmpires.Web/appsettings.Development.json`.
3. Update `README.md`.
4. Update `ai/current-state.md` if useful.
5. Do not add Brevo implementation yet.
6. Do not add secrets.
7. Do not add registration endpoints.
8. Add a safe `Brevo` configuration section with placeholder values only.
9. Document the environment variable overrides for Brevo settings.
10. Document user-secrets examples without real values.
11. State that tests and CI must not call Brevo.

## Files to read first

- `src/VoidEmpires.Web/appsettings.json`
- `src/VoidEmpires.Web/appsettings.Development.json`
- `README.md`
- `ai/current-state.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Web/appsettings.json`
- `src/VoidEmpires.Web/appsettings.Development.json`
- `README.md`
- `ai/current-state.md` if needed

## Acceptance criteria

- The committed configuration files contain only safe placeholder values for Brevo settings.
- The repository documents safe override patterns and user-secrets examples.
- No real Brevo API key or SMTP secret is committed.
- Tests and CI remain network-free with respect to Brevo.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- No real Brevo API key or SMTP secret is committed.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `chore: add Brevo email configuration contract`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
