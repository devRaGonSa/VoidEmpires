import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchRankingUiState } from "../api/rankingApi";
import { CockpitHero } from "../components/CockpitHero";
import { PageContextStrip } from "../components/PageContextStrip";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import {
  buildRankingCategoryCards,
  buildRankingComparisonCards,
  formatRankingRequestFailure,
  getRankingPrimaryAction,
  getRankingStaticLabels,
  mapRankingUiStateToViewModel,
  selectDominantRankingCategory,
  selectRecommendedRankingFocus,
  selectWeakestRankingFocus,
  type RankingUiState,
} from "../utils/rankingPresentation";
import {
  buildAllianceUrl,
  buildDevelopmentHelperUrl,
  buildEspionageUrl,
  buildGalaxyUrl,
  buildMarketUrl,
} from "../utils/routeUrls";
import { formatCompactGuid } from "../utils/domainPresentation";
import { isOperatorMode } from "../utils/playableSession";

const rankingLabels = getRankingStaticLabels();
const rankingHandoffCards = [
  {
    key: "galaxy",
    label: "Galaxia",
    title: "Vista estrategica",
    description: "Recupera el sistema, la visibilidad y el frente que sostienen la lectura de inteligencia y expansion.",
    ctaLabel: "Volver a Galaxia",
  },
  {
    key: "market",
    label: "Mercado",
    title: "Economia visible",
    description: "Explica reservas, produccion y presion comercial que alimentan la potencia economica.",
    ctaLabel: "Abrir Mercado",
  },
  {
    key: "espionage",
    label: "Espionaje",
    title: "Cobertura e inteligencia",
    description: "Detalla visibilidad, senales pasivas y objetivos observados sin convertir Ranking en una vista de inteligencia.",
    ctaLabel: "Abrir Espionaje",
  },
  {
    key: "alliance",
    label: "Alianzas",
    title: "Postura diplomatica",
    description: "Recupera contactos, pactos visibles y limites diplomaticos que sostienen la lectura de diplomacia.",
    ctaLabel: "Abrir Alianzas",
  },
] as const;

function getRankingReadinessStatus(uiState: RankingUiState | null) {
  if (!uiState?.summary) return "Esperando indice";
  if ((uiState.summary.categories.length ?? 0) > 0 && uiState.comparisons.length > 0) return "Indice preparado";
  if ((uiState.summary.categories.length ?? 0) > 0) return "Categorias visibles";
  return "Indice limitado";
}

function formatRankingProductLabel(label: string) {
  return label
    .replace(/solo\s+lect\S+/gi, "consulta interna")
    .replace(/lectura\s+de\s+ranking/gi, "indice de poder")
    .replace(/lectura\s+interna/gi, "clasificacion interna")
    .replace(/lectura/gi, "indice")
    .replace(/demo/gi, "referencia")
    .replace(/demostraci\S+/gi, "referencia")
    .replace(/desarrollo/gi, "fase actual")
    .replace(/backend/gi, "servicio")
    .replace(/mutaci\S+/gi, "actualizacion");
}

