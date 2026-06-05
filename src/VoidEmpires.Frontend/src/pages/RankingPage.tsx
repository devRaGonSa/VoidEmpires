import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchRankingUiState } from "../api/rankingApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import {
  getRankingPrimaryAction,
  getRankingStaticLabels,
  groupRankingCategories,
  mapRankingUiStateToViewModel,
  selectRecommendedRankingFocus,
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

function formatRankingRequestFailure(rawError: string | null | undefined) {
  const technicalDetail = rawError?.trim() || null;

  switch (technicalDetail) {
    case "Civilization id is required.":
      return {
        primaryMessage: "Falta el id de civilizacion para abrir Ranking.",
        followUp: "Introduce un id valido o entra desde otra cabina para conservar el contexto.",
        technicalDetail,
      };
    case "Civilization was not found.":
      return {
        primaryMessage: "No se pudo cargar el indice de poder.",
        followUp: "La civilizacion solicitada no existe dentro del contexto visible.",
        technicalDetail,
      };
    case "Request failed with status 404.":
      return {
        primaryMessage: "La ruta de Ranking no esta disponible fuera del entorno de desarrollo.",
        followUp: null,
        technicalDetail,
      };
    case "Request failed with status 503.":
      return {
        primaryMessage: "La persistencia de desarrollo no esta disponible.",
        followUp: "Aplica cockpit-validation antes de revisar el indice de poder.",
        technicalDetail,
      };
    default:
      return {
        primaryMessage: "No se pudo cargar el indice de poder.",
        followUp: "Revisa el contexto actual y abre el diagnostico secundario si el problema persiste.",
        technicalDetail,
      };
  }
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
  const categoryGroups = useMemo(
    () => groupRankingCategories(uiState?.summary?.categories ?? []),
    [uiState?.summary?.categories],
  );

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
          const failure = formatRankingRequestFailure(response.errors[0] ?? null);
          setUiState(null);
          setError(failure.primaryMessage);
          setErrorFollowUp(failure.followUp);
          setTechnicalErrorDetail(failure.technicalDetail);
          return;
        }

        setUiState(mapRankingUiStateToViewModel(response.uiState));
      } catch (requestError) {
        const failure = formatRankingRequestFailure(requestError instanceof Error ? requestError.message : null);
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
      const failure = formatRankingRequestFailure("Civilization id is required.");
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
        description="Cabina de solo lectura para un indice de poder interno, comparativas demo y referencias futuras todavia desactivadas."
        developmentNote="Ranking no publica una clasificacion global, no entrega recompensas y no ejecuta emparejamiento. Solo resume el estado visible de la civilizacion actual."
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
            <p>La cabina resume poder, categorias y comparativas demo sin crear un ladder publico.</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.readOnly}</UiBadge>
        </div>
        <div className="alliance-summary-grid">
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">{rankingLabels.powerIndex}</p>
            <strong>{uiState?.summary?.totalPowerIndexLabel ?? "Sin lectura"}</strong>
            <span>{primaryAction}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Civilizacion</p>
            <strong>{uiState?.identity?.civilizationName ?? "Sin contexto cargado"}</strong>
            <span>{uiState?.identity?.displayName ?? rankingLabels.unclassifiedMetric}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Estado</p>
            <strong>{uiState?.publication?.stateLabel ?? rankingLabels.unpublishedRanking}</strong>
            <span>{uiState?.publication?.summaryLabel ?? rankingLabels.unpublishedRanking}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Comparativas demo</p>
            <strong>{uiState?.comparisons.length ?? 0}</strong>
            <span>{rankingLabels.demoComparison}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Futuro visible</p>
            <strong>{uiState?.futureActions.length ?? 0}</strong>
            <span>{rankingLabels.futureSeason}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Foco recomendado</p>
            <strong>{recommendedFocus}</strong>
            <span>{primaryAction}</span>
          </section>
        </div>
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
              <h3>Clasificacion no publicada</h3>
            </div>
            <UiBadge tone="warn">Sin ladder</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Lee el indice de poder y las categorias derivadas de la civilizacion actual.</li>
            <li>Muestra comparativas demo, no jugadores reales ni perfiles publicos.</li>
            <li>No publica temporadas, recompensas ni clasificaciones globales.</li>
            <li>No llama endpoints de mutacion ni actualiza valores de forma optimista.</li>
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
          <p>Consultando categorias, comparativas demo y placeholders futuros sin publicar datos competitivos.</p>
        </UiCard>
      ) : null}

      {uiState?.summary ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Tablero de poder</p>
                <h3>{uiState.summary.totalPowerIndexLabel}</h3>
                <p>Ranking traduce el payload en un tablero estable y deja las claves tecnicas dentro del diagnostico secundario.</p>
              </div>
              <UiBadge tone="resource">{uiState.summary.recommendationLabel}</UiBadge>
            </div>
            <div className="readiness-grid">
              {[...categoryGroups.primary, ...categoryGroups.secondary].map((category) => (
                <section key={category.categoryKey} className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Categoria</p>
                      <h4>{category.label}</h4>
                    </div>
                    <UiBadge tone={category.emphasis}>{category.scoreLabel}</UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Peso</span><strong>{category.weight}</strong></div>
                    <div className="figma-data-row"><span>Lectura</span><strong>{category.scoreLabel}</strong></div>
                    <div className="figma-data-row"><span>Fuente</span><strong>{category.sourceNote}</strong></div>
                  </div>
                </section>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Comparativa demo</p>
                <h3>Referencias no publicadas</h3>
                <p>Las filas visibles comparan la situacion actual con referencias de demostracion y nunca con un ladder real.</p>
              </div>
              <UiBadge tone="warn">{rankingLabels.demoComparison}</UiBadge>
            </div>
            <div className="alliance-catalog-card-grid">
              {uiState.comparisons.map((comparison) => (
                <article key={comparison.rowKey} className="alliance-catalog-card alliance-catalog-card-neutral">
                  <div className="alliance-catalog-card-head">
                    <div>
                      <p className="eyebrow">{comparison.isCurrentCivilization ? "Actual" : "Referencia demo"}</p>
                      <h5>{comparison.displayName}</h5>
                    </div>
                    <UiBadge tone={comparison.isCurrentCivilization ? "good" : "warn"}>{comparison.totalPowerIndexLabel}</UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Delta</span><strong>{comparison.deltaLabel}</strong></div>
                    <div className="figma-data-row"><span>Postura</span><strong>{comparison.postureLabel}</strong></div>
                    <div className="figma-data-row"><span>Lectura</span><strong>{comparison.isDemoOnly ? rankingLabels.demoComparison : rankingLabels.unpublishedRanking}</strong></div>
                  </div>
                </article>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Futuro visible</p>
                <h3>Temporadas y recompensas</h3>
                <p>Los placeholders futuros permanecen deshabilitados y se muestran como referencias de hoja de ruta.</p>
              </div>
              <UiBadge tone="warn">{rankingLabels.futureSeason}</UiBadge>
            </div>
            <div className="market-future-actions-grid">
              {uiState.futureActions.map((action) => (
                <section key={action.key} className="subpanel figma-subpanel market-future-action-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Referencia futura</p>
                      <h4>{action.label}</h4>
                    </div>
                    <UiBadge tone="warn">{action.stateLabel}</UiBadge>
                  </div>
                  <ul className="stack-list compact-list">
                    <li>{action.reasonLabel}</li>
                    <li>No disponible en esta version.</li>
                    <li>Solo lectura en esta cabina.</li>
                  </ul>
                  <button type="button" className="planet-action-button-blocked" disabled>
                    No disponible en esta version
                  </button>
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
        <div className="selection-chip-row">
          <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(activeCivilizationId)}>
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
