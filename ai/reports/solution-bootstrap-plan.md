# Solution Bootstrap Plan

## Objective

Prepare the first production-code task for a new `.NET` solution named `VoidEmpires` without implementing that solution yet.

The bootstrap task should create a small but durable starting point that matches the repository context, roadmap, and architecture index.

## Suggested Solution Name

`VoidEmpires`

## Proposed Initial Layout

```text
src/
  VoidEmpires.Api/
  VoidEmpires.Application/
  VoidEmpires.Domain/
  VoidEmpires.Infrastructure/
tests/
  VoidEmpires.Application.Tests/
  VoidEmpires.Domain.Tests/
```

## Initial Project Roles

- `VoidEmpires.Api`: first executable host, HTTP endpoints, dependency wiring, and health or diagnostics surface
- `VoidEmpires.Application`: use cases, commands, queries, validation, and orchestration boundaries
- `VoidEmpires.Domain`: core domain types, invariants, and gameplay concepts introduced in the smallest possible form
- `VoidEmpires.Infrastructure`: persistence and integration seam definitions or first implementations that the API host can compose later
- `VoidEmpires.Application.Tests`: tests for application behaviors and orchestration rules
- `VoidEmpires.Domain.Tests`: tests for domain invariants and gameplay rule primitives

## Architecture Direction

The bootstrap task should establish the architectural rules early instead of postponing them:

- references should flow inward toward the domain
- `Api` may depend on `Application` and `Infrastructure`
- `Application` may depend on `Domain`
- `Infrastructure` may depend on `Application` and `Domain` only when required by interface or mapping needs
- `Domain` should not depend on delivery or infrastructure projects

If the first task needs dependency injection setup, keep it in the host project and avoid spreading composition logic across the solution.

## Initial Capabilities

The first bootstrap implementation should stay intentionally small.

Recommended capabilities:

1. create the solution and projects with valid references
2. make the solution restore and build successfully
3. expose a minimal API host that starts cleanly
4. add one health or readiness endpoint
5. add one minimal domain concept placeholder that proves the project boundaries compile
6. add one small unit test per test project to prove the test infrastructure is wired

The bootstrap task should not introduce real persistence, authentication, multiplayer, or gameplay complexity yet.

## Suggested First Domain Slice

Use a very small neutral slice such as `EmpireName`, `EmpireId`, or `EmpireSummary` to prove the separation between host, application, and domain layers.

Avoid creating large aggregate roots or speculative data models in the bootstrap task.

## Validation Commands

Once the bootstrap task creates the solution, the expected validation should be:

```powershell
dotnet restore
dotnet build
dotnet test
```

If additional repository-specific validation is added during bootstrap, it should remain lightweight and deterministic.

## First Implementation Task Recommendation

The next executable task should:

1. create the `VoidEmpires` solution and the six initial projects
2. add the minimum viable project references to enforce the intended dependency direction
3. implement a minimal API startup path and health endpoint
4. add a tiny domain primitive plus corresponding unit tests
5. document any deviations from this plan directly in `ai/current-state.md` or follow-up tasks

## Risks

- The first bootstrap task could over-design the solution and introduce empty projects or abstractions with no immediate value.
- Infrastructure concerns could leak into the domain if project references are added carelessly.
- A larger gameplay model could be introduced too early, making Phase 1 harder to validate.
- Test projects could be skipped, which would weaken the baseline for later gameplay work.
- The first host could become a monolithic dumping ground if dependency wiring is not kept explicit and small.

## Non-Goals For Bootstrap

- real database integration
- user authentication
- multiplayer coordination
- background world simulation
- admin tooling
- production deployment assets
- detailed gameplay content

## Definition Of Done For The Future Bootstrap Task

The future bootstrap implementation should be considered complete only if:

- the solution exists and builds successfully
- tests run successfully
- project references reflect the intended architecture
- the API host starts with a minimal endpoint
- no speculative subsystems are added beyond the bootstrap scope
