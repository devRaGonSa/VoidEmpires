import type {
  CancelOrbitalTransferResponse,
  CompleteOrbitalTransfersResponse,
  CreateOrbitalTransferResponse,
  EstimateOrbitalTravelResponse,
  FleetCommandApiResult,
} from "../api/fleetCommandTypes";
import type { ActionManifestAction } from "../api/actionManifestTypes";
import type { FleetActionHint, FleetGroupSummary, FleetUiState } from "../api/fleetTypes";
import { formatCompactGuid, formatPlanetReference } from "./domainPresentation";

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
  surfaceLabel: string;
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

function formatTechnicalId(value?: string | null) {
  return value ? formatCompactGuid(value) : null;
}

export function buildFleetCommandReadiness(group: FleetGroupSummary, actionHints: FleetActionHint[]) {
  return [
    {
      key: "estimate",
      label: getActionLabel("fleet.travel.estimate", actionHints, "Vista de ruta"),
      tone: group.routeFuelReadiness?.canRequestTravelEstimate ? "good" : "warn",
      summary: group.routeFuelReadiness?.canRequestTravelEstimate ? "Vista lista" : "Vista bloqueada",
      details: [
        ...(group.routeFuelReadiness?.requiresDestination ? ["Falta elegir destino."] : []),
        ...(group.routeFuelReadiness?.estimateRoute ? [`Ruta ${group.routeFuelReadiness.estimateRoute}`] : []),
        ...(group.routeFuelReadiness?.fuelReadinessPolicy ? [`Politica ${group.routeFuelReadiness.fuelReadinessPolicy}`] : []),
        ...(group.routeFuelReadiness?.notes ?? []),
      ],
    },
    {
      key: "create-transfer",
      label: getActionLabel("fleet.transfer.create", actionHints, "Crear traslado"),
      tone: group.commands?.canCreateTransfer ? "good" : "warn",
      summary: group.commands?.canCreateTransfer ? "Lista para trazar" : "Bloqueada por el estado actual",
      details: group.hasActiveTransfer ? ["La escuadra ya tiene un traslado activo."] : [],
    },
    {
      key: "split",
      label: getActionLabel("fleet.group.split", actionHints, "Dividir escuadra"),
      tone: group.commands?.canSplit ? "good" : "warn",
      summary: group.commands?.canSplit ? "La cantidad puede separarse" : "Division no disponible",
      details: group.hasActiveTransfer ? ["Un traslado activo impide dividir con seguridad."] : [],
    },
    {
      key: "merge",
      label: getActionLabel("fleet.group.merge", actionHints, "Fusionar escuadras"),
      tone: group.commands?.canMerge ? "good" : "warn",
      summary: group.commands?.canMerge ? "Las escuadras compatibles pueden unirse" : "Fusion no disponible",
      details: group.hasActiveTransfer ? ["Un traslado activo impide fusionar con seguridad."] : [],
    },
    {
      key: "cancel-transfer",
      label: getActionLabel("fleet.transfer.cancel", actionHints, "Anular traslado"),
      tone: group.commands?.canCancelTransfer ? "good" : "neutral",
      summary: group.commands?.canCancelTransfer ? "El traslado activo puede anularse" : "No hay traslados anulables",
      details: group.activeTransfer ? [`ID de traslado ${formatTechnicalId(group.activeTransfer.id)}`] : [],
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
      const mutationSummary = normalizeNotes(action.notes)[0] ?? "Contrato de mutacion visible solo como prototipo.";

      if (action.actionKey === "fleet.transfer.complete") {
        return {
          actionKey: action.actionKey,
          label: action.displayName,
          prototypeLevel: "danger",
          mutationSummary,
          surfaceLabel: "Solo metadata protegida",
          readinessLabel: activeTransfers > 0 ? "Lote global protegido" : "Sin traslados vencidos",
          readinessTone: activeTransfers > 0 ? "warn" : "neutral",
          requiresConfirmation: true,
          confirmationText: "Requeriria una confirmacion de riesgo antes de completar traslados vencidos por lote.",
          disabledReason: "Complete-due sigue desactivado porque es una mutacion global, no una accion rutinaria de cabina.",
        };
      }

      const disabledReasonByAction: Record<string, string> = {
        "fleet.transfer.create": stationedGroups > 0
          ? "Hay un flujo local de confirmacion, pero la ejecucion de ruta sigue limitada en la cabina."
          : "No hay escuadras apostadas listas para preparar un traslado.",
        "fleet.transfer.cancel": activeTransfers > 0
          ? "Hay un flujo local para preparar la anulacion, pero la ejecucion sigue limitada en la cabina."
          : "No hay traslados activos disponibles para anular.",
        "fleet.group.split": splitReadyGroups > 0
          ? "La ejecucion sigue desactivada en la cabina aunque existan escuadras listas para dividir."
          : "No hay escuadras en un estado seguro para dividir.",
        "fleet.group.merge": mergeReadyGroups > 0
          ? "La ejecucion sigue desactivada en la cabina aunque existan escuadras listas para fusionar."
          : "No hay escuadras compatibles en un estado seguro para fusionar.",
      };

      return {
        actionKey: action.actionKey,
        label: action.displayName,
        prototypeLevel: "prototype",
        mutationSummary,
        surfaceLabel:
          action.actionKey === "fleet.transfer.create"
            ? "Confirmacion local con ejecucion controlada"
            : action.actionKey === "fleet.transfer.cancel"
              ? "Confirmacion local sin ejecucion"
              : "Solo metadata protegida",
        readinessLabel:
          action.actionKey === "fleet.transfer.create"
            ? stationedGroups > 0 ? "Lista en metadata" : "Bloqueada"
            : action.actionKey === "fleet.transfer.cancel"
              ? activeTransfers > 0 ? "Lista en metadata" : "Bloqueada"
              : action.actionKey === "fleet.group.split"
                ? splitReadyGroups > 0 ? "Lista en metadata" : "Bloqueada"
                : action.actionKey === "fleet.group.merge"
                  ? mergeReadyGroups > 0 ? "Lista en metadata" : "Bloqueada"
                  : "Protegida",
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
        confirmationText: `Se requeriria confirmacion de prototipo antes de ${action.displayName.toLowerCase()}.`,
        disabledReason: disabledReasonByAction[action.actionKey]
          ?? "Este contrato de mutacion se muestra solo como metadata en la cabina.",
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
            : `La solicitud devolvio ${result.httpStatus}.`;

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
      ...(response?.fuelReadiness ? [`Combustible listo: ${response.fuelReadiness.isFuelReady ? "si" : "no"}`] : []),
      ...(response?.insufficientResources.map((resource) => `Falta ${resource.resourceType}: ${resource.requiredQuantity - resource.availableQuantity}`) ?? []),
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
        ? `El estado de desarrollo cambio correctamente. Distancia ${response.abstractDistanceUnits} hacia ${response.destinationPlanetId ? formatPlanetReference(response.destinationPlanetId) : "destino desconocido"}.`
        : hasExpectedErrorPayload && response.errors[0]
          ? response.errors[0]
          : defaultSummary,
    details: [
      ...(isSuccess ? ["La mutacion reservo el grupo y persistio una transferencia planificada."] : []),
      ...(response?.orbitalTransferId ? [`ID de traslado ${formatTechnicalId(response.orbitalTransferId)}`] : []),
      ...(response?.orbitalGroupId ? [`ID tactico ${formatTechnicalId(response.orbitalGroupId)}`] : []),
      ...(response?.originPlanetId ? [`Origen ${formatPlanetReference(response.originPlanetId)}`] : []),
      ...(response?.destinationPlanetId ? [`Destino ${formatPlanetReference(response.destinationPlanetId)}`] : []),
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
    summary: "No se pudo completar crear traslado porque la solicitud no llego a la API.",
    details: [message],
  };
}

export function presentCancelTransferResult(result: FleetCommandApiResult<CancelOrbitalTransferResponse>): FleetCommandPresentationItem {
  const response = result.response;
  const isSuccess = result.httpStatus === 200 && response?.succeeded;
  const hasExpectedErrorPayload = response && Array.isArray(response.errors);
  const defaultSummary =
    result.httpStatus === 400
      ? "La API rechazo la cancelacion. Revisa civilizationId y orbitalTransferId antes de reenviar."
      : result.httpStatus === 404
        ? "La transferencia activa ya no existe en el estado de desarrollo actual."
        : result.httpStatus === 409
          ? "Conflicto detectado: la transferencia ya no esta activa o el grupo ya no esta reservado."
          : result.httpStatus === 503
            ? "La persistencia no esta configurada para este entorno de desarrollo."
            : getUnexpectedResponseSummary(result);

  return {
    key: "cancel-transfer-result",
    label: isSuccess
      ? "Transferencia orbital cancelada"
      : result.httpStatus === 400
        ? "Validacion rechazada"
        : result.httpStatus === 404
          ? "Datos no encontrados"
          : result.httpStatus === 409
            ? "Conflicto de estado"
            : result.httpStatus === 503
              ? "Persistencia no disponible"
              : "Respuesta inesperada",
    tone: isSuccess ? "good" : "warn",
    summary:
      isSuccess
        ? "La transferencia activa se cancelo y el grupo vuelve a quedar disponible en el estado de desarrollo."
        : hasExpectedErrorPayload && response.errors[0]
          ? response.errors[0]
          : defaultSummary,
    details: [
      ...(isSuccess ? ["La cancelacion no reembolsa los recursos ya cobrados por el create transfer."] : []),
      ...(response?.orbitalTransferId ? [`ID de traslado ${formatTechnicalId(response.orbitalTransferId)}`] : []),
      ...(response?.orbitalGroupId ? [`ID tactico ${formatTechnicalId(response.orbitalGroupId)}`] : []),
      ...(!isSuccess && result.httpStatus === 404
        ? ["La confirmacion local puede estar obsoleta. Recarga la UI si otra accion ya elimino esta transferencia activa."]
        : []),
      ...(!isSuccess && result.httpStatus === 409
        ? ["Recarga la UI de flotas antes de reintentar si otra accion ya cambio esta transferencia."]
        : []),
      ...(!isSuccess && !hasExpectedErrorPayload ? ["La UI no recibio el payload JSON esperado para este comando."] : []),
      ...(response?.errors.slice(1) ?? []),
    ],
  };
}

export function presentCancelTransferNetworkFailure(message: string): FleetCommandPresentationItem {
  return {
    key: "cancel-transfer-network-error",
    label: "Error de red",
    tone: "warn",
    summary: "No se pudo completar anular traslado porque la solicitud no llego a la API.",
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
