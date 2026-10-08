# `ai-platform plan`

## Command purpose

Convert roadmap items or functional requests into detailed task files in `ai/tasks/pending`.

The current CLI `plan` command is only a simple guidance helper. This spec describes a future roadmap-driven task generation command.

## Teams

- Primary team: product
- Supporting teams: orchestration, docs, qa

## Inputs

- roadmap item ID or functional request
- `ai/roadmap.md`
- `ai/current-state.md`
- `ai/task-template.md`
- `ai/teams/`
- `ai/project-memory/`
- relevant repository evidence

## Outputs

- One or more Markdown task files in `ai/tasks/pending`.
- A planning summary listing generated tasks, assumptions, and non-goals.

## Task format expected

Generated tasks must follow `ai/task-template.md` and be implementation-ready rather than terse summaries.

Every generated task must include enough detail to preserve the decisions made during planning. At minimum include:

- `team`: one primary team.
- `supporting_teams`: optional list of directly involved teams.
- a multi-paragraph Goal describing the intended end state and rationale;
- current-behavior/problem context with concrete repository evidence;
- desired behavior and authoritative ownership;
- architectural decisions inherited from predecessor tasks;
- explicit in-scope and out-of-scope boundaries;
- detailed implementation requirements;
- ordered implementation steps that explain data flow, invariants, failure handling, and integration points;
- files to read first, with rationale where useful;
- expected files to modify and the purpose of each change;
- edge cases and failure modes;
- a detailed test plan;
- specific acceptance criteria;
- validation plan;
- change-budget/review notes.

A task is not ready merely because it has a title, a one-line Goal, and three generic steps. Another engineer or agent should be able to execute it without inventing product or architecture decisions that planning already knew.

## Splitting and generation rules

- Prefer one cohesive responsibility per task.
- File count, changed-line count, and commit count are review signals, not hard limits.
- Do not create extra tasks solely to stay below an arbitrary line/file threshold.
- Split when responsibilities are independently deliverable, cross unrelated architecture boundaries, can be safely deferred, or introduce a concrete review/rollback/regression risk.
- Do not generate more than three unplanned follow-up tasks at once unless explicitly requested.
- Do not create duplicate tasks for work already pending, in progress, or done.
- Do not generate tasks without enough context to make them implementation-ready.
- Prefer fewer, richer tasks over many superficial tasks that merely restate method names or filenames.

## Acceptance criteria

- Creates task files only under `ai/tasks/pending`.
- Assigns clear `team` and `supporting_teams`.
- Keeps tasks reviewable and scoped.
- Distinguishes roadmap-driven planning from the current simple CLI helper.
