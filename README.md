# VoidEmpires

VoidEmpires is a game-project repository currently in Phase 0 planning and bootstrap setup.

The repository does not contain production application code yet. Its current purpose is to define the project direction, establish the AI-assisted task workflow, and prepare the first `.NET` solution bootstrap task.

## Current Status

- AI Platform workflow assets are installed under `ai/`
- repository context, roadmap, and architecture planning documents are in place
- the first solution bootstrap plan exists under `ai/reports/solution-bootstrap-plan.md`
- no `.NET` solution or application projects have been created yet

## Key Documents

- `AGENTS.md`: repository workflow rules for task execution
- `ai/repo-context.md`: project purpose, product assumptions, workflow, and constraints
- `ai/current-state.md`: current repository reality and Phase 0 status
- `ai/roadmap.md`: high-level delivery phases for VoidEmpires
- `ai/architecture-index.md`: intended module boundaries for the future solution
- `ai/reports/solution-bootstrap-plan.md`: actionable plan for the first implementation task

## Workflow

Tasks are processed from `ai/tasks/pending` and moved through the lifecycle directories under `ai/tasks/`.

The current operating model is:

`Context -> Roadmap -> Task -> Implementation -> Validation -> Commit -> Push`

## Next Step

The next planned implementation milestone is to create the initial `.NET` solution named `VoidEmpires` using the bootstrap plan already documented in `ai/reports/solution-bootstrap-plan.md`.
