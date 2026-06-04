import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchMarketUiState } from "../api/marketApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { buildConstructionUrl, buildDevelopmentHelperUrl, buildFleetsUrl, buildGalaxyUrl, buildPlanetUrl, buildShipyardUrl, isSuspiciousCabinContext } from "../utils/routeUrls";
import { cockpitNavigationLabels, cockpitStatusLabels } from "../utils/cockpitStatus";
import { getMarketPrimaryAction, getMarketResourceSignalLabel, groupMarketSignals, mapMarketUiStateToViewModel, marketResourceOrder, selectRecommendedMarketFocus, type MarketUiState } from "../utils/marketViewModel";
import { formatMarketResourceAmount, getMarketResourceLabel } from "../utils/marketPresentation";

function formatMarketRequestFailure(message: string | null | undefined) {
  const detail = message?.trim() ?? null;

  switch (detail) {
    case "Civilization was not found.":
      return "La civilizacion no existe en el contexto visible.";
    case "Planet was not found.":
      return "El planeta solicitado no esta disponible para esta cabina.";
    case "Request failed with status 404.":
      return "La ruta de Mercado no esta disponible fuera del entorno de desarrollo.";
    case "Request failed with status 503.":
      return "La persistencia de desarrollo no esta disponible ahora mismo.";
    default:
      return "Mercado no pudo cargarse con el contexto actual.";
  }
}

