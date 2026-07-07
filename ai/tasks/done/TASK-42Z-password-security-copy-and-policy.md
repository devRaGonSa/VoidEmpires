# TASK-42Z-password-security-copy-and-policy

---
id: TASK-42Z
title: Password security copy and policy
status: done
type: fullstack
team: platform
supporting_teams: [frontend]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Document and expose a clear password policy.

## Context
Password policy must be enforced by the backend and explained clearly in the registration UI without logging or exposing passwords.

## Implementation steps

1. Review Identity password configuration and registration validation.
2. Ensure backend validation matches or delegates to the configured Identity password policy.
3. Add concise Spanish helper text to the registration UI.
4. Document the policy in auth/readiness docs.
5. Run secret scan to verify no credentials were introduced.

## Files to read first

- src/VoidEmpires.Web/Program.cs
- src/VoidEmpires.Application/Identity/AccountRegistrationValidator.cs
- src/VoidEmpires.Infrastructure/Identity/IdentityAccountRegistrationService.cs
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- src/VoidEmpires.Web/Program.cs
- src/VoidEmpires.Application/Identity/AccountRegistrationValidator.cs
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- docs/dev/user-account-auth-readiness.md

## Acceptance criteria

- Backend password policy is clear and tested.
- UI helper text matches the backend policy.
- No passwords are logged or committed.
- Secret scan remains green.

## Constraints

- Do not require email verification in this block.
- Do not expose technical Identity internals in primary UI.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
