import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchMarketUiState } from "../api/marketApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { buildConstructionUrl, buildDevelopmentHelperUrl, buildFleetsUrl, buildGalaxyUrl, buildPlanetUrl, buildShipyardUrl, isSuspiciousCabinContext } from "../utils/routeUrls";
import { cockpitNavigationLabels, cockpitStatusLabels } from "../utils/cockpitStatus";
import { getMarketPrimaryAction, getMarketResourceSignalLabel, getMarketTradeSignalSummary, groupMarketSignals, mapMarketUiStateToViewModel, marketResourceOrder, selectRecommendedMarketFocus, type MarketUiState } from "../utils/marketViewModel";
import { formatMarketResourceAmount, getMarketResourceLabel } from "../utils/marketPresentation";

interface MarketErrorPresentation {
  primaryMessage: string;
  followUp: string | null;
  technicalDetail: string | null;
}

function formatMarketRequestFailure(message: string | null | undefined): MarketErrorPresentation {
  const detail = message?.trim() ?? null;

  switch (detail) {
    case "Civilization id is required.":
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "Introduce un id de civilizacion valido antes de abrir Mercado.",
        technicalDetail: detail,
      };
    case "Civilization was not found.":
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "La civilizacion no existe en el contexto visible.",
        technicalDetail: detail,
      };
    case "Planet id is required.":
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "Selecciona un planeta visible o deja que Mercado use el planeta conocido por defecto.",
        technicalDetail: detail,
      };
    case "Planet was not found.":
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "El planeta solicitado no esta disponible para esta cabina.",
        technicalDetail: detail,
      };
    case "Planet is not owned by the requesting civilization.":
      return {
        primaryMessage: "No hay reservas visibles para esta civilizacion.",
        followUp: "Revisa el planeta seleccionado o vuelve a entrar desde una colonia propia.",
        technicalDetail: detail,
      };
    case "No owned planets were found for the requesting civilization.":
      return {
        primaryMessage: "No hay reservas visibles para esta civilizacion.",
        followUp: "Aplica cockpit-validation para cargar el escenario demo.",
        technicalDetail: detail,
      };
    case "Market read is not available for this civilization.":
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "Aplica cockpit-validation para cargar el escenario demo.",
        technicalDetail: detail,
      };
    case "Market transactions are not supported in this version.":
      return {
        primaryMessage: "Las operaciones de mercado no estan disponibles en esta version.",
        followUp: "Mercado sigue siendo una cabina de solo lectura.",
        technicalDetail: detail,
      };
    case "Request failed with status 404.":
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "La ruta de Mercado no esta disponible fuera del entorno de desarrollo.",
        technicalDetail: detail,
      };
    case "Request failed with status 503.":
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "La persistencia de desarrollo no esta disponible ahora mismo.",
        technicalDetail: detail,
      };
    default:
      return {
        primaryMessage: "No se pudo cargar la lectura economica.",
        followUp: "Mercado no pudo cargarse con el contexto actual.",
        technicalDetail: detail,
      };
  }
}

