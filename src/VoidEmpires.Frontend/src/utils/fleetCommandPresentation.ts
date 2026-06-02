import type {
  CompleteOrbitalTransfersResponse,
  CreateOrbitalTransferResponse,
  EstimateOrbitalTravelResponse,
  FleetCommandApiResult,
} from "../api/fleetCommandTypes";
import type { ActionManifestAction } from "../api/actionManifestTypes";
import type { FleetActionHint, FleetGroupSummary, FleetUiState } from "../api/fleetTypes";

type CommandTone = "neutral" | "good" | "warn";

export interface FleetCommandPresentationItem {
  key: string;
  label: string;
  tone: CommandTone;
  summary: string;
  details: string[];
}

function getUnexpectedResponseSummary(result: FleetCommandApiResult<unknown>) {
  if (result.bodyParseFailed) {
    return `La API devolvio ${result.httpStatus} pero el cuerpo JSON no se pudo leer.`;
  }

  if (!result.hasJsonBody) {
    return `La API devolvio ${result.httpStatus} sin cuerpo JSON util para la UI.`;
  }

  return `La API devolvio ${result.httpStatus} con una forma de respuesta inesperada.`;
}

export interface FleetMutationConfirmationModel {
  actionKey: string;
  label: string;
  prototypeLevel: "prototype" | "danger";
  mutationSummary: string;
  readinessLabel: string;
  readinessTone: CommandTone;
  requiresConfirmation: boolean;
  confirmationText: string;
  disabledReason: string;
}

function getActionLabel(actionKey: string, actionHints: FleetActionHint[], fallback: string) {
  return actionHints.find((hint) => hint.actionKey === actionKey)?.displayName ?? fallback;
}

function normalizeNotes(notes: ActionManifestAction["notes"]) {
  if (!notes) {
    return [];
  }

  return Array.isArray(notes) ? notes : [notes];
}

export function buildFleetCommandReadiness(group: FleetGroupSummary, actionHints: FleetActionHint[]) {
  return [
    {
      key: "estimate",
      label: getActionLabel("fleet.travel.estimate", actionHints, "Travel preview"),
      tone: group.routeFuelReadiness?.canRequestTravelEstimate ? "good" : "warn",
      summary: group.routeFuelReadiness?.canRequestTravelEstimate ? "Preview ready" : "Preview blocked",
      details: [
        ...(group.routeFuelReadiness?.requiresDestination ? ["Destination context required."] : []),
        ...(group.routeFuelReadiness?.estimateRoute ? [`Route ${group.routeFuelReadiness.estimateRoute}`] : []),
        ...(group.routeFuelReadiness?.fuelReadinessPolicy ? [`Policy ${group.routeFuelReadiness.fuelReadinessPolicy}`] : []),
        ...(group.routeFuelReadiness?.notes ?? []),
      ],
    },
    {
      key: "create-transfer",
      label: getActionLabel("fleet.transfer.create", actionHints, "Create transfer"),
      tone: group.commands?.canCreateTransfer ? "good" : "warn",
      summary: group.commands?.canCreateTransfer ? "Ready to plan" : "Blocked by current fleet state",
      details: group.hasActiveTransfer ? ["Group already has an active transfer."] : [],
    },
    {
      key: "split",
      label: getActionLabel("fleet.group.split", actionHints, "Split group"),
      tone: group.commands?.canSplit ? "good" : "warn",
      summary: group.commands?.canSplit ? "Quantity can be partitioned" : "Split unavailable",
      details: group.hasActiveTransfer ? ["Active transfers prevent safe split operations."] : [],
    },
    {
      key: "merge",
      label: getActionLabel("fleet.group.merge", actionHints, "Merge groups"),
      tone: group.commands?.canMerge ? "good" : "warn",
      summary: group.commands?.canMerge ? "Compatible groups can merge" : "Merge unavailable",
      details: group.hasActiveTransfer ? ["Active transfers prevent safe merge operations."] : [],
    },
    {
      key: "cancel-transfer",
      label: getActionLabel("fleet.transfer.cancel", actionHints, "Cancel transfer"),
      tone: group.commands?.canCancelTransfer ? "good" : "neutral",
      summary: group.commands?.canCancelTransfer ? "Active transfer can be cancelled" : "No cancellable transfer",
      details: group.activeTransfer ? [`Transfer ${group.activeTransfer.id}`] : [],
    },
  ] satisfies FleetCommandPresentationItem[];
}

