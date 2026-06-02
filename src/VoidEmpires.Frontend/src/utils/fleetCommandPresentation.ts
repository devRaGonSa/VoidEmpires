import type {
  CancelOrbitalTransferResponse,
  CompleteOrbitalTransfersResponse,
  CreateOrbitalTransferResponse,
  EstimateOrbitalTravelResponse,
  FleetCommandApiResult,
} from "../api/fleetCommandTypes";
import type { ActionManifestAction } from "../api/actionManifestTypes";
import type { FleetActionHint, FleetGroupSummary, FleetUiState } from "../api/fleetTypes";
import {
  formatCompactGuid,
  formatOrbitalGroupStatus,
  formatPlanetPrimaryLabel,
  formatPlanetReference,
  formatResourceType,
  formatSpaceAssetType,
  formatTransferStatus,
} from "./domainPresentation";

type CommandTone = "neutral" | "good" | "warn";

const userFacingActionLabels: Record<string, string> = {
  "fleet.travel.estimate": "Calcular ruta",
  "fleet.transfer.create": "Enviar flota",
  "fleet.transfer.cancel": "Cancelar ruta",
  "fleet.transfer.complete": "Cerrar llegadas vencidas",
  "fleet.group.split": "Dividir escuadra orbital",
  "fleet.group.merge": "Fusionar escuadras orbitales",
  "fleet.uiState.read": "Leer estado de la cabina de flotas",
  "fleet.actionManifest.read": "Leer manifiesto de acciones de flota",
  "strategicMap.actionManifest.read": "Leer manifiesto del mapa estrategico",
  "strategicMap.read": "Leer mapa estrategico",
  "strategicMap.explorationPreview.read": "Leer vista previa de exploracion",
  "exploration.preview.read": "Leer vista previa de exploracion",
  "exploration.mission.create": "Crear mision de exploracion",
  "exploration.mission.completeDue": "Completar misiones de exploracion vencidas",
  "exploration.mission.list": "Listar misiones de exploracion",
  "exploration.knowledge.read": "Leer conocimiento de exploracion",
  "sensor.profile.read": "Leer perfiles de sensores",
  "detection.coverage.read": "Leer cobertura de deteccion",
  "interception.opportunity.read": "Leer oportunidades de intercepcion",
  "alliance.readiness.read": "Leer preparacion de alianzas",
  "alliance.pact.readiness.read": "Leer preparacion de pactos de alianza",
  "diplomacy.contact.read": "Leer contactos diplomaticos",
  "visual.system.read": "Leer estado visual del sistema",
  "visual.planet.read": "Leer estado visual del planeta",
};

export interface FleetCommandPresentationItem {
  key: string;
  label: string;
  tone: CommandTone;
  summary: string;
  details: string[];
  facts?: FleetEstimateFact[];
}

export interface FleetSquadListPresentationItem {
  title: string;
  quantityLabel: string;
  locationLabel: string;
  destinationLabel: string;
  statusLabel: string;
  statusTone: CommandTone;
  readinessLabel: string;
  readinessTone: CommandTone;
  technicalIdLabel: string;
}

export interface FleetEstimateFact {
  label: string;
  value: string;
}

export interface FleetEstimateReviewCard {
  tone: CommandTone;
  title: string;
  statusLabel: string;
  summary: string;
  facts: FleetEstimateFact[];
  warnings: string[];
}

