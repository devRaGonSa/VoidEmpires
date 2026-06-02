# TASK-9R

---
id: TASK-9R
title: Phase 9R - Dev CORS frontend-backend local integration fix
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Phase 9R - Dev CORS frontend-backend local integration fix"
priority: high
---

## Goal

Allow the local Vite frontend development server to call `VoidEmpires.Web` development endpoints without `NetworkError`, while keeping CORS conservative and limited to local development origins.

## Context

Current state:

- `main` includes Phase `9P` EF current schema catch-up migration.
- `main` includes Phase `9Q` test isolation fixes.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes with `528/528` tests.
- The PostgreSQL development database `VoidEmpireDB_Dev` is working.
- The latest migration `20260601165609_AddCurrentGameplaySchemaCatchup` was manually applied to `VoidEmpireDB_Dev`.
- Direct browser calls to the development backend endpoints succeed and return controlled JSON:
  - `http://localhost:5142/api/dev/strategic-map?civilizationId=00000000-0000-0000-0000-000000000001`
  - `http://localhost:5142/api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001`
- The React/Vite frontend starts on `http://localhost:5173` with:
  - `VITE_VOIDEMPIRES_API_BASE_URL=http://localhost:5142`

Current problem:

- From the frontend UI, both Galaxia and Flotas show `NetworkError when attempting to fetch resource`.
- The same backend URLs work when opened directly in the browser.
- This strongly indicates local frontend-backend CORS is missing or incomplete for the Vite dev origin.

## Implementation steps

1. Inspect:
   - `src/VoidEmpires.Web/Program.cs`
   - existing endpoint tests using `WebApplicationFactory`
   - frontend API client configuration only if needed, but avoid frontend changes unless strictly necessary
2. Add a named CORS policy for local frontend development.
3. Allow only these origins:
   - `http://localhost:5173`
   - `http://127.0.0.1:5173`
4. Apply the policy only when development endpoints are enabled or when the ASP.NET Core environment is `Development`.
5. Keep the policy conservative:
   - allow standard methods used by current frontend reads
   - allow required headers
   - do not use `AllowAnyOrigin` globally
   - do not enable credentials unless there is a concrete requirement already present in current frontend behavior
6. Ensure middleware ordering is correct in `Program.cs`.
7. Add or update backend tests proving:
   - allowed Vite origins receive the expected CORS headers for dev endpoint requests and preflight where appropriate
   - disallowed origins are not broadly allowed
   - existing health, auth, and dev endpoint behavior remains intact
8. Do not touch gameplay behavior.
9. Do not modify PostgreSQL or migrations.
10. Do not apply database migrations.
11. Do not add auth production flows.
12. Do not add WebSockets.
13. Do not add Three.js/WebGL.
14. Do not convert dev read-only frontend actions into gameplay mutations.
15. Run validation:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

16. Move this task from `ai/tasks/pending` to `ai/tasks/done` only after validation passes.
17. Leave unrelated untracked files untouched, especially `xuniverse_planet_generator_variety.py`.

## Files to read first

- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/`
- `tests/VoidEmpires.Tests/*EndpointTests.cs`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Web/Program.cs`
- backend tests under `tests/VoidEmpires.Tests/` for CORS behavior

## Acceptance criteria

- The local Vite origin can call the development backend without `NetworkError`.
- The CORS policy is limited to local development use and does not loosen production behavior.
- Allowed origins are restricted to `http://localhost:5173` and `http://127.0.0.1:5173`.
- Disallowed origins are not broadly allowed.
- Existing health, auth, and dev endpoint behavior remains intact.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes, with `528/528` expected or more if new tests are added.
- No database migrations are applied.
- No gameplay contracts are changed.

## Constraints

- Do not commit secrets.
- Do not hardcode database passwords or local private IPs.
- Do not apply EF migrations.
- Do not loosen CORS globally.
- Do not enable credentials unless there is a concrete requirement.
- Do not alter gameplay contracts.
- Do not touch production deployment assumptions.
- Preserve the current successful test baseline.
- Keep the change minimal and backend-focused.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

Expected:

- build passes
- all tests pass
- no new warnings or obvious regressions are introduced

## Manual validation note

After the fix is implemented, the user will validate by:

1. Starting the backend with:
   - `ASPNETCORE_ENVIRONMENT=Development`
   - `ConnectionStrings__DefaultConnection` pointing to `VoidEmpireDB_Dev`
2. Starting the frontend with:
   - `VITE_VOIDEMPIRES_API_BASE_URL=http://localhost:5142`
   - `npm run dev`
3. Opening:
   - `http://localhost:5173`
4. Verifying Galaxia and Flotas:
   - no `NetworkError`
   - no `503`
   - no missing-table error
   - empty or controlled payload is rendered cleanly

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only the intended backend files and tests changed.
4. Commit with a clear message.
5. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