export function buildFleetMutationConfirmations(
  actions: ActionManifestAction[],
  uiState: FleetUiState | null,
): FleetMutationConfirmationModel[] {
  const stationedGroups = uiState?.groups.filter((group) => group.status === "Stationed").length ?? 0;
  const activeTransfers = uiState?.groups.filter((group) => group.hasActiveTransfer).length ?? 0;
  const mergeReadyGroups = uiState?.groups.filter((group) => group.commands?.canMerge).length ?? 0;
  const splitReadyGroups = uiState?.groups.filter((group) => group.commands?.canSplit).length ?? 0;

  return actions
    .filter((action) => !action.isReadOnly)
    .map((action) => {
      const mutationSummary = normalizeNotes(action.notes)[0] ?? "Prototype-only mutation contract.";

      if (action.actionKey === "fleet.transfer.complete") {
        return {
          actionKey: action.actionKey,
          label: action.displayName,
          prototypeLevel: "danger",
          mutationSummary,
          readinessLabel: activeTransfers > 0 ? "Global batch guarded" : "Waiting for due transfers",
          readinessTone: activeTransfers > 0 ? "warn" : "neutral",
          requiresConfirmation: true,
          confirmationText: "Danger confirmation required before completing due transfers in a batch.",
          disabledReason: "Complete-due stays disabled because it is a global batch mutation, not a routine page action.",
        };
      }

      const disabledReasonByAction: Record<string, string> = {
        "fleet.transfer.create": stationedGroups > 0
          ? "A dedicated local confirmation flow is available, but route execution remains disabled on the Fleet page."
          : "No stationed groups are currently available to prepare a transfer.",
        "fleet.transfer.cancel": activeTransfers > 0
          ? "Execution remains disabled on the Fleet page even when active transfers exist."
          : "No active transfers are currently available to cancel.",
        "fleet.group.split": splitReadyGroups > 0
          ? "Execution remains disabled on the Fleet page even when split-ready groups exist."
          : "No groups are currently in a safe state to split.",
        "fleet.group.merge": mergeReadyGroups > 0
          ? "Execution remains disabled on the Fleet page even when merge-ready groups exist."
          : "No compatible groups are currently in a safe state to merge.",
      };

      return {
        actionKey: action.actionKey,
        label: action.displayName,
        prototypeLevel: "prototype",
        mutationSummary,
        readinessLabel:
          action.actionKey === "fleet.transfer.create"
            ? stationedGroups > 0 ? "Ready in metadata" : "Blocked"
            : action.actionKey === "fleet.transfer.cancel"
              ? activeTransfers > 0 ? "Ready in metadata" : "Blocked"
              : action.actionKey === "fleet.group.split"
                ? splitReadyGroups > 0 ? "Ready in metadata" : "Blocked"
                : action.actionKey === "fleet.group.merge"
                  ? mergeReadyGroups > 0 ? "Ready in metadata" : "Blocked"
                  : "Guarded",
        readinessTone:
          action.actionKey === "fleet.transfer.create"
            ? stationedGroups > 0 ? "good" : "warn"
            : action.actionKey === "fleet.transfer.cancel"
              ? activeTransfers > 0 ? "good" : "warn"
              : action.actionKey === "fleet.group.split"
                ? splitReadyGroups > 0 ? "good" : "warn"
                : action.actionKey === "fleet.group.merge"
                  ? mergeReadyGroups > 0 ? "good" : "warn"
                  : "neutral",
        requiresConfirmation: true,
        confirmationText: `Prototype confirmation required before ${action.displayName.toLowerCase()}.`,
        disabledReason: disabledReasonByAction[action.actionKey]
          ?? "This mutation contract is intentionally metadata-only on the Fleet page.",
      };
    });
}

export function presentEstimateResult(result: FleetCommandApiResult<EstimateOrbitalTravelResponse>): FleetCommandPresentationItem {
  const response = result.response;
  const errors = response?.errors ?? [];
  const costSummary = response?.resourceCosts.length
    ? response.resourceCosts.map((cost) => `${cost.resourceType} ${cost.quantity}`).join(", ")
    : "No projected cost components.";
  const defaultSummary =
    result.httpStatus === 400
      ? "Validacion rechazada por la API de desarrollo."
      : result.httpStatus === 404
        ? "Ruta o datos de desarrollo no encontrados."
        : result.httpStatus === 409
          ? "Conflicto detectado en el estado actual de la flota."
          : result.httpStatus === 503
            ? "Persistencia no configurada para este entorno."
            : `Request returned ${result.httpStatus}.`;

  return {
    key: "estimate-result",
    label: "Estimacion",
    tone: result.httpStatus === 200 && response?.succeeded ? (response.canAfford ? "good" : "warn") : "warn",
    summary:
      result.httpStatus === 200 && response?.succeeded
        ? `Solo lectura completada. Distancia ${response.abstractDistanceUnits} con ${response.estimatedDuration ?? "duracion desconocida"}.`
        : errors[0] ?? defaultSummary,
    details: [
      "No ejecuta movimiento.",
      costSummary,
      ...(response?.routeProfile
        ? [
            `Clase de ruta: ${response.routeProfile.routeClass}`,
            `Banda de riesgo: ${response.routeProfile.riskBand}`,
          ]
        : []),
      ...(response?.fuelReadiness ? [`Fuel ready: ${response.fuelReadiness.isFuelReady ? "yes" : "no"}`] : []),
      ...(response?.insufficientResources.map((resource) => `Missing ${resource.resourceType}: ${resource.requiredQuantity - resource.availableQuantity}`) ?? []),
      ...errors.slice(1),
    ],
  };
}