export function MarketPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorFollowUp, setErrorFollowUp] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
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
  const futureMarketOperations = useMemo(
    () => [
      "Comprar recursos",
      "Vender recursos",
      "Crear oferta",
      "Crear ruta comercial",
      "Exportar recursos",
      "Importar recursos",
    ],
    [],
  );

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setPlanetIdInput(queryPlanetId ?? "");

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
        const response = await fetchMarketUiState(queryCivilizationId, queryPlanetId);
        if (!response.succeeded || !response.uiState) {
          setUiState(null);
          const failure = formatMarketRequestFailure(response.errors[0] ?? null);
          setError(failure.primaryMessage);
          setErrorFollowUp(failure.followUp);
          setTechnicalErrorDetail(failure.technicalDetail);
          return;
        }

        const nextState = mapMarketUiStateToViewModel(response.uiState);
        setUiState(nextState);
        setErrorFollowUp(null);
        setTechnicalErrorDetail(null);

        if (nextState.selectedPlanetId && nextState.selectedPlanetId !== queryPlanetId) {
          const nextParams = new URLSearchParams(searchParams);
          nextParams.set("civilizationId", queryCivilizationId);
          nextParams.set("planetId", nextState.selectedPlanetId);
          setSearchParams(nextParams, { replace: true });
        }
      } catch (requestError) {
        setUiState(null);
        const failure = formatMarketRequestFailure(
          requestError instanceof Error ? requestError.message : null,
        );
        setError(failure.primaryMessage);
        setErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
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
      const failure = formatMarketRequestFailure("Civilization id is required.");
      setError(failure.primaryMessage);
      setErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
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
        description="Lectura economica de reservas, produccion y presion comercial visible para orientar la siguiente decision del imperio."
        developmentNote="Esta cabina no ejecuta compras ni ventas. Solo ordena la lectura economica y mantiene las operaciones futuras fuera de esta superficie."
        badges={(
          <>
            <UiBadge tone="resource">Lectura economica</UiBadge>
            <UiBadge>{cockpitStatusLabels.readOnly}</UiBadge>
            <UiBadge tone="warn">Referencias orientativas</UiBadge>
          </>
        )}
      />

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto de lectura</p>
              <h3>Cargar lectura economica</h3>
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
              {isLoading ? "Cargando..." : "Abrir lectura"}
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
              Introduce un `civilizationId` valido o entra desde otra cabina para conservar el contexto de Mercado.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado visible</p>
              <h3>Resumen de lectura</h3>
            </div>
            <UiBadge>{market?.selectedPlanetName ?? "Sin planeta"}</UiBadge>
          </div>
          {market ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Prioridad de lectura</span><strong>{recommendedFocus}</strong></div>
              <div className="figma-data-row"><span>Resumen principal</span><strong>{primaryActionLabel}</strong></div>
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
              <h3>Mercado mantiene una lectura economica</h3>
            </div>
            <UiBadge tone="warn">Sin ejecucion</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Mercado lee reservas, produccion y presion comercial visible.</li>
            <li>Esta cabina no ejecuta compras ni ventas.</li>
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

      {error && queryCivilizationId && !isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Lectura no disponible</p>
              <h3>No se pudo cargar la lectura economica.</h3>
            </div>
            <UiBadge tone="warn">Sin lectura</UiBadge>
          </div>
          <p className="error-text">{error}</p>
          {errorFollowUp ? <p>{errorFollowUp}</p> : null}
        </UiCard>
      ) : null}

      {market ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Resumen economico</p>
                <h3>Panorama economico</h3>
                <p>La cabina resume reservas, flujo, presion comercial y referencias orientativas antes de pasar al detalle.</p>
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
                <p>Mercado observa presion y excedente, pero deja cualquier confirmacion comercial fuera de esta cabina.</p>
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
                <h3>Mercado visible</h3>
                <p>La cabina interpreta la economia visible con contexto real y ordena donde mirar primero.</p>
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
                    <h4>Orientacion de ruta</h4>
                  </div>
                  <UiBadge tone="warn">{market.routePlaceholders.length} referencias</UiBadge>
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
                <div className="market-resource-grid">
                  {reserveCards.map((entry) => (
                    <section key={`reserve-${entry.resourceType}`} className="subpanel figma-subpanel market-resource-card">
                      <div className="market-resource-head">
                        <div>
                          <p className="eyebrow">{entry.label}</p>
                          <h5>{entry.signalLabel}</h5>
                        </div>
                        <UiBadge tone="resource">{entry.label}</UiBadge>
                      </div>
                      <div className="figma-data-list">
                        <div className="figma-data-row"><span>Lectura de civilizacion</span><strong>{entry.civilizationReserve ? entry.civilizationReserve.quantityLabel : formatMarketResourceAmount(null, entry.resourceType)}</strong></div>
                        <div className="figma-data-row"><span>{market.selectedPlanetName ? `Reservas de ${market.selectedPlanetName}` : "Reserva local"}</span><strong>{entry.selectedReserve ? entry.selectedReserve.quantityLabel : "Sin lectura local"}</strong></div>
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
                <div className="market-resource-grid">
                  {productionCards.map((entry) => (
                    <section key={`flow-${entry.resourceType}`} className="subpanel figma-subpanel market-resource-card">
                      <div className="market-resource-head">
                        <div>
                          <p className="eyebrow">{entry.label}</p>
                          <h5>{entry.flow ? "Produccion estimada" : "Sin produccion visible"}</h5>
                        </div>
                        <UiBadge tone={entry.flow ? "good" : "warn"}>
                          {entry.flow ? "Flujo visible" : "Sin perfil"}
                        </UiBadge>
                      </div>
                      <div className="figma-data-list">
                        <div className="figma-data-row"><span>Produccion estimada</span><strong>{entry.flow?.quantityLabel ?? "Produccion no visible"}</strong></div>
                        <div className="figma-data-row"><span>Alcance</span><strong>{entry.flow ? `Produccion estimada de ${market.selectedPlanetName ?? "planeta activo"}` : "No visible en esta lectura"}</strong></div>
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
                <h3>Referencias orientativas</h3>
                <p>Estas comparaciones reutilizan referencias deterministas de la cabina y sirven para interpretar presion economica, no para cerrar un intercambio.</p>
              </div>
              <UiBadge tone="warn">Solo lectura</UiBadge>
            </div>
            <p className="figma-panel-note">
              Mercado muestra una referencia de intercambio para lectura rapida. No es una oferta activa y no ejecuta compra, venta o traslado.
            </p>
            <div className="readiness-grid">
              {market.referenceComparisons.length > 0 ? market.referenceComparisons.map((comparison) => (
                <section key={comparison.key} className="subpanel figma-subpanel market-reference-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Ratio orientativo</p>
                      <h4>{comparison.pairLabel}</h4>
                    </div>
                    <UiBadge tone="warn">{comparison.ratioLabel}</UiBadge>
                  </div>
                  <p className="figma-panel-note market-reference-note">No es una oferta activa.</p>
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Referencia de intercambio</span><strong>{comparison.ratioLabel}</strong></div>
                    <div className="figma-data-row"><span>Lectura</span><strong>{comparison.advisoryLabel}</strong></div>
                    <div className="figma-data-row"><span>Limite</span><strong>{comparison.executionLabel}</strong></div>
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

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Senales comerciales</p>
                <h3>Presion comercial visible</h3>
                <p>Mercado relaciona excedente, tension y contexto de ruta sin apropiarse del flujo real que sigue perteneciendo a Flotas y Galaxia.</p>
              </div>
              <UiBadge tone="warn">Secundario</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Lecturas visibles</p>
                    <h4>Senales de economia</h4>
                  </div>
                  <UiBadge>{market.signals.length} senales</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {market.signals.length > 0 ? market.signals.map((signal) => (
                    <li key={`${signal.signalKey}-${signal.resourceType ?? "global"}`}>
                      <strong>{signal.signalLabel}</strong>: {getMarketTradeSignalSummary(signal)}
                    </li>
                  )) : (
                    <li>Sin presion comercial visible en esta lectura.</li>
                  )}
                </ul>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Rutas comerciales futuras</p>
                    <h4>Referencias de ruta</h4>
                  </div>
                  <UiBadge tone="warn">{market.routePlaceholders.length} visibles</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {market.routePlaceholders.length > 0 ? market.routePlaceholders.map((route) => (
                    <li key={route.actionKey}>
                      <strong>{route.label}</strong>: {route.reasonLabel}
                    </li>
                  )) : (
                    <li>Sin rutas futuras visibles para este contexto.</li>
                  )}
                </ul>
                <p className="figma-panel-note">
                  Estas rutas son solo contexto. Mercado no crea rutas comerciales ni mueve flotas desde esta seccion.
                </p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Dependencia de cabina</p>
                    <h4>Continuar en logistica</h4>
                  </div>
                  <UiBadge tone="warn">Sin ejecucion</UiBadge>
                </div>
                <p className="figma-panel-note">
                  Esta cabina mantiene visible la presion comercial, pero la resolucion real permanece en Flotas y Galaxia.
                </p>
              </section>
            </div>
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
                Revisar logistica en Flotas
              </Link>
              <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, undefined, selectedPlanetId)}>
                Ver contexto de ruta en Galaxia
              </Link>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Operaciones futuras</p>
                <h3>Operaciones no disponibles en esta version</h3>
                <p>La hoja de ruta sigue visible como orientacion. Esta cabina no ejecuta compras ni ventas.</p>
              </div>
              <UiBadge tone="warn">{cockpitStatusLabels.safePlaceholder}</UiBadge>
            </div>
            <div className="market-future-actions-grid">
              {futureMarketOperations.map((actionLabel) => (
                <section key={actionLabel} className="subpanel figma-subpanel market-future-action-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Operacion futura</p>
                      <h4>{actionLabel}</h4>
                    </div>
                    <UiBadge tone="warn">No disponible</UiBadge>
                  </div>
                  <button type="button" className="market-future-action-button" disabled>
                    No disponible en esta version
                  </button>
                  <ul className="stack-list compact-list">
                    <li>No disponible en esta version.</li>
                    <li>Esta cabina no ejecuta compras ni ventas.</li>
                    <li>La operacion queda visible como referencia futura, pero no se puede confirmar aqui.</li>
                  </ul>
                </section>
              ))}
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

      {uiState && !market && !error && queryCivilizationId && !isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Lectura incompleta</p>
              <h3>No hay reservas visibles para esta civilizacion.</h3>
            </div>
            <UiBadge tone="warn">Sin economia visible</UiBadge>
          </div>
          <p className="figma-panel-note">
            Aplica cockpit-validation para cargar el escenario demo o revisa si la civilizacion aun no expone stockpile ni planeta util para Mercado.
          </p>
        </UiCard>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Pasar a otra cabina</p>
            <h3>Limites de Mercado y siguiente paso</h3>
            <p>Mercado interpreta la economia visible y te deriva hacia la cabina propietaria cuando hace falta gestionar reservas locales, consumo, produccion orbital o contexto de ruta.</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
        </div>
        <div className="readiness-grid">
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Planeta</p>
                <h4>Reservas y produccion local</h4>
              </div>
              <UiBadge>Cabina vecina</UiBadge>
            </div>
            <p className="figma-panel-note">
              Usa Planeta para revisar la colonia activa, sus reservas persistidas y la produccion del mundo seleccionado.
            </p>
            <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>
              Abrir Planeta
            </Link>
          </section>
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Construccion</p>
                <h4>Sumideros e infraestructura</h4>
              </div>
              <UiBadge>Cabina vecina</UiBadge>
            </div>
            <p className="figma-panel-note">
              Usa Construccion para validar donde se consumen recursos y que mejoras estan tensionando la economia visible.
            </p>
            <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
              Abrir Construccion
            </Link>
          </section>
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Astillero</p>
                <h4>Produccion orbital y stock</h4>
              </div>
              <UiBadge>Cabina vecina</UiBadge>
            </div>
            <p className="figma-panel-note">
              Usa Astillero para revisar produccion orbital, cola local y stock que aun no se convierte en movimiento de flota.
            </p>
            <Link className="selection-chip" to={buildShipyardUrl(activeCivilizationId, selectedPlanetId)}>
              Abrir Astillero
            </Link>
          </section>
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Flotas</p>
                <h4>Logistica y movimiento</h4>
              </div>
              <UiBadge>Cabina vecina</UiBadge>
            </div>
            <p className="figma-panel-note">
              Usa Flotas para revisar grupos orbitales, dependencias logisticas y cualquier traslado que Mercado solo puede anticipar.
            </p>
            <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
              Abrir Flotas
            </Link>
          </section>
          <section className="subpanel figma-subpanel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Galaxia</p>
                <h4>Ruta y sistema</h4>
              </div>
              <UiBadge>Cabina vecina</UiBadge>
            </div>
            <p className="figma-panel-note">
              Usa Galaxia para recuperar el sistema activo y el contexto espacial que explica por que una ruta futura podria importar.
            </p>
            <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, undefined, selectedPlanetId)}>
              Volver a Galaxia
            </Link>
          </section>
        </div>
      </UiCard>

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

      {(technicalErrorDetail || uiState?.diagnostics.playerFacing.length || uiState?.diagnostics.technical.length) ? (
        <details className="technical-disclosure">
          <summary>
            <div>
              <p className="eyebrow">Diagnostico secundario</p>
              <strong>Errores y notas tecnicas</strong>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </summary>
          <div className="technical-disclosure-body">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Soporte de Mercado</p>
                  <h3>Lectura tecnica</h3>
                </div>
                <UiBadge>{cockpitStatusLabels.diagnostics}</UiBadge>
              </div>
              {technicalErrorDetail ? (
                <>
                  <p className="figma-panel-note">Ultimo detalle tecnico conservado por la cabina.</p>
                  <ul className="stack-list compact-list">
                    <li>{technicalErrorDetail}</li>
                  </ul>
                </>
              ) : null}
              {uiState?.diagnostics.playerFacing.length ? (
                <>
                  <div className="figma-section-header module-boundary-spacer">
                    <div>
                      <p className="eyebrow">Notas visibles</p>
                      <h4>Contexto de lectura</h4>
                    </div>
                  </div>
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.playerFacing.map((note) => (
                      <li key={note}>{note}</li>
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
                    {uiState.diagnostics.technical.map((note) => (
                      <li key={note}>{note}</li>
                    ))}
                  </ul>
                </>
              ) : null}
            </UiCard>
          </div>
        </details>
      ) : null}
    </section>
  );
}
