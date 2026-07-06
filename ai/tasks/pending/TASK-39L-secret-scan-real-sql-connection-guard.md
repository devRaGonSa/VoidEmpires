# TASK-39L

---
id: TASK-39L
title: Strengthen secret scan for real SQL Server connection strings
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Strengthen repository secret scanning so accidental real SQL Server credentials or connection strings are caught before commit.

## Context

The repo must allow placeholder examples while rejecting real-looking SQL Server passwords, especially near the known target server and appsettings files.

## Implementation steps

1. Read the current secret scan script and QA script.
2. Ensure `scripts/check-repo-secret-scan.ps1` catches:
   - `Password=` followed by a non-placeholder value;
   - `User Id=sa` with a real-looking `Password=`;
   - `192.168.178.28` with a password on the same line;
   - connection strings in appsettings files with real passwords.
3. Allow safe placeholders:
   - `<PASSWORD>`
   - `YOUR_PASSWORD`
   - `${SQL_PASSWORD}`
   - `%SQL_PASSWORD%`
4. Avoid false positives in guard scripts themselves, either by excluding self-test text safely or by structuring patterns so the guard can contain its own rules.
5. Validate with existing QA and copy regression scripts.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- scripts/check-repo-secret-scan.ps1
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Web/appsettings.json

## Expected files to modify

- scripts/check-repo-secret-scan.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/final-db-security-notes.md

## Acceptance criteria

- Real-looking SQL Server passwords are rejected.
- Approved placeholders are allowed.
- Known target server plus same-line password is rejected.
- The guard avoids false positives from its own implementation text.
- Existing QA scripts pass.

## Constraints

- Do not commit real secrets.
- Do not weaken existing secret scan coverage.
- Keep the change narrowly scoped to guard behavior and documentation.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `git diff --stat`
- `git diff --name-only`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote if the branch is configured for remote collaboration.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.
