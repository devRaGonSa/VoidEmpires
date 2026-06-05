import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchRankingUiState } from "../api/rankingApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import {
  buildRankingCategoryCards,
  buildRankingComparisonCards,
  buildRankingFutureCapabilityCards,
  buildRankingFutureLeaderboardCards,
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
  const futureLeaderboardCards = useMemo(
    () => buildRankingFutureLeaderboardCards(uiState?.futureActions ?? []),
    [uiState?.futureActions],
  );
  const futureCapabilityCards = useMemo(
    () => buildRankingFutureCapabilityCards(uiState?.futureActions ?? []),
    [uiState?.futureActions],
  );

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
                <p className="eyebrow">Futuro visible</p>
                <h3>Leaderboards y recompensas deshabilitadas</h3>
                <p>Las referencias futuras permanecen deshabilitadas y se muestran como hoja de ruta secundaria, nunca como acciones ejecutables.</p>
              </div>
              <UiBadge tone="warn">{rankingLabels.futureSeason}</UiBadge>
            </div>
            <div className="market-future-actions-grid">
              {futureLeaderboardCards.map((action) => (
                <section key={action.key} className="subpanel figma-subpanel market-future-action-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Referencia futura</p>
                      <h4>{action.title}</h4>
                    </div>
                    <UiBadge tone="warn">{action.stateLabel}</UiBadge>
                  </div>
                  <ul className="stack-list compact-list">
                    <li>{action.reasonLabel}</li>
                    <li>No disponible en esta version.</li>
                    <li>Solo lectura en esta cabina.</li>
                    <li>No publica perfiles, temporadas ni recompensas reales.</li>
                  </ul>
                  <button type="button" className="planet-action-button-blocked" disabled>
                    No disponible en esta version
                  </button>
                </section>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Funciones futuras</p>
                <h3>Funciones futuras de clasificacion</h3>
                <p>Estas capacidades permanecen como referencias secundarias y no exponen navegacion, mutacion ni confirmacion desde Ranking.</p>
              </div>
              <UiBadge tone="warn">{cockpitStatusLabels.safePlaceholder}</UiBadge>
            </div>
            <div className="market-future-actions-grid">
              {futureCapabilityCards.map((action) => (
                <section key={action.key} className="subpanel figma-subpanel market-future-action-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Accion futura</p>
                      <h4>{action.title}</h4>
                    </div>
                    <UiBadge tone="warn">{action.stateLabel}</UiBadge>
                  </div>
                  <div className="market-future-action-state" aria-hidden="true">
                    No disponible en esta version
                  </div>
                  <ul className="stack-list compact-list">
                    <li>No disponible en esta version.</li>
                    <li>Solo lectura en esta cabina.</li>
                    <li>La funcion queda visible como referencia futura, pero no se puede ejecutar.</li>
                    <li>{action.reasonLabel}</li>
                  </ul>
                </section>
              ))}
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
