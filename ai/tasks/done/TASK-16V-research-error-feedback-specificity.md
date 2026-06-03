# TASK-16V-research-error-feedback-specificity

---
id: TASK-16V-research-error-feedback-specificity
title: Research error feedback specificity
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Replace the generic `API rejected by validation` message with specific, actionable Spanish error messages.

## Purpose
For QA and development, it must be obvious whether the failure is due to resources, queue state, ownership, technology id, target level, or contract mismatch. A generic validation rejection does not help the user recover or help us identify the fault.

## Current Problem
The current submit failure is too generic:

- `La API rechazo la solicitud por validacion. Revisa los requisitos y vuelve a preparar la accion.`

That message does not say what failed or how to correct it.

## Context
- Research already has error mapping, but this specific submit failure is still too vague.
- The primary UI should surface backend reason codes clearly while keeping raw payloads in diagnostics.
- If the backend currently only returns a generic code, the backend should be improved rather than hiding the problem in the UI.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- Backend Research result and error code types

## Implementation Requirements
1. Map enqueue errors to specific Spanish messages, such as:
   - `Tecnologia no encontrada.`
   - `El nivel objetivo no es valido.`
   - `El planeta no pertenece a la civilizacion.`
   - `La cola de investigacion no admite nuevas ordenes.`
   - `Recursos insuficientes en Aurelia.`
   - `La tecnologia ya esta en investigacion.`
   - `La tecnologia ya esta completada.`
   - `El contrato de envio no coincide con la accion preparada.`
2. If the backend only returns a generic error, improve backend result codes if it can be done safely.
3. Keep raw technical details in diagnostics.
4. Make the confirmation panel display the specific failure near the action.

## UI/UX Requirements
- Error text must be short but actionable.
- No English backend message should appear as the first-level UI message when a Spanish equivalent is available.
- The message should tell the user what to do next.

## Backend/API Requirements
- Add tests if result codes are added or changed.
- Do not weaken validation.
- Do not turn a valid rejection into a UI-only warning.

## Safety Constraints
- Do not treat error mapping as a successful action.
- Do not hide backend rejection.
- Do not pretend a contract mismatch is a resource problem unless the code proves it.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- Backend result-code files and tests if needed

## Acceptance Criteria
- The previous generic error is replaced by a specific message.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.
- Backend tests pass if the backend changed.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made.

## Notes / Residual Risks
- Specific errors reduce future debugging time.
- Keep the mapper easy to extend as new Research reasons appear.
- If a new code is introduced, document it once and reuse it across the cockpit.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a shared error mapper over inline string checks.
