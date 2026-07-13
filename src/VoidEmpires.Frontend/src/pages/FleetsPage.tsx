import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import type { EstimateOrbitalTravelResponse, FleetCommandApiResult } from "../api/fleetCommandTypes";
import type { FleetGroupSummary, FleetOrbitalStock, FleetUiState } from "../api/fleetTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { GameModal } from "../components/GameModal";
import { LiveQueueCountdown } from "../components/LiveQueueCountdown";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatOrbitalGroupStatus, formatResourceType, formatSpaceAssetType } from "../utils/domainPresentation";
import { usePlayableRouteContext } from "../utils/usePlayableRouteContext";

interface FleetCreationSelection { stock: FleetOrbitalStock; quantity: number; }
interface EstimateSnapshot { groupId: string; originPlanetId: string; destinationPlanetId: string; estimatedArrivalAtUtc: string; }

function positiveInteger(value: number) {
  return Number.isFinite(value) && value > 0 ? Math.floor(value) : 1;
}

function durationMilliseconds(value?: string | null) {
  const match = /^(?:(\d+)\.)?(\d{2}):(\d{2}):(\d{2})$/.exec(value ?? "");
  if (!match) return 0;
  return ((Number(match[1] ?? 0) * 86400) + (Number(match[2]) * 3600) + (Number(match[3]) * 60) + Number(match[4])) * 1000;
}

function formatDuration(value?: string | null) {
  const milliseconds = durationMilliseconds(value);
  if (!milliseconds) return "No disponible";
  const minutes = Math.ceil(milliseconds / 60000);
  return minutes < 60 ? `${minutes} min` : `${Math.floor(minutes / 60)} h ${minutes % 60} min`;
}

