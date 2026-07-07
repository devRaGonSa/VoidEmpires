# TASK-42X-dev-qa-registration-helper

---
id: TASK-42X
title: Dev QA registration helper
status: done
type: tooling
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Add an optional QA script to register a test user safely.

## Context
The helper is for local development and QA convenience. It must not commit secrets or real credentials and should generate unique user data by default.

## Implementation steps

1. Review existing dev QA PowerShell script conventions and parser guard.
2. Add a script that accepts `BaseUrl` and optional user fields.
3. Generate unique email/name and a temporary password by default.
4. Do not print supplied passwords; if printing a generated password, clearly mark it local temporary output.
5. Call the registration endpoint and print safe resulting account/world summary.
6. Add the script to the dev QA script parser/check if required.

## Files to read first

- scripts/dev-qa-common.ps1
- scripts/dev-qa-baseline.ps1
- scripts/check-dev-qa-scripts.ps1
- scripts/dev-qa-prepare-playable-session-state.ps1
- src/VoidEmpires.Web/AccountEndpoints.cs

## Expected files to modify

- scripts/dev-qa-register-test-user.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/development-tools-action-inventory.md

## Acceptance criteria

- Script generates unique local test registration data by default.
- Script accepts `BaseUrl`.
- Script does not commit or require secrets.
- Parser/check script passes.

## Constraints

- Do not print real/supplied passwords.
- Do not expose this as normal player UI.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
