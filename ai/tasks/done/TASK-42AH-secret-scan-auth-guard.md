# TASK-42AH-secret-scan-auth-guard

---
id: TASK-42AH
title: Secret scan auth guard
status: done
type: security
team: security
supporting_teams: [platform]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Ensure registration/auth work did not introduce secrets.

## Context
Auth work can accidentally introduce passwords, tokens, or real connection strings. The repository secret scan must remain green and should understand any safe local/generated test data.

## Implementation steps

1. Review the secret scan script and current auth-related changes.
2. Scan for passwords, tokens, connection strings, and committed credentials.
3. Tighten the guard if auth changes created new risky patterns.
4. Ensure generated test credentials are clearly fake/local and not real-looking secrets.
5. Run secret scan, build, and tests.

## Files to read first

- scripts/check-repo-secret-scan.ps1
- src/VoidEmpires.Web/appsettings.json
- src/VoidEmpires.Web/appsettings.Development.json
- scripts/dev-qa-register-test-user.ps1
- tests/VoidEmpires.Tests/AccountRegistrationEndpointTests.cs

## Expected files to modify

- scripts/check-repo-secret-scan.ps1
- docs/dev/final-db-security-notes.md

## Acceptance criteria

- Secret scan passes.
- No real passwords, tokens, or connection strings are committed.
- Any fake/generated test credentials are clearly local and safe.

## Constraints

- Do not print or commit real credentials.
- Do not weaken existing scan coverage.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
