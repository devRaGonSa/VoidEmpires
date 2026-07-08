import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { fetchDefensesUiState } from "../api/defenseApi";
import { CockpitHero } from "../components/CockpitHero";
import { DefenseCatalogCard } from "../components/DefenseCatalogCard";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatDefenseRequestFailure } from "../utils/defensePresentation";
import {
  mapDefensesUiStateToViewModel,
  type DefensesViewModel,
} from "../utils/defenseViewModel";
import { isSuspiciousCabinContext } from "../utils/routeUrls";
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

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const defenses = uiState?.defenses ?? null;
  const hasSafeDefenseEnqueue = false;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const catalogOptions = useMemo(() => defenses?.options ?? [], [defenses?.options]);

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
        const response = await fetchDefensesUiState(queryCivilizationId, queryPlanetId);
        if (!response.succeeded || !response.uiState) {
          const failure = formatDefenseRequestFailure(response.errors[0] ?? null);
          setUiState(null);
          setError(failure.primaryMessage);
          setErrorFollowUp(failure.followUp);
          setTechnicalErrorDetail(failure.technicalDetail);
          return;
        }

        const nextState = mapDefensesUiStateToViewModel(response.uiState);
        setUiState(nextState);

        if (nextState.selectedPlanetId && nextState.selectedPlanetId !== queryPlanetId) {
          const nextParams = new URLSearchParams(searchParams);
          nextParams.set("civilizationId", queryCivilizationId);
          nextParams.set("planetId", nextState.selectedPlanetId);
          setSearchParams(nextParams, { replace: true });
        }
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

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Defensas v1"
        title="Defensa planetaria"
        description="Sistemas defensivos, cobertura local y estructuras de proteccion para la colonia seleccionada."
        developmentNote="Produccion defensiva pendiente de activacion: sin combate ni intercepcion en esta version."
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
                      <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {item.targetLevel}</strong></div>
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
            <p className="figma-panel-note">No hay un contexto defensivo util para este planeta. La vista mantiene acceso, contexto y explicacion de limites.</p>
          </UiCard>
        ) : null
      )}

    </section>
  );
}
