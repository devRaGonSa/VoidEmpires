import { FormEvent, useEffect, useMemo, useRef, useState } from "react";
import type { ActionManifestAction } from "../api/actionManifestTypes";
import type {
  EstimateOrbitalTravelResponse,
  FleetCommandApiResult,
} from "../api/fleetCommandTypes";
import type { FleetCommandPresentationItem } from "../utils/fleetCommandPresentation";
import type { FleetGroupSummary, FleetUiState } from "../api/fleetTypes";
import type { ReadinessNote } from "../api/strategicMapTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { ActionManifestPanel } from "../components/ActionManifestPanel";
import { FleetSummaryPanel } from "../components/FleetSummaryPanel";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { UiProgressBar } from "../components/ui/UiProgressBar";
import {
  formatBooleanLabel,
  formatCompactGuid,
  formatFuelReadinessPolicy,
  formatOrbitalGroupStatus,
  formatPlanetReference,
  formatResourceType,
  formatSpaceAssetType,
  formatTransferStatus,
} from "../utils/domainPresentation";
import {
  buildFleetCommandReadiness,
  buildFleetMutationConfirmations,
  presentCancelTransferNetworkFailure,
  presentCancelTransferResult,
  presentCreateTransferNetworkFailure,
  presentCreateTransferResult,
  presentEstimateResult,
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

  return note.note ?? "Readiness metadata present.";
}

function getTransferProgress(group: FleetGroupSummary) {
  const activeTransfer = group.activeTransfer;
  if (!activeTransfer) {
    return null;
  }

  const departure = Date.parse(activeTransfer.departureAtUtc);
  const arrival = Date.parse(activeTransfer.arrivalAtUtc);
  const now = Date.now();

  if (Number.isNaN(departure) || Number.isNaN(arrival) || arrival <= departure) {
    return null;
  }

  return Math.max(0, Math.min(100, ((now - departure) / (arrival - departure)) * 100));
}

