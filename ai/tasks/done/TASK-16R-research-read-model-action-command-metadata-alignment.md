# TASK-16R-research-read-model-action-command-metadata-alignment

---
id: TASK-16R-research-read-model-action-command-metadata-alignment
title: Research read model action-command metadata alignment
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: high
---

## Goal
Ensure the Research read model exposes the exact command metadata required for enqueue.

## Purpose
The frontend can only submit safely if the read model provides stable command metadata that matches the backend contract. If the UI reconstructs the command from display labels or partial state, it can submit the wrong payload even when the card looks available.

## Current Problem
The available Research card and confirmation panel are built from the read model, but the backend still rejects the request. That often happens when the read model omits stable command fields or when the frontend derives request data from presentation-only values.

## Context
- Other cockpit flows are most reliable when the read model carries the exact action metadata needed by the mutation endpoint.
- Research should follow the same pattern.
- Command metadata should be authoritative, not inferred from Spanish display copy.

## Files to Inspect First
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Infrastructure/Research/`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `tests/VoidEmpires.Tests/`

## Implementation Requirements
1. Inspect the current Research UI-state DTO.
2. Ensure each available research item includes stable enqueue metadata:
   - technology key or id expected by the enqueue endpoint;
   - target level;
   - civilization id;
   - planet id if required;
   - endpoint or action name if a manifest-like pattern exists;
   - mutability flag;
   - disabled reason if not available.
3. If the metadata already exists, ensure the frontend uses it directly.
4. If the metadata is missing, add it to the development read model.
5. Add tests proving that the available seeded item has metadata matching the enqueue endpoint contract.
6. Do not expose this as a production API.
7. Keep raw metadata secondary in UI diagnostics only.

## UI/UX Requirements
- Primary card labels remain Spanish.
- Diagnostics can show command metadata for developers.
- The user-facing UI must not depend on raw DTO names.

## Backend/API Requirements
- Add or adjust Development-only DTO fields if needed.
- Preserve backward compatibility where reasonable.
- Do not mark blocked items as mutable.

## Safety Constraints
- Do not infer command data from translated labels.
- Do not expose production-auth assumptions.
- Do not add hidden action tokens unless they are required and testable.

## Expected Files to Modify
- Backend Research UI-state model or query implementation if metadata is missing.
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- Tests that cover the available item contract

## Acceptance Criteria
- The available item carries the exact command metadata needed to enqueue.
- The frontend can submit using backend-provided metadata.
- Tests pass.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- This prevents future label/id drift.
- If the backend uses a manifest concept elsewhere, prefer reusing that pattern instead of inventing a second action schema.
- Keep the metadata explicit enough that a future request mismatch is easy to debug.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one read-model field extension rather than a new parallel contract.
