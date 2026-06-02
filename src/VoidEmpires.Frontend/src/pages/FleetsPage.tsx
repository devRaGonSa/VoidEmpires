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
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Phase 9M fleet alignment</UiBadge>
          <h2>Fleet UI state and action manifests</h2>
          <p>
            Fleet groups, active transfers, and route/readiness metadata are
            grouped into compact Figma-style cards while mutating actions remain
            visible as development-only contracts and remain unwired.
          </p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Manifest metadata only</UiBadge>
          <UiBadge tone="warn">Prototype-only mutation contracts</UiBadge>
          <UiBadge>Progress bars for active transfers</UiBadge>
          <UiBadge tone="warn">Create transfer guarded</UiBadge>
        </div>
      </UiCard>

      <div className="figma-two-column">
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Development endpoint</p>
              <h3>Load fleet inspection state</h3>
            </div>
            <UiBadge>Read-only fleet surface</UiBadge>
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
              <p className="eyebrow">Current limits</p>
              <h3>Constraints</h3>
            </div>
            <UiBadge tone="warn">No mutations</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Action manifests remain documentation and readiness metadata only.</li>
            <li>Create transfer stays behind explicit confirmation, while cancel remains preparation-only.</li>
            <li>Route/fuel and interception details remain non-authoritative hints.</li>
            <li>All responses are development tooling, not production gameplay APIs.</li>
          </ul>
        </UiCard>
      </div>

      {uiState && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estimacion</p>
              <h3>Solo lectura orbital</h3>
              <p>No ejecuta movimiento, no reserva grupos y no consume recursos.</p>
            </div>
            <UiBadge tone="good">POST read-only</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleEstimateSubmit}>
            <label className="field">
                <span>Grupo elegible</span>
              <select
                value={effectiveGroupId}
                onChange={(event) => {
                  setSelectedGroupId(event.target.value);
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
            <button
              type="submit"
              disabled={
                isEstimating ||
                estimateEligibleGroups.length === 0 ||
                destinationOptions.length === 0
              }
            >
              {isEstimating ? "Calculando..." : "Calcular estimacion"}
            </button>
          </form>
          <div className="figma-badge-row">
            <UiBadge>Estimacion</UiBadge>
            <UiBadge tone="good">Solo lectura</UiBadge>
            <UiBadge tone="warn">No ejecuta movimiento</UiBadge>
          </div>
          {estimateStaleMessage ? <p className="error-text">{estimateStaleMessage}</p> : null}
          {estimateNetworkError ? <p className="error-text">Network error: {estimateNetworkError}</p> : null}
          {createTransferNetworkError ? <p className="error-text">{createTransferNetworkError}</p> : null}
          {estimateResult ? (
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Resultado</p>
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
                  <p>Mutara datos de desarrollo cuando la ejecucion real se habilite en una fase posterior.</p>
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
              {createTransferConfirmationState.blockReason ? (
                <p className="error-text">{createTransferConfirmationState.blockReason}</p>
              ) : null}
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
                <p className="figma-panel-note">
                  La accion sigue marcada como desarrollo, exige una estimacion vigente, y nunca se ejecuta sin esta confirmacion explicita.
                </p>
              </div>
            </section>
          ) : null}
          {createTransferResult ? (
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Resultado de mutacion</p>
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
        </UiCard>
      )}

      {uiState && mutationConfirmations.length > 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Prototype only</p>
              <h3>Guarded mutation controls</h3>
              <p>Visible for discoverability, disabled by design, and never executed from this page.</p>
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

      {summary && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Operational summary</p>
              <h3>Fleet footprint</h3>
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
              <p>
                This civilization currently has no orbital groups, active transfers,
                or local resource contexts in the development dataset.
              </p>
            </div>
            <UiBadge tone="warn">Safe zero-state</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Fleet counters remain at zero until groups are seeded or created elsewhere.</li>
            <li>Action manifests below still document available contracts, but no gameplay mutation can be executed from this screen.</li>
          </ul>
        </UiCard>
      )}

      {uiState?.interceptionNotes?.length ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Readiness</p>
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

      {uiState?.resourceContexts?.length ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Local stockpiles</p>
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
                  <UiBadge tone="resource">
                    {(context.balances ?? []).length} balances
                  </UiBadge>
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

      {uiState?.groups.length ? (
        <div className="figma-fleet-grid">
          {uiState.groups.map((group) => {
            const transferProgress = getTransferProgress(group);
            const readinessItems = buildFleetCommandReadiness(group, uiState.actionHints ?? []);

            return (
              <UiCard key={group.id} className="panel figma-fleet-card">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Fleet group</p>
                    <h3>{formatSpaceAssetType(group.assetType)}</h3>
                    <p>{formatCompactGuid(group.id)}</p>
                  </div>
                  <UiBadge tone={getGroupTone(group)}>{formatOrbitalGroupStatus(group.status)}</UiBadge>
                </div>

                <div className="figma-stat-grid">
                  <SummaryMetric label="Quantity" value={group.quantity} />
                  <SummaryMetric
                    label="Transfer distance"
                    value={group.activeTransfer?.abstractDistanceUnits ?? 0}
                  />
                </div>

                <div className="figma-data-list">
                  <FleetDataRow label="Current planet" value={formatPlanetReference(group.currentPlanetId)} />
                  <FleetDataRow label="Origin planet" value={formatPlanetReference(group.originPlanetId)} />
                  <FleetDataRow
                    label="Stationed away"
                    value={formatBooleanLabel(group.isStationedAwayFromOrigin)}
                  />
                </div>

                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Command readiness</p>
                    <h4>Read-only command planning state</h4>
                  </div>
                  <UiBadge tone="warn">Inspection only</UiBadge>
                </div>

                <div className="figma-data-list">
                  {readinessItems.map((item) => (
                    <FleetDataRow
                      key={`${group.id}-${item.key}`}
                      label={item.label}
                      value={item.summary}
                    />
                  ))}
                </div>

                {readinessItems.some((item) => item.details.length > 0) ? (
                  <ul className="stack-list compact-list">
                    {readinessItems.flatMap((item) =>
                      item.details.map((detail) => (
                        <li key={`${group.id}-${item.key}-${detail}`}>{item.label}: {detail}</li>
                      )),
                    )}
                  </ul>
                ) : null}

                <p className="figma-panel-note">
                  Future command execution must stay behind explicit development or prototype affordances. This Fleet page does not send mutation requests.
                </p>

                {group.routeFuelReadiness?.fuelReadinessPolicy && (
                  <div className="figma-badge-row">
                    <UiBadge>{formatFuelReadinessPolicy(group.routeFuelReadiness.fuelReadinessPolicy)}</UiBadge>
                  </div>
                )}

                {group.activeTransfer && (
                  <div className="figma-transfer-card">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Active transfer</p>
                        <h4>{formatTransferStatus(group.activeTransfer.status)}</h4>
                      </div>
                      <UiBadge tone="warn">{formatPlanetReference(group.activeTransfer.destinationPlanetId)}</UiBadge>
                    </div>
                    {transferProgress !== null && (
                      <UiProgressBar value={transferProgress} tone="neutral" />
                    )}
                    <div className="figma-data-list">
                      <FleetDataRow
                        label="Transfer"
                        value={formatCompactGuid(group.activeTransfer.id)}
                      />
                      <FleetDataRow
                        label="Destination"
                        value={formatPlanetReference(group.activeTransfer.destinationPlanetId)}
                      />
                      <FleetDataRow
                        label="Departure"
                        value={group.activeTransfer.departureAtUtc}
                      />
                      <FleetDataRow
                        label="Arrival"
                        value={group.activeTransfer.arrivalAtUtc}
                      />
                      {transferProgress !== null ? (
                        <FleetDataRow
                          label="Progress"
                          value={formatTransferProgressLabel(group) ?? "Sin progreso"}
                        />
                      ) : null}
                    </div>
                    <div className="transfer-confirmation-actions">
                      <button
                        type="button"
                        className="prototype-control-button transfer-prepare-button"
                        onClick={() => handlePrepareCancelTransfer(group.activeTransfer?.id ?? "")}
                        disabled={!group.activeTransfer?.id || !group.commands?.canCancelTransfer}
                      >
                        {preparedCancelTransferId === group.activeTransfer.id
                          ? "Ocultar confirmacion"
                          : "Preparar cancelacion"}
                      </button>
                    </div>
                    {preparedCancelTransferId === group.activeTransfer.id && (
                      <section className="subpanel transfer-confirmation-panel">
                        <div className="figma-section-header">
                          <div>
                            <p className="eyebrow">Accion de desarrollo</p>
                            <h4>Cancelar transferencia orbital</h4>
                            <p>Mutara datos de desarrollo cuando la ejecucion controlada se habilite.</p>
                          </div>
                          <div className="figma-badge-row">
                            <UiBadge tone="warn">Accion de desarrollo</UiBadge>
                            <UiBadge tone="warn">No reembolsa recursos</UiBadge>
                          </div>
                        </div>
                        <div className="figma-data-list">
                          <FleetDataRow label="Transfer" value={formatCompactGuid(group.activeTransfer.id)} />
                          <FleetDataRow label="Grupo" value={formatCompactGuid(group.id)} />
                          <FleetDataRow label="Origen" value={formatPlanetReference(group.originPlanetId)} />
                          <FleetDataRow label="Planeta actual" value={formatPlanetReference(group.currentPlanetId)} />
                          <FleetDataRow label="Destino" value={formatPlanetReference(group.activeTransfer.destinationPlanetId)} />
                          <FleetDataRow label="Llegada" value={group.activeTransfer.arrivalAtUtc} />
                          {transferProgress !== null ? (
                            <FleetDataRow
                              label="Progreso"
                              value={formatTransferProgressLabel(group) ?? "Sin progreso"}
                            />
                          ) : null}
                        </div>
                        <ul className="stack-list compact-list">
                          <li>Transfer id conocido y visible antes de confirmar.</li>
                          <li>Requiere confirmacion explicita.</li>
                          <li>La ejecucion del endpoint sigue bloqueada en esta fase.</li>
                        </ul>
                        <div className="transfer-confirmation-flow">
                          <label className="confirmation-checkbox">
                            <input
                              type="checkbox"
                              checked={hasCancelTransferAcknowledgement}
                              onChange={(event) =>
                                setHasCancelTransferAcknowledgement(event.target.checked)
                              }
                            />
                            <span>Requiere confirmacion explicita</span>
                          </label>
                          <div className="transfer-confirmation-actions">
                            <button type="button" disabled>
                              {hasCancelTransferAcknowledgement
                                ? "Cancelar transferencia orbital pendiente"
                                : "Cancelar transferencia orbital"}
                            </button>
                          </div>
                          <p className="figma-panel-note">
                            No reembolsa recursos y todavia no llama a `fleet.transfer.cancel`.
                          </p>
                        </div>
                      </section>
                    )}
                    {group.activeTransfer.interceptionReadiness?.readinessNote && (
                      <p className="figma-panel-note">
                        {group.activeTransfer.interceptionReadiness.readinessNote}
                      </p>
                    )}
                  </div>
                )}

                {group.routeFuelReadiness?.notes?.length ? (
                  <ul className="stack-list compact-list">
                    {group.routeFuelReadiness.notes.map((note) => (
                      <li key={note}>{note}</li>
                    ))}
                  </ul>
                ) : null}
              </UiCard>
            );
          })}
        </div>
      ) : null}

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
    </section>
  );
}