export function MarketPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<MarketUiState | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const market = uiState?.market ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);

  const groupedSignals = useMemo(
    () => groupMarketSignals(market?.signals ?? []),
    [market?.signals],
  );
  const recommendedFocus = useMemo(
    () => selectRecommendedMarketFocus(market),
    [market],
  );
  const primaryActionLabel = useMemo(
    () => getMarketPrimaryAction(market),
    [market],
  );
  const reserveCards = useMemo(
    () => marketResourceOrder.map((resourceType) => ({
      resourceType,
      label: getMarketResourceLabel(resourceType),
      civilizationReserve: market?.civilizationReserves.find((entry) => entry.resourceType === resourceType) ?? null,
      selectedReserve: market?.selectedPlanetReserves.find((entry) => entry.resourceType === resourceType) ?? null,
      signalLabel: getMarketResourceSignalLabel(resourceType, market?.signals ?? []),
    })),
    [market],
  );
  const productionCards = useMemo(
    () => marketResourceOrder.map((resourceType) => ({
      resourceType,
      label: getMarketResourceLabel(resourceType),
      flow: market?.production.find((entry) => entry.resourceType === resourceType) ?? null,
    })),
    [market],
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
        const response = await fetchMarketUiState(queryCivilizationId, queryPlanetId);
        if (!response.succeeded || !response.uiState) {
          setUiState(null);
          setError(formatMarketRequestFailure(response.errors[0] ?? null));
          return;
        }

        const nextState = mapMarketUiStateToViewModel(response.uiState);
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
          formatMarketRequestFailure(
            requestError instanceof Error ? requestError.message : null,
          ),
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
      setUiState(null);
      setError("El id de civilizacion es obligatorio.");
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
      <CockpitHero
        versionLabel="Mercado v1"
        title="Mercado"
        description="Mercado lee reservas, flujo economico y presion comercial visible sin ejecutar compras, ventas ni traslados."
        developmentNote="La cabina sigue siendo una lectura de desarrollo: orienta economia y potencial comercial, pero no confirma operaciones activas."
        badges={(
          <>
            <UiBadge tone="resource">Economia visible</UiBadge>
            <UiBadge>{cockpitStatusLabels.readOnly}</UiBadge>
            <UiBadge tone="warn">Sin compra ni venta</UiBadge>
          </>
        )}
      />

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Entrada de mercado</p>
              <h3>Cargar contexto economico</h3>
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
              {isLoading ? "Cargando..." : "Abrir Mercado"}
            </button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? (
            <p className="figma-panel-note">
              Introduce un `civilizationId` valido o entra desde otra cabina para conservar el contexto de Mercado.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado visible</p>
              <h3>Resumen de cabina</h3>
            </div>
            <UiBadge>{market?.selectedPlanetName ?? "Sin planeta"}</UiBadge>
          </div>
          {market ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Foco recomendado</span><strong>{recommendedFocus}</strong></div>
              <div className="figma-data-row"><span>Accion principal</span><strong>{primaryActionLabel}</strong></div>
              <div className="figma-data-row"><span>Planeta activo</span><strong>{market.selectedPlanetName ?? "Sin seleccion"}</strong></div>
              <div className="figma-data-row"><span>Sistema</span><strong>{market.selectedSolarSystemName ?? "Sin sistema"}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">
              Cuando el contexto sea valido, Mercado mostrara colonia activa, enfoque economico y señales visibles.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite actual</p>
              <h3>Lectura de economia y potencial comercial</h3>
            </div>
            <UiBadge tone="warn">Sin ejecucion</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Mercado lee reservas, produccion y presion comercial visible.</li>
            <li>Esta version no ejecuta compras, ventas, subastas ni traslados.</li>
            <li>Las rutas comerciales futuras aparecen solo como orientacion, no como ordenes activas.</li>
            <li>Los detalles tecnicos permanecen en diagnostico secundario.</li>
          </ul>
        </UiCard>
      </div>

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El identificador de civilizacion no parece valido para Mercado.</h3>
            </div>
            <UiBadge tone="warn">Revisar contexto</UiBadge>
          </div>
          <p className="figma-panel-note">
            Revisa que no hayas usado el id del planeta como civilizacion.
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
              <h3>Sincronizando lectura economica</h3>
            </div>
            <UiBadge>Cargando...</UiBadge>
          </div>
          <p>Consultando reservas, produccion visible, señales y limites del Mercado.</p>
        </UiCard>
      ) : null}

      {!queryCivilizationId && !isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Sin contexto</p>
              <h3>Mercado necesita una civilizacion antes de mostrar economia visible.</h3>
            </div>
            <UiBadge tone="warn">Contexto requerido</UiBadge>
          </div>
          <p className="figma-panel-note">
            Usa el formulario superior o entra desde Planeta, Astillero, Construccion o Flotas para preservar el contexto.
          </p>
        </UiCard>
      ) : null}

      {market ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Resumen economico</p>
                <h3>Apertura de Mercado</h3>
                <p>La cabina resume postura de reservas, flujo, potencial comercial y referencias antes de entrar en detalle.</p>
              </div>
              <UiBadge tone="good">{market.summary.primaryActionLabel}</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Reservas</p>
                    <h4>{market.summary.reservePosture}</h4>
                  </div>
                  <UiBadge>{market.civilizationReserves.length} tipos</UiBadge>
                </div>
                <p>{market.summary.recommendedFocus}</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Produccion</p>
                    <h4>{market.summary.productionPosture}</h4>
                  </div>
                  <UiBadge>{market.production.length > 0 ? `${market.production.length} lecturas` : "Sin perfil"}</UiBadge>
                </div>
                <p>
                  {market.production.length > 0
                    ? "La cabina ya puede leer flujo estimado del planeta activo sin prometer una economia global persistida."
                    : "El flujo productivo sigue incompleto; Mercado mantiene una lectura honesta en lugar de inventar cifras."}
                </p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Potencial comercial</p>
                    <h4>{market.summary.tradePotential}</h4>
                  </div>
                  <UiBadge tone="warn">{market.routePlaceholders.length} rutas futuras</UiBadge>
                </div>
                <p>Mercado observa presion y excedente, pero sigue derivando la accion real hacia otras cabinas o futuras fases.</p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Referencias</p>
                    <h4>{market.summary.referenceAvailability}</h4>
                  </div>
                  <UiBadge>{market.references.length} ratios</UiBadge>
                </div>
                <p>Las referencias actuales son orientativas y sirven para comparar presion economica, no para confirmar una orden.</p>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Lectura lista</p>
                <h3>Mercado ya no es un placeholder</h3>
                <p>La cabina interpreta economia visible con contexto real y prepara la base para paneles mas profundos.</p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone="good">{market.summary.recommendedFocus}</UiBadge>
                <UiBadge>{market.summary.totalReserveTypes} recursos</UiBadge>
                <UiBadge tone="warn">{market.summary.activeSignalCount} senales</UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Planetas conocidos</p>
                    <h4>Contexto local</h4>
                  </div>
                  <UiBadge>{uiState?.knownPlanets.length ?? 0} visibles</UiBadge>
                </div>
                <div className="selection-chip-row">
                  {uiState?.knownPlanets.map((planet) => (
                    <button
                      key={planet.planetId}
                      type="button"
                      className={`selection-chip${planet.planetId === selectedPlanetId ? " selection-chip-active" : ""}`}
                      onClick={() => setSearchParams(new URLSearchParams({
                        civilizationId: activeCivilizationId,
                        planetId: planet.planetId,
                      }))}
                    >
                      {planet.planetName}
                    </button>
                  ))}
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Senales visibles</p>
                    <h4>Grupos de lectura</h4>
                  </div>
                  <UiBadge>{Object.keys(groupedSignals).length} grupos</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {Object.entries(groupedSignals).map(([key, signals]) => (
                    <li key={key}>
                      {signals[0]?.signalLabel ?? key}: {signals.length} lectura{signals.length === 1 ? "" : "s"}
                    </li>
                  ))}
                </ul>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Rutas futuras</p>
                    <h4>Orientacion comercial</h4>
                  </div>
                  <UiBadge tone="warn">{market.routePlaceholders.length} placeholder{market.routePlaceholders.length === 1 ? "" : "s"}</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {market.routePlaceholders.length > 0 ? market.routePlaceholders.map((route) => (
                    <li key={route.actionKey}>{route.label}: {route.reasonLabel}</li>
                  )) : (
                    <li>Sin rutas futuras visibles.</li>
                  )}
                </ul>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Reservas y produccion</p>
                <h3>Lectura economica visible</h3>
                <p>Mercado agrega la lectura actual sin sustituir la gestion local que sigue perteneciendo a Planeta y otras cabinas.</p>
              </div>
              <UiBadge tone="resource">{market.selectedPlanetName ? `Reservas de ${market.selectedPlanetName}` : "Lectura de civilizacion"}</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Reservas</p>
                    <h4>Lectura de civilizacion</h4>
                  </div>
                  <UiBadge>{reserveCards.filter((entry) => entry.civilizationReserve).length} visibles</UiBadge>
                </div>
                <div className="readiness-grid">
                  {reserveCards.map((entry) => (
                    <section key={`reserve-${entry.resourceType}`} className="subpanel figma-subpanel">
                      <div className="figma-data-list">
                        <div className="figma-data-row"><span>{entry.label}</span><strong>{entry.civilizationReserve ? entry.civilizationReserve.quantityLabel : formatMarketResourceAmount(null, entry.resourceType)}</strong></div>
                        <div className="figma-data-row"><span>Estado</span><strong>{entry.signalLabel}</strong></div>
                        <div className="figma-data-row"><span>Reserva local</span><strong>{entry.selectedReserve ? entry.selectedReserve.quantityLabel : "Sin lectura local"}</strong></div>
                      </div>
                    </section>
                  ))}
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Flujo</p>
                    <h4>Produccion estimada</h4>
                  </div>
                  <UiBadge>{market.production.length > 0 ? `${market.production.length} recursos` : "Sin perfil"}</UiBadge>
                </div>
                <div className="readiness-grid">
                  {productionCards.map((entry) => (
                    <section key={`flow-${entry.resourceType}`} className="subpanel figma-subpanel">
                      <div className="figma-data-list">
                        <div className="figma-data-row"><span>{entry.label}</span><strong>{entry.flow?.quantityLabel ?? "Produccion no visible"}</strong></div>
                        <div className="figma-data-row"><span>Alcance</span><strong>{entry.flow ? `Produccion estimada de ${market.selectedPlanetName ?? "planeta activo"}` : "No soportada en esta fase"}</strong></div>
                      </div>
                    </section>
                  ))}
                </div>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Referencias de intercambio</p>
                <h3>Ratios orientativos</h3>
                <p>Estas comparaciones reutilizan referencias deterministas de la cabina y no representan ofertas activas ni operaciones confirmables.</p>
              </div>
              <UiBadge tone="warn">Solo lectura</UiBadge>
            </div>
            <p className="figma-panel-note">
              Mercado muestra una referencia de intercambio para lectura rapida. Ningun ratio de esta seccion ejecuta compra, venta o traslado.
            </p>
            <div className="readiness-grid">
              {market.referenceComparisons.length > 0 ? market.referenceComparisons.map((comparison) => (
                <section key={comparison.key} className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Ratio orientativo</p>
                      <h4>{comparison.pairLabel}</h4>
                    </div>
                    <UiBadge>{comparison.ratioLabel}</UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Referencia de intercambio</span><strong>{comparison.ratioLabel}</strong></div>
                    <div className="figma-data-row"><span>Lectura</span><strong>{comparison.advisoryLabel}</strong></div>
                    <div className="figma-data-row"><span>Operacion</span><strong>{comparison.executionLabel}</strong></div>
                  </div>
                </section>
              )) : (
                <section className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Sin comparativas</p>
                      <h4>No hay ratios visibles</h4>
                    </div>
                    <UiBadge tone="warn">Referencia ausente</UiBadge>
                  </div>
                  <p className="figma-panel-note">
                    La cabina no recibio suficientes referencias deterministas para construir comparaciones seguras en esta fase.
                  </p>
                </section>
              )}
            </div>
          </UiCard>

          <details className="technical-disclosure">
            <summary>
              <div>
                <p className="eyebrow">Diagnostico secundario</p>
                <strong>Notas de soporte y limites actuales</strong>
              </div>
              <UiBadge tone="warn">Contraido por defecto</UiBadge>
            </summary>
            <div className="technical-disclosure-body">
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Lectura tecnica</p>
                    <h3>Notas de Mercado</h3>
                  </div>
                  <UiBadge>{cockpitStatusLabels.diagnostics}</UiBadge>
                </div>
                {uiState?.diagnostics.playerFacing.length ? (
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.playerFacing.map((note) => (
                      <li key={note}>{note}</li>
                    ))}
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
                      {uiState.diagnostics.limitations.map((note) => (
                        <li key={note}>{note}</li>
                      ))}
                    </ul>
                  </>
                ) : null}
              </UiCard>
            </div>
          </details>
        </>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>{cockpitNavigationLabels.relatedCabins}</h3>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
        </div>
        <div className="selection-chip-row">
          {selectedPlanetId ? (
            <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>
              {cockpitNavigationLabels.returnToPlanet}
            </Link>
          ) : null}
          <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.openConstruction}
          </Link>
          <Link className="selection-chip" to={buildShipyardUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.openShipyard}
          </Link>
          <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.openFleets}
          </Link>
          <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, undefined, selectedPlanetId)}>
            {cockpitNavigationLabels.returnToGalaxy}
          </Link>
        </div>
      </UiCard>
    </section>
  );
}
