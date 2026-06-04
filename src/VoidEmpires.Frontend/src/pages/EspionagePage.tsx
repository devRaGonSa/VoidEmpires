import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchEspionageUiState } from "../api/espionageApi";
import type { EspionageViewModel, IntelligenceSystemTargetGroup, IntelligenceTargetViewModel } from "../utils/espionageViewModel";
import { getEspionagePrimaryAction, mapEspionageUiStateToViewModel } from "../utils/espionageViewModel";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import {
  buildDevelopmentHelperUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  buildResearchUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";

function pickFocusedGroup(groups: readonly IntelligenceSystemTargetGroup[], systemId: string | null, recommendedTarget: EspionageViewModel["recommendedTarget"]) {
  return groups.find((group) => group.systemId === systemId)
    ?? groups.find((group) => group.systemId === recommendedTarget?.systemId)
    ?? groups[0]
    ?? null;
}

function pickFocusedTarget(group: IntelligenceSystemTargetGroup | null, planetId: string | null, recommendedTarget: EspionageViewModel["recommendedTarget"]) {
  return group?.targets.find((target) => target.planetId === planetId)
    ?? group?.targets.find((target) => target.planetId === recommendedTarget?.planetId)
    ?? group?.targets[0]
    ?? null;
}

export function EspionagePage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [systemIdInput, setSystemIdInput] = useState(searchParams.get("systemId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [viewModel, setViewModel] = useState<EspionageViewModel | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const querySystemId = searchParams.get("systemId");
  const queryPlanetId = searchParams.get("planetId");
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setSystemIdInput(querySystemId ?? "");
    setPlanetIdInput(queryPlanetId ?? "");

    async function load() {
      if (!queryCivilizationId) {
        setViewModel(null);
        setError(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const response = await fetchEspionageUiState(queryCivilizationId);
        if (!response.succeeded || !response.uiState) {
          setViewModel(null);
          setError(response.errors[0] ?? "La cabina de espionaje no pudo cargarse.");
          return;
        }

        setViewModel(mapEspionageUiStateToViewModel(response.uiState));
      } catch (requestError) {
        setViewModel(null);
        setError(requestError instanceof Error ? requestError.message : "La cabina de espionaje no pudo cargarse.");
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, querySystemId, queryPlanetId]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationIdInput.trim();
    if (!trimmedCivilizationId) {
      setError("El id de civilizacion es obligatorio.");
      setViewModel(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    if (systemIdInput.trim()) nextParams.set("systemId", systemIdInput.trim());
    if (planetIdInput.trim()) nextParams.set("planetId", planetIdInput.trim());
    setSearchParams(nextParams);
  }

  const focusedGroup = useMemo(
    () => pickFocusedGroup(viewModel?.groups ?? [], querySystemId, viewModel?.recommendedTarget ?? null),
    [querySystemId, viewModel],
  );
  const focusedTarget = useMemo(
    () => pickFocusedTarget(focusedGroup, queryPlanetId, viewModel?.recommendedTarget ?? null),
    [focusedGroup, queryPlanetId, viewModel],
  );
  const activePlanetId = focusedTarget?.planetId ?? queryPlanetId ?? null;

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Espionaje v1</UiBadge>
          <h2>Espionaje</h2>
          <p>La cabina analiza cobertura, objetivos observados y senales pasivas sin abrir misiones activas ni acciones encubiertas.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Solo lectura</UiBadge>
          <UiBadge tone="warn">Sin operaciones activas</UiBadge>
          <UiBadge tone="warn">Cobertura honesta</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace de inteligencia</p>
              <h3>Cargar cobertura</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input type="text" value={civilizationIdInput} onChange={(event) => setCivilizationIdInput(event.target.value)} placeholder="00000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <label className="field">
              <span>Id de sistema opcional</span>
              <input type="text" value={systemIdInput} onChange={(event) => setSystemIdInput(event.target.value)} placeholder="20000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <label className="field">
              <span>Id de planeta opcional</span>
              <input type="text" value={planetIdInput} onChange={(event) => setPlanetIdInput(event.target.value)} placeholder="40000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <button type="submit" disabled={isLoading}>{isLoading ? "Cargando..." : "Cargar espionaje"}</button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? <p className="figma-panel-note">Introduce un `civilizationId` valido para abrir la cabina de espionaje.</p> : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite de la cabina</p>
              <h3>Lectura de inteligencia</h3>
            </div>
            <UiBadge tone="warn">Sin ejecucion</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Resume objetivos propios, visibles, conocidos y parciales.</li>
            <li>Lee senales pasivas, trayectorias y cobertura ya expuestas por la cabina estrategica.</li>
            <li>No crea misiones de espionaje, infiltracion ni sabotaje.</li>
          </ul>
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado actual</p>
              <h3>Resumen de cobertura</h3>
            </div>
            <UiBadge>{viewModel ? `${viewModel.summary.passiveSignalCount} senales` : "Sin lectura"}</UiBadge>
          </div>
          {viewModel ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Objetivos propios</span><strong>{viewModel.summary.ownedTargetCount}</strong></div>
              <div className="figma-data-row"><span>Objetivos visibles</span><strong>{viewModel.summary.visibleTargetCount}</strong></div>
              <div className="figma-data-row"><span>Lecturas conocidas</span><strong>{viewModel.summary.knownTargetCount}</strong></div>
              <div className="figma-data-row"><span>Objetivos parciales</span><strong>{viewModel.summary.partialTargetCount}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">La cabina mostrara su resumen cuando exista un contexto valido de civilizacion.</p>
          )}
        </UiCard>
      </div>

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El identificador de civilizacion no parece valido para esta cabina.</h3>
            </div>
            <UiBadge tone="warn">Revisar contexto</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildDevelopmentHelperUrl()}>Abrir contexto de desarrollo</Link>
          </div>
        </UiCard>
      ) : null}

      {viewModel ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Foco recomendado</p>
              <h3>{focusedTarget?.label ?? "Sin objetivo priorizado"}</h3>
              <p>La cabina prioriza objetivos parciales con senales pasivas o comparaciones visibles antes que prometer operaciones no implementadas.</p>
            </div>
            <UiBadge tone={focusedTarget?.hasPassiveSignals ? "good" : "warn"}>{getEspionagePrimaryAction(viewModel)}</UiBadge>
          </div>
          <div className="figma-detail-grid strategic-detail-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Objetivo</span><strong>{focusedTarget?.label ?? "Sin objetivo"}</strong></div>
                <div className="figma-data-row"><span>Visibilidad</span><strong>{focusedTarget?.visibilityLabel ?? "Sin lectura"}</strong></div>
                <div className="figma-data-row"><span>Inteligencia</span><strong>{focusedTarget?.intelligenceLabel ?? "Sin lectura"}</strong></div>
                <div className="figma-data-row"><span>Confianza</span><strong>{focusedTarget?.confidenceLabel ?? "Sin lectura"}</strong></div>
                <div className="figma-data-row"><span>Cobertura</span><strong>{focusedTarget?.coverageLabel ?? "Sin lectura"}</strong></div>
              </div>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Catalogo agrupado</p>
                  <h4>{focusedGroup?.label ?? "Sin sistema"}</h4>
                </div>
                <UiBadge>{focusedGroup?.targets.length ?? 0} objetivos</UiBadge>
              </div>
              {focusedGroup?.targets.length ? (
                <ul className="stack-list compact-list">
                  {focusedGroup.targets.map((target) => (
                    <li key={`${target.systemId}-${target.planetId ?? target.kind}`}>
                      <strong>{target.label}</strong>: {target.intelligenceLabel}, {target.observationLabel}.
                    </li>
                  ))}
                </ul>
              ) : <p className="figma-panel-note">Todavia no hay objetivos agrupados para este foco.</p>}
            </section>
          </div>
        </UiCard>
      ) : null}

      {viewModel?.diagnostics.technical.length ? (
        <details className="technical-disclosure">
          <summary>
            <div>
              <p className="eyebrow">Diagnostico secundario</p>
              <strong>Lecturas tecnicas</strong>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </summary>
          <div className="technical-disclosure-body">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Limitaciones</p>
                  <h3>Estado honesto del modulo</h3>
                </div>
                <UiBadge>{viewModel.limitations.length} limites</UiBadge>
              </div>
              <ul className="stack-list compact-list">
                {viewModel.limitations.map((item) => <li key={item}>{item}</li>)}
              </ul>
              <details className="json-details">
                <summary>Detalle tecnico</summary>
                <ul className="stack-list compact-list">
                  {viewModel.diagnostics.technical.map((item) => <li key={item}>{item}</li>)}
                </ul>
              </details>
            </UiCard>
          </div>
        </details>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>Siguientes cabinas</h3>
          </div>
          <UiBadge tone="warn">Contexto conservado</UiBadge>
        </div>
        <div className="selection-chip-row">
          <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(queryCivilizationId, querySystemId, activePlanetId)}>Volver a Galaxia</Link>
          {activePlanetId ? <Link className="selection-chip" to={buildPlanetUrl(queryCivilizationId, activePlanetId)}>Abrir Planeta</Link> : null}
          <Link className="selection-chip" to={buildFleetsUrl(queryCivilizationId, activePlanetId)}>Abrir Flotas</Link>
          <Link className="selection-chip" to={buildResearchUrl(queryCivilizationId, activePlanetId)}>Abrir Investigacion</Link>
        </div>
      </UiCard>
    </section>
  );
}
