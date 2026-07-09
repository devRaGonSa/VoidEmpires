import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { enqueueDefenseProduction, fetchDefensesUiState } from "../api/defenseApi";
import { CockpitHero } from "../components/CockpitHero";
import { DefenseCatalogCard } from "../components/DefenseCatalogCard";
import { GameModal } from "../components/GameModal";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatDefenseRequestFailure } from "../utils/defensePresentation";
import {
  mapDefensesUiStateToViewModel,
  type DefenseOption,
  type DefensesViewModel,
} from "../utils/defenseViewModel";
import { buildDefensesUrl, isSuspiciousCabinContext } from "../utils/routeUrls";
import { cockpitStatusLabels } from "../utils/cockpitStatus";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

export function DefensesPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorFollowUp, setErrorFollowUp] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<DefensesViewModel | null>(null);
  const [quantityByBuildingType, setQuantityByBuildingType] = useState<Record<string, number>>({});
  const [enqueueFeedback, setEnqueueFeedback] = useState<string | null>(null);
  const [enqueueError, setEnqueueError] = useState<string | null>(null);
  const [isSubmittingEnqueue, setIsSubmittingEnqueue] = useState(false);
  const [reviewSelection, setReviewSelection] = useState<{ option: DefenseOption; quantity: number } | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const defenses = uiState?.defenses ?? null;
  const hasSafeDefenseEnqueue = defenses?.actionAvailability.enqueue.supported ?? false;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const catalogOptions = useMemo(() => defenses?.options ?? [], [defenses?.options]);

  async function reloadDefensesState(replaceParams = false) {
    const response = await fetchDefensesUiState(queryCivilizationId, queryPlanetId);
    if (!response.succeeded || !response.uiState) {
      const failure = formatDefenseRequestFailure(response.errors[0] ?? null);
      setUiState(null);
      setError(failure.primaryMessage);
      setErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
      return null;
    }

    const nextState = mapDefensesUiStateToViewModel(response.uiState);
    setUiState(nextState);

    if (nextState.selectedPlanetId && nextState.selectedPlanetId !== queryPlanetId) {
      const nextRoute = buildDefensesUrl(queryCivilizationId, nextState.selectedPlanetId);
      const nextParams = new URLSearchParams(nextRoute.split("?")[1] ?? "");
      setSearchParams(nextParams, { replace: replaceParams });
    }

    return nextState;
  }

  useEffect(() => {
    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        setErrorFollowUp(null);
        setTechnicalErrorDetail(null);
        return;
      }

      setIsLoading(true);
      setError(null);
      setErrorFollowUp(null);
      setTechnicalErrorDetail(null);

      try {
        await reloadDefensesState(true);
      } catch (requestError) {
        const failure = formatDefenseRequestFailure(requestError instanceof Error ? requestError.message : null);
        setUiState(null);
        setError(failure.primaryMessage);
        setErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, queryPlanetId, searchParams, setSearchParams]);

  function handleQuantityChange(buildingType: string, quantity: number) {
    setQuantityByBuildingType((current) => ({
      ...current,
      [buildingType]: Number.isFinite(quantity) && quantity > 0 ? Math.floor(quantity) : 1,
    }));
  }

  function handleReviewDefense(option: DefenseOption, quantity: number) {
    if (!option.assetType || option.statusKey !== "Available") {
      return;
    }

    setReviewSelection({
      option,
      quantity: Number.isFinite(quantity) && quantity > 0 ? Math.floor(quantity) : 1,
    });
    setEnqueueFeedback(null);
    setEnqueueError(null);
    setTechnicalErrorDetail(null);
  }

  function handleCancelReview() {
    setReviewSelection(null);
    setEnqueueError(null);
    setTechnicalErrorDetail(null);
  }

  async function handleConfirmDefenseProduction() {
    if (!defenses || !reviewSelection?.option.assetType || reviewSelection.option.statusKey !== "Available" || isSubmittingEnqueue) {
      return;
    }

    setIsSubmittingEnqueue(true);
    setEnqueueFeedback(null);
    setEnqueueError(null);
    setTechnicalErrorDetail(null);

    try {
      const result = await enqueueDefenseProduction({
        civilizationId: queryCivilizationId,
        planetId: defenses.planetId,
        assetType: reviewSelection.option.assetType,
        quantity: reviewSelection.quantity,
        requestedAtUtc: new Date().toISOString(),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        setEnqueueError(result.response?.errors[0] ?? "No se pudo enviar la produccion defensiva.");
        return;
      }

      setEnqueueFeedback("Produccion defensiva enviada a la cola.");
      setReviewSelection(null);
      await reloadDefensesState(false);
    } catch (requestError) {
      const failure = formatDefenseRequestFailure(requestError instanceof Error ? requestError.message : null);
      setEnqueueError(failure.primaryMessage);
      setTechnicalErrorDetail(failure.technicalDetail);
    } finally {
      setIsSubmittingEnqueue(false);
    }
  }

  return (
    <section className="page-grid">
      <CockpitHero
        title="Defensa planetaria"
        description="Catalogo compacto de defensas, produccion por unidades y cola defensiva activa."
        badges={
          <>
            <UiBadge>Sistemas defensivos</UiBadge>
            <UiBadge>Produccion pendiente</UiBadge>
            <UiBadge tone="warn">Sin combate ni intercepcion</UiBadge>
          </>
        }
      />

      {error ? (
        <UiCard className="panel">
          <p className="error-text">{error}</p>
          {errorFollowUp ? <p className="figma-panel-note">{errorFollowUp}</p> : null}
        </UiCard>
      ) : null}

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El identificador de civilizacion no parece valido para esta vista.</h3>
            </div>
            <UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
        </UiCard>
      ) : null}

      {defenses ? (
        <>
          {defenses.queue.length > 0 ? (
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Produccion defensiva</p>
                <h3>Produccion defensiva</h3>
              </div>
              <UiBadge tone={defenses.queue.length > 0 ? "warn" : "neutral"}>
                {defenses.queue.length > 0 ? `${defenses.queue.length} visibles` : "0 visibles"}
              </UiBadge>
            </div>
            {defenses.queue.length === 0 ? (
              <p className="figma-panel-note">Sin produccion defensiva</p>
            ) : (
              <div className="readiness-grid">
                {defenses.queue.map((item) => (
                  <article key={item.orderId} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">{item.actionLabel}</p>
                        <h4>{item.structureLabel}</h4>
                      </div>
                      <UiBadge tone={item.isDue ? "warn" : "neutral"}>{item.statusLabel}</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Planeta</span><strong>{defenses.planetName}</strong></div>
                      <div className="figma-data-row">
                        <span>{item.productionModel === "unit" ? "Unidades" : "Objetivo"}</span>
                        <strong>{item.productionModel === "unit" ? item.targetLevel : `Nivel ${item.targetLevel}`}</strong>
                      </div>
                      <div className="figma-data-row"><span>Inicio</span><strong>{formatDateTime(item.startsAtUtc)}</strong></div>
                      <div className="figma-data-row"><span>Fin</span><strong>{formatDateTime(item.endsAtUtc)}</strong></div>
                      <div className="figma-data-row"><span>Coste</span><strong>{item.estimatedCostLabel}</strong></div>
                    </div>
                    <p>{item.isDue ? "La orden ya vencio en la lectura actual y permanece pendiente de una via segura de cierre." : "La orden sigue visible en la ventana temporal actual."}</p>
                  </article>
                ))}
              </div>
            )}
          </UiCard>
          ) : null}

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Catalogo defensivo</p>
                <h3>Estructuras defensivas</h3>
              </div>
              <UiBadge tone="resource">{catalogOptions.length} entradas</UiBadge>
            </div>
            {catalogOptions.length > 0 ? (
              <div className="readiness-grid defense-catalog-grid">
                {catalogOptions.map((option) => (
                  <DefenseCatalogCard
                    key={`${option.buildingType}-${option.targetLevel}`}
                    option={option}
                    hasProductionAction={hasSafeDefenseEnqueue}
                    quantity={quantityByBuildingType[option.buildingType] ?? 1}
                    onQuantityChange={handleQuantityChange}
                    onBuild={handleReviewDefense}
                  />
                ))}
              </div>
            ) : (
              <p className="figma-panel-note">No hay entradas defensivas visibles para esta colonia.</p>
            )}
          </UiCard>

          {technicalErrorDetail ? (
            <UiCard className="panel">
              <p className="figma-panel-note">{technicalErrorDetail}</p>
            </UiCard>
          ) : null}
          {enqueueFeedback ? <p>{enqueueFeedback}</p> : null}
          {enqueueError ? <p className="error-text">{enqueueError}</p> : null}
        </>
      ) : (
        !isLoading && queryCivilizationId && !error ? (
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado vacio</p>
                <h3>Sistema defensivo pendiente</h3>
              </div>
              <UiBadge tone="warn">Preparacion limitada</UiBadge>
            </div>
            <p className="figma-panel-note">No hay un estado de defensas util para este planeta. La vista mantiene acceso, contexto y explicacion de limites.</p>
          </UiCard>
        ) : null
      )}

      {reviewSelection && defenses ? (
        <GameModal
          actionScope="gameplay"
          canClose={!isSubmittingEnqueue}
          closeLabel="Cerrar"
          description="Revisa defensa, coste y duracion antes de enviar la produccion defensiva a la cola."
          isBusy={isSubmittingEnqueue}
          isOpen
          onClose={handleCancelReview}
          primaryAction={{
            label: "Construir defensas",
            onClick: () => void handleConfirmDefenseProduction(),
          }}
          secondaryAction={{
            label: "Cancelar",
            onClick: handleCancelReview,
            disabled: isSubmittingEnqueue,
          }}
          title="Construir defensas"
        >
          <div className="figma-data-list">
            <div className="figma-data-row"><span>Planeta</span><strong>{defenses.planetName}</strong></div>
            <div className="figma-data-row"><span>Defensa</span><strong>{reviewSelection.option.structureLabel}</strong></div>
            <div className="figma-data-row"><span>Cantidad actual</span><strong>{reviewSelection.option.currentLevel}</strong></div>
            <div className="figma-data-row"><span>Unidades</span><strong>{reviewSelection.quantity}</strong></div>
            <div className="figma-data-row"><span>Coste</span><strong>{reviewSelection.option.estimatedCostLabel}</strong></div>
            <div className="figma-data-row"><span>Duracion</span><strong>{reviewSelection.option.estimatedDurationLabel}</strong></div>
            <div className="figma-data-row"><span>Estado</span><strong>{reviewSelection.option.statusLabel}</strong></div>
          </div>
          <p className="figma-panel-note">
            La produccion defensiva entrara en su cola propia cuando el backend acepte la orden.
          </p>
        </GameModal>
      ) : null}

    </section>
  );
}