function formatTransferProgressLabel(group: FleetGroupSummary) {
  const progress = getTransferProgress(group);
  return progress === null ? null : `${Math.round(progress)}% completado`;
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

export function FleetsPage() {
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
  const [estimateResult, setEstimateResult] = useState<FleetCommandPresentationItem | null>(null);
  const [estimateNetworkError, setEstimateNetworkError] = useState<string | null>(null);
  const [estimateSnapshot, setEstimateSnapshot] = useState<EstimateSnapshot | null>(null);
  const [estimateStaleMessage, setEstimateStaleMessage] = useState<string | null>(null);
  const [hasCreateTransferAcknowledgement, setHasCreateTransferAcknowledgement] = useState(false);
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
        formatPlanetReference(left).localeCompare(formatPlanetReference(right)),
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
          ? liveEstimateResponse.fuelReadiness?.notReadyReason ?? "La metadata de fuel readiness sigue bloqueando la accion."
          : null,
      routeSummary: `${formatPlanetReference(selectedGroup.currentPlanetId)} -> ${formatPlanetReference(liveEstimateResponse.destinationPlanetId ?? effectiveDestinationPlanetId)}`,
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
              `Fuel readiness ${liveEstimateResponse.fuelReadiness.isFuelReady ? "lista" : "bloqueada"}`,
            ]
          : []),
        ...missingResources,
      ],
    };
  }, [effectiveDestinationPlanetId, liveEstimateResponse, selectedGroup]);

  function clearEstimateState() {
    setEstimateApiResult(null);
    setEstimateResult(null);
    setEstimateNetworkError(null);
    setEstimateSnapshot(null);
  }

  function invalidateEstimate(reason: string) {
    if (estimateSnapshot || estimateApiResult || estimateResult) {
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
      throw new Error(uiStateResponse.errors[0] ?? "Fleet UI state refresh failed.");
    }

    setUiState(uiStateResponse.uiState);
  }

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
      setError("Civilization id is required.");
      setUiState(null);
      return;
    }

    setIsLoading(true);
    setError(null);
    if (uiState && estimateSnapshot) {
      invalidateEstimate("La UI de flotas se actualizo. La estimacion anterior ya no es valida.");
    } else {
      clearEstimateState();
      setEstimateStaleMessage(null);
      resetCreateTransferAcknowledgement();
    }

    try {
      const [uiStateResponse, fleetManifestResponse, strategicMapManifestResponse] =
        await Promise.all([
          voidEmpiresApi.getFleetUiState(trimmedCivilizationId),
          voidEmpiresApi.getFleetActionManifest(),
          voidEmpiresApi.getStrategicMapActionManifest(),
        ]);

      if (!uiStateResponse.succeeded || !uiStateResponse.uiState) {
        setUiState(null);
        setError(uiStateResponse.errors[0] ?? "Fleet UI state request failed.");
        return;
      }

      setUiState(uiStateResponse.uiState);
      setFleetManifest(fleetManifestResponse.manifest?.actions ?? []);
      setStrategicMapManifest(strategicMapManifestResponse.manifest?.actions ?? []);
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Fleet requests failed.";
      setUiState(null);
      setError(message);
    } finally {
      setIsLoading(false);
    }
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
      setEstimateResult(presentEstimateResult(result));
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
          : "Network error while requesting travel estimate.";
      setEstimateNetworkError(message);
    } finally {
      setIsEstimating(false);
    }
  }

  async function handleCreateTransfer() {
    if (createTransferInFlightRef.current || isCreatingTransfer) {
      setCreateTransferNetworkError("Ya hay una solicitud de create transfer en curso.");
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
      setCreateTransferNetworkError("Confirma la accion explicita antes de enviar create transfer.");
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
          : "Network error while creating the transfer.";
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

  async function handleCancelTransfer(group: FleetGroupSummary) {
    const transferId = group.activeTransfer?.id;

    if (isCancellingTransfer) {
      setCancelTransferNetworkError("Ya hay una solicitud de cancel transfer en curso.");
      return;
    }

    if (!uiState?.civilizationId || !transferId || preparedCancelTransferId !== transferId) {
      setCancelTransferNetworkError("Falta contexto visible y confirmado para cancelar la transferencia.");
      return;
    }

    if (!hasCancelTransferAcknowledgement) {
      setCancelTransferNetworkError("Confirma la accion explicita antes de enviar cancel transfer.");
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

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Phase 11X cockpit layout</UiBadge>
          <h2>Fleet command cockpit</h2>
          <p>Scan the fleet footprint, focus one orbital group at a time, and keep development-only execution behind explicit confirmation.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Summary deck</UiBadge>
          <UiBadge>Group rail</UiBadge>
          <UiBadge>Selected detail</UiBadge>
          <UiBadge tone="warn">Guarded create and cancel</UiBadge>
        </div>
      </UiCard>

      <div className="fleet-cockpit-top">
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Development endpoint</p>
              <h3>Load cockpit state</h3>
            </div>
            <UiBadge>Development surface</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Civilization id</span>
              <input
                type="text"
                value={civilizationId}
                onChange={(event) => setCivilizationId(event.target.value)}
                placeholder="00000000-0000-0000-0000-000000000000"
                spellCheck={false}
                aria-label="Civilization id"
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Loading..." : "Load fleet panels"}
            </button>
          </form>
          {error && <p className="error-text">{error}</p>}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Execution boundaries</p>
              <h3>Command rules</h3>
            </div>
            <UiBadge tone="warn">Prototype guarded</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Estimate remains read-only and never reserves groups or spends resources.</li>
            <li>Create transfer requires a matching fresh estimate plus explicit confirmation.</li>
            <li>Cancel transfer requires an active visible transfer plus explicit confirmation.</li>
            <li>Complete-due, split, and merge stay visible as prototype metadata only.</li>
          </ul>
        </UiCard>
      </div>

      {summary && (
        <UiCard className="panel fleet-summary-deck">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Operational summary</p>
              <h3>Command deck</h3>
              <p>Compact status for the loaded civilization and the current fleet-readiness footprint.</p>
            </div>
            <UiBadge>{formatCompactGuid(uiState?.civilizationId)}</UiBadge>
          </div>
          <div className="figma-stat-grid">
            <SummaryMetric label="Groups" value={summary.groups} />
            <SummaryMetric label="Active transfers" value={summary.transfers} />
            <SummaryMetric label="Resource contexts" value={summary.resourceContexts} />
            <SummaryMetric label="Action hints" value={summary.actionHints} />
          </div>
        </UiCard>
      )}

      {uiState && summary && summary.groups === 0 && summary.resourceContexts === 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Empty development state</p>
              <h3>No orbital groups deployed yet</h3>
              <p>This civilization currently has no orbital groups, active transfers, or local resource contexts.</p>
            </div>
            <UiBadge tone="warn">Safe zero-state</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Fleet counters remain at zero until groups are seeded or created elsewhere.</li>
            <li>Action manifests remain available as contract context for later validation.</li>
          </ul>
        </UiCard>
      )}

      {uiState && (
        <div className="fleet-cockpit-layout">
          <UiCard className="panel fleet-group-rail">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Orbital group list</p>
                <h3>Group rail</h3>
                <p>Use the rail to focus the command cockpit on one group at a time.</p>
              </div>
              <UiBadge>{uiState.groups.length} tracked</UiBadge>
            </div>
            <div className="fleet-summary-list">
              {uiState.groups.map((group) => (
                <FleetSummaryPanel
                  key={group.id}
                  group={group}
                  isSelected={inspectedGroup?.id === group.id}
                  onSelect={setInspectedGroupId}
                />
              ))}
            </div>
          </UiCard>

          <div className="fleet-command-column">
            {inspectedGroup ? (
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Selected group</p>
                    <h3>{formatSpaceAssetType(inspectedGroup.assetType)}</h3>
                    <p>{formatCompactGuid(inspectedGroup.id)}</p>
                  </div>
                  <div className="figma-badge-row">
                    <UiBadge tone={getGroupTone(inspectedGroup)}>{formatOrbitalGroupStatus(inspectedGroup.status)}</UiBadge>
                    {inspectedGroup.routeFuelReadiness?.fuelReadinessPolicy ? (
                      <UiBadge>{formatFuelReadinessPolicy(inspectedGroup.routeFuelReadiness.fuelReadinessPolicy)}</UiBadge>
                    ) : null}
                  </div>
                </div>

                <div className="figma-stat-grid">
                  <SummaryMetric label="Quantity" value={inspectedGroup.quantity} />
                  <SummaryMetric label="Transfer distance" value={inspectedGroup.activeTransfer?.abstractDistanceUnits ?? 0} />
                </div>

                <div className="fleet-selected-grid">
                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Position</p>
                        <h4>Deployment context</h4>
                      </div>
                      <UiBadge>Live UI state</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <FleetDataRow label="Current planet" value={formatPlanetReference(inspectedGroup.currentPlanetId)} />
                      <FleetDataRow label="Origin planet" value={formatPlanetReference(inspectedGroup.originPlanetId)} />
                      <FleetDataRow label="Stationed away" value={formatBooleanLabel(inspectedGroup.isStationedAwayFromOrigin)} />
                    </div>
                  </section>

                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Command readiness</p>
                        <h4>Read-only planning state</h4>
                      </div>
                      <UiBadge tone="warn">Inspection only</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      {inspectedReadinessItems.map((item) => (
                        <FleetDataRow key={`${inspectedGroup.id}-${item.key}`} label={item.label} value={item.summary} />
                      ))}
                    </div>
                    {inspectedReadinessItems.some((item) => item.details.length > 0) ? (
                      <ul className="stack-list compact-list">
                        {inspectedReadinessItems.flatMap((item) =>
                          item.details.map((detail) => (
                            <li key={`${inspectedGroup.id}-${item.key}-${detail}`}>{item.label}: {detail}</li>
                          )),
                        )}
                      </ul>
                    ) : null}
                  </section>
                </div>

                {inspectedGroup.activeTransfer ? (
                  <div className="figma-transfer-card">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Transfer status</p>
                        <h4>{formatTransferStatus(inspectedGroup.activeTransfer.status)}</h4>
                      </div>
                      <UiBadge tone="warn">{formatPlanetReference(inspectedGroup.activeTransfer.destinationPlanetId)}</UiBadge>
                    </div>
                    {getTransferProgress(inspectedGroup) !== null ? (
                      <UiProgressBar value={getTransferProgress(inspectedGroup) ?? 0} tone="neutral" />
                    ) : null}
                    <div className="figma-data-list">
                      <FleetDataRow label="Transfer" value={formatCompactGuid(inspectedGroup.activeTransfer.id)} />
                      <FleetDataRow label="Destination" value={formatPlanetReference(inspectedGroup.activeTransfer.destinationPlanetId)} />
                      <FleetDataRow label="Departure" value={inspectedGroup.activeTransfer.departureAtUtc} />
                      <FleetDataRow label="Arrival" value={inspectedGroup.activeTransfer.arrivalAtUtc} />
                      {formatTransferProgressLabel(inspectedGroup) ? (
                        <FleetDataRow label="Progress" value={formatTransferProgressLabel(inspectedGroup) ?? "Sin progreso"} />
                      ) : null}
                    </div>
                    <div className="transfer-confirmation-actions">
                      <button
                        type="button"
                        className="prototype-control-button transfer-prepare-button"
                        onClick={() => handlePrepareCancelTransfer(inspectedGroup.activeTransfer?.id ?? "")}
                        disabled={!inspectedGroup.activeTransfer?.id || !inspectedGroup.commands?.canCancelTransfer}
                      >
                        {preparedCancelTransferId === inspectedGroup.activeTransfer.id ? "Ocultar confirmacion" : "Preparar cancelacion"}
                      </button>
                    </div>
                    {preparedCancelTransferId === inspectedGroup.activeTransfer.id && (
                      <section className="subpanel transfer-confirmation-panel">
                        <div className="figma-section-header">
                          <div>
                            <p className="eyebrow">Accion de desarrollo</p>
                            <h4>Cancelar transferencia orbital</h4>
                            <p>La cancelacion sigue protegida y no reembolsa recursos ya cobrados.</p>
                          </div>
                          <div className="figma-badge-row">
                            <UiBadge tone="warn">Accion de desarrollo</UiBadge>
                            <UiBadge tone="warn">No reembolsa recursos</UiBadge>
                          </div>
                        </div>
                        <div className="figma-data-list">
                          <FleetDataRow label="Transfer" value={formatCompactGuid(inspectedGroup.activeTransfer.id)} />
                          <FleetDataRow label="Grupo" value={formatCompactGuid(inspectedGroup.id)} />
                          <FleetDataRow label="Origen" value={formatPlanetReference(inspectedGroup.originPlanetId)} />
                          <FleetDataRow label="Planeta actual" value={formatPlanetReference(inspectedGroup.currentPlanetId)} />
                          <FleetDataRow label="Destino" value={formatPlanetReference(inspectedGroup.activeTransfer.destinationPlanetId)} />
                          <FleetDataRow label="Llegada" value={inspectedGroup.activeTransfer.arrivalAtUtc} />
                        </div>
                        <div className="transfer-confirmation-flow">
                          <label className="confirmation-checkbox">
                            <input
                              type="checkbox"
                              checked={hasCancelTransferAcknowledgement}
                              onChange={(event) => setHasCancelTransferAcknowledgement(event.target.checked)}
                            />
                            <span>Requiere confirmacion explicita</span>
                          </label>
                          <div className="transfer-confirmation-actions">
                            <button
                              type="button"
                              onClick={() => handleCancelTransfer(inspectedGroup)}
                              disabled={
                                isCancellingTransfer ||
                                !hasCancelTransferAcknowledgement ||
                                preparedCancelTransferId !== inspectedGroup.activeTransfer.id
                              }
                            >
                              {isCancellingTransfer ? "Cancelando..." : "Cancelar transferencia orbital"}
                            </button>
                          </div>
                        </div>
                      </section>
                    )}
                    {inspectedGroup.activeTransfer.interceptionReadiness?.readinessNote ? (
                      <p className="figma-panel-note">{inspectedGroup.activeTransfer.interceptionReadiness.readinessNote}</p>
                    ) : null}
                  </div>
                ) : null}

                {inspectedGroup.routeFuelReadiness?.notes?.length ? (
                  <ul className="stack-list compact-list">
                    {inspectedGroup.routeFuelReadiness.notes.map((note) => (
                      <li key={note}>{note}</li>
                    ))}
                  </ul>
                ) : null}
              </UiCard>
            ) : null}

            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Command execution</p>
                  <h3>Estimate and guarded actions</h3>
                  <p>The command column keeps read-only preview, explicit confirmation, and result feedback together.</p>
                </div>
                <UiBadge tone="good">POST read-only + guarded mutation</UiBadge>
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
                          {formatSpaceAssetType(group.assetType)} - {formatPlanetReference(group.currentPlanetId)} - {formatCompactGuid(group.id)}
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
                          {formatPlanetReference(planetId)}
                        </option>
                      ))
                    )}
                  </select>
                </label>
                <button type="submit" disabled={isEstimating || estimateEligibleGroups.length === 0 || destinationOptions.length === 0}>
                  {isEstimating ? "Calculando..." : "Calcular estimacion"}
                </button>
              </form>
              <div className="figma-badge-row">
                <UiBadge>Estimacion</UiBadge>
                <UiBadge tone="good">Solo lectura</UiBadge>
                <UiBadge tone="warn">Mutaciones protegidas</UiBadge>
              </div>
              {estimateStaleMessage ? <p className="error-text">{estimateStaleMessage}</p> : null}
              {estimateNetworkError ? <p className="error-text">Network error: {estimateNetworkError}</p> : null}
              {createTransferNetworkError ? <p className="error-text">{createTransferNetworkError}</p> : null}
              {cancelTransferStaleMessage ? <p className="figma-panel-note">{cancelTransferStaleMessage}</p> : null}
              {cancelTransferNetworkError ? <p className="error-text">{cancelTransferNetworkError}</p> : null}
              {estimateResult ? (
                <section className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Estimate result</p>
                      <h4>{estimateResult.label}</h4>
                    </div>
                    <UiBadge tone={estimateResult.tone}>{estimateResult.tone === "good" ? "Listo" : "Atencion"}</UiBadge>
                  </div>
                  <p>{estimateResult.summary}</p>
                  {estimateResult.details.length > 0 ? (
                    <ul className="stack-list compact-list">
                      {estimateResult.details.map((detail) => (
                        <li key={detail}>{detail}</li>
                      ))}
                    </ul>
                  ) : null}
                </section>
              ) : null}
              {createTransferConfirmationState ? (
                <section className="subpanel transfer-confirmation-panel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Accion de desarrollo</p>
                      <h4>Crear transferencia orbital</h4>
                      <p>Solo se habilita cuando la ultima estimacion sigue vigente para este grupo y destino.</p>
                    </div>
                    <div className="figma-badge-row">
                      <UiBadge tone="warn">Accion de desarrollo</UiBadge>
                      <UiBadge tone={createTransferConfirmationState.canPrepare ? "good" : "warn"}>
                        {createTransferConfirmationState.canPrepare ? "Lista para confirmar" : "Bloqueada"}
                      </UiBadge>
                    </div>
                  </div>
                  <div className="figma-data-list">
                    <FleetDataRow label="Grupo" value={formatCompactGuid(selectedGroup?.id ?? "")} />
                    <FleetDataRow label="Ruta" value={createTransferConfirmationState.routeSummary} />
                    <FleetDataRow label="Coste estimado" value={createTransferConfirmationState.costSummary} />
                  </div>
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
                      <span>Requiere confirmacion explicita</span>
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
                        {isCreatingTransfer ? "Creando..." : "Crear transferencia orbital"}
                      </button>
                    </div>
                    <p className="figma-panel-note">La accion sigue marcada como desarrollo y nunca se ejecuta sin esta confirmacion explicita.</p>
                  </div>
                </section>
              ) : null}
              {createTransferResult ? (
                <section className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Mutation result</p>
                      <h4>{createTransferResult.label}</h4>
                    </div>
                    <UiBadge tone={createTransferResult.tone}>
                      {createTransferResult.tone === "good" ? "Mutacion aplicada" : "No aplicada"}
                    </UiBadge>
                  </div>
                  <p>{createTransferResult.summary}</p>
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
                <section className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Mutation result</p>
                      <h4>{cancelTransferResult.label}</h4>
                    </div>
                    <UiBadge tone={cancelTransferResult.tone}>
                      {cancelTransferResult.tone === "good" ? "Mutacion aplicada" : "No aplicada"}
                    </UiBadge>
                  </div>
                  <p>{cancelTransferResult.summary}</p>
                  {cancelTransferResult.details.length > 0 ? (
                    <ul className="stack-list compact-list">
                      {cancelTransferResult.details.map((detail) => (
                        <li key={detail}>{detail}</li>
                      ))}
                    </ul>
                  ) : null}
                </section>
              ) : null}
            </UiCard>

            {uiState?.resourceContexts?.length ? (
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Transfer support</p>
                    <h3>Resource contexts</h3>
                  </div>
                  <UiBadge>{uiState.resourceContexts.length} planets</UiBadge>
                </div>
                <div className="readiness-grid">
                  {uiState.resourceContexts.map((context) => (
                    <section key={context.planetId} className="subpanel figma-subpanel">
                      <div className="figma-section-header">
                        <div>
                          <p className="eyebrow">Current planet</p>
                          <h4>{formatPlanetReference(context.planetId)}</h4>
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
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Transfer status</p>
                    <h3>Interception notes</h3>
                  </div>
                  <UiBadge tone="warn">Informational only</UiBadge>
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

      {uiState && mutationConfirmations.length > 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Prototype only</p>
              <h3>Guarded mutation manifest</h3>
              <p>Visible for discoverability, but still blocked from ordinary fleet execution.</p>
            </div>
            <UiBadge tone="warn">Mutacion protegida</UiBadge>
          </div>
          <div className="prototype-control-grid">
            {mutationConfirmations.map((control) => (
              <section key={control.actionKey} className="subpanel prototype-control-card">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Mutation contract</p>
                    <h4>{control.label}</h4>
                    <p>{control.mutationSummary}</p>
                  </div>
                  <div className="figma-badge-row">
                    <UiBadge tone="warn">Mutacion</UiBadge>
                    <UiBadge tone={control.readinessTone}>{control.readinessLabel}</UiBadge>
                    <UiBadge tone={control.prototypeLevel === "danger" ? "warn" : "neutral"}>
                      {control.prototypeLevel === "danger" ? "Danger" : "Prototype only"}
                    </UiBadge>
                  </div>
                </div>
                <button type="button" className="prototype-control-button" disabled>
                  {control.label}
                </button>
                <div className="figma-data-list">
                  <FleetDataRow label="Confirmation" value={control.confirmationText} />
                  <FleetDataRow label="Disabled reason" value={control.disabledReason} />
                </div>
              </section>
            ))}
          </div>
        </UiCard>
      )}

      {(fleetManifest.length > 0 || strategicMapManifest.length > 0) && (
        <div className="fleet-manifest-grid">
          {fleetManifest.length > 0 && (
            <ActionManifestPanel
              title="Fleet action manifest"
              actions={fleetManifest}
              mutationConfirmations={mutationConfirmations}
            />
          )}
          {strategicMapManifest.length > 0 && (
            <ActionManifestPanel
              title="Strategic map action manifest"
              actions={strategicMapManifest}
            />
          )}
        </div>
      )}
    </section>
  );
}

