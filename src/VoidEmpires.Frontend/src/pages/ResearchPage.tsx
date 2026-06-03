import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchResearchUiState } from "../api/researchApi";
import type { ResearchUiState } from "../utils/researchPresentation";
import {
  getResearchPrimaryAction,
  groupResearchTechnologiesByCategory,
  mapResearchUiStateToViewModel,
  selectRecommendedResearch,
} from "../utils/researchPresentation";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { buildConstructionUrl, buildFleetsUrl, buildGalaxyUrl, buildPlanetUrl, isSuspiciousCabinContext } from "../utils/routeUrls";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

export function ResearchPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<ResearchUiState | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const recommendedResearch = useMemo(() => selectRecommendedResearch(uiState?.catalog ?? []), [uiState?.catalog]);
  const catalogGroups = useMemo(() => groupResearchTechnologiesByCategory(uiState?.catalog ?? []), [uiState?.catalog]);
  const availableResearchCount = useMemo(
    () => uiState?.catalog.filter((item) => item.availability.canEnqueue).length ?? 0,
    [uiState?.catalog],
  );
  const blockedResearchCount = useMemo(
    () => uiState?.catalog.filter((item) => !item.availability.canEnqueue && !item.availability.canCompleteDue).length ?? 0,
    [uiState?.catalog],
  );
  const dueQueueCount = useMemo(
    () => uiState?.queue.filter((item) => item.isDue).length ?? 0,
    [uiState?.queue],
  );

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setPlanetIdInput(queryPlanetId ?? "");

    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const response = await fetchResearchUiState(queryCivilizationId, queryPlanetId);
        if (!response.succeeded || !response.uiState) {
          setUiState(null);
          setError(response.errors[0] ?? "La cabina de investigacion no pudo cargarse.");
          return;
        }

        const nextState = mapResearchUiStateToViewModel(response.uiState);
        setUiState(nextState);

        if (nextState.selectedPlanetId && nextState.selectedPlanetId !== queryPlanetId) {
          const nextParams = new URLSearchParams(searchParams);
          nextParams.set("civilizationId", queryCivilizationId);
          nextParams.set("planetId", nextState.selectedPlanetId);
          setSearchParams(nextParams, { replace: true });
        }
      } catch (requestError) {
        setUiState(null);
        setError(requestError instanceof Error ? requestError.message : "La cabina de investigacion no pudo cargarse.");
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, queryPlanetId, searchParams, setSearchParams]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmedCivilizationId = civilizationIdInput.trim();

    if (!trimmedCivilizationId) {
      setError("El id de civilizacion es obligatorio.");
      setUiState(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    if (planetIdInput.trim()) {
      nextParams.set("planetId", planetIdInput.trim());
    }
    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Investigacion v1</UiBadge>
          <h2>Investigacion</h2>
          <p>Cabina de investigacion con carga de contexto, catalogo y diagnostico sin exponer DTOs crudos en la superficie principal.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Cabina de lectura</UiBadge>
          <UiBadge tone="warn">Sin mutaciones por ahora</UiBadge>
          <UiBadge tone="warn">Contexto conservado entre saltos</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace cientifico</p>
              <h3>Cargar contexto de investigacion</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input type="text" value={civilizationIdInput} onChange={(event) => setCivilizationIdInput(event.target.value)} placeholder="00000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <label className="field">
              <span>Id de planeta opcional</span>
              <input type="text" value={planetIdInput} onChange={(event) => setPlanetIdInput(event.target.value)} placeholder="40000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <button type="submit" disabled={isLoading}>{isLoading ? "Cargando..." : "Abrir cabina"}</button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? <p className="figma-panel-note">Introduce un `civilizationId` valido o entra desde Galaxia para fijar el contexto automaticamente.</p> : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Resumen</p>
              <h3>Estado de investigacion</h3>
            </div>
            <UiBadge>{uiState ? `${uiState.catalog.length} tecnologias` : "Sin datos"}</UiBadge>
          </div>
          {uiState ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Planeta seleccionado</span><strong>{uiState.selectedPlanetName ?? "Sin planeta"}</strong></div>
              <div className="figma-data-row"><span>Disponibles</span><strong>{availableResearchCount}</strong></div>
              <div className="figma-data-row"><span>Bloqueadas</span><strong>{blockedResearchCount}</strong></div>
              <div className="figma-data-row"><span>Cola</span><strong>{uiState.queue.length}</strong></div>
              <div className="figma-data-row"><span>En espera de cierre</span><strong>{dueQueueCount}</strong></div>
              <div className="figma-data-row"><span>Proyectos</span><strong>{uiState.projects.length}</strong></div>
              <div className="figma-data-row"><span>Recomendacion</span><strong>{recommendedResearch ? recommendedResearch.label : "Sin recomendacion"}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">La cabina mostrara catalogo, cola y diagnostico cuando exista un contexto valido.</p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limites de cabina</p>
              <h3>Que entra aqui</h3>
            </div>
            <UiBadge tone="good">Lectura segura</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>La cabina prioriza lectura de catalogo, cola y proyectos completados.</li>
            <li>La navegacion conserva `civilizationId` y `planetId` siempre que existen.</li>
            <li>Las mutaciones y la confirmacion de ordenes quedan para tareas posteriores.</li>
            <li>El diagnostico tecnico se mantiene aparte para no ensuciar la vista principal.</li>
          </ul>
        </UiCard>
      </div>

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El id de civilizacion no parece valido para esta cabina.</h3>
            </div>
            <UiBadge tone="warn">Revisar contexto</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>Abrir Planeta</Link>
          </div>
        </UiCard>
      ) : null}

      {isLoading ? <UiCard className="panel"><p>Cargando investigacion, catalogo y diagnostico del contexto seleccionado.</p></UiCard> : null}

      {!isLoading && uiState && uiState.catalog.length === 0 ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Catalogo vacio</p>
              <h3>No hay tecnologias disponibles en esta build.</h3>
            </div>
            <UiBadge tone="warn">Sin catalogo</UiBadge>
          </div>
          <p className="figma-panel-note">La cabina esta preparada, pero el backend no devolvio tecnologias de investigacion para este contexto.</p>
        </UiCard>
      ) : null}

      {uiState ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Catalogo</p>
                <h3>Tecnologias por categoria</h3>
              </div>
              <UiBadge tone="good">Vista normalizada</UiBadge>
            </div>
            <div className="planet-building-groups">
              {catalogGroups.map((group) => (
                <section key={group.key} className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">{group.label}</p>
                      <h4>{group.label}</h4>
                    </div>
                    <UiBadge>{group.technologies.length}</UiBadge>
                  </div>
                  <div className="planet-building-grid">
                    {group.technologies.map((technology) => (
                      <article key={`${technology.researchType}`} className="subpanel figma-subpanel">
                        <div className="figma-section-header">
                          <div>
                            <p className="eyebrow">{technology.bonusLabel}</p>
                            <h4>{technology.label}</h4>
                          </div>
                          <UiBadge tone={technology.availability.canEnqueue ? "good" : "warn"}>{technology.availability.label}</UiBadge>
                        </div>
                        <div className="figma-data-list">
                          <div className="figma-data-row"><span>Nivel</span><strong>{`${technology.currentLevel} -> ${technology.nextLevel}`}</strong></div>
                          <div className="figma-data-row"><span>Coste</span><strong>{technology.estimatedCostLabel}</strong></div>
                          <div className="figma-data-row"><span>Duracion</span><strong>{technology.estimatedDurationLabel}</strong></div>
                          <div className="figma-data-row"><span>Accion</span><strong>{getResearchPrimaryAction(technology)}</strong></div>
                        </div>
                      </article>
                    ))}
                  </div>
                </section>
              ))}
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Cola y progreso</p>
                <h3>Elementos activos y completados</h3>
              </div>
              <UiBadge tone="warn">Contexto conservado</UiBadge>
            </div>
            <div className="figma-two-column">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Cola</p><h4>Ordenes activas</h4></div>
                  <UiBadge>{uiState.queue.length}</UiBadge>
                </div>
                {uiState.queue.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {uiState.queue.map((item) => (
                      <li key={item.orderId}>
                        {item.label} nivel {item.targetLevel} | {item.isDue ? "Lista para cierre" : item.statusLabel} | cierre {formatDateTime(item.endsAtUtc)}
                      </li>
                    ))}
                  </ul>
                ) : <p className="figma-panel-note">No hay ordenes activas en la cola.</p>}
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Completadas</p><h4>Proyectos cerrados</h4></div>
                  <UiBadge>{uiState.projects.length}</UiBadge>
                </div>
                {uiState.projects.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {uiState.projects.map((item) => (
                      <li key={`${item.researchType}`}>{item.label} nivel {item.currentLevel}</li>
                    ))}
                  </ul>
                ) : <p className="figma-panel-note">No hay proyectos completados para mostrar.</p>}
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Navegacion</p>
                <h3>Siguientes cabinas</h3>
              </div>
              <UiBadge tone="warn">Contexto conservado</UiBadge>
            </div>
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>Volver a Planeta</Link>
              <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>Abrir Construccion</Link>
              {selectedPlanetId ? <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>Abrir Flotas</Link> : null}
              <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, null, selectedPlanetId)}>Volver a Galaxia</Link>
            </div>
          </UiCard>

          <details className="technical-disclosure">
            <summary>
              <div>
                <p className="eyebrow">Diagnostico secundario</p>
                <strong>Ids, limites y lectura tecnica</strong>
              </div>
              <UiBadge tone="warn">Contraido por defecto</UiBadge>
            </summary>
            <div className="technical-disclosure-body">
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Metadatos</p><h3>Soporte tecnico</h3></div>
                  <UiBadge>Diagnostico</UiBadge>
                </div>
                <div className="figma-data-list">
                  {uiState.diagnostics.lines.map((line) => <div key={line} className="figma-data-row"><span>Linea</span><strong>{line}</strong></div>)}
                </div>
                {uiState.diagnostics.limitations.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.limitations.map((item) => <li key={item}>{item}</li>)}
                  </ul>
                ) : null}
              </UiCard>
            </div>
          </details>
        </>
      ) : null}
    </section>
  );
}
