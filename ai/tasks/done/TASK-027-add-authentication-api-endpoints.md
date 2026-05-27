# TASK-027

---
id: TASK-027
title: Add minimal authentication API endpoints
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: medium
---

## Goal
Expose minimal API endpoints for registration and email confirmation.

## Context
The service layer now supports registration and email confirmation. VoidEmpires needs minimal backend endpoints to allow future UI integration and manual testing. Keep the endpoints simple and API-first.

## Implementation steps

1. Modify `src/VoidEmpires.Web`.
2. Modify tests.
3. Add minimal endpoints such as `POST /api/auth/register` and `GET` or `POST /api/auth/confirm-email`.
4. Validate request bodies.
5. Do not expose sensitive Identity internals.
6. Do not return raw confirmation tokens except in development or test-only paths if absolutely necessary.
7. Do not log passwords or tokens.
8. Keep API responses deterministic.
9. Override services with fakes in tests where needed.
10. Do not add a complex UI.
11. Do not add gameplay systems.
12. Do not call real Brevo in tests.
13. Do not connect tests to the real PostgreSQL database.

## Files to read first

- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`
- `src/VoidEmpires.Application/*`
- `ai/reports/identity-email-foundation.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- The authentication endpoints exist and are minimal.
- Invalid registration input returns HTTP 400.
- Valid registration invokes the service or fake service.
- Confirmation endpoint handles missing or invalid input.
- Tests do not call real Brevo or PostgreSQL.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Endpoint tests pass.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(api): add minimal auth endpoints`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
