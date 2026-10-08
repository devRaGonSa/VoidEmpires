# TASK-XXX

---
id: TASK-XXX
title: Clear, concise task title
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
execution_order:
dependencies: []
---

Allowed task statuses:

- `pending`
- `in-progress`
- `review`
- `done`
- `blocked`
- `obsolete`

## Goal

Describe the intended end state in enough detail that the implementation target is unambiguous.

Explain:
- what behavior or capability will exist when the task is complete;
- why this change is needed now;
- which authoritative component should own the behavior;
- what important behavior must remain unchanged.

Do not reduce the Goal to a one-line "implement X" statement when there are meaningful domain, persistence, API, UI, lifecycle, compatibility, or validation implications.

## Problem statement / current behavior

Describe the relevant current repository behavior before the change.

Include, when applicable:
- the current data flow;
- existing entities/services/endpoints/components involved;
- known limitations or placeholders;
- relevant behavior inherited from predecessor tasks;
- why the current state is insufficient for the requested capability.

Use concrete repository terminology and file/component names.

## Desired behavior

Describe the target behavior from the perspective of the authoritative backend/domain and, where applicable, the user-facing flow.

Make explicit:
- normal happy-path behavior;
- ownership of state and time;
- lifecycle/state transitions;
- persistence expectations;
- authorization/visibility boundaries;
- idempotency/concurrency expectations;
- compatibility with existing behavior;
- what should happen on invalid input or partial failure.

## Context and architectural decisions

Record the architectural context that the implementation must preserve.

Include:
- completed predecessor tasks and decisions that govern this task;
- relevant boundaries between Domain, Application, Infrastructure, Web, Frontend, workers, scripts, and persistence;
- components that must be reused rather than duplicated;
- naming or migration/cutover decisions already made;
- provider/runtime constraints;
- assumptions that are intentionally deferred to later tasks.

The task should not force the implementation agent to rediscover decisions that are already known.

## Scope

### In scope

List the concrete responsibilities this task owns.

### Out of scope

List adjacent behavior that must NOT be implemented here, especially functionality assigned to later tasks.

## Detailed implementation requirements

Describe the implementation contract in detail.

Use subsections when helpful, for example:
- Domain model and invariants
- Application contracts
- Persistence and EF configuration
- Services and transactions
- API behavior
- Authorization and visibility
- Frontend/read-model behavior
- Background processing
- Compatibility and migration
- Error handling and recovery
- Idempotency and concurrency

State required fields, enums, transitions, validation rules, calculations, keys, indexes, relationships, or request/response semantics explicitly when known.

If a design choice has already been made, state it as a requirement rather than leaving it open-ended.

## Implementation steps

Provide an ordered implementation plan with substantive steps.

Each step should explain:
1. what to inspect or change;
2. how it fits the existing architecture;
3. which invariants or compatibility rules must be preserved;
4. which tests or validation demonstrate that the step is correct.

Prefer enough detail that another engineer or agent can execute the work without inventing missing requirements.

## Files to read first

List the most relevant files that should be inspected before implementing the task.

For each file, briefly state why it matters when that is not obvious.

Examples:

- `src/FeatureModule/FeatureService.cs` — current authoritative behavior to extend.
- `src/Infrastructure/Persistence/Configurations/FeatureConfiguration.cs` — existing EF conventions.
- `tests/FeatureServiceTests.cs` — current invariants and regression expectations.
- `ai/orchestrator/di-analysis.md` — required before composition-root or service-registration changes.

Rules:

- Read these files before making changes.
- Include current implementation, related configuration, nearby tests, and governing architecture docs.
- Keep the list relevant, but do not omit a necessary source merely to satisfy an arbitrary count.

## Expected files to modify

List the files expected to change and explain the purpose of each change.

Rules:

- Modify only files justified by the task.
- Additional files are allowed when genuinely required by the cohesive implementation; explain why.
- Do not modify unrelated files.
- File-count and line-count totals are review signals, not hard limits.

## Edge cases and failure modes

Document the important non-happy-path behavior.

Consider, where relevant:
- invalid identifiers or enum values;
- zero/negative/overflow values;
- stale state;
- repeated requests;
- conflicting transitions;
- missing persistence rows;
- authorization failures;
- cross-player isolation;
- partial failures/rollback;
- concurrency races;
- UTC/time-boundary behavior;
- legacy data or compatibility states.

If an edge case is intentionally deferred, name the later task or explicitly state the deferral.

## Test plan

Describe the required automated coverage in detail.

Include:
- focused unit/domain tests;
- persistence/model tests;
- endpoint/service tests;
- happy path;
- edge cases;
- idempotent repeats;
- regressions from predecessor behavior;
- authorization/isolation where applicable;
- integration or relational evidence when actually configured.

Do not claim EF InMemory proves relational concurrency or provider-specific SQL behavior.

## Acceptance criteria

Use concrete, observable criteria.

A good acceptance section should make it possible to decide whether the task is done without interpretation.

Include:
- required resulting behavior;
- required invariants;
- required persistence/API/UI results;
- required regression behavior;
- required tests;
- explicit exclusions that must remain absent.

Avoid generic-only criteria such as "implementation complete" or "tests pass".

## Constraints

List task-specific constraints in addition to repository-wide rules.

Always preserve:
- existing architecture and repository conventions;
- backend authority for gameplay state;
- no unrelated refactors;
- no secrets;
- no automatic migration/SQL/seed application unless explicitly authorized;
- provider-independent ordinary tests where the repository requires them.

## Validation

Before completing the task, run the repository-relevant validation and report actual results.

Typical validation:
- `dotnet restore`
- `dotnet build --no-restore`
- focused tests for this task
- `dotnet test --no-build`
- frontend build/guards when frontend is changed
- repository scripts relevant to the modified surface
- integration tests only when genuinely configured

Also run:
- `git diff --stat`
- `git diff --name-only`
- `git status`

Report actual counts/results rather than reusing historical baselines.

## Commit and push

At the end:

1. Verify the intended task lifecycle transition.
2. Stage only files justified by the task.
3. Commit with a clear focused message.
4. Push the configured feature branch.
5. Do not continue to another task when the execution instruction says to stop.

## Change Budget

- Keep the implementation focused on one cohesive responsibility.
- Treat file count, changed-line count, and commit count as review signals, not hard limits.
- Do not split a cohesive implementation solely because it exceeds an arbitrary numeric threshold.
- Keep directly related tests, EF configuration, documentation, and lifecycle changes with the implementation they validate.
- Split only for separable responsibilities, unrelated architectural scope, safely deferrable work, or a concrete review/rollback/regression risk.
- Run `git diff --stat` and `git diff --name-only` before completion and justify every changed path.

## Completion report

When the task is finished, report:
- branch and commit SHA;
- files changed;
- lifecycle state;
- behavior implemented;
- important architecture decisions preserved;
- tests and validation results;
- warnings/errors;
- integration-test status;
- migrations/SQL/seed/manual-QA status when relevant;
- remaining dependent work or intentionally deferred decisions.
