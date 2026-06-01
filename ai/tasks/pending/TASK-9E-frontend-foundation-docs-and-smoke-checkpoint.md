# TASK-9E

---

id: TASK-9E
title: Add frontend foundation docs and smoke checkpoint
status: pending
type: hardening
team: frontend
supporting_teams:
  - backend
  - docs
  - tests
roadmap_item: "Phase 9E - Frontend foundation docs and smoke checkpoint"
priority: high

---

## Goal

Add documentation and smoke validation for the frontend foundation.

This task should verify that the first frontend slice is buildable, documented, and explicitly constrained as a dev/readiness prototype.

It must not add new gameplay behavior, production auth, backend endpoints, WebSockets, final UI, or complex 3D rendering.

## Context

The frontend foundation now includes:

* shell/routing/API client
* strategic map read slice
* fleet UI state read slice
* action manifest read-only panels

This checkpoint should make it easy to run and validate the frontend without confusing it with final production UI.

## Implementation steps

1. Add or update frontend README:

   * how to install dependencies
   * how to configure backend base URL
   * how to run dev server
   * how to build
   * which endpoints are consumed
   * current limitations
2. Add frontend smoke checklist doc or section:

   * backend dev endpoints must be enabled
   * sample civilization id required
   * strategic map page loads
   * fleet page loads
   * manifests load
   * no mutating buttons are present
3. Add package scripts if missing:

   * `build`
   * `typecheck` if practical
   * maybe `lint` only if dependencies are already included
4. Add a lightweight smoke test script only if it can run deterministically without a live backend.

   * Prefer build/typecheck over flaky browser tests.
5. Update root README or docs index if appropriate.
6. Update `docs/dev/pre-frontend-contract-checkpoint.md` with the frontend foundation status.
7. Update `ai/current-state.md` to document Phase 9E and final test/build baseline.

## Files to read first

* src/VoidEmpires.Frontend/package.json
* src/VoidEmpires.Frontend/README.md if it exists
* docs/dev/pre-frontend-contract-checkpoint.md
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md
* README.md

## Expected files to modify

Expected:

* src/VoidEmpires.Frontend/README.md
* src/VoidEmpires.Frontend/package.json if scripts need adjustment
* docs/dev/pre-frontend-contract-checkpoint.md
* ai/current-state.md

May also modify:

* README.md
* docs/dev/frontend-foundation-smoke-checklist.md
* src/VoidEmpires.Frontend/src/App.tsx only if a visible prototype limitation label is missing
* src/VoidEmpires.Frontend/src/styles.css only if needed for smoke/readability

## Acceptance criteria

* Frontend README exists and explains setup/run/build.
* Frontend limitations are explicit.
* Consumed dev endpoints are listed.
* Smoke checklist exists.
* Frontend build/typecheck passes where environment supports it.
* Backend validation remains green.
* No mutating gameplay calls are wired.
* `ai/current-state.md` documents Phase 9E and final baseline.

## Constraints

* Prefer docs/build-script hardening.
* Do not add gameplay mutations.
* Do not add production auth.
* Do not add backend endpoints.
* Do not add backend gameplay changes.
* Do not add WebSockets.
* Do not add final UI design.
* Do not add complex 3D rendering.
* Keep docs practical.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Also run frontend validation if npm is available:

```powershell
cd src/VoidEmpires.Frontend
npm install
npm run build
cd ../..
```

Expected result:

* backend clean build
* backend tests passing
* frontend build passing where environment supports it

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify changed files are expected frontend/docs/current-state unless small fixes were necessary.
4. Commit with a clear message, for example:
   `docs(frontend): add foundation smoke checkpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

