import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchShipyardUiState } from "../api/shipyardApi";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatPlanetPrimaryLabel, formatPlanetSecondaryLabel, formatResourceType } from "../utils/domainPresentation";
import {
  getShipyardPrimaryAction,
  groupAssetOptionsByCategory,
  mapShipyardUiStateToViewModel,
  selectRecommendedAssetProduction,
  type ShipyardViewModel,
} from "../utils/shipyardViewModel";
import {
  buildConstructionUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

export function ShipyardPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<ShipyardViewModel | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const shipyard = uiState?.shipyard ?? null;
  const categoryGroups = useMemo(() => groupAssetOptionsByCategory(shipyard?.catalog ?? []), [shipyard?.catalog]);
  const recommendedAsset = useMemo(() => selectRecommendedAssetProduction(shipyard?.catalog ?? []), [shipyard?.catalog]);

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setPlanetIdInput(queryPlanetId ?? "");

    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        setTechnicalErrorDetail(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const response = await fetchShipyardUiState(queryCivilizationId, queryPlanetId);
        if (!response.succeeded || !response.uiState) {
          setUiState(null);
          setError(response.errors[0] ?? "La cabina del astillero no pudo cargarse.");
          setTechnicalErrorDetail(response.errors[0] ?? null);
          return;
        }

        const nextState = mapShipyardUiStateToViewModel(response.uiState);
        setUiState(nextState);
        setTechnicalErrorDetail(null);

        if (nextState.selectedPlanetId && nextState.selectedPlanetId !== queryPlanetId) {
          const nextParams = new URLSearchParams(searchParams);
          nextParams.set("civilizationId", queryCivilizationId);
          nextParams.set("planetId", nextState.selectedPlanetId);
          setSearchParams(nextParams, { replace: true });
        }
      } catch (requestError) {
        const detail = requestError instanceof Error ? requestError.message : "La cabina del astillero no pudo cargarse.";
        setUiState(null);
        setError("No se pudo cargar el contexto del astillero.");
        setTechnicalErrorDetail(detail);
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
      setTechnicalErrorDetail("Civilization id is required.");
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
          <UiBadge tone="resource">Astillero v1</UiBadge>
          <h2>Astillero</h2>
          <p>Cabina de produccion orbital para preparar activos, revisar capacidad local y derivar el movimiento real hacia Flotas.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge tone="good">Carga contexto real</UiBadge>
          <UiBadge tone="warn">Sin ordenes de produccion todavia</UiBadge>
          <UiBadge>Flotas mueve grupos ya existentes</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Entrada de cabina</p>
              <h3>Cargar contexto del astillero</h3>
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
              {isLoading ? "Cargando..." : "Abrir astillero"}
            </button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {isLoading ? <p className="figma-panel-note">Cargando catalogo, cola, stock y capacidad orbital...</p> : null}
          {!queryCivilizationId && !isLoading ? (
            <p className="figma-panel-note">Introduce un `civilizationId` valido para abrir la cabina del astillero.</p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Resumen de planeta</p>
              <h3>Contexto cargado</h3>
            </div>
            <UiBadge>{selectedPlanetId ? formatPlanetSecondaryLabel(selectedPlanetId) ?? formatPlanetPrimaryLabel(selectedPlanetId) : "Sin planeta"}</UiBadge>
          </div>
          {shipyard ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Planeta</span><strong>{shipyard.planetName}</strong></div>
              <div className="figma-data-row"><span>Sistema</span><strong>{shipyard.solarSystemName}</strong></div>
              <div className="figma-data-row"><span>Control</span><strong>{shipyard.isOwnedByRequestingCivilization ? "Propio" : "Sin control local"}</strong></div>
              <div className="figma-data-row"><span>Accion principal</span><strong>{getShipyardPrimaryAction(recommendedAsset)}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">La cabina mostrara el planeta seleccionado cuando el endpoint del astillero devuelva un contexto valido.</p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite del modulo</p>
              <h3>Que hace esta cabina</h3>
            </div>
            <UiBadge tone="warn">Frontera visible</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Astillero prepara o produce activos orbitales.</li>
            <li>Flotas mueve escuadras orbitales ya existentes.</li>
            <li>La cola, el stock y los bloqueos deben mostrarse de forma honesta aunque falten acciones ejecutables.</li>
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

      {shipyard ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado de shell</p>
                <h3>Cockpit listo para ampliar</h3>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={recommendedAsset?.statusKey === "Available" ? "good" : "neutral"}>
                  {recommendedAsset?.statusLabel ?? "Sin recomendacion"}
                </UiBadge>
                <UiBadge tone={shipyard.actionAvailability.completeDue.supported ? "warn" : "neutral"}>
                  {shipyard.actionAvailability.completeDue.label}
                </UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Capacidad local</p>
                    <h4>Infraestructura visible</h4>
                  </div>
                  <UiBadge>{shipyard.catalog.length} activos</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Astillero</span><strong>Nivel {shipyard.buildingReadiness.shipyardLevel}</strong></div>
                  <div className="figma-data-row"><span>Mando de flota</span><strong>Nivel {shipyard.buildingReadiness.fleetCommandCenterLevel}</strong></div>
                  <div className="figma-data-row"><span>Centro logistico</span><strong>Nivel {shipyard.buildingReadiness.logisticsHubLevel}</strong></div>
                  <div className="figma-data-row"><span>Tripulacion</span><strong>{shipyard.buildingReadiness.hasPopulationProfile ? "Perfil disponible" : "Sin perfil"}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Reservas y cola</p>
                    <h4>Lectura actual</h4>
                  </div>
                  <UiBadge tone={shipyard.queue.length > 0 ? "warn" : "neutral"}>
                    {shipyard.queue.length > 0 ? `${shipyard.queue.length} ordenes` : "Sin cola"}
                  </UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Reservas visibles</span><strong>{shipyard.stockpile.length}</strong></div>
                  <div className="figma-data-row"><span>Stock orbital</span><strong>{shipyard.orbitalStock.length}</strong></div>
                  <div className="figma-data-row"><span>Ordenes vencidas</span><strong>{shipyard.actionAvailability.completeDue.supported ? "Si" : "No"}</strong></div>
                  <div className="figma-data-row"><span>Enqueue</span><strong>{shipyard.actionAvailability.enqueue.reasonLabel}</strong></div>
                </div>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Lectura cargada</p>
                <h3>Estados utiles antes de la mutacion</h3>
                <p>La shell ya muestra datos reales, pero las acciones de produccion llegaran en tareas posteriores.</p>
              </div>
              <UiBadge tone="warn">Solo lectura</UiBadge>
            </div>
            {shipyard.catalog.length === 0 ? (
              <p className="figma-panel-note">El backend no devolvio catalogo orbital util todavia. La cabina mantiene el shell y deja visible la frontera del modulo.</p>
            ) : (
              <div className="readiness-grid">
                {categoryGroups.map((group) => (
                  <section key={group.key} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Categoria</p>
                        <h4>{group.label}</h4>
                      </div>
                      <UiBadge>{group.assets.length} activos</UiBadge>
                    </div>
                    <ul className="stack-list compact-list">
                      {group.assets.slice(0, 3).map((asset) => (
                        <li key={asset.assetType}>{asset.label}: {asset.reasonLabel}</li>
                      ))}
                    </ul>
                  </section>
                ))}
              </div>
            )}
          </UiCard>

          {shipyard.queue.length > 0 ? (
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Cola orbital</p>
                  <h3>Ordenes visibles</h3>
                </div>
                <UiBadge tone="warn">{shipyard.queue.length} activas</UiBadge>
              </div>
              <div className="readiness-grid">
                {shipyard.queue.map((item) => (
                  <section key={item.orderId} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Orden {item.sequence}</p>
                        <h4>{item.label}</h4>
                      </div>
                      <UiBadge tone={item.isDue ? "warn" : "neutral"}>{item.statusLabel}</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Cantidad</span><strong>{item.quantityLabel}</strong></div>
                      <div className="figma-data-row"><span>Inicio</span><strong>{formatDateTime(item.startsAtUtc)}</strong></div>
                      <div className="figma-data-row"><span>Fin</span><strong>{formatDateTime(item.endsAtUtc)}</strong></div>
                    </div>
                  </section>
                ))}
              </div>
            </UiCard>
          ) : null}

          {shipyard.stockpile.length > 0 ? (
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Reservas locales</p>
                  <h3>Recursos disponibles</h3>
                </div>
                <UiBadge tone="resource">{shipyard.stockpile.length} balances</UiBadge>
              </div>
              <div className="readiness-grid">
                {shipyard.stockpile.map((entry) => (
                  <section key={entry.resourceType} className="subpanel figma-subpanel">
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>{formatResourceType(entry.resourceType)}</span><strong>{entry.quantity}</strong></div>
                    </div>
                  </section>
                ))}
              </div>
            </UiCard>
          ) : null}

          {shipyard.diagnostics.playerFacing.length > 0 || technicalErrorDetail ? (
            <details className="fleet-technical-disclosure">
              <summary>
                <span>Diagnosticos de desarrollo</span>
                <UiBadge tone="warn">Secundario</UiBadge>
              </summary>
              <UiCard className="panel fleet-technical-panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Diagnosticos</p>
                    <h3>Notas del endpoint</h3>
                  </div>
                  <UiBadge tone="warn">Dev only</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {shipyard.diagnostics.playerFacing.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                  {technicalErrorDetail ? <li>{technicalErrorDetail}</li> : null}
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
                <h3>Sin datos de astillero</h3>
              </div>
              <UiBadge tone="warn">Vacio</UiBadge>
            </div>
            <p className="figma-panel-note">El backend no devolvio un contexto de astillero util para este planeta. La shell sigue visible para mantener el acceso y el contexto de navegacion.</p>
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
