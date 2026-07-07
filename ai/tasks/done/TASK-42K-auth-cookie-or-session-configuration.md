# TASK-42K-auth-cookie-or-session-configuration

---
id: TASK-42K
title: Auth cookie or session configuration
status: done
type: backend
team: platform
supporting_teams: [frontend]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Configure and document the auth/session approach for SPA use.

## Context
Prefer an HTTP-only auth cookie when feasible. Any session choice must be explicit, secure by default, and compatible with the local Vite frontend in Development without committing secrets.

## Implementation steps

1. Review ASP.NET Core Identity registration, existing CORS, and frontend API base URL configuration.
2. Choose and document the session approach: HTTP-only cookie, Identity API endpoint behavior, or another safe Identity-backed option.
3. Configure cookie/CORS/credentials only as required for local Vite development.
4. Keep production-safe defaults and avoid secrets in configuration.
5. Add non-browser tests or configuration tests where practical.
6. Update docs with the final auth/session choice and tradeoffs.

## Files to read first

- src/VoidEmpires.Web/Program.cs
- src/VoidEmpires.Web/appsettings.json
- src/VoidEmpires.Web/appsettings.Development.json
- src/VoidEmpires.Frontend/src/config.ts
- docs/dev/user-account-auth-readiness.md

## Expected files to modify

- src/VoidEmpires.Web/Program.cs
- docs/dev/user-account-auth-readiness.md
- tests/VoidEmpires.Tests/DevelopmentCorsEndpointTests.cs

## Acceptance criteria

- Auth/session approach is explicit in code and docs.
- SPA-compatible local development is supported without exposing secrets.
- Tests do not require a browser.
- No localStorage auth token is introduced.

## Constraints

- Do not weaken cookie security for production.
- Do not commit secrets or real origins beyond local development defaults.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