export function FleetsPage() {
  const [searchParams] = useSearchParams();
  const queryCivilizationId = searchParams.get("civilizationId");
  const queryPlanetId = searchParams.get("planetId");
  const routeContext = usePlayableRouteContext(queryCivilizationId);
  const civilizationId = queryCivilizationId ?? routeContext.resolvedWorldEntry?.civilizationId ?? "";
  const requestedPlanetId = queryPlanetId ?? routeContext.resolvedWorldEntry?.planetId ?? "";

  const [uiState, setUiState] = useState<FleetUiState | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [quantityByAssetType, setQuantityByAssetType] = useState<Record<string, number>>({});
  const [creationSelection, setCreationSelection] = useState<FleetCreationSelection | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [selectedGroupId, setSelectedGroupId] = useState("");
  const [destinationPlanetId, setDestinationPlanetId] = useState("");
  const [estimateResult, setEstimateResult] = useState<FleetCommandApiResult<EstimateOrbitalTravelResponse> | null>(null);
  const [estimateSnapshot, setEstimateSnapshot] = useState<EstimateSnapshot | null>(null);
  const [isEstimating, setIsEstimating] = useState(false);
  const [isTransferReviewOpen, setIsTransferReviewOpen] = useState(false);
  const [isSending, setIsSending] = useState(false);
  const [isCancelling, setIsCancelling] = useState(false);
  const [arrivalRefreshError, setArrivalRefreshError] = useState<string | null>(null);

  async function reloadFleetState() {
    const response = await voidEmpiresApi.getFleetUiState(civilizationId, requestedPlanetId || undefined);
    if (!response.succeeded || !response.uiState) throw new Error(response.errors[0] ?? "No se pudo actualizar Flotas.");
    setUiState(response.uiState);
    return response.uiState;
  }

  useEffect(() => {
    async function load() {
      if (!civilizationId) return;
      setIsLoading(true);
      setError(null);
      try { await reloadFleetState(); }
      catch (requestError) { setUiState(null); setError(requestError instanceof Error ? requestError.message : "No se pudo cargar Flotas."); }
      finally { setIsLoading(false); }
    }
    void load();
  }, [civilizationId, requestedPlanetId]);

  const selectedPlanetId = uiState?.selectedPlanetId ?? requestedPlanetId;
  const planets = uiState?.planets ?? [];
  const planetName = (planetId: string) => planets.find((planet) => planet.planetId === planetId)?.planetName ?? "Planeta desconocido";
  const stationedGroups = useMemo(() => uiState?.groups.filter((group) => group.currentPlanetId === selectedPlanetId && group.status === "Stationed" && !group.activeTransfer) ?? [], [selectedPlanetId, uiState]);
  const activeGroups = useMemo(() => uiState?.groups.filter((group) => Boolean(group.activeTransfer)) ?? [], [uiState]);
  const selectedGroup = stationedGroups.find((group) => group.id === selectedGroupId) ?? null;
  const destinations = planets.filter((planet) => planet.planetId !== selectedGroup?.currentPlanetId);
  const estimate = estimateResult?.httpStatus === 200 && estimateResult.response?.succeeded ? estimateResult.response : null;
  const estimateIsCurrent = Boolean(estimate && estimateSnapshot && selectedGroup && estimateSnapshot.groupId === selectedGroup.id && estimateSnapshot.originPlanetId === selectedGroup.currentPlanetId && estimateSnapshot.destinationPlanetId === destinationPlanetId);
  const canSendEstimate = Boolean(estimateIsCurrent && estimate?.canAfford && (estimate.fuelReadiness?.isFuelReady ?? true));
  const originResources = uiState?.resourceContexts?.find((context) => context.planetId === selectedGroup?.currentPlanetId)?.balances ?? [];
  const summary = {
    total: uiState?.groups.length ?? 0,
    stationed: uiState?.groups.filter((group) => group.status === "Stationed" && !group.activeTransfer).length ?? 0,
    transit: activeGroups.length,
    ships: uiState?.groups.reduce((total, group) => total + group.quantity, 0) ?? 0,
  };

  function clearEstimate() {
    setEstimateResult(null);
    setEstimateSnapshot(null);
    setIsTransferReviewOpen(false);
  }

  function selectGroup(group: FleetGroupSummary) {
    setSelectedGroupId(group.id);
    setDestinationPlanetId("");
    clearEstimate();
  }

  async function confirmFleetCreation() {
    if (!creationSelection || !selectedPlanetId || isCreating) return;
    setIsCreating(true);
    setError(null);
    try {
      const result = await voidEmpiresApi.createOrbitalGroup({ civilizationId, planetId: selectedPlanetId, assetType: creationSelection.stock.assetType, quantity: creationSelection.quantity });
      if (result.httpStatus !== 201 || !result.response?.succeeded) { setError(result.response?.errors[0] ?? "No se pudo crear la flota."); return; }
      setCreationSelection(null);
      setFeedback("Flota creada y estacionada.");
      await reloadFleetState();
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo crear la flota."); }
    finally { setIsCreating(false); }
  }

  async function calculateRoute() {
    if (!selectedGroup || !destinationPlanetId || isEstimating) return;
    setIsEstimating(true);
    setError(null);
    try {
      const result = await voidEmpiresApi.estimateOrbitalTravel({ civilizationId, orbitalGroupId: selectedGroup.id, destinationPlanetId });
      setEstimateResult(result);
      if (result.httpStatus === 200 && result.response?.succeeded) {
        setEstimateSnapshot({ groupId: selectedGroup.id, originPlanetId: selectedGroup.currentPlanetId, destinationPlanetId, estimatedArrivalAtUtc: new Date(Date.now() + durationMilliseconds(result.response.estimatedDuration)).toISOString() });
      } else {
        setEstimateSnapshot(null);
        setError(result.response?.errors[0] ?? "No se pudo calcular la ruta.");
      }
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo calcular la ruta."); }
    finally { setIsEstimating(false); }
  }

  async function confirmTransfer() {
    if (!selectedGroup || !estimateSnapshot || !canSendEstimate || isSending) return;
    setIsSending(true);
    setError(null);
    try {
      const result = await voidEmpiresApi.createOrbitalTransfer({ civilizationId, orbitalGroupId: selectedGroup.id, destinationPlanetId: estimateSnapshot.destinationPlanetId, requestedAtUtc: new Date().toISOString() });
      if (result.httpStatus !== 201 || !result.response?.succeeded) { setError(result.response?.errors[0] ?? "No se pudo enviar la flota."); return; }
      setIsTransferReviewOpen(false);
      setSelectedGroupId("");
      clearEstimate();
      setFeedback("Flota enviada.");
      await reloadFleetState();
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo enviar la flota."); }
    finally { setIsSending(false); }
  }

  async function cancelTransfer(group: FleetGroupSummary) {
    if (!group.activeTransfer || !group.commands?.canCancelTransfer || isCancelling) return;
    setIsCancelling(true);
    setError(null);
    try {
      const result = await voidEmpiresApi.cancelOrbitalTransfer({ civilizationId, orbitalTransferId: group.activeTransfer.id });
      if (result.httpStatus !== 200 || !result.response?.succeeded) { setError(result.response?.errors[0] ?? "No se pudo cancelar el movimiento."); return; }
      setFeedback("Movimiento cancelado; la flota vuelve a estar disponible.");
      await reloadFleetState();
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo cancelar el movimiento."); }
    finally { setIsCancelling(false); }
  }

  async function refreshArrivals() {
    setArrivalRefreshError(null);
    try { await reloadFleetState(); }
    catch { setArrivalRefreshError("No se pudo actualizar la llegada. Reintentar."); }
  }

  const estimatedCosts = estimate?.resourceCosts.map((cost) => `${formatResourceType(cost.resourceType)} ${cost.quantity}`).join(" | ") || "Sin coste";
  const resourceAvailability = originResources.map((resource) => `${formatResourceType(resource.resourceType)} ${resource.quantity}`).join(" | ") || "Sin reservas visibles";

  return (
    <section className="page-grid">
      <CockpitHero title="Flotas" description="Crea flotas desde el stock orbital y gestiona movimientos entre planetas." badges={<><UiBadge>{selectedPlanetId ? planetName(selectedPlanetId) : "Sin planeta"}</UiBadge><UiBadge>Grupos orbitales</UiBadge></>} />
      {error ? <UiCard className="panel"><p className="error-text">{error}</p></UiCard> : null}
      {feedback ? <p>{feedback}</p> : null}

      {uiState ? <UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Resumen</p><h3>Estado de flotas</h3></div></div><div className="figma-stat-grid"><div className="figma-stat"><strong>{summary.total}</strong><span>Flotas</span></div><div className="figma-stat"><strong>{summary.stationed}</strong><span>Estacionadas</span></div><div className="figma-stat"><strong>{summary.transit}</strong><span>En transito</span></div><div className="figma-stat"><strong>{summary.ships}</strong><span>Naves</span></div></div></UiCard> : null}

      {uiState ? <UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Stock orbital local</p><h3>Crear flota</h3></div><UiBadge>{uiState.localStock?.length ?? 0} tipos</UiBadge></div>{uiState.localStock?.length ? <div className="readiness-grid shipyard-catalog-grid">{uiState.localStock.map((stock) => { const quantity = quantityByAssetType[stock.assetType] ?? 1; const canCreate = stock.quantity > 0; return <article key={stock.assetType} className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">Naves disponibles</p><h4>{formatSpaceAssetType(stock.assetType)}</h4></div><UiBadge tone={canCreate ? "good" : "neutral"}>Stock {stock.quantity}</UiBadge></div><div className="transfer-confirmation-actions production-action-row"><label className="field production-quantity-field"><span>Unidades</span><input type="number" min={1} max={stock.quantity} step={1} value={quantity} disabled={!canCreate} onChange={(event) => setQuantityByAssetType((current) => ({ ...current, [stock.assetType]: positiveInteger(Number(event.target.value)) }))} /></label><button type="button" disabled={!canCreate} onClick={() => setCreationSelection({ stock, quantity: Math.min(stock.quantity, positiveInteger(quantity)) })}>Crear flota</button></div>{!canCreate ? <p className="figma-panel-note">No hay unidades disponibles.</p> : null}</article>; })}</div> : <p className="figma-panel-note">No hay stock orbital disponible en este planeta.</p>}</UiCard> : null}

      {uiState ? <div className="readiness-grid"><UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Flotas estacionadas</p><h3>{selectedPlanetId ? planetName(selectedPlanetId) : "Planeta"}</h3></div><UiBadge>{stationedGroups.length}</UiBadge></div>{stationedGroups.length ? <ul className="stack-list">{stationedGroups.map((group) => <li key={group.id}><div><strong>{formatSpaceAssetType(group.assetType)}</strong><p>{group.quantity} naves | {formatOrbitalGroupStatus(group.status)}</p></div><button type="button" onClick={() => selectGroup(group)}>{selectedGroup?.id === group.id ? "Seleccionada" : "Mover"}</button></li>)}</ul> : <p className="figma-panel-note">No hay flotas estacionadas en este planeta.</p>}</UiCard>
      <UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Movimiento</p><h3>Preparar ruta</h3></div><UiBadge>{selectedGroup ? "Flota seleccionada" : "Selecciona una flota"}</UiBadge></div>{selectedGroup ? <div className="figma-data-list"><div className="figma-data-row"><span>Flota</span><strong>{formatSpaceAssetType(selectedGroup.assetType)} | {selectedGroup.quantity} naves</strong></div><div className="figma-data-row"><span>Origen</span><strong>{planetName(selectedGroup.currentPlanetId)}</strong></div><label className="field"><span>Destino</span><select value={destinationPlanetId} onChange={(event) => { setDestinationPlanetId(event.target.value); clearEstimate(); }}><option value="">Seleccionar destino</option>{destinations.map((planet) => <option key={planet.planetId} value={planet.planetId}>{planet.planetName}</option>)}</select></label><button type="button" disabled={!destinationPlanetId || isEstimating} onClick={() => void calculateRoute()}>{isEstimating ? "Calculando..." : "Calcular ruta"}</button>{!destinationPlanetId ? <p className="figma-panel-note">Selecciona un destino distinto del origen.</p> : null}{estimateIsCurrent && estimate ? <><div className="figma-data-row"><span>Duracion</span><strong>{formatDuration(estimate.estimatedDuration)}</strong></div><div className="figma-data-row"><span>Coste</span><strong>{estimatedCosts}</strong></div><div className="figma-data-row"><span>Llegada estimada</span><strong>{estimateSnapshot ? new Date(estimateSnapshot.estimatedArrivalAtUtc).toLocaleString("es-ES") : "No disponible"}</strong></div><button type="button" disabled={!canSendEstimate} onClick={() => setIsTransferReviewOpen(true)}>Revisar envio</button>{!canSendEstimate ? <p className="figma-panel-note">La ruta no puede enviarse con los recursos o disponibilidad actuales.</p> : null}</> : null}</div> : <p className="figma-panel-note">Elige una flota estacionada para preparar su movimiento.</p>}</UiCard></div> : null}

      {uiState && activeGroups.length ? <UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Movimientos activos</p><h3>Flotas en transito</h3></div><UiBadge tone="warn">{activeGroups.length}</UiBadge></div><div className="readiness-grid">{activeGroups.map((group) => group.activeTransfer ? <article key={group.activeTransfer.id} className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">{formatSpaceAssetType(group.assetType)}</p><h4>{planetName(group.currentPlanetId)} → {planetName(group.activeTransfer.destinationPlanetId)}</h4></div><UiBadge>{formatOrbitalGroupStatus(group.status)}</UiBadge></div><div className="figma-data-list"><div className="figma-data-row"><span>Naves</span><strong>{group.quantity}</strong></div><div className="figma-data-row"><span>Estado</span><strong>{formatOrbitalGroupStatus(group.status)}</strong></div><div className="figma-data-row"><span>Llegada</span><strong><LiveQueueCountdown endsAtUtc={group.activeTransfer.arrivalAtUtc} expireKey={group.activeTransfer.id} onExpire={refreshArrivals} /></strong></div></div>{group.commands?.canCancelTransfer ? <button type="button" disabled={isCancelling} onClick={() => void cancelTransfer(group)}>Cancelar movimiento</button> : <p className="figma-panel-note">Este movimiento ya no admite cancelacion.</p>}</article> : null)}</div>{arrivalRefreshError ? <div className="transfer-confirmation-actions"><p className="error-text">{arrivalRefreshError}</p><button type="button" onClick={() => void refreshArrivals()}>Reintentar</button></div> : null}</UiCard> : null}

      {creationSelection && selectedPlanetId ? <GameModal actionScope="gameplay" canClose={!isCreating} closeLabel="Cerrar" description="Confirma la conversion de stock orbital en una flota estacionada." isBusy={isCreating} isOpen onClose={() => setCreationSelection(null)} primaryAction={{ label: "Crear flota", onClick: () => void confirmFleetCreation() }} secondaryAction={{ label: "Cancelar", onClick: () => setCreationSelection(null), disabled: isCreating }} title="Crear flota"><div className="figma-data-list"><div className="figma-data-row"><span>Planeta</span><strong>{planetName(selectedPlanetId)}</strong></div><div className="figma-data-row"><span>Nave</span><strong>{formatSpaceAssetType(creationSelection.stock.assetType)}</strong></div><div className="figma-data-row"><span>Stock actual</span><strong>{creationSelection.stock.quantity}</strong></div><div className="figma-data-row"><span>Unidades</span><strong>{creationSelection.quantity}</strong></div><div className="figma-data-row"><span>Stock restante</span><strong>{creationSelection.stock.quantity - creationSelection.quantity}</strong></div></div></GameModal> : null}

      {isTransferReviewOpen && selectedGroup && estimate && estimateSnapshot ? <GameModal actionScope="gameplay" canClose={!isSending} closeLabel="Cerrar" description="Revisa la ruta calculada antes de reservar y enviar la flota." isBusy={isSending} isOpen onClose={() => setIsTransferReviewOpen(false)} primaryAction={{ label: "Enviar flota", onClick: () => void confirmTransfer(), disabled: !canSendEstimate }} secondaryAction={{ label: "Cancelar", onClick: () => setIsTransferReviewOpen(false), disabled: isSending }} title="Confirmar movimiento"><div className="figma-data-list"><div className="figma-data-row"><span>Flota</span><strong>{formatSpaceAssetType(selectedGroup.assetType)} | {selectedGroup.quantity} naves</strong></div><div className="figma-data-row"><span>Origen</span><strong>{planetName(selectedGroup.currentPlanetId)}</strong></div><div className="figma-data-row"><span>Destino</span><strong>{planetName(estimateSnapshot.destinationPlanetId)}</strong></div><div className="figma-data-row"><span>Duracion</span><strong>{formatDuration(estimate.estimatedDuration)}</strong></div><div className="figma-data-row"><span>Llegada</span><strong>{new Date(estimateSnapshot.estimatedArrivalAtUtc).toLocaleString("es-ES")}</strong></div><div className="figma-data-row"><span>Coste</span><strong>{estimatedCosts}</strong></div><div className="figma-data-row"><span>Recursos actuales</span><strong>{resourceAvailability}</strong></div></div></GameModal> : null}

      {!uiState && !isLoading && civilizationId && !error ? <p className="figma-panel-note">No hay estado de flotas disponible.</p> : null}
    </section>
  );
}