export function presentCreateTransferResult(result: FleetCommandApiResult<CreateOrbitalTransferResponse>): FleetCommandPresentationItem {
  const response = result.response;
  const isSuccess = (result.httpStatus === 200 || result.httpStatus === 201) && response?.succeeded;
  const hasExpectedErrorPayload = response && Array.isArray(response.errors);
  const defaultSummary =
    result.httpStatus === 400
      ? "La API rechazo la solicitud. Revisa grupo, destino y fecha UTC antes de reenviar."
      : result.httpStatus === 404
        ? "El grupo o destino ya no existen en el estado de desarrollo actual."
        : result.httpStatus === 409
          ? "Conflicto detectado: la estimacion puede estar obsoleta o el grupo ya tiene una transferencia activa."
          : result.httpStatus === 503
            ? "La persistencia no esta configurada para este entorno de desarrollo."
            : getUnexpectedResponseSummary(result);

  return {
    key: "create-transfer-result",
    label: isSuccess
      ? "Transferencia orbital creada"
      : result.httpStatus === 400
        ? "Validacion rechazada"
        : result.httpStatus === 404
          ? "Datos no encontrados"
          : result.httpStatus === 409
            ? "Conflicto de estado"
            : result.httpStatus === 503
              ? "Persistencia no disponible"
              : "Respuesta inesperada",
    tone:
      isSuccess
        ? "good"
        : "warn",
    summary:
      isSuccess
        ? `El estado de desarrollo cambio correctamente. Distancia ${response.abstractDistanceUnits} hacia ${response.destinationPlanetId ?? "destino desconocido"}.`
        : hasExpectedErrorPayload && response.errors[0]
          ? response.errors[0]
          : defaultSummary,
    details: [
      ...(isSuccess ? ["La mutacion reservo el grupo y persistio una transferencia planificada."] : []),
      ...(response?.orbitalTransferId ? [`Transfer ${response.orbitalTransferId}`] : []),
      ...(response?.orbitalGroupId ? [`Grupo ${response.orbitalGroupId}`] : []),
      ...(response?.originPlanetId ? [`Origen ${response.originPlanetId}`] : []),
      ...(response?.destinationPlanetId ? [`Destino ${response.destinationPlanetId}`] : []),
      ...(response?.departureAtUtc ? [`Salida ${response.departureAtUtc}`] : []),
      ...(response?.arrivalAtUtc ? [`Llegada ${response.arrivalAtUtc}`] : []),
      ...(!isSuccess && result.httpStatus === 409
        ? ["Vuelve a cargar la UI o recalcula la estimacion antes de reintentar."]
        : []),
      ...(!isSuccess && !hasExpectedErrorPayload ? ["La UI no recibio el payload JSON esperado para este comando."] : []),
      ...(response?.errors.slice(1) ?? []),
    ],
  };
}

export function presentCreateTransferNetworkFailure(message: string): FleetCommandPresentationItem {
  return {
    key: "create-transfer-network-error",
    label: "Error de red",
    tone: "warn",
    summary: "No se pudo completar create transfer porque la solicitud no llego a la API.",
    details: [message],
  };
}

export function presentCompletionResult(result: FleetCommandApiResult<CompleteOrbitalTransfersResponse>): FleetCommandPresentationItem {
  const response = result.response;

  return {
    key: "complete-result",
    label: "Complete due transfers",
    tone: result.httpStatus === 200 && response?.succeeded ? "good" : "warn",
    summary:
      result.httpStatus === 200 && response?.succeeded
        ? `${response.completedCount} transfer${response.completedCount === 1 ? "" : "s"} completed.`
        : response?.errors[0] ?? `Request returned ${result.httpStatus}.`,
    details: [
      ...(response?.completedTransferIds.length ? [`Transfers ${response.completedTransferIds.join(", ")}`] : []),
      ...(response?.completedOrbitalGroupIds.length ? [`Groups ${response.completedOrbitalGroupIds.join(", ")}`] : []),
      ...(response?.errors.slice(1) ?? []),
    ],
  };
}
