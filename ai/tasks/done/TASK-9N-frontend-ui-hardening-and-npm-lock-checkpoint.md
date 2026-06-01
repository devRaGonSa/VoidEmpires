# TASK-9N

---
id: TASK-9N
title: Add frontend UI hardening and npm lock checkpoint
status: done
type: hardening
team: frontend
supporting_teams:
  - docs
  - architecture
roadmap_item: "Phase 9N - Frontend UI hardening and npm lock checkpoint"
priority: high
---

## Goal

Add a frontend UI hardening checkpoint after Figma alignment and resolve npm package-lock tracking deterministically.

This task should primarily harden docs, smoke validation, accessibility and readability basics, and dependency reproducibility.

It must not add gameplay mutations, production auth, backend endpoints, WebSockets, Three.js/WebGL, or final gameplay UI.

## Context

During Phase 9G-9J validation, `npm install` generated:

- `src/VoidEmpires.Frontend/package-lock.json`

It was reported as untracked. For deterministic npm installs, decide and implement the correct repository policy. For this project, the recommended policy is to commit `package-lock.json` for the frontend app.

This task should also document the Figma alignment status and smoke checks.

## Implementation steps

1. Inspect the frontend package:
   - `src/VoidEmpires.Frontend/package.json`
   - `src/VoidEmpires.Frontend/package-lock.json` if present locally
   - `.gitignore`
2. Ensure npm lock policy is deterministic:
   - commit `src/VoidEmpires.Frontend/package-lock.json` if it exists and is valid
   - if the lock file is not present in the runner environment, run `npm install` in `src/VoidEmpires.Frontend` to generate it
   - do not commit `node_modules` or `dist`
3. Update `.gitignore` only if needed:
   - ignore `node_modules`
   - ignore `dist`
   - do not ignore `package-lock.json`
4. Add or update frontend smoke docs:
   - Figma token alignment present
   - shell, sidebar, and topbar aligned
   - map and fleet panels aligned
   - npm install and build workflow
   - no mutating controls
5. Add basic accessibility and readability hardening where low-risk:
   - buttons and interactive elements have accessible labels where applicable
   - disabled placeholder nav entries are clear
   - contrast uses Figma tokens
6. Update `ai/current-state.md` to document Phase 9N and the final baseline.
7. Do not introduce new dependencies unless strictly necessary.

## Files to read first

- `src/VoidEmpires.Frontend/package.json`
- `src/VoidEmpires.Frontend/package-lock.json` if present
- `.gitignore`
- `src/VoidEmpires.Frontend/README.md`
- `docs/dev/frontend-figma-alignment.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/pre-frontend-contract-checkpoint.md`
- `ai/current-state.md`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Frontend/package-lock.json`
- `.gitignore` if needed
- `src/VoidEmpires.Frontend/README.md`
- `docs/dev/frontend-figma-alignment.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/pre-frontend-contract-checkpoint.md`
- `ai/current-state.md`

May also modify:

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- `package-lock.json` is tracked for the frontend app.
- `node_modules` and `dist` remain untracked and ignored.
- Frontend README documents `npm install` and `npm run build`.
- Docs document Figma alignment status.
- Smoke checklist covers Figma shell, map, fleet panels, and visual-state preview.
- No mutating gameplay calls are added.
- No backend code is changed.
- Backend validation remains green.
- Frontend build passes.
- `ai/current-state.md` documents Phase 9N and the final baseline.

## Constraints

- Do not add gameplay mutations.
- Do not call mutating endpoints.
- Do not add production auth.
- Do not add backend endpoints.
- Do not add backend gameplay changes.
- Do not add WebSockets.
- Do not add Three.js/WebGL.
- Do not commit `node_modules`.
- Do not commit `dist`.
- Do not ignore `package-lock.json`.
- Keep dependency changes minimal.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Also run frontend validation:

```powershell
cd src/VoidEmpires.Frontend
npm install
npm run build
cd ../..
```

Expected result:

- backend clean build
- backend tests passing
- frontend build passing
- `package-lock.json` tracked
- `node_modules` and `dist` not tracked

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify no `node_modules` or `dist` files are tracked.
4. Verify `package-lock.json` is tracked.
5. Commit with a clear message: `chore(frontend): harden figma ui foundation`
6. Push the branch to the remote.

The AI Platform runner must push after successful validation.
