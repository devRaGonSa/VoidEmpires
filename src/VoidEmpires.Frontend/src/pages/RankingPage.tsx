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
    description: "Detalla visibilidad, senales pasivas y objetivos observados sin convertir Ranking en una cabina de inteligencia.",
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
  if (!uiState?.summary) return "Esperando lectura";
  if ((uiState.summary.categories.length ?? 0) > 0 && uiState.comparisons.length > 0) return "Indice preparado";
  if ((uiState.summary.categories.length ?? 0) > 0) return "Categorias visibles";
  return "Lectura limitada";
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
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const recommendedFocus = useMemo(
    () => selectRecommendedRankingFocus(uiState),
    [uiState],
  );
  const primaryAction = useMemo(
    () => getRankingPrimaryAction(uiState),
    [uiState],
  );
  const dominantCategory = useMemo(
    () => selectDominantRankingCategory(uiState),
    [uiState],
  );
  const weakestCategory = useMemo(
    () => selectWeakestRankingFocus(uiState),
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
          setError(failure.primaryMessage);
          setErrorFollowUp(failure.followUp);
          setTechnicalErrorDetail(failure.technicalDetail);
          return;
        }

        setUiState(mapRankingUiStateToViewModel(response.uiState));
      } catch (requestError) {
        const failure = formatRankingRequestFailure(requestError);
        setUiState(null);
        setError(failure.primaryMessage);
        setErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
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
      setError(failure.primaryMessage);
      setErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
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
        title="Ranking"
        description="Cabina de solo lectura para un indice de poder interno, comparativas de validación y referencias futuras todavía desactivadas."
        developmentNote="Ranking no publica una clasificación global, no entrega recompensas y no ejecuta emparejamiento. Solo resume el estado visible de la civilización actual."
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
          title={uiState?.identity?.civilizationName ?? "Lectura de Ranking"}
          purpose="Indice de poder por categorias, comparativas demo y dependencias de ladder sin clasificacion publica ni recompensas."
          statusLabel={rankingReadinessStatus}
          statusTone={uiState?.summary ? "good" : "warn"}
          contextItems={[
            { label: "Civilizacion", value: formatCompactGuid(activeCivilizationId) },
            {
              label: "Poder",
              value: uiState?.summary?.totalPowerIndexLabel ?? "Sin lectura",
              detail: uiState?.publication?.stateLabel ?? rankingLabels.unpublishedRanking,
            },
            {
              label: "Categoria fuerte",
              value: dominantCategory,
              detail: weakestCategory ? `Area critica: ${weakestCategory}` : undefined,
            },
            {
              label: "Comparativa",
              value: uiState ? `${uiState.comparisons.length} filas demo` : "Sin lectura",
              detail: rankingLabels.demoScenarioReference,
            },
          ]}
          resourceItems={[
            { label: "Indice", value: "Solo lectura", tone: "good" },
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
            <h3>Lectura rapida de Ranking</h3>
            <p>Esta cabina no publica ranking global ni recompensas.</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.readOnly}</UiBadge>
        </div>
        <div className="alliance-summary-grid">
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">{rankingLabels.powerIndex}</p>
            <strong>{uiState?.summary?.totalPowerIndexLabel ?? "Sin lectura"}</strong>
            <span>La lectura actual se calcula desde el escenario de validación.</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Potencia total</p>
            <strong>{uiState?.summary?.totalPowerIndexLabel ?? "Sin lectura"}</strong>
            <span>{uiState?.publication?.stateLabel ?? rankingLabels.unpublishedRanking}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Categoria dominante</p>
            <strong>{dominantCategory}</strong>
            <span>{uiState?.summary?.recommendationLabel ?? rankingLabels.unclassifiedMetric}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Área crítica</p>
            <strong>{weakestCategory}</strong>
            <span>{rankingLabels.unclassifiedMetric}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Comparativa de validación</p>
            <strong>{uiState?.comparisons[0]?.totalPowerIndexLabel ?? "Sin lectura"}</strong>
            <span>{rankingLabels.demoComparison}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Foco recomendado</p>
            <strong>{recommendedFocus}</strong>
            <span>{primaryAction}</span>
          </section>
        </div>
        <p className="ranking-summary-note">
          Esta cabina no publica ranking global ni recompensas. La lectura actual se calcula desde el escenario de validación.
        </p>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto de ranking</p>
              <h3>Cargar indice de poder</h3>
            </div>
            <UiBadge>{cockpitStatusLabels.developmentOnly}</UiBadge>
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
              Introduce un `civilizationId` valido para convertir esta ruta en una cabina de ranking real.
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
              Cuando exista un contexto valido, la cabina mostrara identidad, indice y postura publica de la lectura actual.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite de la cabina</p>
              <h3>Lectura interna no publicada</h3>
            </div>
            <UiBadge tone="warn">Sin ladder</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Lee el indice de poder y las categorias derivadas de la civilizacion actual.</li>
            <li>Usa comparativas de validación, no jugadores reales ni perfiles públicos.</li>
            <li>No publica temporadas, recompensas ni clasificaciones globales.</li>
            <li>No llama endpoints de mutación ni actualiza valores de forma optimista.</li>
          </ul>
        </UiCard>
      </div>

      {!queryCivilizationId && !isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Sin contexto</p>
              <h3>Ranking necesita una civilizacion antes de mostrar una lectura util.</h3>
            </div>
            <UiBadge tone="warn">Contexto requerido</UiBadge>
          </div>
          <p className="figma-panel-note">
            Usa el formulario superior o entra desde Galaxia, Mercado, Espionaje o Alianza para conservar `civilizationId`.
          </p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildDevelopmentHelperUrl()}>
              Abrir contexto de desarrollo
            </Link>
          </div>
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
          <p>Consultando categorías de validación y referencias futuras sin publicar datos competitivos.</p>
        </UiCard>
      ) : null}

      {uiState?.summary ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Tablero de poder</p>
                <h3>{uiState.summary.totalPowerIndexLabel}</h3>
                <p>Ranking convierte la lectura técnica en tarjetas estables por dominio y deja las claves técnicas dentro del diagnóstico secundario.</p>
              </div>
              <UiBadge tone="resource">{uiState.summary.recommendationLabel}</UiBadge>
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
                    <div className="figma-data-row"><span>Lectura</span><strong>{category.scoreLabel}</strong></div>
                    <div className="figma-data-row"><span>Confianza</span><strong>{category.readinessLabel}</strong></div>
                  </div>
                </article>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Comparativa de validación</p>
                <h3>Referencias de validación</h3>
                <p>Las filas visibles comparan la situación actual con referencias internas y nunca con una clasificación real.</p>
              </div>
              <UiBadge tone="warn">{rankingLabels.demoComparison}</UiBadge>
            </div>
            <div className="selection-chip-row ranking-state-chip-row">
              <span className="selection-chip selection-chip-active">{rankingLabels.unpublishedRanking}</span>
              <span className="selection-chip">{rankingLabels.readOnly}</span>
              <span className="selection-chip">{rankingLabels.demoScenarioReference}</span>
            </div>
            <div className="alliance-catalog-card-grid">
              {comparisonCards.map((comparison) => (
                <article key={comparison.key} className={`alliance-catalog-card alliance-catalog-card-${comparison.emphasis}`}>
                  <div className="alliance-catalog-card-head">
                    <div>
                      <p className="eyebrow">{comparison.visibilityLabel}</p>
                      <h5>{comparison.title}</h5>
                    </div>
                    <UiBadge tone={comparison.emphasis}>{comparison.scoreLabel}</UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Diferencia</span><strong>{comparison.deltaLabel}</strong></div>
                    <div className="figma-data-row"><span>Postura</span><strong>{comparison.postureLabel}</strong></div>
                    <div className="figma-data-row"><span>Estado</span><strong>{comparison.stateLabel}</strong></div>
                    <div className="figma-data-row"><span>Visibilidad</span><strong>{comparison.visibilityLabel}</strong></div>
                  </div>
                  <p className="alliance-catalog-note">{comparison.note}</p>
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
                  <UiBadge tone="warn">{rankingLabels.unpublishedRanking}</UiBadge>
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
                <p className="figma-panel-note">La lectura actual no abre ligas, emparejamiento ni cortes de temporada.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Recompensas y perfiles</p>
                    <h4>Sin premios ni perfil publico</h4>
                  </div>
                  <UiBadge tone="warn">{rankingLabels.rewardsUnavailable}</UiBadge>
                </div>
                <p className="figma-panel-note">No se entregan recompensas, insignias, paginas publicas ni ventajas por esta lectura.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Dependencias finales</p>
                    <h4>Modelo competitivo no final</h4>
                  </div>
                  <UiBadge tone="warn">No final</UiBadge>
                </div>
                <p className="figma-panel-note">Persistencia final, autorizacion de produccion, activos finales, combate, mercado y alianzas siguen fuera de esta cabina.</p>
              </section>
            </div>
          </UiCard>
        </>
      ) : null}

      {(technicalErrorDetail || uiState?.diagnostics.technical.length || uiState?.diagnostics.limitations.length) ? (
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
            <p className="eyebrow">Pasar a otras cabinas</p>
            <h3>Handoffs relacionados</h3>
            <p>Ranking no duplica las superficies vecinas y deriva a las cabinas que ya explican las fuentes de cada categoria.</p>
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
