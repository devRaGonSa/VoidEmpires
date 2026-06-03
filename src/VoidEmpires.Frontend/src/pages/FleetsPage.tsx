import { FormEvent, useEffect, useMemo, useRef, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import type { ActionManifestAction } from "../api/actionManifestTypes";
import type {
  EstimateOrbitalTravelResponse,
  FleetCommandApiResult,
} from "../api/fleetCommandTypes";
import type { FleetCommandPresentationItem } from "../utils/fleetCommandPresentation";
import type { FleetGroupSummary, FleetUiState } from "../api/fleetTypes";
import type { ReadinessNote } from "../api/strategicMapTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { FleetSelectedGroupPanel } from "../components/FleetSelectedGroupPanel";
import { FleetSummaryPanel } from "../components/FleetSummaryPanel";
import { ActionManifestPanel } from "../components/ActionManifestPanel";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { UiProgressBar } from "../components/ui/UiProgressBar";
import {
  formatCompactGuid,
  formatPlanetPrimaryLabel,
  formatPlanetSecondaryLabel,
  formatResourceType,
  formatSpaceAssetType,
} from "../utils/domainPresentation";
import {
  buildConstructionUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  buildShipyardUrl,
} from "../utils/routeUrls";
import {
  buildFleetCommandReadiness,
  buildFleetEstimateReviewCard,
  buildFleetMutationConfirmations,
  presentFleetActiveTransferItem,
  presentFleetSquadListItem,
  presentCancelTransferNetworkFailure,
  presentCancelTransferResult,
  presentCompletionResult,
  presentCreateTransferNetworkFailure,
  presentCreateTransferResult,
} from "../utils/fleetCommandPresentation";

const knownDevelopmentPlanetIds = [
  "40000000-0000-0000-0000-000000000001",
  "40000000-0000-0000-0000-000000000002",
  "40000000-0000-0000-0000-000000000003",
];

function formatNote(note: ReadinessNote) {
  if (typeof note === "string") {
    return note;
  }

  return note.note ?? "Hay metadatos de disponibilidad.";
}

function getGroupTone(group: FleetGroupSummary): "good" | "warn" | "neutral" {
  if (group.hasActiveTransfer) {
    return "warn";
  }

  if (group.commands?.canCreateTransfer) {
    return "good";
  }

  return "neutral";
}

interface SummaryMetricProps {
  label: string;
  value: number;
}

function SummaryMetric({ label, value }: SummaryMetricProps) {
  return (
    <div className="figma-stat">
      <strong>{value}</strong>
      <span>{label}</span>
    </div>
  );
}

interface FleetDataRowProps {
  label: string;
  value: string;
}

function FleetDataRow({ label, value }: FleetDataRowProps) {
  return (
    <div className="figma-data-row">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

interface EstimateSnapshot {
  orbitalGroupId: string;
  currentPlanetId: string;
  destinationPlanetId: string;
}

function formatSquadIdentity(group: FleetGroupSummary) {
  return `${formatSpaceAssetType(group.assetType)} en ${formatPlanetPrimaryLabel(group.currentPlanetId)}`;
}

function formatSquadOptionLabel(group: FleetGroupSummary) {
  return `${formatSquadIdentity(group)} | ${group.quantity} unidades | ref. ${formatCompactGuid(group.id)}`;
}

function formatFleetDestinationOptionLabel(planetId: string) {
  const primaryLabel = formatPlanetPrimaryLabel(planetId);
  const secondaryLabel = formatPlanetSecondaryLabel(planetId);

  return secondaryLabel ? `${primaryLabel} | ${secondaryLabel}` : primaryLabel;
}

function isTransferDueForUi(group: FleetGroupSummary) {
  const arrivalAtUtc = group.activeTransfer?.arrivalAtUtc;
  if (!arrivalAtUtc) {
    return false;
  }

  const arrivalTime = Date.parse(arrivalAtUtc);
  return !Number.isNaN(arrivalTime) && arrivalTime <= Date.now();
}

export function FleetsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationId, setCivilizationId] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<FleetUiState | null>(null);
  const [fleetManifest, setFleetManifest] = useState<ActionManifestAction[]>([]);
  const [strategicMapManifest, setStrategicMapManifest] = useState<
    ActionManifestAction[]
  >([]);
  const [inspectedGroupId, setInspectedGroupId] = useState("");
  const [selectedGroupId, setSelectedGroupId] = useState("");
  const [selectedDestinationPlanetId, setSelectedDestinationPlanetId] = useState("");
  const [isEstimating, setIsEstimating] = useState(false);
  const [estimateApiResult, setEstimateApiResult] =
    useState<FleetCommandApiResult<EstimateOrbitalTravelResponse> | null>(null);
  const [estimateNetworkError, setEstimateNetworkError] = useState<string | null>(null);
  const [estimateSnapshot, setEstimateSnapshot] = useState<EstimateSnapshot | null>(null);
  const [estimateStaleMessage, setEstimateStaleMessage] = useState<string | null>(null);
  const [hasCreateTransferAcknowledgement, setHasCreateTransferAcknowledgement] = useState(false);
  const [preparedCompleteDueGroupId, setPreparedCompleteDueGroupId] = useState("");
  const [hasCompleteDueAcknowledgement, setHasCompleteDueAcknowledgement] = useState(false);
  const [isCompletingDueTransfers, setIsCompletingDueTransfers] = useState(false);
  const [completeDueTransferResult, setCompleteDueTransferResult] = useState<FleetCommandPresentationItem | null>(null);
  const [completeDueTransferNetworkError, setCompleteDueTransferNetworkError] = useState<string | null>(null);
  const [completeDueTransferStaleMessage, setCompleteDueTransferStaleMessage] = useState<string | null>(null);
  const [preparedCancelTransferId, setPreparedCancelTransferId] = useState("");
  const [hasCancelTransferAcknowledgement, setHasCancelTransferAcknowledgement] = useState(false);
  const [isCancellingTransfer, setIsCancellingTransfer] = useState(false);
  const [cancelTransferResult, setCancelTransferResult] = useState<FleetCommandPresentationItem | null>(null);
  const [cancelTransferNetworkError, setCancelTransferNetworkError] = useState<string | null>(null);
  const [cancelTransferStaleMessage, setCancelTransferStaleMessage] = useState<string | null>(null);
  const [isCreatingTransfer, setIsCreatingTransfer] = useState(false);
  const [createTransferResult, setCreateTransferResult] = useState<FleetCommandPresentationItem | null>(null);
  const [createTransferNetworkError, setCreateTransferNetworkError] = useState<string | null>(null);
  const createTransferInFlightRef = useRef(false);
  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");

  const summary = useMemo(() => {
    if (!uiState) {
      return null;
    }

    return {
      groups: uiState.groups.length,
      transfers: uiState.groups.filter((group) => group.activeTransfer).length,
      resourceContexts: uiState.resourceContexts?.length ?? 0,
      actionHints: uiState.actionHints?.length ?? 0,
    };
  }, [uiState]);

  const estimateEligibleGroups = useMemo(
    () => uiState?.groups.filter((group) => group.routeFuelReadiness?.canRequestTravelEstimate) ?? [],
    [uiState],
  );
  const activeTransferGroups = useMemo(
    () => uiState?.groups.filter((group) => group.activeTransfer) ?? [],
    [uiState],
  );
  const dueTransferGroups = useMemo(
    () => activeTransferGroups.filter((group) => isTransferDueForUi(group)),
    [activeTransferGroups],
  );
  const cancellableTransferCount = useMemo(
    () => activeTransferGroups.filter((group) => group.commands?.canCancelTransfer).length,
    [activeTransferGroups],
  );
  const hasCompleteDueAction = useMemo(
    () => fleetManifest.some((action) => action.actionKey === "fleet.transfer.complete"),
    [fleetManifest],
  );
  const activeTransferItems = useMemo(
    () =>
      activeTransferGroups
        .flatMap((group) => {
          const presentation = presentFleetActiveTransferItem(group);
          return presentation ? [{ group, presentation }] : [];
        }),
    [activeTransferGroups],
  );
  const squadListItems = useMemo(
    () =>
      uiState?.groups.map((group) => ({
        group,
        presentation: presentFleetSquadListItem(group, uiState.actionHints ?? []),
      })) ?? [],
    [uiState],
  );
  const inspectedGroup = useMemo(
    () => uiState?.groups.find((group) => group.id === inspectedGroupId) ?? uiState?.groups[0] ?? null,
    [inspectedGroupId, uiState],
  );

  const effectiveGroupId = selectedGroupId || estimateEligibleGroups[0]?.id || "";
  const selectedGroup = useMemo(
    () => estimateEligibleGroups.find((group) => group.id === effectiveGroupId) ?? null,
    [effectiveGroupId, estimateEligibleGroups],
  );

  const destinationOptions = useMemo(() => {
    const candidates = new Set<string>(knownDevelopmentPlanetIds);

    uiState?.groups.forEach((group) => {
      candidates.add(group.currentPlanetId);
      candidates.add(group.originPlanetId);
      if (group.activeTransfer?.destinationPlanetId) {
        candidates.add(group.activeTransfer.destinationPlanetId);
      }
    });

    uiState?.resourceContexts?.forEach((context) => {
      candidates.add(context.planetId);
    });

    return [...candidates]
      .filter((planetId) => planetId && planetId !== selectedGroup?.currentPlanetId)
      .sort((left, right) =>
        formatPlanetPrimaryLabel(left).localeCompare(formatPlanetPrimaryLabel(right)),
      );
  }, [selectedGroup?.currentPlanetId, uiState]);

  const effectiveDestinationPlanetId =
    selectedDestinationPlanetId && destinationOptions.includes(selectedDestinationPlanetId)
      ? selectedDestinationPlanetId
      : destinationOptions[0] || "";
  const mutationConfirmations = useMemo(
    () => buildFleetMutationConfirmations(fleetManifest, uiState),
    [fleetManifest, uiState],
  );
  const visiblePreparedCancelTransfer = useMemo(
    () =>
      uiState?.groups.find(
        (group) =>
          group.activeTransfer?.id === preparedCancelTransferId && group.commands?.canCancelTransfer,
      ) ?? null,
    [preparedCancelTransferId, uiState],
  );
  const visiblePreparedCompleteDueGroup = useMemo(
    () =>
      uiState?.groups.find(
        (group) => group.id === preparedCompleteDueGroupId && isTransferDueForUi(group),
      ) ?? null,
    [preparedCompleteDueGroupId, uiState],
  );
  const liveEstimateResponse = useMemo(() => {
    const response = estimateApiResult?.response;

    if (
      !estimateSnapshot ||
      !estimateApiResult ||
      estimateApiResult.httpStatus !== 200 ||
      !response?.succeeded ||
      !selectedGroup ||
      selectedGroup.status !== "Stationed" ||
      selectedGroup.hasActiveTransfer ||
      !selectedGroup.commands?.canCreateTransfer ||
      response.orbitalGroupId !== estimateSnapshot.orbitalGroupId ||
      response.currentPlanetId !== estimateSnapshot.currentPlanetId ||
      response.destinationPlanetId !== estimateSnapshot.destinationPlanetId ||
      selectedGroup.id !== estimateSnapshot.orbitalGroupId ||
      selectedGroup.currentPlanetId !== estimateSnapshot.currentPlanetId ||
      effectiveDestinationPlanetId !== estimateSnapshot.destinationPlanetId
    ) {
      return null;
    }

    return response;
  }, [effectiveDestinationPlanetId, estimateApiResult, estimateSnapshot, selectedGroup]);
  const createTransferConfirmationState = useMemo(() => {
    if (
      !liveEstimateResponse ||
      !selectedGroup ||
      !effectiveDestinationPlanetId
    ) {
      return null;
    }

    const fuelReady = liveEstimateResponse.fuelReadiness?.isFuelReady ?? true;
    const canPrepare = liveEstimateResponse.canAfford && fuelReady;
    const missingResources = liveEstimateResponse.insufficientResources.map(
      (resource) =>
        `${formatResourceType(resource.resourceType)}: faltan ${
          resource.requiredQuantity - resource.availableQuantity
        }`,
    );

    return {
      canPrepare,
      blockReason: !liveEstimateResponse.canAfford
        ? "La estimacion indica que faltan recursos para esta transferencia."
        : !fuelReady
          ? liveEstimateResponse.fuelReadiness?.notReadyReason ?? "La regla de combustible sigue bloqueando la accion."
          : null,
      routeSummary: `${formatPlanetPrimaryLabel(selectedGroup.currentPlanetId)} -> ${formatPlanetPrimaryLabel(liveEstimateResponse.destinationPlanetId ?? effectiveDestinationPlanetId)}`,
      costSummary: liveEstimateResponse.resourceCosts.length
        ? liveEstimateResponse.resourceCosts
            .map((cost) => `${formatResourceType(cost.resourceType)} ${cost.quantity}`)
            .join(", ")
        : "Sin coste proyectado.",
      details: [
        `Distancia abstracta ${liveEstimateResponse.abstractDistanceUnits}`,
        `Duracion estimada ${liveEstimateResponse.estimatedDuration ?? "desconocida"}`,
        ...(liveEstimateResponse.routeProfile
          ? [
              `Clase de ruta ${liveEstimateResponse.routeProfile.routeClass}`,
              `Riesgo ${liveEstimateResponse.routeProfile.riskBand}`,
            ]
          : []),
        ...(liveEstimateResponse.fuelReadiness
          ? [
              `Combustible ${liveEstimateResponse.fuelReadiness.isFuelReady ? "listo" : "bloqueado"}`,
            ]
          : []),
        ...missingResources,
      ],
    };
  }, [effectiveDestinationPlanetId, liveEstimateResponse, selectedGroup]);
  const estimateReviewCard = useMemo(
    () =>
      buildFleetEstimateReviewCard(
        estimateApiResult,
        selectedGroup ? formatSquadIdentity(selectedGroup) : "Sin escuadra lista",
        selectedGroup?.currentPlanetId,
        effectiveDestinationPlanetId,
      ),
    [effectiveDestinationPlanetId, estimateApiResult, selectedGroup],
  );

  function clearEstimateState() {
    setEstimateApiResult(null);
    setEstimateNetworkError(null);
    setEstimateSnapshot(null);
  }

  function invalidateEstimate(reason: string) {
    if (estimateSnapshot || estimateApiResult) {
      clearEstimateState();
      setEstimateStaleMessage(reason);
    } else {
      setEstimateStaleMessage(null);
    }

    resetCreateTransferAcknowledgement();
  }

  function resetCreateTransferAcknowledgement() {
    setHasCreateTransferAcknowledgement(false);
    setCreateTransferResult(null);
    setCreateTransferNetworkError(null);
  }

  async function refreshFleetUiState(civilizationIdValue: string) {
    const uiStateResponse = await voidEmpiresApi.getFleetUiState(civilizationIdValue);

    if (!uiStateResponse.succeeded || !uiStateResponse.uiState) {
      throw new Error(uiStateResponse.errors[0] ?? "No se pudo refrescar el estado de flotas.");
    }

    setUiState(uiStateResponse.uiState);
  }

  useEffect(() => {
    setCivilizationId(queryCivilizationId);
  }, [queryCivilizationId]);

  useEffect(() => {
    async function loadFleetCockpit() {
      if (!queryCivilizationId) {
        return;
      }

      setIsLoading(true);
      setError(null);
      clearEstimateState();
      setEstimateStaleMessage(null);
      resetCreateTransferAcknowledgement();

      try {
        const [uiStateResponse, fleetManifestResponse, strategicMapManifestResponse] =
          await Promise.all([
            voidEmpiresApi.getFleetUiState(queryCivilizationId),
            voidEmpiresApi.getFleetActionManifest(),
            voidEmpiresApi.getStrategicMapActionManifest(),
          ]);

        if (!uiStateResponse.succeeded || !uiStateResponse.uiState) {
          setUiState(null);
          setError(uiStateResponse.errors[0] ?? "La solicitud del estado de flotas fallo.");
          return;
        }

        const preferredGroup =
          uiStateResponse.uiState.groups.find(
            (group) =>
              group.currentPlanetId === queryPlanetId ||
              group.originPlanetId === queryPlanetId ||
              group.activeTransfer?.destinationPlanetId === queryPlanetId,
          ) ??
          uiStateResponse.uiState.groups[0] ??
          null;

        setUiState(uiStateResponse.uiState);
        setFleetManifest(fleetManifestResponse.manifest?.actions ?? []);
        setStrategicMapManifest(strategicMapManifestResponse.manifest?.actions ?? []);
        setInspectedGroupId(preferredGroup?.id ?? "");
        setSelectedGroupId(preferredGroup?.id ?? "");
      } catch (requestError) {
        const message =
          requestError instanceof Error
            ? requestError.message
            : "Las solicitudes de flota fallaron.";
        setUiState(null);
        setError(message);
      } finally {
        setIsLoading(false);
      }
    }

    void loadFleetCockpit();
  }, [queryCivilizationId, queryPlanetId]);

  useEffect(() => {
    if (!estimateSnapshot) {
      return;
    }

    if (!selectedGroup || selectedGroup.id !== estimateSnapshot.orbitalGroupId) {
      invalidateEstimate("El grupo estimado ya no esta disponible. Calcula una nueva estimacion.");
      return;
    }

    if (
      selectedGroup.status !== "Stationed" ||
      selectedGroup.hasActiveTransfer ||
      !selectedGroup.commands?.canCreateTransfer
    ) {
      invalidateEstimate("El grupo estimado ya no puede crear transferencias. Recalcula antes de continuar.");
      return;
    }

    if (selectedGroup.currentPlanetId !== estimateSnapshot.currentPlanetId) {
      invalidateEstimate("El grupo estimado cambio de planeta. Calcula una nueva estimacion.");
      return;
    }

    if (effectiveDestinationPlanetId !== estimateSnapshot.destinationPlanetId) {
      invalidateEstimate("El destino cambio desde la ultima estimacion. Calcula una nueva estimacion.");
    }
  }, [effectiveDestinationPlanetId, estimateSnapshot, selectedGroup]);

  useEffect(() => {
    if (!preparedCompleteDueGroupId) {
      return;
    }

    if (!visiblePreparedCompleteDueGroup) {
      setPreparedCompleteDueGroupId("");
      setHasCompleteDueAcknowledgement(false);
      setCompleteDueTransferStaleMessage(
        "La llegada preparada ya no figura como vencida en la UI. Estado actualizado desde la API.",
      );
    }
  }, [preparedCompleteDueGroupId, visiblePreparedCompleteDueGroup]);

  useEffect(() => {
    if (!preparedCancelTransferId) {
      return;
    }

    if (!visiblePreparedCancelTransfer) {
      setPreparedCancelTransferId("");
      setHasCancelTransferAcknowledgement(false);
      setCancelTransferStaleMessage(
        "La transferencia preparada ya no esta activa en la UI. Estado actualizado desde la API.",
      );
    }
  }, [preparedCancelTransferId, visiblePreparedCancelTransfer]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationId.trim();
    if (!trimmedCivilizationId) {
      setError("El civilizationId es obligatorio.");
      setUiState(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    if (queryPlanetId) {
      nextParams.set("planetId", queryPlanetId);
    }
    setSearchParams(nextParams);
  }

  async function handleEstimateSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!uiState?.civilizationId || !effectiveGroupId || !effectiveDestinationPlanetId) {
      clearEstimateState();
      setEstimateNetworkError("Selecciona grupo y destino antes de calcular la estimacion.");
      return;
    }

    setIsEstimating(true);
    clearEstimateState();
    setEstimateStaleMessage(null);
    setEstimateNetworkError(null);
    resetCreateTransferAcknowledgement();

    try {
      const result = await voidEmpiresApi.estimateOrbitalTravel({
        civilizationId: uiState.civilizationId,
        orbitalGroupId: effectiveGroupId,
        destinationPlanetId: effectiveDestinationPlanetId,
      });

      setEstimateApiResult(result);
      setEstimateSnapshot(
        result.httpStatus === 200 && result.response?.succeeded && selectedGroup
          ? {
              orbitalGroupId: selectedGroup.id,
              currentPlanetId: selectedGroup.currentPlanetId,
              destinationPlanetId: effectiveDestinationPlanetId,
            }
          : null,
      );
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Error de red al pedir la estimacion de viaje.";
      setEstimateNetworkError(message);
    } finally {
      setIsEstimating(false);
    }
  }

  async function handleCreateTransfer() {
    if (createTransferInFlightRef.current || isCreatingTransfer) {
      setCreateTransferNetworkError("Ya hay una solicitud de crear traslado en curso.");
      return;
    }

    if (!uiState?.civilizationId || !selectedGroup || !effectiveDestinationPlanetId) {
      setCreateTransferNetworkError("Falta contexto valido para crear la transferencia.");
      return;
    }

    if (!liveEstimateResponse || !estimateSnapshot) {
      setCreateTransferNetworkError("Necesitas una estimacion vigente para este grupo y destino antes de crear la transferencia.");
      return;
    }

    if (!hasCreateTransferAcknowledgement) {
      setCreateTransferNetworkError("Confirma la accion explicita antes de enviar crear traslado.");
      return;
    }

    createTransferInFlightRef.current = true;
    setIsCreatingTransfer(true);
    setCreateTransferResult(null);
    setCreateTransferNetworkError(null);

    try {
      const result = await voidEmpiresApi.createOrbitalTransfer({
        civilizationId: uiState.civilizationId,
        orbitalGroupId: selectedGroup.id,
        destinationPlanetId: effectiveDestinationPlanetId,
        requestedAtUtc: new Date().toISOString(),
      });

      setCreateTransferResult(presentCreateTransferResult(result));

      if ((result.httpStatus === 200 || result.httpStatus === 201) && result.response?.succeeded) {
        clearEstimateState();
        setEstimateStaleMessage("La transferencia actualizo la UI. Calcula una nueva estimacion antes de volver a crear otra transferencia.");
        setSelectedGroupId("");
        setSelectedDestinationPlanetId("");
        setHasCreateTransferAcknowledgement(false);

        try {
          await refreshFleetUiState(uiState.civilizationId);
          setCreateTransferResult((currentResult) =>
            currentResult
              ? {
                  ...currentResult,
                  details: ["Estado actualizado desde la API.", ...currentResult.details],
                }
              : currentResult,
          );
        } catch (refreshError) {
          setCreateTransferNetworkError(
            refreshError instanceof Error
              ? `Transferencia creada, pero no se pudo refrescar la UI: ${refreshError.message}`
              : "Transferencia creada, pero no se pudo refrescar la UI.",
          );
        }
      }
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Error de red al crear la transferencia.";
      setCreateTransferResult(presentCreateTransferNetworkFailure(message));
    } finally {
      createTransferInFlightRef.current = false;
      setIsCreatingTransfer(false);
    }
  }

  function handlePrepareCancelTransfer(orbitalTransferId: string) {
    setPreparedCancelTransferId((currentValue) =>
      currentValue === orbitalTransferId ? "" : orbitalTransferId,
    );
    setHasCancelTransferAcknowledgement(false);
    setCancelTransferResult(null);
    setCancelTransferNetworkError(null);
    setCancelTransferStaleMessage(null);
  }

  function handlePrepareCompleteDueTransfer(groupId: string) {
    setPreparedCompleteDueGroupId((currentValue) => (currentValue === groupId ? "" : groupId));
    setHasCompleteDueAcknowledgement(false);
    setCompleteDueTransferResult(null);
    setCompleteDueTransferNetworkError(null);
    setCompleteDueTransferStaleMessage(null);
  }

  async function handleCompleteDueTransfers(group: FleetGroupSummary) {
    if (isCompletingDueTransfers) {
      setCompleteDueTransferNetworkError("Ya hay una solicitud de completar vencidos en curso.");
      return;
    }

    if (!hasCompleteDueAction) {
      setCompleteDueTransferNetworkError("La API de desarrollo no expone completar vencidos en este entorno.");
      return;
    }

    if (preparedCompleteDueGroupId !== group.id || !isTransferDueForUi(group)) {
      setCompleteDueTransferNetworkError("Falta una llegada vencida y visible para ejecutar completar vencidos.");
      return;
    }

    if (!hasCompleteDueAcknowledgement) {
      setCompleteDueTransferNetworkError("Confirma la accion explicita antes de completar vencidos.");
      return;
    }

    if (!uiState?.civilizationId) {
      setCompleteDueTransferNetworkError("Primero carga una cabina valida antes de ejecutar completar vencidos.");
      return;
    }

    setIsCompletingDueTransfers(true);
    setCompleteDueTransferResult(null);
    setCompleteDueTransferNetworkError(null);

    try {
      const result = await voidEmpiresApi.completeDueOrbitalTransfers({
        nowUtc: new Date().toISOString(),
      });

      setCompleteDueTransferResult(presentCompletionResult(result));

      if (result.httpStatus === 200 && result.response?.succeeded) {
        setPreparedCompleteDueGroupId("");
        setHasCompleteDueAcknowledgement(false);
        setCompleteDueTransferStaleMessage(
          "El lote controlado se ejecuto. La cabina debe refrescarse para confirmar que las llegadas vencidas ya no siguen activas.",
        );

        try {
          await refreshFleetUiState(uiState.civilizationId);
          setCompleteDueTransferResult((currentResult) =>
            currentResult
              ? {
                  ...currentResult,
                  details: ["Estado actualizado desde la API.", ...currentResult.details],
                }
              : currentResult,
          );
        } catch (refreshError) {
          setCompleteDueTransferNetworkError(
            refreshError instanceof Error
              ? `Lote aplicado, pero no se pudo refrescar la UI: ${refreshError.message}`
              : "Lote aplicado, pero no se pudo refrescar la UI.",
          );
        }
      }
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Error de red al completar traslados vencidos.";
      setCompleteDueTransferNetworkError(message);
    } finally {
      setIsCompletingDueTransfers(false);
    }
  }

  async function handleCancelTransfer(group: FleetGroupSummary) {
    const transferId = group.activeTransfer?.id;

    if (isCancellingTransfer) {
      setCancelTransferNetworkError("Ya hay una solicitud de anular traslado en curso.");
      return;
    }

    if (!uiState?.civilizationId || !transferId || preparedCancelTransferId !== transferId) {
      setCancelTransferNetworkError("Falta contexto visible y confirmado para cancelar la transferencia.");
      return;
    }

    if (!hasCancelTransferAcknowledgement) {
      setCancelTransferNetworkError("Confirma la accion explicita antes de enviar anular traslado.");
      return;
    }

    setIsCancellingTransfer(true);
    setCancelTransferResult(null);
    setCancelTransferNetworkError(null);

    try {
      const result = await voidEmpiresApi.cancelOrbitalTransfer({
        civilizationId: uiState.civilizationId,
        orbitalTransferId: transferId,
      });

      setCancelTransferResult(presentCancelTransferResult(result));

      if (result.httpStatus === 200 && result.response?.succeeded) {
        setPreparedCancelTransferId("");
        setHasCancelTransferAcknowledgement(false);
        setCancelTransferStaleMessage(
          "Estado actualizado desde la API. La transferencia cancelada ya no debe aparecer como activa.",
        );

        try {
          await refreshFleetUiState(uiState.civilizationId);
          setCancelTransferResult((currentResult) =>
            currentResult
              ? {
                  ...currentResult,
                  details: ["Estado actualizado desde la API.", ...currentResult.details],
                }
              : currentResult,
          );
        } catch (refreshError) {
          setCancelTransferNetworkError(
            refreshError instanceof Error
              ? `Cancelacion aplicada, pero no se pudo refrescar la UI: ${refreshError.message}`
              : "Cancelacion aplicada, pero no se pudo refrescar la UI.",
          );
        }
      }
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Network error while cancelling the transfer.";
      setCancelTransferResult(presentCancelTransferNetworkFailure(message));
    } finally {
      setIsCancellingTransfer(false);
    }
  }

  const inspectedReadinessItems = inspectedGroup
    ? buildFleetCommandReadiness(inspectedGroup, uiState?.actionHints ?? [])
    : [];
  const orderSteps = [
    {
      number: "1",
      label: "Escuadra",
      state: selectedGroup ? (effectiveDestinationPlanetId ? "complete" : "current") : "pending",
    },
    {
      number: "2",
      label: "Destino",
      state: effectiveDestinationPlanetId
        ? (estimateReviewCard || isEstimating ? "complete" : "current")
        : selectedGroup
          ? "current"
          : "pending",
    },
    {
      number: "3",
      label: "Calcular",
      state: isEstimating
        ? "current"
        : estimateReviewCard
          ? "complete"
          : selectedGroup && effectiveDestinationPlanetId
            ? "current"
            : "pending",
    },
    {
      number: "4",
      label: "Revisar",
      state: createTransferConfirmationState || createTransferResult
        ? "complete"
        : estimateReviewCard
          ? "current"
          : "pending",
    },
    {
      number: "5",
      label: "Confirmar",
      state: createTransferResult
        ? "complete"
        : createTransferConfirmationState
          ? "current"
          : "pending",
    },
  ] as const;

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Cabina fase 11X</UiBadge>
          <h2>Cabina de mando orbital</h2>
          <p>Revisa tu flota, elige una escuadra y confirma cada orden importante antes de moverla.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Resumen tactico</UiBadge>
          <UiBadge>Rail de escuadras</UiBadge>
          <UiBadge>Detalle enfocado</UiBadge>
          <UiBadge tone="warn">Ordenes protegidas</UiBadge>
        </div>
      </UiCard>

      <div className="fleet-preflight-strip">
        <UiCard className="panel fleet-dev-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Entrada de desarrollo</p>
              <h3>Cargar cabina</h3>
              <p>El cargador sigue visible, pero ya no ocupa la cabecera principal.</p>
            </div>
            <UiBadge tone="neutral">Secundario</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Identificador de civilizacion</span>
              <input
                type="text"
                value={civilizationId}
                onChange={(event) => setCivilizationId(event.target.value)}
                placeholder="00000000-0000-0000-0000-000000000000"
                spellCheck={false}
                aria-label="Identificador de civilizacion"
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Cargar flotas"}
            </button>
          </form>
          {error && <p className="error-text">{error}</p>}
        </UiCard>

        <UiCard className="panel fleet-dev-rules-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Seguridad operativa</p>
              <h3>Protecciones activas</h3>
              <p>Las advertencias siguen presentes, pero bajan a un rol de contexto antes de jugar.</p>
            </div>
            <UiBadge tone="warn">Protegido</UiBadge>
          </div>
          <ul className="stack-list compact-list fleet-dev-rules-list">
            <li>La estimacion sigue en solo lectura y nunca reserva escuadras ni gasta recursos.</li>
            <li>Crear traslado exige una estimacion vigente y una confirmacion explicita.</li>
            <li>Anular traslado exige un traslado activo visible y una confirmacion explicita.</li>
            <li>Cerrar llegadas, dividir y fusionar siguen visibles solo como referencia de prototipo.</li>
          </ul>
        </UiCard>
      </div>

      {summary && (
        <UiCard className="panel fleet-summary-deck">
          <div className="figma-section-header">
            <div className="fleet-identity-block">
              <p className="eyebrow">Resumen operativo</p>
              <h3>Resumen de flota</h3>
              <p>Estado compacto de la civilizacion cargada y su situacion orbital.</p>
              <p className="dev-meta">ID de civilizacion {formatCompactGuid(uiState?.civilizationId)}</p>
            </div>
            <UiBadge>Civilizacion cargada</UiBadge>
          </div>
          <div className="figma-stat-grid">
            <SummaryMetric label="Escuadras" value={summary.groups} />
            <SummaryMetric label="Traslados activos" value={summary.transfers} />
            <SummaryMetric label="Reservas locales" value={summary.resourceContexts} />
            <SummaryMetric label="Senales tacticas" value={summary.actionHints} />
          </div>
        </UiCard>
      )}

      {uiState && summary && summary.groups === 0 && summary.resourceContexts === 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado vacio</p>
              <h3>Aun no hay escuadras desplegadas</h3>
              <p>Esta civilizacion todavia no tiene escuadras orbitales, traslados activos ni reservas locales.</p>
            </div>
            <UiBadge tone="warn">Cero seguro</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Los contadores seguiran a cero hasta que otras herramientas creen escuadras.</li>
            <li>Los manifiestos siguen visibles como contexto contractual para futuras validaciones.</li>
          </ul>
        </UiCard>
      )}

      {!uiState ? (
        <UiCard className="panel fleet-transfer-overview-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Traslados activos</p>
              <h3>Movimientos en curso</h3>
              <p>Carga primero una flota para ver rutas activas, cancelaciones disponibles y llegadas vencidas.</p>
            </div>
            <UiBadge tone="neutral">Sin datos</UiBadge>
          </div>
          <p className="figma-panel-note">Todavia no hay datos de flota cargados en esta sesion.</p>
        </UiCard>
      ) : null}

      {uiState && (
        <div className="fleet-cockpit-layout">
          <UiCard className="panel fleet-group-rail">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Lista orbital</p>
                <h3>Escuadras</h3>
                <p>Esta vista siempre se centra en una sola escuadra operativa.</p>
              </div>
              <UiBadge>{uiState.groups.length} seguidas</UiBadge>
            </div>
            <div className="fleet-summary-list">
              {squadListItems.map(({ group, presentation }) => (
                <FleetSummaryPanel
                  key={group.id}
                  group={group}
                  presentation={presentation}
                  isSelected={inspectedGroup?.id === group.id}
                  hasDueTransfer={isTransferDueForUi(group)}
                  onSelect={setInspectedGroupId}
                />
              ))}
            </div>
          </UiCard>

          <div className="fleet-command-column">
            {inspectedGroup && uiState ? (
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Contexto cruzado</p>
                    <h3>Cabinas relacionadas</h3>
                  </div>
                  <UiBadge>Planeta y ruta</UiBadge>
                </div>
                <div className="selection-chip-row">
                  <Link
                    className="selection-chip selection-chip-active"
                    to={buildGalaxyUrl(uiState.civilizationId, undefined, inspectedGroup.currentPlanetId)}
                  >
                    Volver a Galaxia
                  </Link>
                  <Link
                    className="selection-chip"
                    to={buildPlanetUrl(uiState.civilizationId, inspectedGroup.currentPlanetId)}
                  >
                    Abrir planeta actual
                  </Link>
                  <Link
                    className="selection-chip"
                    to={buildConstructionUrl(uiState.civilizationId, inspectedGroup.currentPlanetId)}
                  >
                    Abrir construccion
                  </Link>
                  <Link
                    className="selection-chip"
                    to={buildShipyardUrl(uiState.civilizationId, inspectedGroup.currentPlanetId)}
                  >
                    Abrir Astillero
                  </Link>
                  {inspectedGroup.activeTransfer?.destinationPlanetId ? (
                    <Link
                      className="selection-chip"
                      to={buildPlanetUrl(uiState.civilizationId, inspectedGroup.activeTransfer.destinationPlanetId)}
                    >
                      Ver destino
                    </Link>
                  ) : null}
                </div>
              </UiCard>
            ) : null}

            {inspectedGroup ? (
              <FleetSelectedGroupPanel
                group={inspectedGroup}
                readinessItems={inspectedReadinessItems}
                groupTone={getGroupTone(inspectedGroup)}
                canCompleteDueTransfers={hasCompleteDueAction}
                dueTransferCount={dueTransferGroups.length}
                preparedCompleteDueGroupId={preparedCompleteDueGroupId}
                hasCompleteDueAcknowledgement={hasCompleteDueAcknowledgement}
                isCompletingDueTransfers={isCompletingDueTransfers}
                preparedCancelTransferId={preparedCancelTransferId}
                hasCancelTransferAcknowledgement={hasCancelTransferAcknowledgement}
                isCancellingTransfer={isCancellingTransfer}
                onPrepareCompleteDueTransfer={handlePrepareCompleteDueTransfer}
                onCompleteDueAcknowledgementChange={setHasCompleteDueAcknowledgement}
                onCompleteDueTransfers={handleCompleteDueTransfers}
                onPrepareCancelTransfer={handlePrepareCancelTransfer}
                onCancelAcknowledgementChange={setHasCancelTransferAcknowledgement}
                onCancelTransfer={handleCancelTransfer}
              />
            ) : null}

            <UiCard className="panel fleet-orders-panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Secuencia de orden</p>
                  <h3>Mover una escuadra</h3>
                  <p>El flujo de juego queda en cinco pasos: eliges escuadra, eliges destino, calculas, revisas y confirmas.</p>
                </div>
                <UiBadge tone="good">Flujo principal</UiBadge>
              </div>
              <div className="fleet-action-stage">
                <div className="fleet-order-step-grid">
                  {orderSteps.map((step) => (
                    <div
                      key={step.number}
                      className={`fleet-order-step fleet-order-step-${step.state}${step.number === "5" ? " fleet-order-step-warn" : ""}`}
                    >
                      <strong>{step.number}</strong>
                      <span>{step.label}</span>
                    </div>
                  ))}
                </div>
                <div className="subpanel fleet-action-context">
                  <FleetDataRow
                    label="Escuadra en foco"
                    value={
                      selectedGroup
                        ? formatSquadIdentity(selectedGroup)
                        : "Sin escuadra lista"
                    }
                  />
                  <FleetDataRow
                    label="Destino previsto"
                    value={effectiveDestinationPlanetId ? formatPlanetPrimaryLabel(effectiveDestinationPlanetId) : "Sin destino"}
                  />
                </div>
              </div>
              <form className="query-form" onSubmit={handleEstimateSubmit}>
                <label className="field">
                  <span>Grupo elegible</span>
                  <select
                    value={effectiveGroupId}
                    onChange={(event) => {
                      setSelectedGroupId(event.target.value);
                      setInspectedGroupId(event.target.value);
                      invalidateEstimate("El grupo cambio. Calcula una nueva estimacion antes de crear la transferencia.");
                    }}
                    disabled={isEstimating || estimateEligibleGroups.length === 0}
                    aria-label="Grupo elegible"
                  >
                    {estimateEligibleGroups.length === 0 ? (
                      <option value="">No hay grupos listos para estimacion</option>
                    ) : (
                      estimateEligibleGroups.map((group) => (
                        <option key={group.id} value={group.id}>
                          {formatSquadOptionLabel(group)}
                        </option>
                      ))
                    )}
                  </select>
                </label>
                <label className="field">
                  <span>Planeta destino</span>
                  <select
                    value={effectiveDestinationPlanetId}
                    onChange={(event) => {
                      setSelectedDestinationPlanetId(event.target.value);
                      invalidateEstimate("El destino cambio. Calcula una nueva estimacion antes de crear la transferencia.");
                    }}
                    disabled={isEstimating || destinationOptions.length === 0}
                    aria-label="Planeta destino"
                  >
                    {destinationOptions.length === 0 ? (
                      <option value="">Sin destinos disponibles</option>
                    ) : (
                      destinationOptions.map((planetId) => (
                        <option key={planetId} value={planetId}>
                          {formatFleetDestinationOptionLabel(planetId)}
                        </option>
                      ))
                    )}
                  </select>
                </label>
                <button type="submit" disabled={isEstimating || estimateEligibleGroups.length === 0 || destinationOptions.length === 0}>
                  {isEstimating ? "Calculando..." : "Calcular ruta"}
                </button>
              </form>
              <div className="figma-badge-row">
                <UiBadge>Estimacion</UiBadge>
                <UiBadge tone="good">Solo lectura</UiBadge>
                <UiBadge tone="warn">Ordenes protegidas</UiBadge>
              </div>
              {estimateStaleMessage ? <p className="error-text">{estimateStaleMessage}</p> : null}
              {estimateNetworkError ? <p className="error-text">Error de red: {estimateNetworkError}</p> : null}
              {createTransferNetworkError ? <p className="error-text">{createTransferNetworkError}</p> : null}
                {completeDueTransferStaleMessage ? <p className="figma-panel-note">{completeDueTransferStaleMessage}</p> : null}
                {completeDueTransferNetworkError ? <p className="error-text">{completeDueTransferNetworkError}</p> : null}
                {cancelTransferStaleMessage ? <p className="figma-panel-note">{cancelTransferStaleMessage}</p> : null}
                {cancelTransferNetworkError ? <p className="error-text">{cancelTransferNetworkError}</p> : null}
                {estimateReviewCard ? (
                  <section className="subpanel figma-subpanel fleet-estimate-digest fleet-estimate-review-card">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Paso 4</p>
                        <h4>{estimateReviewCard.title}</h4>
                        <p>{estimateReviewCard.summary}</p>
                      </div>
                      <div className="figma-badge-row">
                        <UiBadge tone={estimateReviewCard.tone}>{estimateReviewCard.statusLabel}</UiBadge>
                        {selectedGroup ? <UiBadge tone="neutral">{selectedGroup.quantity} unidades</UiBadge> : null}
                      </div>
                    </div>
                    <div className="fleet-estimate-facts fleet-estimate-review-grid">
                      {estimateReviewCard.facts.map((fact) => (
                        <FleetDataRow key={fact.label} label={fact.label} value={fact.value} />
                      ))}
                    </div>
                    {estimateReviewCard.warnings.length > 0 ? (
                      <ul className="stack-list compact-list">
                        {estimateReviewCard.warnings.map((detail) => (
                          <li key={detail}>{detail}</li>
                        ))}
                      </ul>
                    ) : null}
                    <p className="figma-panel-note fleet-estimate-next-step">
                      {createTransferConfirmationState
                        ? "Paso siguiente: confirma la orden protegida con esta misma revision vigente."
                        : "Paso siguiente: la confirmacion se desbloquea solo con una estimacion vigente para este grupo y destino."}
                    </p>
                  </section>
                ) : null}
              {createTransferConfirmationState ? (
                <section className="subpanel transfer-confirmation-panel fleet-action-primary-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Paso 5</p>
                      <h4>Enviar flota</h4>
                      <p>Solo se habilita cuando la ultima estimacion sigue vigente para este grupo y destino.</p>
                    </div>
                    <div className="figma-badge-row">
                      <UiBadge tone="warn">Accion protegida</UiBadge>
                      <UiBadge tone={createTransferConfirmationState.canPrepare ? "good" : "warn"}>
                        {createTransferConfirmationState.canPrepare ? "Lista para confirmar" : "Bloqueada"}
                      </UiBadge>
                    </div>
                  </div>
                  <div className="figma-data-list">
                    <FleetDataRow
                      label="Escuadra"
                      value={
                        selectedGroup
                          ? formatSquadIdentity(selectedGroup)
                          : "Sin escuadra"
                      }
                    />
                    <FleetDataRow label="Ruta" value={createTransferConfirmationState.routeSummary} />
                    <FleetDataRow label="Coste estimado" value={createTransferConfirmationState.costSummary} />
                  </div>
                  <p className="dev-meta">ID tactico {formatCompactGuid(selectedGroup?.id ?? "")}</p>
                  <ul className="stack-list compact-list">
                    {createTransferConfirmationState.details.map((detail) => (
                        <li key={detail}>{detail}</li>
                      ))}
                  </ul>
                  {createTransferConfirmationState.blockReason ? <p className="error-text">{createTransferConfirmationState.blockReason}</p> : null}
                  <div className="transfer-confirmation-flow">
                      <label className="confirmation-checkbox">
                        <input
                          type="checkbox"
                          checked={hasCreateTransferAcknowledgement}
                          onChange={(event) => setHasCreateTransferAcknowledgement(event.target.checked)}
                          disabled={!createTransferConfirmationState.canPrepare}
                        />
                        <span>Confirmo el envio de esta flota</span>
                      </label>
                    <div className="transfer-confirmation-actions">
                      <button
                        type="button"
                        onClick={handleCreateTransfer}
                        disabled={
                          isCreatingTransfer ||
                          !createTransferConfirmationState.canPrepare ||
                          !hasCreateTransferAcknowledgement
                        }
                      >
                        {isCreatingTransfer ? "Enviando..." : "Enviar flota"}
                      </button>
                    </div>
                    <p className="figma-panel-note">Sigue siendo una accion protegida de prototipo y nunca se ejecuta sin esta confirmacion.</p>
                  </div>
                </section>
              ) : null}
              {createTransferResult ? (
                <section className="subpanel figma-subpanel fleet-action-primary-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Orden confirmada</p>
                      <h4>{createTransferResult.label}</h4>
                    </div>
                    <UiBadge tone={createTransferResult.tone}>
                      {createTransferResult.tone === "good" ? "Mutacion aplicada" : "No aplicada"}
                    </UiBadge>
                  </div>
                  <p>{createTransferResult.summary}</p>
                  {createTransferResult.facts?.length ? (
                    <div className="fleet-estimate-facts fleet-estimate-review-grid">
                      {createTransferResult.facts.map((fact) => (
                        <FleetDataRow key={`create-result-${fact.label}`} label={fact.label} value={fact.value} />
                      ))}
                    </div>
                  ) : null}
                  {createTransferResult.details.length > 0 ? (
                    <ul className="stack-list compact-list">
                      {createTransferResult.details.map((detail) => (
                        <li key={detail}>{detail}</li>
                      ))}
                    </ul>
                  ) : null}
                </section>
              ) : null}
              {cancelTransferResult ? (
                <section className="subpanel figma-subpanel fleet-action-primary-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Resultado de accion</p>
                      <h4>{cancelTransferResult.label}</h4>
                    </div>
                    <UiBadge tone={cancelTransferResult.tone}>
                      {cancelTransferResult.tone === "good" ? "Mutacion aplicada" : "No aplicada"}
                    </UiBadge>
                  </div>
                  <p>{cancelTransferResult.summary}</p>
                  {cancelTransferResult.facts?.length ? (
                    <div className="fleet-estimate-facts fleet-estimate-review-grid">
                      {cancelTransferResult.facts.map((fact) => (
                        <FleetDataRow key={`cancel-result-${fact.label}`} label={fact.label} value={fact.value} />
                      ))}
                    </div>
                  ) : null}
                  {cancelTransferResult.details.length > 0 ? (
                    <ul className="stack-list compact-list">
                      {cancelTransferResult.details.map((detail) => (
                        <li key={detail}>{detail}</li>
                      ))}
                    </ul>
                  ) : null}
                </section>
              ) : null}
              {completeDueTransferResult ? (
                <section className="subpanel figma-subpanel fleet-action-primary-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Resultado de accion</p>
                      <h4>{completeDueTransferResult.label}</h4>
                    </div>
                    <UiBadge tone={completeDueTransferResult.tone}>
                      {completeDueTransferResult.tone === "good" ? "Mutacion aplicada" : "No aplicada"}
                    </UiBadge>
                  </div>
                  <p>{completeDueTransferResult.summary}</p>
                  {completeDueTransferResult.facts?.length ? (
                    <div className="fleet-estimate-facts fleet-estimate-review-grid">
                      {completeDueTransferResult.facts.map((fact) => (
                        <FleetDataRow key={`complete-result-${fact.label}`} label={fact.label} value={fact.value} />
                      ))}
                    </div>
                  ) : null}
                  {completeDueTransferResult.details.length > 0 ? (
                    <ul className="stack-list compact-list">
                      {completeDueTransferResult.details.map((detail) => (
                        <li key={detail}>{detail}</li>
                      ))}
                    </ul>
                  ) : null}
                </section>
              ) : null}
            </UiCard>

            <UiCard className="panel fleet-transfer-overview-panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Traslados activos</p>
                  <h3>Movimientos en curso</h3>
                  <p>Vista compacta de las escuadras que ya salieron y de las acciones controladas que realmente estan disponibles.</p>
                </div>
                <UiBadge tone={activeTransferGroups.length > 0 ? "warn" : "good"}>
                  {activeTransferGroups.length > 0 ? `${activeTransferGroups.length} en ruta` : "Sin movimientos"}
                </UiBadge>
              </div>
              {activeTransferGroups.length > 0 ? (
                <div className="figma-badge-row">
                  <UiBadge tone={cancellableTransferCount > 0 ? "good" : "neutral"}>
                    {cancellableTransferCount > 0
                      ? `${cancellableTransferCount} anulable${cancellableTransferCount === 1 ? "" : "s"}`
                      : "Sin anulaciones"}
                  </UiBadge>
                  <UiBadge tone={dueTransferGroups.length > 0 ? "warn" : "neutral"}>
                    {dueTransferGroups.length > 0
                      ? `${dueTransferGroups.length} vencido${dueTransferGroups.length === 1 ? "" : "s"}`
                      : "Sin vencidos"}
                  </UiBadge>
                </div>
              ) : null}
              {activeTransferGroups.length > 0 && cancellableTransferCount === 0 ? (
                <p className="figma-panel-note">Ninguna ruta visible puede cancelarse ahora mismo.</p>
              ) : null}
              {activeTransferGroups.length > 0 && dueTransferGroups.length === 0 ? (
                <p className="figma-panel-note">No hay llegadas vencidas; completar vencidos seguira oculto hasta que alguna ruta llegue a tiempo.</p>
              ) : null}
              {activeTransferItems.length > 0 ? (
                <div className="fleet-transfer-overview-grid">
                  {activeTransferItems.map(({ group, presentation }) => (
                    <section key={group.id} className="subpanel figma-subpanel fleet-transfer-overview-card">
                      <div className="figma-section-header">
                        <div>
                          <p className="eyebrow">Escuadra en transito</p>
                          <h4>{presentation.title}</h4>
                          <p className="dev-meta">ID tactico {formatCompactGuid(group.id)}</p>
                        </div>
                        <div className="figma-badge-row">
                          <UiBadge tone={presentation.statusTone}>{presentation.statusLabel}</UiBadge>
                          <UiBadge tone={presentation.phaseTone}>{presentation.phaseLabel}</UiBadge>
                        </div>
                      </div>
                      {presentation.progressValue !== null ? (
                        <UiProgressBar value={presentation.progressValue} tone="neutral" />
                      ) : null}
                      <div className="figma-data-list">
                        <FleetDataRow label="Origen" value={presentation.originLabel} />
                        <FleetDataRow label="Destino" value={presentation.destinationLabel} />
                        <FleetDataRow label="Salida" value={presentation.departureLabel} />
                        <FleetDataRow label="Llegada" value={presentation.arrivalLabel} />
                        <FleetDataRow label="Avance" value={presentation.progressLabel} />
                      </div>
                      <div className="figma-badge-row">
                        {presentation.canCancel ? <UiBadge tone="good">Cancelar ruta</UiBadge> : null}
                        {presentation.canCompleteDue ? <UiBadge tone="warn">Cerrar llegada</UiBadge> : null}
                      </div>
                      {presentation.hasTimingGap ? (
                        <p className="figma-panel-note">Faltan marcas horarias legibles para decidir progreso o completado desde esta tarjeta.</p>
                      ) : null}
                      {!presentation.canCancel && !presentation.canCompleteDue ? (
                        <p className="figma-panel-note">Esta ruta sigue visible, pero ninguna mutacion controlada esta habilitada todavia.</p>
                      ) : null}
                      <div className="transfer-confirmation-actions">
                        <button
                          type="button"
                          onClick={() => {
                            setInspectedGroupId(group.id);
                            setSelectedGroupId(group.id);
                          }}
                        >
                          Ver escuadra
                        </button>
                      </div>
                    </section>
                  ))}
                </div>
              ) : (
                <p className="figma-panel-note">No hay traslados activos en esta civilizacion.</p>
              )}
            </UiCard>

            {uiState?.resourceContexts?.length ? (
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Apoyo logistico</p>
                    <h3>Reservas por planeta</h3>
                  </div>
                  <UiBadge>{uiState.resourceContexts.length} planetas</UiBadge>
                </div>
                <div className="readiness-grid">
                  {uiState.resourceContexts.map((context) => (
                    <section key={context.planetId} className="subpanel figma-subpanel">
                      <div className="figma-section-header">
                        <div>
                          <p className="eyebrow">Planeta actual</p>
                          <h4>{formatPlanetPrimaryLabel(context.planetId)}</h4>
                          {formatPlanetSecondaryLabel(context.planetId) ? (
                            <p className="dev-meta">{formatPlanetSecondaryLabel(context.planetId)}</p>
                          ) : null}
                        </div>
                        <UiBadge tone="resource">{(context.balances ?? []).length} balances</UiBadge>
                      </div>
                      <div className="figma-data-list">
                        {(context.balances ?? []).map((balance) => (
                          <FleetDataRow
                            key={`${context.planetId}-${balance.resourceType}`}
                            label={formatResourceType(balance.resourceType)}
                            value={String(balance.quantity)}
                          />
                        ))}
                      </div>
                    </section>
                  ))}
                </div>
              </UiCard>
            ) : null}

            {uiState?.interceptionNotes?.length ? (
              <UiCard className="panel fleet-technical-panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Tecnico secundario</p>
                    <h3>Alertas de intercepcion</h3>
                    <p>Se mantienen visibles como lectura operativa, sin entrar en el flujo principal de ordenes.</p>
                  </div>
                  <UiBadge tone="warn">Solo informativo</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {uiState.interceptionNotes.map((note, index) => (
                    <li key={`interception-${index}`}>{formatNote(note)}</li>
                  ))}
                </ul>
              </UiCard>
            ) : null}
          </div>
        </div>
      )}

      {((fleetManifest.length > 0 || strategicMapManifest.length > 0) ||
        (uiState && mutationConfirmations.length > 0)) && (
        <details className="fleet-technical-disclosure">
          <summary>
            <span>Detalles de desarrollo y contratos</span>
            <UiBadge tone="warn">Secundario</UiBadge>
          </summary>

          {(fleetManifest.length > 0 || strategicMapManifest.length > 0) && (
            <div className="fleet-manifest-grid">
              {fleetManifest.length > 0 && (
                <ActionManifestPanel
                  title="Tecnico: manifiesto de acciones de flota"
                  actions={fleetManifest}
                  mutationConfirmations={mutationConfirmations}
                />
              )}
              {strategicMapManifest.length > 0 && (
                <ActionManifestPanel
                  title="Tecnico: manifiesto del mapa estrategico"
                  actions={strategicMapManifest}
                />
              )}
            </div>
          )}

          {uiState && mutationConfirmations.length > 0 && (
            <UiCard className="panel fleet-prototype-panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Solo prototipo</p>
                  <h3>Futuras mutaciones de flota</h3>
                  <p>Se dejan visibles como referencia tecnica, pero no forman parte del flujo principal de ordenes.</p>
                </div>
                <UiBadge tone="warn">Solo prototipo</UiBadge>
              </div>
              <div className="prototype-control-grid prototype-control-grid-compact">
                {mutationConfirmations.map((control) => (
                  <section key={control.actionKey} className="subpanel prototype-control-card">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Accion futura</p>
                        <h4>{control.label}</h4>
                      </div>
                      <div className="figma-badge-row">
                        <UiBadge tone={control.readinessTone}>{control.readinessLabel}</UiBadge>
                        <UiBadge tone={control.prototypeLevel === "danger" ? "warn" : "neutral"}>
                          {control.prototypeLevel === "danger" ? "Riesgo" : "Solo prototipo"}
                        </UiBadge>
                      </div>
                    </div>
                    <p className="figma-panel-note">{control.mutationSummary}</p>
                    <div className="figma-data-list">
                      <FleetDataRow label="Requiere confirmacion" value={control.confirmationText} />
                      <FleetDataRow label="Motivo del bloqueo" value={control.disabledReason} />
                    </div>
                  </section>
                ))}
              </div>
            </UiCard>
          )}
        </details>
      )}
    </section>
  );
}

