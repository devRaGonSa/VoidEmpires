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
import { FleetSelectedGroupPanel } from "../components/FleetSelectedGroupPanel";
import { ActionManifestPanel } from "../components/ActionManifestPanel";
import { FleetSummaryPanel } from "../components/FleetSummaryPanel";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import {
  formatCompactGuid,
  formatPlanetReference,
  formatResourceType,
  formatSpaceAssetType,
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
          ? liveEstimateResponse.fuelReadiness?.notReadyReason ?? "La regla de combustible sigue bloqueando la accion."
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
              `Combustible ${liveEstimateResponse.fuelReadiness.isFuelReady ? "listo" : "bloqueado"}`,
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
      throw new Error(uiStateResponse.errors[0] ?? "No se pudo refrescar el estado de flotas.");
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
      setError("El civilizationId es obligatorio.");
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
        setError(uiStateResponse.errors[0] ?? "La solicitud del estado de flotas fallo.");
        return;
      }

      setUiState(uiStateResponse.uiState);
      setFleetManifest(fleetManifestResponse.manifest?.actions ?? []);
      setStrategicMapManifest(strategicMapManifestResponse.manifest?.actions ?? []);
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

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Cabina fase 11X</UiBadge>
          <h2>Cabina de mando orbital</h2>
          <p>Revisa la huella de flota, enfoca una escuadra cada vez y deja las acciones de desarrollo tras confirmacion explicita.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Resumen tactico</UiBadge>
          <UiBadge>Rail de escuadras</UiBadge>
          <UiBadge>Detalle enfocado</UiBadge>
          <UiBadge tone="warn">Crear y anular protegidos</UiBadge>
        </div>
      </UiCard>

      <div className="fleet-cockpit-top">
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Entrada de desarrollo</p>
              <h3>Cargar estado de cabina</h3>
            </div>
            <UiBadge>Superficie de desarrollo</UiBadge>
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
              {isLoading ? "Cargando..." : "Cargar paneles"}
            </button>
          </form>
          {error && <p className="error-text">{error}</p>}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limites de ejecucion</p>
              <h3>Reglas de mando</h3>
            </div>
            <UiBadge tone="warn">Prototipo protegido</UiBadge>
          </div>
          <ul className="stack-list">
            <li>La estimacion sigue en solo lectura y nunca reserva escuadras ni gasta recursos.</li>
            <li>Crear traslado exige una estimacion vigente y una confirmacion explicita.</li>
            <li>Anular traslado exige un traslado activo visible y una confirmacion explicita.</li>
            <li>Complete-due, split y merge siguen visibles solo como metadata de prototipo.</li>
          </ul>
        </UiCard>
      </div>

      {summary && (
        <UiCard className="panel fleet-summary-deck">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Resumen operativo</p>
              <h3>Puente de mando</h3>
              <p>Estado compacto de la civilizacion cargada y su situacion orbital.</p>
            </div>
            <UiBadge>{formatCompactGuid(uiState?.civilizationId)}</UiBadge>
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

      {uiState && (
        <div className="fleet-cockpit-layout">
          <UiCard className="panel fleet-group-rail">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Lista orbital</p>
                <h3>Rail de escuadras</h3>
                <p>Usa este rail para enfocar la cabina en una escuadra cada vez.</p>
              </div>
              <UiBadge>{uiState.groups.length} seguidas</UiBadge>
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
              <FleetSelectedGroupPanel
                group={inspectedGroup}
                readinessItems={inspectedReadinessItems}
                groupTone={getGroupTone(inspectedGroup)}
                preparedCancelTransferId={preparedCancelTransferId}
                hasCancelTransferAcknowledgement={hasCancelTransferAcknowledgement}
                isCancellingTransfer={isCancellingTransfer}
                onPrepareCancelTransfer={handlePrepareCancelTransfer}
                onCancelAcknowledgementChange={setHasCancelTransferAcknowledgement}
                onCancelTransfer={handleCancelTransfer}
              />
            ) : null}

            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Ejecucion de mando</p>
                  <h3>Estimacion y acciones protegidas</h3>
                  <p>La columna de mando reune vista previa, confirmacion y resultado en un solo lugar.</p>
                </div>
                <UiBadge tone="good">Vista segura + mutacion protegida</UiBadge>
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
                <UiBadge tone="warn">Ordenes protegidas</UiBadge>
              </div>
              {estimateStaleMessage ? <p className="error-text">{estimateStaleMessage}</p> : null}
              {estimateNetworkError ? <p className="error-text">Error de red: {estimateNetworkError}</p> : null}
              {createTransferNetworkError ? <p className="error-text">{createTransferNetworkError}</p> : null}
              {cancelTransferStaleMessage ? <p className="figma-panel-note">{cancelTransferStaleMessage}</p> : null}
              {cancelTransferNetworkError ? <p className="error-text">{cancelTransferNetworkError}</p> : null}
              {estimateResult ? (
                <section className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Resultado de estimacion</p>
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
                      <p className="eyebrow">Resultado de accion</p>
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
                      <p className="eyebrow">Resultado de accion</p>
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
                    <p className="eyebrow">Estado de ruta</p>
                    <h3>Alertas de intercepcion</h3>
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

      {uiState && mutationConfirmations.length > 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Solo prototipo</p>
              <h3>Manifiesto de mutaciones protegidas</h3>
              <p>Visible para consulta, pero aun bloqueado para la operativa normal.</p>
            </div>
            <UiBadge tone="warn">Mutacion protegida</UiBadge>
          </div>
          <div className="prototype-control-grid">
            {mutationConfirmations.map((control) => (
              <section key={control.actionKey} className="subpanel prototype-control-card">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Contrato de mutacion</p>
                    <h4>{control.label}</h4>
                    <p>{control.mutationSummary}</p>
                  </div>
                  <div className="figma-badge-row">
                    <UiBadge tone="warn">Mutacion</UiBadge>
                    <UiBadge tone={control.readinessTone}>{control.readinessLabel}</UiBadge>
                    <UiBadge tone={control.prototypeLevel === "danger" ? "warn" : "neutral"}>
                      {control.prototypeLevel === "danger" ? "Riesgo" : "Solo prototipo"}
                    </UiBadge>
                  </div>
                </div>
                <button type="button" className="prototype-control-button" disabled>
                  {control.label}
                </button>
                <div className="figma-data-list">
                  <FleetDataRow label="Confirmacion" value={control.confirmationText} />
                  <FleetDataRow label="Motivo del bloqueo" value={control.disabledReason} />
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
              title="Manifiesto de acciones de flota"
              actions={fleetManifest}
              mutationConfirmations={mutationConfirmations}
            />
          )}
          {strategicMapManifest.length > 0 && (
            <ActionManifestPanel
              title="Manifiesto de acciones del mapa estrategico"
              actions={strategicMapManifest}
            />
          )}
        </div>
      )}
    </section>
  );
}