export interface FleetActiveTransferPresentationItem {
  title: string;
  statusLabel: string;
  statusTone: CommandTone;
  originLabel: string;
  destinationLabel: string;
  departureLabel: string;
  arrivalLabel: string;
  progressValue: number | null;
  progressLabel: string;
  phaseLabel: string;
  phaseTone: CommandTone;
  canCancel: boolean;
  canCompleteDue: boolean;
  hasTimingGap: boolean;
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

export function getUserFacingActionLabel(actionKey: string, fallback: string) {
  return userFacingActionLabels[actionKey] ?? fallback;
}

function getActionLabel(actionKey: string, actionHints: FleetActionHint[], fallback: string) {
  return getUserFacingActionLabel(
    actionKey,
    actionHints.find((hint) => hint.actionKey === actionKey)?.displayName ?? fallback,
  );
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

export function presentFleetSquadListItem(
  group: FleetGroupSummary,
  actionHints: FleetActionHint[],
): FleetSquadListPresentationItem {
  const isTravelling = group.hasActiveTransfer;
  const canOrder = group.commands?.canCreateTransfer ?? false;
  const canEstimate = group.routeFuelReadiness?.canRequestTravelEstimate ?? false;

  return {
    title: formatSpaceAssetType(group.assetType),
    quantityLabel: `${group.quantity} unidades`,
    locationLabel: formatPlanetPrimaryLabel(group.currentPlanetId),
    destinationLabel: isTravelling && group.activeTransfer
      ? formatPlanetPrimaryLabel(group.activeTransfer.destinationPlanetId)
      : "Sin destino activo",
    statusLabel: formatOrbitalGroupStatus(group.status),
    statusTone: isTravelling ? "warn" : canOrder ? "good" : "neutral",
    readinessLabel: isTravelling
      ? getActionLabel("fleet.transfer.cancel", actionHints, "Mision en curso")
      : canOrder
        ? "Lista para mover"
        : canEstimate
          ? "Lista para estimar"
          : "En espera",
    readinessTone: isTravelling ? "warn" : canOrder || canEstimate ? "good" : "neutral",
    technicalIdLabel: formatTechnicalId(group.id) ?? "Sin ID",
  };
}

export function buildFleetEstimateFacts(
  response: EstimateOrbitalTravelResponse | null,
  currentPlanetId?: string | null,
  destinationPlanetId?: string | null,
): FleetEstimateFact[] {
  if (!response?.succeeded) {
    return [];
  }

  const costLabel = response.resourceCosts.length
    ? response.resourceCosts.map((cost) => `${formatResourceType(cost.resourceType)} ${cost.quantity}`).join(", ")
    : "Sin coste";

  return [
    {
      label: "Origen",
      value: formatPlanetReference(response.currentPlanetId ?? currentPlanetId ?? ""),
    },
    {
      label: "Destino",
      value: formatPlanetReference(response.destinationPlanetId ?? destinationPlanetId ?? ""),
    },
    {
      label: "Duracion",
      value: response.estimatedDuration ?? "Sin duracion visible",
    },
    {
      label: "Coste",
      value: costLabel,
    },
    {
      label: "Lista",
      value: response.canAfford && (response.fuelReadiness?.isFuelReady ?? true) ? "Si" : "No",
    },
  ];
}

export function buildFleetEstimateReviewCard(
  result: FleetCommandApiResult<EstimateOrbitalTravelResponse> | null,
  squadLabel: string,
  currentPlanetId?: string | null,
  destinationPlanetId?: string | null,
): FleetEstimateReviewCard | null {
  if (!result) {
    return null;
  }

  const response = result.response;
  const isSuccess = result.httpStatus === 200 && response?.succeeded;
  const isReady = isSuccess && response.canAfford && (response.fuelReadiness?.isFuelReady ?? true);
  const costLabel = response?.resourceCosts.length
    ? response.resourceCosts.map((cost) => `${formatResourceType(cost.resourceType)} ${cost.quantity}`).join(", ")
    : "Sin coste proyectado";
  const warnings = isSuccess
    ? [
        ...(response.fuelReadiness && !response.fuelReadiness.isFuelReady
          ? [response.fuelReadiness.notReadyReason ?? "La regla de combustible sigue bloqueando la salida."]
          : []),
        ...response.insufficientResources.map(
          (resource) =>
            `${formatResourceType(resource.resourceType)}: faltan ${resource.requiredQuantity - resource.availableQuantity}`,
        ),
      ]
    : response?.errors ?? [];

  return {
    tone: isReady ? "good" : "warn",
    title: "Ruta prevista",
    statusLabel: isReady ? "Lista para confirmar" : "Requiere ajustes",
    summary: isSuccess
      ? "Simulacion completada. Revisa la ruta antes de enviar la flota."
      : response?.errors[0] ?? `La solicitud devolvio ${result.httpStatus}.`,
    facts: isSuccess
      ? [
          { label: "Escuadra", value: squadLabel },
          { label: "Origen", value: formatPlanetPrimaryLabel(response.currentPlanetId ?? currentPlanetId ?? "") },
          { label: "Destino", value: formatPlanetPrimaryLabel(response.destinationPlanetId ?? destinationPlanetId ?? "") },
          { label: "Distancia", value: `${response.abstractDistanceUnits} tramos` },
          { label: "Duracion", value: response.estimatedDuration ?? "Sin duracion visible" },
          { label: "Coste", value: costLabel || "Sin coste proyectado" },
          { label: "Perfil", value: response.routeProfile?.routeClass ? `Ruta ${response.routeProfile.routeClass}` : "Perfil no disponible" },
          { label: "Disponibilidad", value: isReady ? "Lista" : "Bloqueada" },
        ]
      : [
          { label: "Escuadra", value: squadLabel },
          { label: "Ruta", value: `${formatPlanetPrimaryLabel(currentPlanetId ?? "")} -> ${formatPlanetPrimaryLabel(destinationPlanetId ?? "")}` },
        ],
    warnings,
  };
}

export function presentFleetActiveTransferItem(group: FleetGroupSummary): FleetActiveTransferPresentationItem | null {
  const transfer = group.activeTransfer;
  if (!transfer) {
    return null;
  }

  const departure = Date.parse(transfer.departureAtUtc);
  const arrival = Date.parse(transfer.arrivalAtUtc);
  const now = Date.now();
  const progressValue =
    Number.isNaN(departure) || Number.isNaN(arrival) || arrival <= departure
      ? null
      : Math.max(0, Math.min(100, ((now - departure) / (arrival - departure)) * 100));
  const hasTimingGap = progressValue === null;
  const canCompleteDue = !hasTimingGap && now >= arrival;
  const phaseLabel = hasTimingGap
    ? "Horario no legible"
    : canCompleteDue
      ? "Llegada vencida"
      : "En curso";
  const phaseTone: CommandTone = hasTimingGap ? "neutral" : canCompleteDue ? "warn" : "good";

  return {
    title: formatSpaceAssetType(group.assetType),
    statusLabel: formatTransferStatus(transfer.status),
    statusTone: canCompleteDue ? "warn" : "neutral",
    originLabel: formatPlanetPrimaryLabel(group.originPlanetId),
    destinationLabel: formatPlanetPrimaryLabel(transfer.destinationPlanetId),
    departureLabel: transfer.departureAtUtc,
    arrivalLabel: transfer.arrivalAtUtc,
    progressValue,
    progressLabel: progressValue === null ? "Avance no disponible" : `${Math.round(progressValue)}% completado`,
    phaseLabel,
    phaseTone,
    canCancel: group.commands?.canCancelTransfer ?? false,
    canCompleteDue,
    hasTimingGap,
  };
}

export function buildFleetCommandReadiness(group: FleetGroupSummary, actionHints: FleetActionHint[]) {
  const estimateSummary = group.routeFuelReadiness?.canRequestTravelEstimate
    ? "Disponible"
    : group.routeFuelReadiness?.requiresDestination
      ? "Falta elegir destino."
      : group.hasActiveTransfer
        ? "La escuadra esta reservada."
        : "No disponible";
  const createTransferSummary = group.commands?.canCreateTransfer
    ? "Lista para mover"
    : group.hasActiveTransfer
      ? "Ya existe un traslado activo."
      : "Requiere una estimacion valida.";
  const splitSummary = group.commands?.canSplit
    ? "Divisible"
    : group.hasActiveTransfer
      ? "La escuadra esta reservada."
      : "No disponible";
  const mergeSummary = group.commands?.canMerge
    ? "Fusionable"
    : group.hasActiveTransfer
      ? "La escuadra esta reservada."
      : "No disponible";

  return [
    {
      key: "estimate",
      label: getActionLabel("fleet.travel.estimate", actionHints, "Calcular ruta"),
      tone: group.routeFuelReadiness?.canRequestTravelEstimate ? "good" : "warn",
      summary: estimateSummary,
      details: [
        ...(group.routeFuelReadiness?.estimateRoute ? [`Ruta tecnica ${group.routeFuelReadiness.estimateRoute}`] : []),
        ...(group.routeFuelReadiness?.fuelReadinessPolicy ? [`Politica ${group.routeFuelReadiness.fuelReadinessPolicy}`] : []),
        ...(group.routeFuelReadiness?.notes ?? []),
      ],
    },
    {
      key: "create-transfer",
      label: getActionLabel("fleet.transfer.create", actionHints, "Enviar flota"),
      tone: group.commands?.canCreateTransfer ? "good" : "warn",
      summary: createTransferSummary,
      details: group.hasActiveTransfer ? ["No puede abrir otra orden mientras el traslado actual siga activo."] : [],
    },
    {
      key: "split",
      label: getActionLabel("fleet.group.split", actionHints, "Division"),
      tone: group.commands?.canSplit ? "good" : "warn",
      summary: splitSummary,
      details: group.hasActiveTransfer ? ["Un traslado activo impide dividir con seguridad."] : [],
    },
    {
      key: "merge",
      label: getActionLabel("fleet.group.merge", actionHints, "Fusion"),
      tone: group.commands?.canMerge ? "good" : "warn",
      summary: mergeSummary,
      details: group.hasActiveTransfer ? ["Un traslado activo impide fusionar con seguridad."] : [],
    },
    {
      key: "cancel-transfer",
      label: getActionLabel("fleet.transfer.cancel", actionHints, "Cancelar ruta"),
      tone: group.commands?.canCancelTransfer ? "good" : "neutral",
      summary: group.commands?.canCancelTransfer ? "Disponible" : "Sin traslado anulable",
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
          label: getUserFacingActionLabel(action.actionKey, action.displayName),
          prototypeLevel: "danger",
          mutationSummary,
          surfaceLabel: "Solo metadata protegida",
          readinessLabel: activeTransfers > 0 ? "Lote global protegido" : "Sin traslados vencidos",
          readinessTone: activeTransfers > 0 ? "warn" : "neutral",
          requiresConfirmation: true,
          confirmationText: "Requeriria una confirmacion de riesgo antes de completar traslados vencidos por lote.",
          disabledReason: "Completar vencidos sigue desactivado porque es una mutacion global, no una accion rutinaria de cabina.",
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
        label: getUserFacingActionLabel(action.actionKey, action.displayName),
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
        confirmationText: `Se requeriria confirmacion de prototipo antes de ${getUserFacingActionLabel(action.actionKey, action.displayName).toLowerCase()}.`,
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
    : "Sin costes proyectados visibles.";
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
        ? `Orden orbital registrada. La escuadra ya tiene un traslado planificado hacia ${response.destinationPlanetId ? formatPlanetReference(response.destinationPlanetId) : "destino desconocido"}.`
        : hasExpectedErrorPayload && response.errors[0]
          ? response.errors[0]
          : defaultSummary,
    facts: isSuccess
      ? [
          { label: "Origen", value: response.originPlanetId ? formatPlanetReference(response.originPlanetId) : "Origen no visible" },
          { label: "Destino", value: response.destinationPlanetId ? formatPlanetReference(response.destinationPlanetId) : "Destino no visible" },
          { label: "Distancia", value: `${response.abstractDistanceUnits} tramos` },
          { label: "Salida", value: response.departureAtUtc ?? "Salida pendiente de confirmar" },
          { label: "Llegada", value: response.arrivalAtUtc ?? "Llegada pendiente de confirmar" },
          { label: "Estado", value: "Traslado planificado" },
        ]
      : undefined,
    details: [
      ...(isSuccess ? ["La mutacion reservo el grupo y persistio una transferencia planificada."] : []),
      ...(response?.originPlanetId ? [`Origen ${formatPlanetReference(response.originPlanetId)}`] : []),
      ...(response?.destinationPlanetId ? [`Destino ${formatPlanetReference(response.destinationPlanetId)}`] : []),
      ...(response?.departureAtUtc ? [`Salida ${response.departureAtUtc}`] : []),
      ...(response?.arrivalAtUtc ? [`Llegada ${response.arrivalAtUtc}`] : []),
      ...(response?.orbitalTransferId ? [`ID de traslado ${formatTechnicalId(response.orbitalTransferId)}`] : []),
      ...(response?.orbitalGroupId ? [`ID tactico ${formatTechnicalId(response.orbitalGroupId)}`] : []),
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
    facts: undefined,
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
    facts: undefined,
    details: [
      ...(isSuccess ? ["La cancelacion no reembolsa los recursos ya cobrados por el create transfer."] : []),
      ...(!isSuccess && result.httpStatus === 404
        ? ["La confirmacion local puede estar obsoleta. Recarga la UI si otra accion ya elimino esta transferencia activa."]
        : []),
      ...(!isSuccess && result.httpStatus === 409
        ? ["Recarga la UI de flotas antes de reintentar si otra accion ya cambio esta transferencia."]
        : []),
      ...(response?.orbitalTransferId ? [`ID de traslado ${formatTechnicalId(response.orbitalTransferId)}`] : []),
      ...(response?.orbitalGroupId ? [`ID tactico ${formatTechnicalId(response.orbitalGroupId)}`] : []),
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
    facts: undefined,
  };
}

export function presentCompletionResult(result: FleetCommandApiResult<CompleteOrbitalTransfersResponse>): FleetCommandPresentationItem {
  const response = result.response;

  return {
    key: "complete-result",
    label: "Completar traslados vencidos",
    tone: result.httpStatus === 200 && response?.succeeded ? "good" : "warn",
    summary:
      result.httpStatus === 200 && response?.succeeded
        ? `${response.completedCount} traslado${response.completedCount === 1 ? "" : "s"} completado${response.completedCount === 1 ? "" : "s"}.`
        : response?.errors[0] ?? `La solicitud devolvio ${result.httpStatus}.`,
    details: [
      ...(response?.completedTransferIds.length ? [`Traslados ${response.completedTransferIds.join(", ")}`] : []),
      ...(response?.completedOrbitalGroupIds.length ? [`Escuadras ${response.completedOrbitalGroupIds.join(", ")}`] : []),
      ...(response?.errors.slice(1) ?? []),
    ],
  };
}
