import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { enqueueGroundTraining, fetchGroundArmyUiState } from "../api/groundArmyApi";
import { CockpitHero } from "../components/CockpitHero";
import { GameModal } from "../components/GameModal";
import { GroundArmyCatalogCard, formatGroundTrainingTotalDuration } from "../components/GroundArmyCatalogCard";
import { LiveQueueCountdown } from "../components/LiveQueueCountdown";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatGroundArmyRequestFailure, formatGroundTrainingCost } from "../utils/groundArmyPresentation";
import { mapGroundArmyUiStateToViewModel, type GroundArmyOption, type GroundArmyViewModel } from "../utils/groundArmyViewModel";
import { isSuspiciousCabinContext } from "../utils/routeUrls";
import { cockpitStatusLabels } from "../utils/cockpitStatus";

interface TrainingSelection {
  option: GroundArmyOption;
  quantity: number;
}

export function GroundArmyPage() {
  const [searchParams] = useSearchParams();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<GroundArmyViewModel | null>(null);
  const [quantityByAssetType, setQuantityByAssetType] = useState<Record<string, number>>({});
  const [selection, setSelection] = useState<TrainingSelection | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [enqueueError, setEnqueueError] = useState<string | null>(null);
  const [queueRefreshError, setQueueRefreshError] = useState<string | null>(null);

  const civilizationId = searchParams.get("civilizationId") ?? "";
  const planetId = searchParams.get("planetId");
  const groundArmy = uiState?.groundArmy ?? null;
  const catalog = useMemo(() => groundArmy?.catalog ?? [], [groundArmy?.catalog]);
  const isSuspiciousContext = isSuspiciousCabinContext(civilizationId, planetId);

  async function reloadGroundArmyState() {
    const response = await fetchGroundArmyUiState(civilizationId, planetId);
    if (!response.succeeded || !response.uiState) {
      throw new Error(response.errors[0] ?? "No se pudo actualizar el ejercito terrestre.");
    }

    const nextState = mapGroundArmyUiStateToViewModel(response.uiState);
    setUiState(nextState);
    return nextState;
  }

  useEffect(() => {
    async function load() {
      if (!civilizationId) {
        setUiState(null);
        setError("Falta el contexto de civilizacion para mostrar el ejercito terrestre.");
        return;
      }

      setIsLoading(true);
      setError(null);
      try {
        await reloadGroundArmyState();
      } catch (requestError) {
        const failure = formatGroundArmyRequestFailure(requestError instanceof Error ? requestError.message : null);
        setUiState(null);
        setError(failure.primaryMessage);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [civilizationId, planetId]);

  function handleQuantityChange(assetType: string, quantity: number) {
    setQuantityByAssetType((current) => ({
      ...current,
      [assetType]: Number.isFinite(quantity) && quantity > 0 ? Math.floor(quantity) : 1,
    }));
  }

  function handleReview(option: GroundArmyOption, quantity: number) {
    if (option.statusKey !== "Available") return;
    setSelection({ option, quantity: Math.max(1, Math.floor(quantity)) });
    setFeedback(null);
    setEnqueueError(null);
  }

  async function handleQueueRefresh() {
    setQueueRefreshError(null);
    try {
      await reloadGroundArmyState();
    } catch {
      setQueueRefreshError("No se pudo actualizar la cola. Reintentar.");
    }
  }

  async function handleConfirmTraining() {
    if (!groundArmy || !selection || isSubmitting) return;
    setIsSubmitting(true);
    setEnqueueError(null);
    try {
      const result = await enqueueGroundTraining({
        civilizationId,
        planetId: groundArmy.planetId,
        assetType: selection.option.assetType,
        quantity: selection.quantity,
        requestedAtUtc: new Date().toISOString(),
      });
      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        setEnqueueError(result.response?.errors[0] ?? "No se pudo iniciar el entrenamiento.");
        return;
      }

      setSelection(null);
      setFeedback("Entrenamiento enviado a la cola.");
      await reloadGroundArmyState();
    } catch (requestError) {
      const failure = formatGroundArmyRequestFailure(requestError instanceof Error ? requestError.message : null);
      setEnqueueError(failure.primaryMessage);
    } finally {
      setIsSubmitting(false);
    }
  }

  const selectedTotalCost = selection
    ? formatGroundTrainingCost(selection.option.cost.map((entry) => ({ ...entry, quantity: entry.quantity * selection.quantity })))
    : "";

  return (
    <section className="page-grid">
      <CockpitHero
        title="Ejercito de Tierra"
        description="Entrena unidades terrestres y revisa la guarnicion de la colonia seleccionada."
        badges={<><UiBadge>Guarnicion planetaria</UiBadge><UiBadge>Entrenamiento terrestre</UiBadge></>}
      />

      {error ? <UiCard className="panel"><p className="error-text">{error}</p></UiCard> : null}

      {isSuspiciousContext ? (
        <UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Contexto sospechoso</p><h3>El identificador de civilizacion no parece valido para esta vista.</h3></div><UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge></div><p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p></UiCard>
      ) : null}

      {groundArmy?.queue.length ? (
        <UiCard className="panel">
          <div className="figma-section-header"><div><p className="eyebrow">Entrenamiento activo</p><h3>Cola terrestre</h3></div><UiBadge tone="warn">{groundArmy.queue.length} activa</UiBadge></div>
          <div className="readiness-grid">
            {groundArmy.queue.map((item) => (
              <article key={item.orderId} className="subpanel figma-subpanel">
                <div className="figma-section-header"><div><p className="eyebrow">{item.quantity} unidades</p><h4>{item.label}</h4></div><UiBadge>{item.statusLabel}</UiBadge></div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Estado</span><strong>{item.statusLabel}</strong></div>
                  <div className="figma-data-row"><span>Tiempo restante</span><strong><LiveQueueCountdown endsAtUtc={item.endsAtUtc} expireKey={item.orderId} onExpire={handleQueueRefresh} /></strong></div>
                </div>
              </article>
            ))}
          </div>
          {queueRefreshError ? <div className="transfer-confirmation-actions"><p className="error-text">{queueRefreshError}</p><button type="button" onClick={() => void handleQueueRefresh()}>Reintentar</button></div> : null}
        </UiCard>
      ) : null}

      {groundArmy ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header"><div><p className="eyebrow">Guarnicion</p><h3>Unidades disponibles</h3></div><UiBadge>{groundArmy.garrison.reduce((total, unit) => total + unit.quantity, 0)} unidades</UiBadge></div>
            {groundArmy.garrison.length ? <ul className="stack-list compact-list">{groundArmy.garrison.map((unit) => <li key={unit.assetType}>{unit.label}: {unit.quantity}</li>)}</ul> : <p className="figma-panel-note">La colonia no tiene unidades terrestres.</p>}
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header"><div><p className="eyebrow">Catalogo terrestre</p><h3>Entrenar unidades</h3></div><UiBadge tone="resource">{catalog.length} unidades</UiBadge></div>
            <div className="readiness-grid ground-army-catalog-grid">
              {catalog.map((option) => <GroundArmyCatalogCard key={option.assetType} option={option} quantity={quantityByAssetType[option.assetType] ?? 1} onQuantityChange={handleQuantityChange} onTrain={handleReview} />)}
            </div>
          </UiCard>
          {feedback ? <p>{feedback}</p> : null}
          {enqueueError && !selection ? <p className="error-text">{enqueueError}</p> : null}
        </>
      ) : !isLoading && civilizationId && !error ? <p className="figma-panel-note">No hay datos terrestres disponibles.</p> : null}

      {selection && groundArmy ? (
        <GameModal
          actionScope="gameplay"
          canClose={!isSubmitting}
          closeLabel="Cerrar"
          description="Revisa la cantidad, el coste y la duracion antes de entrenar."
          isBusy={isSubmitting}
          isOpen
          onClose={() => setSelection(null)}
          primaryAction={{ label: "Entrenar", onClick: () => void handleConfirmTraining() }}
          secondaryAction={{ label: "Cancelar", onClick: () => setSelection(null), disabled: isSubmitting }}
          title="Confirmar entrenamiento"
        >
          <div className="figma-data-list">
            <div className="figma-data-row"><span>Planeta</span><strong>{groundArmy.planetName}</strong></div>
            <div className="figma-data-row"><span>Unidad</span><strong>{selection.option.label}</strong></div>
            <div className="figma-data-row"><span>Cantidad actual</span><strong>{selection.option.currentStock}</strong></div>
            <div className="figma-data-row"><span>Unidades seleccionadas</span><strong>{selection.quantity}</strong></div>
            <div className="figma-data-row"><span>Coste total</span><strong>{selectedTotalCost}</strong></div>
            <div className="figma-data-row"><span>Duracion total</span><strong>{formatGroundTrainingTotalDuration(selection.option.estimatedDuration, selection.quantity)}</strong></div>
            <div className="figma-data-row"><span>Requisito</span><strong>{selection.option.requirementLabel}</strong></div>
          </div>
          {enqueueError ? <p className="error-text">{enqueueError}</p> : null}
        </GameModal>
      ) : null}
    </section>
  );
}
