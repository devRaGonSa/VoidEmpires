import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchDefensesUiState } from "../api/defenseApi";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import {
  getDefensePrimaryAction,
  groupDefenseOptionsByCategory,
  mapDefensesUiStateToViewModel,
  selectRecommendedDefenseAction,
  type DefensesViewModel,
} from "../utils/defenseViewModel";
import {
  buildConstructionUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  buildShipyardUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";

function getProtectionPosture(viewModel: DefensesViewModel["defenses"]) {
  if (!viewModel) {
    return "Sin contexto defensivo";
  }

  if (!viewModel.isOwnedByRequestingCivilization) {
    return "Observacion externa";
  }

  if (viewModel.protectionSummary.structureCount > 0) {
    return "Proteccion desplegada";
  }

  if (viewModel.protectionSummary.availableOptionCount > 0) {
    return "Fortificacion lista";
  }

  if (viewModel.protectionSummary.queueItemCount > 0) {
    return "Refuerzo en cola";
  }

  return "Proteccion inicial";
}

function getRecommendedNextStep(viewModel: DefensesViewModel["defenses"]) {
  if (!viewModel) {
    return "Cargar contexto defensivo";
  }

  if (!viewModel.isOwnedByRequestingCivilization) {
    return "Volver a una colonia propia";
  }

  if (viewModel.protectionSummary.queueItemCount > 0) {
    return "Revisar cola defensiva";
  }

  if (viewModel.protectionSummary.availableOptionCount > 0) {
    return "Preparar fortificacion";
  }

  if (viewModel.options.length > 0) {
    return "Resolver bloqueo visible";
  }

  return "Handoff a Construccion";
}

function getResourcePressureSummary(viewModel: DefensesViewModel["defenses"]) {
  if (!viewModel || viewModel.stockpile.length === 0) {
    return "Sin reservas visibles";
  }

  const ordered = [...viewModel.stockpile]
    .sort((left, right) => right.quantity - left.quantity)
    .slice(0, 3)
    .map((entry) => `${entry.resourceType} ${entry.quantity}`);

  return ordered.join(" | ");
}

export function DefensesPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<DefensesViewModel | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const defenses = uiState?.defenses ?? null;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const optionGroups = useMemo(() => groupDefenseOptionsByCategory(defenses?.options ?? []), [defenses?.options]);
  const recommendedAction = useMemo(() => selectRecommendedDefenseAction(defenses?.options ?? []), [defenses?.options]);
  const protectionPosture = useMemo(() => getProtectionPosture(defenses), [defenses]);
  const recommendedNextStep = useMemo(() => getRecommendedNextStep(defenses), [defenses]);
  const resourcePressureSummary = useMemo(() => getResourcePressureSummary(defenses), [defenses]);
  const availableOptions = useMemo(() => (defenses?.options ?? []).filter((option) => option.statusKey === "Available"), [defenses?.options]);
  const blockedOptions = useMemo(() => (defenses?.options ?? []).filter((option) => option.statusKey !== "Available"), [defenses?.options]);

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
        const response = await fetchDefensesUiState(queryCivilizationId, queryPlanetId);
        if (!response.succeeded || !response.uiState) {
          setUiState(null);
          setError(response.errors[0] ?? "La cabina de defensas no pudo cargarse.");
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
        setUiState(null);
        setError(
          requestError instanceof Error
            ? requestError.message
            : "La cabina de defensas no pudo cargarse.",
        );
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

    const trimmedPlanetId = planetIdInput.trim();
    if (trimmedPlanetId) {
      nextParams.set("planetId", trimmedPlanetId);
    }

    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Defensas v1</UiBadge>
          <h2>Defensas</h2>
          <p>Cabina de proteccion planetaria para leer readiness defensivo, estructuras locales y limites reales de esta build.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge tone="good">Carga contexto real</UiBadge>
          <UiBadge tone="warn">Sin combate ni intercepcion</UiBadge>
          <UiBadge>Construccion sigue siendo el flujo seguro</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Entrada de cabina</p>
              <h3>Cargar contexto defensivo</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
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
            <label className="field">
              <span>Id de planeta opcional</span>
              <input
                type="text"
                value={planetIdInput}
                onChange={(event) => setPlanetIdInput(event.target.value)}
                placeholder="40000000-0000-0000-0000-000000000000"
                spellCheck={false}
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Abrir defensas"}
            </button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {isLoading ? <p className="figma-panel-note">Cargando estructuras, opciones y limites defensivos...</p> : null}
          {!queryCivilizationId && !isLoading ? (
            <p className="figma-panel-note">Introduce un `civilizationId` valido para abrir la cabina de defensas.</p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto activo</p>
              <h3>Planeta y ownership</h3>
            </div>
            <UiBadge>{defenses ? defenses.planetName : "Sin planeta"}</UiBadge>
          </div>
          {defenses ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Planeta</span><strong>{defenses.planetName}</strong></div>
              <div className="figma-data-row"><span>Sistema</span><strong>{defenses.solarSystemName}</strong></div>
              <div className="figma-data-row"><span>Control</span><strong>{defenses.isOwnedByRequestingCivilization ? "Colonia propia" : "Sin control local"}</strong></div>
              <div className="figma-data-row"><span>Cabina</span><strong>Defensas</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">La cabina mostrara el planeta seleccionado cuando el contexto incluya `civilizationId` y, si aplica, `planetId`.</p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite actual</p>
              <h3>Que pertenece aqui</h3>
            </div>
            <UiBadge tone="warn">Readiness</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Preparacion de proteccion planetaria y lectura de estructuras defensivas.</li>
            <li>Contexto de recursos, cola y capacidad para futuras fortificaciones.</li>
            <li>Explicacion clara de limites y handoff hacia Construccion, Astillero y Flotas.</li>
          </ul>
          <div className="figma-section-header module-boundary-spacer">
            <div>
              <p className="eyebrow">Fuera de alcance</p>
              <h4>Sin combate activo</h4>
            </div>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Esta build no ejecuta combate, intercepcion, dano, bombardeo ni invasion.</li>
            <li>La infraestructura general sigue perteneciendo a Construccion.</li>
            <li>La movilidad y el stock orbital siguen perteneciendo a Flotas y Astillero.</li>
          </ul>
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
        </UiCard>
      ) : null}

      {defenses ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Dashboard defensivo</p>
                <h3>Postura actual y siguiente paso</h3>
                <p>La primera lectura resume proteccion real, presion visible y la siguiente accion segura para esta colonia.</p>
              </div>
              <UiBadge tone={recommendedAction?.statusKey === "Available" ? "good" : "neutral"}>
                {getDefensePrimaryAction(recommendedAction)}
              </UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Postura</span><strong>{protectionPosture}</strong></div>
                  <div className="figma-data-row"><span>Estructuras</span><strong>{defenses.protectionSummary.structureCount}</strong></div>
                  <div className="figma-data-row"><span>Nivel total</span><strong>{defenses.protectionSummary.totalDefenseLevel}</strong></div>
                  <div className="figma-data-row"><span>Siguiente paso</span><strong>{recommendedNextStep}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Presion de recursos</span><strong>{resourcePressureSummary}</strong></div>
                  <div className="figma-data-row"><span>Cola defensiva</span><strong>{defenses.protectionSummary.queueItemCount}</strong></div>
                  <div className="figma-data-row"><span>Vencidas</span><strong>{defenses.protectionSummary.dueQueueItemCount}</strong></div>
                  <div className="figma-data-row"><span>Opciones disponibles</span><strong>{defenses.protectionSummary.availableOptionCount}</strong></div>
                  <div className="figma-data-row"><span>Opciones bloqueadas</span><strong>{defenses.protectionSummary.blockedOptionCount}</strong></div>
                </div>
              </section>
            </div>
            <p>
              {recommendedAction
                ? `${recommendedAction.structureLabel}: ${recommendedAction.reasonLabel}.`
                : "Todavia no hay una accion defensiva recomendada para este contexto."}
            </p>
            <p className="figma-panel-note">
              {defenses.actionAvailability.completeDue.supported
                ? "La lectura detecta obras vencidas, pero esta cabina no las cierra porque la accion global sigue fuera de alcance."
                : "La proteccion actual sigue anclada a readiness, recursos y cola; no implica eficacia de combate ni mitigacion real."}
            </p>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Reservas locales</p>
                <h3>Scope real de affordability</h3>
                <p>Las preparaciones defensivas se comparan contra las reservas visibles del planeta activo, no contra una economia global inventada.</p>
              </div>
              <UiBadge tone="resource">{defenses.stockpile.length} recursos</UiBadge>
            </div>
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Scope</span><strong>{`Reservas de ${defenses.planetName}`}</strong></div>
              <div className="figma-data-row"><span>Lectura</span><strong>{resourcePressureSummary}</strong></div>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estructuras y preparacion</p>
                <h3>Lo que ya protege y lo que falta preparar</h3>
                <p>Las cartas separan defensa desplegada, opciones viables y bloqueos visibles sin fingir que toda accion se ejecuta desde aqui.</p>
              </div>
              <UiBadge tone="resource">{defenses.options.length} opciones</UiBadge>
            </div>
            {defenses.structures.length > 0 ? (
              <>
                <div className="figma-section-header module-boundary-spacer">
                  <div>
                    <p className="eyebrow">Proteccion actual</p>
                    <h4>Estructuras desplegadas</h4>
                  </div>
                </div>
                <div className="readiness-grid">
                  {defenses.structures.map((structure) => (
                    <article key={`${structure.buildingType}-${structure.level}`} className="subpanel figma-subpanel">
                      <div className="figma-section-header">
                        <div>
                          <p className="eyebrow">{structure.categoryLabel}</p>
                          <h4>{structure.label}</h4>
                        </div>
                        <UiBadge tone="good">Nivel {structure.level}</UiBadge>
                      </div>
                      <div className="figma-data-list">
                        <div className="figma-data-row"><span>Estado</span><strong>Ya desplegada</strong></div>
                        <div className="figma-data-row"><span>Huella</span><strong>{structure.footprint}</strong></div>
                        <div className="figma-data-row"><span>Lectura primaria</span><strong>Proteccion estructural</strong></div>
                      </div>
                    </article>
                  ))}
                </div>
              </>
            ) : (
              <p className="figma-panel-note">Todavia no hay estructuras defensivas visibles en esta colonia. La cabina enfoca la preparacion y los bloqueos reales del siguiente paso.</p>
            )}
            <div className="figma-section-header module-boundary-spacer">
              <div>
                <p className="eyebrow">Preparaciones disponibles</p>
                <h4>Opciones viables en esta lectura</h4>
              </div>
              <UiBadge tone={availableOptions.length > 0 ? "good" : "neutral"}>{availableOptions.length}</UiBadge>
            </div>
            {availableOptions.length > 0 ? (
              <div className="readiness-grid">
                {availableOptions.map((option) => (
                  <article key={`${option.buildingType}-${option.targetLevel}`} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">{option.categoryLabel}</p>
                        <h4>{option.structureLabel}</h4>
                      </div>
                      <UiBadge tone="good">{option.statusLabel}</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Accion</span><strong>{option.actionLabel}</strong></div>
                      <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {option.targetLevel}</strong></div>
                      <div className="figma-data-row"><span>Coste</span><strong>{option.estimatedCostLabel}</strong></div>
                      <div className="figma-data-row"><span>Duracion</span><strong>{option.estimatedDurationLabel}</strong></div>
                    </div>
                    <p>{option.reasonLabel}</p>
                    <p className="figma-panel-note">Reservas de {defenses.planetName}. Preparacion visible desde Defensas, gestion segura desde Construccion.</p>
                    <div className="selection-chip-row">
                      <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
                        Gestionar construccion desde Construccion
                      </Link>
                    </div>
                  </article>
                ))}
              </div>
            ) : (
              <p className="figma-panel-note">No hay una preparacion defensiva habilitada en este momento. Revisa la cola, los recursos o el ownership del planeta.</p>
            )}
            <div className="figma-section-header module-boundary-spacer">
              <div>
                <p className="eyebrow">Bloqueos visibles</p>
                <h4>Preparaciones que aun no pueden avanzar</h4>
              </div>
              <UiBadge tone={blockedOptions.length > 0 ? "warn" : "neutral"}>{blockedOptions.length}</UiBadge>
            </div>
            {blockedOptions.length > 0 ? (
              <div className="readiness-grid">
                {blockedOptions.map((option) => (
                  <article key={`${option.buildingType}-${option.targetLevel}`} className="subpanel figma-subpanel figma-mini-card-warn">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">{option.categoryLabel}</p>
                        <h4>{option.structureLabel}</h4>
                      </div>
                      <UiBadge tone="warn">{option.statusLabel}</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Accion</span><strong>{option.actionLabel}</strong></div>
                      <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {option.targetLevel}</strong></div>
                      <div className="figma-data-row"><span>Coste</span><strong>{option.estimatedCostLabel}</strong></div>
                      <div className="figma-data-row"><span>Duracion</span><strong>{option.estimatedDurationLabel}</strong></div>
                    </div>
                    <p>{option.reasonLabel}</p>
                    {option.affordabilityLabel ? <p className="figma-panel-note">{option.affordabilityLabel}</p> : null}
                    {option.requirementLabel ? <p className="figma-panel-note">{option.requirementLabel}</p> : null}
                  </article>
                ))}
              </div>
            ) : null}
            {optionGroups.length === 0 ? (
              <p className="figma-panel-note">El backend no expone una lista defensiva mas amplia todavia. La cabina conserva el contexto y explica el limite con honestidad.</p>
            ) : null}
          </UiCard>

          {defenses.diagnostics.playerFacing.length > 0 || defenses.diagnostics.limitations.length > 0 ? (
            <details className="fleet-technical-disclosure">
              <summary>
                <span>Diagnosticos de desarrollo</span>
                <UiBadge tone="warn">Secundario</UiBadge>
              </summary>
              <UiCard className="panel fleet-technical-panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Diagnosticos</p>
                    <h3>Notas y limitaciones</h3>
                  </div>
                  <UiBadge tone="warn">Dev only</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {defenses.diagnostics.playerFacing.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                  {defenses.diagnostics.limitations.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                </ul>
              </UiCard>
            </details>
          ) : null}
        </>
      ) : (
        !isLoading && queryCivilizationId && !error ? (
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado vacio</p>
                <h3>Sin datos defensivos</h3>
              </div>
              <UiBadge tone="warn">Readiness limitado</UiBadge>
            </div>
            <p className="figma-panel-note">El backend no devolvio un contexto defensivo util para este planeta. La shell mantiene acceso, contexto y explicacion de limites.</p>
          </UiCard>
        ) : null
      )}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>Siguientes cabinas</h3>
          </div>
          <UiBadge tone="warn">Contexto conservado</UiBadge>
        </div>
        <div className="selection-chip-row">
          <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>
            Volver a Planeta
          </Link>
          <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
            Abrir Construccion
          </Link>
          <Link className="selection-chip" to={buildShipyardUrl(activeCivilizationId, selectedPlanetId)}>
            Abrir Astillero
          </Link>
          <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
            Abrir Flotas
          </Link>
          <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, undefined, selectedPlanetId)}>
            Volver a Galaxia
          </Link>
        </div>
      </UiCard>
    </section>
  );
}
