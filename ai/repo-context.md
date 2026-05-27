# Repository Context

## Project Overview

VoidEmpires is the future home of the `VoidEmpires` game and its supporting services.

At this stage the repository is still in Phase 0. It contains AI Platform workflow assets and project-planning documentation, but no production application code yet.

The current purpose of the repository is to define the product direction, architecture direction, and delivery workflow before the first .NET solution bootstrap task begins.

## Product Direction

VoidEmpires is being planned as a strategy game focused on building, expanding, and sustaining an empire in a hostile void setting.

Until gameplay specifications are written in more detail, future tasks should treat the following as the working product pillars:

- empire growth through expansion and territorial control
- strategic decision-making around economy, technology, and military power
- conflict between competing powers in a persistent science-fiction setting
- systems that can later support both moment-to-moment gameplay loops and longer-term progression

These pillars are planning assumptions for repository setup. They should be refined by later product and design tasks rather than treated as final game design.

## Technical Direction

The initial implementation direction is a .NET-based solution named `VoidEmpires`.

The early technical bias is:

- start with a small, modular solution rather than a monolith with premature infrastructure
- keep domain rules isolated from delivery and infrastructure concerns
- establish clear boundaries for gameplay/domain logic, application orchestration, infrastructure, and external interfaces
- prefer API-first and service-oriented seams so additional clients or tools can be added later
- document architecture decisions before introducing heavy dependencies

No production solution, projects, databases, or deployment assets exist yet. Those should be introduced only through explicit follow-up tasks.

## Workflow Expectations

This repository uses the AI Platform task lifecycle under `ai/tasks/`.

Expected operating model:

`Context -> Roadmap -> Task -> Implementation -> Validation -> Commit -> Push`

Rules for future work:

- process the first pending task first
- keep changes small and directly related to the active task
- prefer updating existing docs or components over creating parallel structures
- run repository-relevant validation for each task
- record durable project knowledge in `ai/` rather than leaving it implicit in commits

## Team Workflow

The documented team model under `ai/teams/` is used for planning and review guidance.

Current practical expectations:

- Platform owns repository scaffolding, workflow conventions, and bootstrap planning
- Product and Docs clarify game direction and durable repository knowledge
- Backend, Frontend, Database, DevOps, QA, and Security guidance become active once real application work starts

There is no automated team routing yet. Task metadata and human review still provide coordination.

## Constraints

Current constraints for repository work:

- the repository is still in documentation and bootstrap planning mode
- no application source code should be added until the bootstrap task explicitly requires it
- no `.NET` solution should be created before the planned bootstrap task
- workflow scripts should remain stable unless a task explicitly targets them
- repository context documents should stay deterministic, concise, and easy for future agents to reuse

## Immediate Priorities

The next planning milestones are:

1. replace generic template documentation with VoidEmpires-specific planning documents
2. define the initial roadmap and architecture index
3. prepare the first implementation plan for creating the `VoidEmpires` solution

Until those are complete, future tasks should avoid speculative implementation and focus on aligning repository guidance with the intended product direction.
