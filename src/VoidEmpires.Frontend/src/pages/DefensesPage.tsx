import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { fetchDefensesUiState } from "../api/defenseApi";
import { CockpitHero } from "../components/CockpitHero";
import { DefenseCatalogCard } from "../components/DefenseCatalogCard";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatDefenseRequestFailure } from "../utils/defensePresentation";
import {
  getDefensePrimaryAction,
  groupDefenseOptionsByCategory,
  mapDefensesUiStateToViewModel,
  selectRecommendedDefenseAction,
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
    return "Continuar desde Planeta";
  }

  if (!viewModel.isOwnedByRequestingCivilization) {
    return "Volver a una colonia propia";
  }

  if (viewModel.protectionSummary.queueItemCount > 0) {
    return "Revisar cola defensiva";
  }

  if (viewModel.protectionSummary.availableOptionCount > 0) {
    return "Abrir Construccion";
  }

  if (viewModel.options.length > 0) {
    return "Resolver bloqueo visible";
  }

  return "Continuar en Construccion";
}

function getProductDefenseStatusLabel(statusKey: string, fallbackLabel: string) {
  if (statusKey === "Unsupported" || statusKey === "ReadOnly") {
    return "Produccion pendiente";
  }

  return fallbackLabel;
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
  const optionGroups = useMemo(() => groupDefenseOptionsByCategory(defenses?.options ?? []), [defenses?.options]);
  const recommendedAction = useMemo(() => selectRecommendedDefenseAction(defenses?.options ?? []), [defenses?.options]);
  const protectionPosture = useMemo(() => getProtectionPosture(defenses), [defenses]);
  const recommendedNextStep = useMemo(() => getRecommendedNextStep(defenses), [defenses]);
  const availableOptions = useMemo(() => (defenses?.options ?? []).filter((option) => option.statusKey === "Available"), [defenses?.options]);
  const blockedOptions = useMemo(() => (defenses?.options ?? []).filter((option) => option.statusKey !== "Available"), [defenses?.options]);

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
                <p className="eyebrow">Dashboard defensivo</p>
                <h3>Postura actual y siguiente paso</h3>
                <p>El resumen muestra proteccion real, presion visible y la siguiente accion segura para esta colonia.</p>
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
                ? "Hay obras vencidas, pero Defensas no las cierra hasta que exista una confirmacion acotada a la colonia."
                : "La proteccion actual combina estructuras, recursos y cola; no implica eficacia de combate ni mitigacion real."}
            </p>
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
                        <div className="figma-data-row"><span>Cobertura</span><strong>Proteccion estructural</strong></div>
                      </div>
                    </article>
                  ))}
                </div>
              </>
            ) : (
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Inventario pendiente</p>
                    <h4>Sin estructuras defensivas visibles</h4>
                  </div>
                  <UiBadge tone="warn">Sin despliegue</UiBadge>
                </div>
                <p className="figma-panel-note">Todavia no hay estructuras defensivas visibles en esta colonia. La vista conserva el contexto y mostrara preparaciones o bloqueos reales cuando existan.</p>
              </section>
            )}
            <div className="figma-section-header module-boundary-spacer">
              <div>
                <p className="eyebrow">Preparaciones disponibles</p>
                <h4>Opciones viables para esta colonia</h4>
              </div>
              <UiBadge tone={availableOptions.length > 0 ? "good" : "neutral"}>{availableOptions.length}</UiBadge>
            </div>
            {availableOptions.length > 0 ? (
              <div className="readiness-grid defense-catalog-grid">
                {availableOptions.map((option) => (
                  <DefenseCatalogCard
                    key={`${option.buildingType}-${option.targetLevel}`}
                    option={option}
                    hasProductionAction={hasSafeDefenseEnqueue}
                  />
                ))}
              </div>
            ) : (
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Catalogo defensivo</p>
                    <h4>Sin preparaciones disponibles</h4>
                  </div>
                  <UiBadge tone="neutral">Produccion pendiente</UiBadge>
                </div>
                <p className="figma-panel-note">No hay una preparacion defensiva habilitada en este momento. Revisa la cola, los recursos o el control del planeta desde Construccion.</p>
              </section>
            )}
            <div className="figma-section-header module-boundary-spacer">
              <div>
                <p className="eyebrow">Bloqueos visibles</p>
                <h4>Preparaciones que aun no pueden avanzar</h4>
              </div>
              <UiBadge tone={blockedOptions.length > 0 ? "warn" : "neutral"}>{blockedOptions.length}</UiBadge>
            </div>
            {blockedOptions.length > 0 ? (
              <div className="readiness-grid defense-catalog-grid">
                {blockedOptions.map((option) => (
                  <DefenseCatalogCard
                    key={`${option.buildingType}-${option.targetLevel}`}
                    option={option}
                    hasProductionAction={hasSafeDefenseEnqueue}
                  />
                ))}
              </div>
            ) : (
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Bloqueos</p>
                    <h4>Sin bloqueos defensivos visibles</h4>
                  </div>
                  <UiBadge tone="neutral">Sin bloqueos</UiBadge>
                </div>
                <p className="figma-panel-note">No hay preparaciones bloqueadas visibles para esta colonia. No se fabrica una lista defensiva adicional.</p>
              </section>
            )}
            {optionGroups.length === 0 ? (
              <p className="figma-panel-note">El catalogo defensivo aun no tiene mas opciones para esta colonia. La vista conserva el contexto y explica el limite con honestidad.</p>
            ) : null}
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Cola defensiva</p>
                <h3>Ordenes visibles y cierre conservador</h3>
                <p>Esta seccion usa la cola de construccion filtrada para defensas y mantiene el cierre vencido como accion secundaria no ejecutable.</p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={defenses.queue.length > 0 ? "warn" : "neutral"}>
                  {defenses.queue.length > 0 ? `${defenses.queue.length} visibles` : "Sin cola"}
                </UiBadge>
                <UiBadge tone={defenses.actionAvailability.completeDue.supported ? "warn" : "neutral"}>
                  {defenses.actionAvailability.completeDue.supported ? "Cierre no habilitado aqui" : "Sin cierre seguro"}
                </UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Estado general</p>
                    <h4>Estado de cola</h4>
                  </div>
                  <UiBadge tone="neutral">{cockpitStatusLabels.readOnly}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Origen</span><strong>{defenses.planetName}</strong></div>
                  <div className="figma-data-row"><span>Filtro</span><strong>Construccion filtrada para defensas</strong></div>
                  <div className="figma-data-row"><span>Vencidas</span><strong>{defenses.protectionSummary.dueQueueItemCount}</strong></div>
                  <div className="figma-data-row"><span>Cierre visible</span><strong>{defenses.actionAvailability.completeDue.supported ? "Detectado pero no habilitado" : "No soportado"}</strong></div>
                </div>
                <div className="selection-chip-row">
                  <span className="planet-action-handoff-message">
                    Cierre pendiente de ruta acotada
                  </span>
                </div>
                <p className="figma-panel-note">
                  El cierre disponible no esta acotado a esta colonia. Defensas no lo ejecuta hasta que exista una ruta segura y limitada al planeta.
                </p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Interpretacion</p>
                    <h4>Como leer esta seccion</h4>
                  </div>
                  <UiBadge tone="warn">Sin auto-cierre</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  <li>Una orden visible confirma preparacion de construccion, no combate ni cierre automatico.</li>
                  <li>Una orden vencida sigue siendo una senal de cola pendiente, no una autorizacion para completar desde aqui.</li>
                  <li>Los identificadores tecnicos quedan fuera de la vista principal y se reservan para diagnosticos.</li>
                </ul>
              </section>
            </div>
            {defenses.queue.length === 0 ? (
              <p className="figma-panel-note">No hay ordenes defensivas en cola.</p>
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

          {defenses.diagnostics.playerFacing.length > 0 || defenses.diagnostics.limitations.length > 0 || technicalErrorDetail ? (
            <details className="technical-disclosure">
              <summary>
                <div>
                  <p className="eyebrow">Diagnostico secundario</p>
                  <strong>Lectura tecnica</strong>
                </div>
                <UiBadge tone="warn">Contraido por defecto</UiBadge>
              </summary>
              <div className="technical-disclosure-body">
              <UiCard className="panel fleet-technical-panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Diagnosticos</p>
                    <h3>Notas y limitaciones</h3>
                  </div>
                  <UiBadge tone="warn">{cockpitStatusLabels.developmentOnly}</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {defenses.diagnostics.playerFacing.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                  {defenses.diagnostics.limitations.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                  {technicalErrorDetail ? <li>{technicalErrorDetail}</li> : null}
                </ul>
              </UiCard>
              </div>
            </details>
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
