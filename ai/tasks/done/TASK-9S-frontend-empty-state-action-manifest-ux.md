# TASK-9S

---
id: TASK-9S
title: Phase 9S - Frontend empty-state and action manifest UX hardening
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Phase 9S - Frontend empty-state and action manifest UX hardening"
priority: high
---

## Goal

Improve the Galaxia and Flotas frontend development screens now that backend, PostgreSQL, migrations, and CORS are working, with focus on empty-state clarity and action manifest rendering quality.

## Context

Current validated state:

- Backend starts successfully against PostgreSQL development database `VoidEmpireDB_Dev`.
- Latest EF catch-up migration has been applied manually to `VoidEmpireDB_Dev`.
- Direct backend dev endpoints return controlled JSON.
- Development CORS is fixed.
- Frontend Vite can call backend without `NetworkError`.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes with `533/533` tests.
- `ai/tasks/pending` is empty.

Manual frontend validation:

- Galaxia and Flotas now load through the frontend from `VITE_VOIDEMPIRES_API_BASE_URL=http://localhost:5142`.

Observed UX issues:

1. The frontend no longer has `NetworkError`, but empty development database states need clearer UI.
2. Galaxia correctly shows `0` systems, `0` planets, `0` fleet markers, and similar empty values, but the copy can be improved.
3. Flotas shows valid action manifest and readiness data, but some fields render poorly:
   - required fields appear as `[object Object]` or `[objeto Objeto]`
   - HTTP methods appear translated incorrectly, for example:
     - `GET` displayed as `CONSEGUIR`
     - `POST` displayed as `CORREO`
   - action manifest cards are too verbose and difficult to scan
   - read-only actions and mutation actions should be visually separated more clearly
4. This is a development-only command surface. It must remain conservative and must not enable gameplay mutations from the UI.

## Implementation steps

1. Inspect frontend files under `src/VoidEmpires.Frontend`.
2. Identify the components or pages for:
   - Galaxia / strategic map screen
   - Flotas / fleet UI state screen
   - action manifest rendering
3. Add small reusable formatting helpers if appropriate:
   - `formatHttpMethod`
   - `formatRequiredField`
   - `formatRequiredFields`
   - `formatManifestActionType`
   - `safeCompactJson` or equivalent
4. Ensure the UI remains stable when arrays are empty:
   - `systems: []`
   - `orbital groups: []`
   - `transfers: []`
   - `resource contexts: []`
   - action hints present
5. Preserve current API contracts.
6. Add or update frontend tests if the project has a test setup.
   - If no frontend test setup exists, do not introduce a large new test framework in this task.
   - Prefer lightweight component or helper tests only if existing tooling supports them.
7. Run validation:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm install if needed
npm run build from src/VoidEmpires.Frontend
```

8. Move the task to `ai/tasks/done` only after validation passes.
9. Leave unrelated untracked files untouched, especially `xuniverse_planet_generator_variety.py`.

## Files to read first

- `src/VoidEmpires.Frontend`
- `src/VoidEmpires.Frontend/*`
- frontend files related to Galaxia, Flotas, and action manifest rendering
- `AGENTS.md`

## Expected files to modify

Expected:

- frontend UI and formatting components under `src/VoidEmpires.Frontend`
- frontend tests only if the existing setup supports them

## Acceptance criteria

- Galaxia empty state clearly explains that no relevant systems were found for the civilization yet.
- Flotas empty state clearly explains that there are no orbital groups, transfers, or resource contexts yet.
- Action manifest fields no longer render raw object stringification such as `[object Object]`.
- HTTP methods are shown correctly and do not get corrupted by locale translation.
- Read-only and mutation action manifest entries are easier to scan.
- Mutation actions remain metadata only and are not executable from this prototype.
- Existing shell language and visual direction remain aligned with the current frontend.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes with `533/533` expected or more if legitimate tests are added.
- `npm run build` from `src/VoidEmpires.Frontend` passes.
- No database migrations are applied.
- No backend behavior changes are introduced unless strictly necessary.

## Constraints

- Do not commit secrets.
- Do not change database connection strings.
- Do not apply EF migrations.
- Do not change backend behavior unless strictly necessary.
- Do not loosen CORS.
- Do not add gameplay mutations.
- Do not add real command execution controls.
- Do not add production authentication.
- Keep the change incremental and reviewable.
- Preserve the current backend test baseline.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Expected:

- backend build passes
- backend tests pass
- frontend build passes
- no new warnings or obvious regressions are introduced

## Manual validation note

After implementation, the user will start:

- backend with `ASPNETCORE_ENVIRONMENT=Development`
- `ConnectionStrings__DefaultConnection` pointing to `VoidEmpireDB_Dev`
- frontend with `VITE_VOIDEMPIRES_API_BASE_URL=http://localhost:5142` and `npm run dev`

Then validate:

- Galaxia loads without `NetworkError`
- Galaxia empty state is clear
- Flotas loads without `NetworkError`
- Flotas no longer shows `[object Object]` or `[objeto Objeto]`
- HTTP methods are shown correctly
- read-only vs mutation manifest entries are easier to scan
- no gameplay mutation is executable from the UI

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only the intended frontend files and tests changed.
4. Commit with a clear message.
5. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