export function RankingPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorFollowUp, setErrorFollowUp] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<RankingUiState | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const operatorMode = isOperatorMode(searchParams);
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const recommendedFocus = useMemo(
    () => formatRankingProductLabel(selectRecommendedRankingFocus(uiState)),
    [uiState],
  );
  const primaryAction = useMemo(
    () => formatRankingProductLabel(getRankingPrimaryAction(uiState)),
    [uiState],
  );
  const dominantCategory = useMemo(
    () => formatRankingProductLabel(selectDominantRankingCategory(uiState)),
    [uiState],
  );
  const weakestCategory = useMemo(
    () => formatRankingProductLabel(selectWeakestRankingFocus(uiState)),
    [uiState],
  );
  const categoryCards = useMemo(
    () => buildRankingCategoryCards(uiState?.summary?.categories ?? []),
    [uiState?.summary?.categories],
  );
  const comparisonCards = useMemo(
    () => buildRankingComparisonCards(uiState?.comparisons ?? []),
    [uiState?.comparisons],
  );
  const rankingReadinessStatus = getRankingReadinessStatus(uiState);

  function presentRankingFailure(failure: ReturnType<typeof formatRankingRequestFailure>) {
    setError(formatRankingProductLabel(failure.primaryMessage));
    setErrorFollowUp(failure.followUp ? formatRankingProductLabel(failure.followUp) : null);
    setTechnicalErrorDetail(failure.technicalDetail);
  }

  useEffect(() => {
    document.title = "Ranking";
  }, []);

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);

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
        const response = await fetchRankingUiState(queryCivilizationId);
        if (!response.succeeded || !response.uiState) {
          const failure = formatRankingRequestFailure(new Error(response.errors[0] ?? ""));
          setUiState(null);
          presentRankingFailure(failure);
          return;
        }

        setUiState(mapRankingUiStateToViewModel(response.uiState));
      } catch (requestError) {
        const failure = formatRankingRequestFailure(requestError);
        setUiState(null);
        presentRankingFailure(failure);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationIdInput.trim();
    if (!trimmedCivilizationId) {
      const failure = formatRankingRequestFailure(new Error("Civilization id is required."));
      presentRankingFailure(failure);
      setUiState(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Ranking v1"
        title="Indice de poder"
        description="Clasificacion imperial interna con poder por categorias, comparativas de referencia y dependencias competitivas todavia desactivadas."
        developmentNote="Ranking no publica una clasificacion global, no entrega recompensas y no ejecuta emparejamiento. Resume el estado visible de la civilizacion actual."
        badges={(
          <>
            <UiBadge tone="resource">{uiState?.summary?.totalPowerIndexLabel ?? "Sin indice"}</UiBadge>
            <UiBadge>{uiState?.publication?.stateLabel ?? rankingLabels.unpublishedRanking}</UiBadge>
            <UiBadge tone="warn">{recommendedFocus}</UiBadge>
          </>
        )}
      />

      {queryCivilizationId ? (
        <PageContextStrip
          eyebrow="Indice interno"
          title={uiState?.identity?.civilizationName ?? "Clasificacion imperial"}
          purpose="Indice de poder por categorias, comparativas de referencia y dependencias de ladder sin clasificacion publica ni recompensas."
          statusLabel={rankingReadinessStatus}
          statusTone={uiState?.summary ? "good" : "warn"}
          contextItems={[
            { label: "Civilizacion", value: formatCompactGuid(activeCivilizationId) },
            {
              label: "Poder",
              value: uiState?.summary?.totalPowerIndexLabel ?? "Sin indice",
              detail: formatRankingProductLabel(uiState?.publication?.stateLabel ?? rankingLabels.unpublishedRanking),
            },
            {
              label: "Categoria fuerte",
              value: dominantCategory,
              detail: weakestCategory ? `Area critica: ${weakestCategory}` : undefined,
            },
            {
              label: "Comparativa",
              value: uiState ? `${uiState.comparisons.length} referencias` : "Sin indice",
              detail: formatRankingProductLabel(rankingLabels.demoScenarioReference),
            },
          ]}
          resourceItems={[
            { label: "Indice", value: "Consulta interna", tone: "good" },
            { label: "Ladder", value: "No publicada", tone: "warn" },
            { label: "Recompensas", value: "No disponibles", tone: "neutral" },
          ]}
          primaryAction={
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(activeCivilizationId, undefined, uiState?.identity?.homePlanetId ?? null)}>
                Abrir Galaxia
              </Link>
              <Link className="selection-chip" to={buildMarketUrl(activeCivilizationId, uiState?.identity?.homePlanetId ?? null)}>
                Abrir Mercado
              </Link>
              <Link className="selection-chip" to={buildAllianceUrl(activeCivilizationId)}>
                Abrir Alianzas
              </Link>
            </div>
          }
        />
      ) : null}

      <UiCard className="panel alliance-summary-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Resumen de poder</p>
            <h3>Clasificacion imperial</h3>
            <p>Esta vista no publica ranking global ni recompensas.</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.readOnly}</UiBadge>
        </div>
        <div className="alliance-summary-grid">
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">{rankingLabels.powerIndex}</p>
            <strong>{uiState?.summary?.totalPowerIndexLabel ?? "Sin indice"}</strong>
            <span>El indice actual se calcula desde referencias internas.</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Potencia total</p>
            <strong>{uiState?.summary?.totalPowerIndexLabel ?? "Sin indice"}</strong>
            <span>{formatRankingProductLabel(uiState?.publication?.stateLabel ?? rankingLabels.unpublishedRanking)}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Categoria dominante</p>
            <strong>{dominantCategory}</strong>
            <span>{formatRankingProductLabel(uiState?.summary?.recommendationLabel ?? rankingLabels.unclassifiedMetric)}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Área crítica</p>
            <strong>{weakestCategory}</strong>
            <span>{formatRankingProductLabel(rankingLabels.unclassifiedMetric)}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Comparativa de validación</p>
            <strong>{uiState?.comparisons[0]?.totalPowerIndexLabel ?? "Sin indice"}</strong>
            <span>{formatRankingProductLabel(rankingLabels.demoComparison)}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Foco recomendado</p>
            <strong>{recommendedFocus}</strong>
            <span>{primaryAction}</span>
          </section>
        </div>
        <p className="ranking-summary-note">
          Esta vista no publica ranking global ni recompensas. El indice actual se calcula desde referencias internas.
        </p>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto de ranking</p>
              <h3>Cargar indice de poder</h3>
            </div>
            <UiBadge>Contexto activo</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input
                type="text"
                value={civilizationIdInput}
                onChange={(event) => setCivilizationIdInput(event.target.value)}
                placeholder="00000000-0000-0000-0000-000000000000"
                spellCheck={false}
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Abrir ranking"}
            </button>
          </form>
          {error ? (
            <div className="subpanel figma-subpanel figma-mini-card-warn">
              <p className="error-text">{error}</p>
              {errorFollowUp ? <p className="figma-panel-note">{errorFollowUp}</p> : null}
            </div>
          ) : null}
          {!queryCivilizationId ? (
            <p className="figma-panel-note">
              Introduce un contexto de civilizacion valido para abrir la clasificacion imperial.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Identidad visible</p>
              <h3>Contexto actual</h3>
            </div>
            <UiBadge>{uiState?.identity?.homePlanetLabel ?? "Sin mundo base"}</UiBadge>
          </div>
          {uiState?.identity ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Civilizacion</span><strong>{uiState.identity.civilizationName}</strong></div>
              <div className="figma-data-row"><span>Responsable visible</span><strong>{uiState.identity.displayName}</strong></div>
              <div className="figma-data-row"><span>Mundo base</span><strong>{uiState.identity.homePlanetLabel}</strong></div>
              <div className="figma-data-row"><span>Estado</span><strong>{uiState.publication?.stateLabel ?? rankingLabels.unpublishedRanking}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">
              Cuando exista un contexto valido, la vista mostrara identidad, indice y postura publica del poder actual.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite de la vista</p>
              <h3>Clasificacion interna no publicada</h3>
            </div>
            <UiBadge tone="warn">Sin ladder</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Lee el indice de poder y las categorias derivadas de la civilizacion actual.</li>
            <li>Usa comparativas de referencia, no jugadores reales ni perfiles publicos.</li>
            <li>No publica temporadas, recompensas ni clasificaciones globales.</li>
            <li>No actualiza el ladder ni modifica valores de forma optimista.</li>
          </ul>
        </UiCard>
      </div>

      {!queryCivilizationId && !isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Sin contexto</p>
              <h3>Ranking necesita una civilizacion antes de mostrar un indice util.</h3>
            </div>
            <UiBadge tone="warn">Contexto requerido</UiBadge>
          </div>
          <p className="figma-panel-note">
            Usa el formulario superior o entra desde Galaxia, Mercado, Espionaje o Alianza para conservar el contexto imperial.
          </p>
          {operatorMode ? (
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildDevelopmentHelperUrl()}>
                Abrir contexto de operador
              </Link>
            </div>
          ) : null}
        </UiCard>
      ) : null}

      {isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Cargando</p>
              <h3>Sincronizando indice de poder</h3>
            </div>
            <UiBadge>Cargando...</UiBadge>
          </div>
          <p>Consultando categorias de referencia y dependencias futuras sin publicar datos competitivos.</p>
        </UiCard>
      ) : null}

      {uiState?.summary ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Tablero de poder</p>
                <h3>{uiState.summary.totalPowerIndexLabel}</h3>
                <p>Ranking convierte el indice interno en tarjetas estables por dominio y deja las claves de soporte fuera del modo producto.</p>
              </div>
              <UiBadge tone="resource">{formatRankingProductLabel(uiState.summary.recommendationLabel)}</UiBadge>
            </div>
            <div className="ranking-category-grid">
              {categoryCards.map((category) => (
                <article key={category.key} className={`alliance-catalog-card ranking-category-card alliance-catalog-card-${category.emphasis}`}>
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Categoria</p>
                      <h4>{category.title}</h4>
                    </div>
                    <UiBadge tone={category.emphasis}>{category.scoreLabel}</UiBadge>
                  </div>
                  <p className="ranking-category-copy">{category.explanation}</p>
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Indice</span><strong>{category.scoreLabel}</strong></div>
                    <div className="figma-data-row"><span>Confianza</span><strong>{formatRankingProductLabel(category.readinessLabel)}</strong></div>
                  </div>
                </article>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Comparativa de referencia</p>
                <h3>Referencias internas</h3>
                <p>Las filas visibles comparan la situacion actual con referencias internas y nunca con una clasificacion real.</p>
              </div>
              <UiBadge tone="warn">{formatRankingProductLabel(rankingLabels.demoComparison)}</UiBadge>
            </div>
            <div className="selection-chip-row ranking-state-chip-row">
              <span className="selection-chip selection-chip-active">{formatRankingProductLabel(rankingLabels.unpublishedRanking)}</span>
              <span className="selection-chip">Consulta interna</span>
              <span className="selection-chip">{formatRankingProductLabel(rankingLabels.demoScenarioReference)}</span>
            </div>
            <div className="alliance-catalog-card-grid">
              {comparisonCards.map((comparison) => (
                <article key={comparison.key} className={`alliance-catalog-card alliance-catalog-card-${comparison.emphasis}`}>
                  <div className="alliance-catalog-card-head">
                    <div>
                      <p className="eyebrow">{formatRankingProductLabel(comparison.visibilityLabel)}</p>
                      <h5>{comparison.title}</h5>
                    </div>
                    <UiBadge tone={comparison.emphasis}>{comparison.scoreLabel}</UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Diferencia</span><strong>{comparison.deltaLabel}</strong></div>
                    <div className="figma-data-row"><span>Postura</span><strong>{comparison.postureLabel}</strong></div>
                    <div className="figma-data-row"><span>Estado</span><strong>{formatRankingProductLabel(comparison.stateLabel)}</strong></div>
                    <div className="figma-data-row"><span>Visibilidad</span><strong>{formatRankingProductLabel(comparison.visibilityLabel)}</strong></div>
                  </div>
                  <p className="alliance-catalog-note">{formatRankingProductLabel(comparison.note)}</p>
                </article>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Dependencias de ladder</p>
                <h3>Clasificacion publica fuera de alcance</h3>
                <p>Ranking muestra un indice interno. Publicacion global, temporadas, perfiles y recompensas siguen como dependencias futuras.</p>
              </div>
              <UiBadge tone="warn">{cockpitStatusLabels.safePlaceholder}</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Ladder global</p>
                    <h4>Clasificacion no publicada</h4>
                  </div>
                  <UiBadge tone="warn">{formatRankingProductLabel(rankingLabels.unpublishedRanking)}</UiBadge>
                </div>
                <p className="figma-panel-note">No hay tabla publica, historial persistido ni comparacion real entre jugadores.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Temporadas</p>
                    <h4>Calendario competitivo pendiente</h4>
                  </div>
                  <UiBadge tone="warn">{rankingLabels.futureSeason}</UiBadge>
                </div>
                <p className="figma-panel-note">El indice actual no abre ligas, emparejamiento ni cortes de temporada.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Recompensas y perfiles</p>
                    <h4>Sin premios ni perfil publico</h4>
                  </div>
                  <UiBadge tone="warn">{rankingLabels.rewardsUnavailable}</UiBadge>
                </div>
                <p className="figma-panel-note">No se entregan recompensas, insignias, paginas publicas ni ventajas por este indice.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Dependencias finales</p>
                    <h4>Modelo competitivo no final</h4>
                  </div>
                  <UiBadge tone="warn">No final</UiBadge>
                </div>
                <p className="figma-panel-note">Clasificacion publica, autorizacion competitiva, perfiles, recompensas, combate, mercado y alianzas siguen fuera de esta vista.</p>
              </section>
            </div>
          </UiCard>
        </>
      ) : null}

      {operatorMode && (technicalErrorDetail || uiState?.diagnostics.technical.length || uiState?.diagnostics.limitations.length) ? (
        <details className="technical-disclosure">
          <summary>
            <div>
              <p className="eyebrow">Diagnostico secundario</p>
              <strong>Errores, limites y notas tecnicas</strong>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </summary>
          <div className="technical-disclosure-body">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Soporte de ranking</p>
                  <h3>Lectura tecnica</h3>
                </div>
                <UiBadge>{cockpitStatusLabels.diagnostics}</UiBadge>
              </div>
              {technicalErrorDetail ? (
                <ul className="stack-list compact-list">
                  <li>{technicalErrorDetail}</li>
                </ul>
              ) : null}
              {uiState?.diagnostics.limitations.length ? (
                <>
                  <div className="figma-section-header module-boundary-spacer">
                    <div>
                      <p className="eyebrow">Limitaciones</p>
                      <h4>Fase actual</h4>
                    </div>
                  </div>
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.limitations.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                </>
              ) : null}
              {uiState?.diagnostics.technical.length ? (
                <>
                  <div className="figma-section-header module-boundary-spacer">
                    <div>
                      <p className="eyebrow">Trazas tecnicas</p>
                      <h4>Solo soporte</h4>
                    </div>
                  </div>
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.technical.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                </>
              ) : null}
            </UiCard>
          </div>
        </details>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Pasar a otras sistemas</p>
            <h3>Handoffs relacionados</h3>
            <p>Ranking no duplica las superficies vecinas y deriva a los sistemas que ya explican las fuentes de cada categoria.</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
        </div>
        <div className="readiness-grid">
          {rankingHandoffCards.map((card) => {
            const link = card.key === "galaxy"
              ? buildGalaxyUrl(activeCivilizationId, undefined, uiState?.identity?.homePlanetId ?? null)
              : card.key === "market"
                ? buildMarketUrl(activeCivilizationId, uiState?.identity?.homePlanetId ?? null)
                : card.key === "espionage"
                  ? buildEspionageUrl(activeCivilizationId, undefined, uiState?.identity?.homePlanetId ?? null)
                  : buildAllianceUrl(activeCivilizationId);

            return (
              <section key={card.key} className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{card.label}</p>
                    <h4>{card.title}</h4>
                  </div>
                  <UiBadge tone={card.key === "galaxy" ? "neutral" : "warn"}>{card.label}</UiBadge>
                </div>
                <p className="figma-panel-note">{card.description}</p>
                <Link className={`selection-chip${card.key === "galaxy" ? " selection-chip-active" : ""}`} to={link}>
                  {card.ctaLabel}
                </Link>
              </section>
            );
          })}
        </div>
        <div className="selection-chip-row">
          <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(activeCivilizationId, undefined, uiState?.identity?.homePlanetId ?? null)}>
            Volver a Galaxia
          </Link>
          <Link className="selection-chip" to={buildMarketUrl(activeCivilizationId, uiState?.identity?.homePlanetId ?? null)}>
            Abrir Mercado
          </Link>
          <Link className="selection-chip" to={buildEspionageUrl(activeCivilizationId, undefined, uiState?.identity?.homePlanetId ?? null)}>
            Abrir Espionaje
          </Link>
          <Link className="selection-chip" to={buildAllianceUrl(activeCivilizationId)}>
            Abrir Alianza
          </Link>
        </div>
      </UiCard>
    </section>
  );
}
