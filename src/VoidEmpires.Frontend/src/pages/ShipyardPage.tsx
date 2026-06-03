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
  type ShipyardAssetOption,
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

function formatCountLabel(count: number, singular: string, plural: string) {
  return `${count} ${count === 1 ? singular : plural}`;
}

function getCatalogBucket(asset: ShipyardAssetOption) {
  if (asset.statusKey === "Available") {
    return "available";
  }

  if (asset.reasonKey === "CatalogUnavailable") {
    return "unsupported";
  }

  return "blocked";
}

function formatRequirementLabel(asset: ShipyardAssetOption) {
  return `Astillero ${asset.requirements.buildingLevel}+ | tripulacion ${asset.requirements.operatorCapacity}`;
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
  const catalogBuckets = useMemo(() => {
    const assets = shipyard?.catalog ?? [];
    return {
      available: assets.filter((asset) => getCatalogBucket(asset) === "available"),
      blocked: assets.filter((asset) => getCatalogBucket(asset) === "blocked"),
      unsupported: assets.filter((asset) => getCatalogBucket(asset) === "unsupported"),
    };
  }, [shipyard?.catalog]);
  const readinessNotes = useMemo(() => {
    if (!shipyard) {
      return [];
    }

    const notes = new Set<string>();
    if (!shipyard.isOwnedByRequestingCivilization) {
      notes.add("El planeta no esta bajo control de la civilizacion cargada.");
    }

    if (!shipyard.buildingReadiness.hasPopulationProfile) {
      notes.add("Falta perfil de tripulacion para sostener produccion orbital.");
    }

    if (!shipyard.actionAvailability.enqueue.supported) {
      notes.add(shipyard.actionAvailability.enqueue.reasonLabel);
    }

    if (shipyard.actionAvailability.completeDue.supported) {
      notes.add("Hay produccion vencida pendiente de completar.");
    }

    return [...notes];
  }, [shipyard]);
  const stockDigest = useMemo(() => {
    if (!shipyard || shipyard.orbitalStock.length === 0) {
      return "Sin reservas orbitales registradas";
    }

    const totalUnits = shipyard.orbitalStock.reduce((sum, entry) => sum + entry.quantity, 0);
    const leadStock = [...shipyard.orbitalStock]
      .sort((left, right) => right.quantity - left.quantity)
      .slice(0, 2)
      .map((entry) => `${entry.label} ${entry.quantityLabel}`);

    return `${formatCountLabel(totalUnits, "unidad local", "unidades locales")} en ${formatCountLabel(shipyard.orbitalStock.length, "tipo", "tipos")}${leadStock.length > 0 ? ` · ${leadStock.join(" · ")}` : ""}`;
  }, [shipyard]);
  const resourceDigest = useMemo(() => {
    if (!shipyard || shipyard.stockpile.length === 0) {
      return "El backend todavia no expone reservas locales utiles.";
    }

    return shipyard.stockpile
      .slice()
      .sort((left, right) => right.quantity - left.quantity)
      .slice(0, 3)
      .map((entry) => `${formatResourceType(entry.resourceType)} ${entry.quantity}`)
      .join(" · ");
  }, [shipyard]);
  const readinessTone = shipyard?.actionAvailability.enqueue.supported
    ? "good"
    : shipyard?.actionAvailability.completeDue.supported
      ? "warn"
      : "neutral";
  const recommendedActionSummary = !recommendedAsset
    ? "El catalogo orbital aun no ofrece una siguiente produccion clara."
    : recommendedAsset.statusKey === "Available"
      ? `${recommendedAsset.label} puede entrar en cola con ${recommendedAsset.estimatedCostLabel} y ${recommendedAsset.estimatedDurationLabel}.`
      : `${recommendedAsset.label} sigue bloqueada: ${recommendedAsset.reasonLabel}.`;

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
                <p className="eyebrow">Overview orbital</p>
                <h3>Resumen estrategico del astillero</h3>
                <p>Lectura rapida de capacidad, cola, stock y accion recomendada antes de revisar el catalogo.</p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={readinessTone}>
                  {shipyard.actionAvailability.enqueue.supported ? "Produccion habilitable" : "Produccion bloqueada"}
                </UiBadge>
                <UiBadge tone={shipyard.queue.length > 0 ? "warn" : "neutral"}>
                  {shipyard.queue.length > 0 ? `${shipyard.queue.length} en cola` : "Cola vacia"}
                </UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Mando actual</p>
                    <h4>Contexto seleccionado</h4>
                  </div>
                  <UiBadge>{formatPlanetSecondaryLabel(shipyard.planetId) ?? "Planeta activo"}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Civilizacion</span><strong>{uiState?.civilizationId ?? activeCivilizationId}</strong></div>
                  <div className="figma-data-row"><span>Planeta</span><strong>{shipyard.planetName}</strong></div>
                  <div className="figma-data-row"><span>Sistema</span><strong>{shipyard.solarSystemName}</strong></div>
                  <div className="figma-data-row"><span>Control</span><strong>{shipyard.isOwnedByRequestingCivilization ? "Propio" : shipyard.ownerCivilizationName ?? "Sin control local"}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Capacidad de produccion</p>
                    <h4>Readiness orbital</h4>
                  </div>
                  <UiBadge tone={readinessTone}>{shipyard.actionAvailability.enqueue.reasonLabel}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Nivel astillero</span><strong>{shipyard.buildingReadiness.shipyardLevel}</strong></div>
                  <div className="figma-data-row"><span>Mando de flota</span><strong>{shipyard.buildingReadiness.fleetCommandCenterLevel}</strong></div>
                  <div className="figma-data-row"><span>Centro logistico</span><strong>{shipyard.buildingReadiness.logisticsHubLevel}</strong></div>
                  <div className="figma-data-row"><span>Catalogo visible</span><strong>{formatCountLabel(shipyard.catalog.length, "activo", "activos")}</strong></div>
                </div>
                {readinessNotes.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {readinessNotes.map((note) => (
                      <li key={note}>{note}</li>
                    ))}
                  </ul>
                ) : (
                  <p className="figma-panel-note">No hay bloqueos visibles en la lectura actual.</p>
                )}
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Reservas y stock</p>
                    <h4>Estado local</h4>
                  </div>
                  <UiBadge tone="resource">{formatCountLabel(shipyard.stockpile.length, "recurso", "recursos")}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Recursos clave</span><strong>{resourceDigest}</strong></div>
                  <div className="figma-data-row"><span>Stock orbital</span><strong>{stockDigest}</strong></div>
                  <div className="figma-data-row"><span>Cola activa</span><strong>{shipyard.queue.length > 0 ? formatCountLabel(shipyard.queue.length, "orden abierta", "ordenes abiertas") : "Sin ordenes abiertas"}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Siguiente paso</p>
                    <h4>Accion recomendada</h4>
                  </div>
                  <UiBadge tone={recommendedAsset?.statusKey === "Available" ? "good" : "warn"}>
                    {getShipyardPrimaryAction(recommendedAsset)}
                  </UiBadge>
                </div>
                <p>{recommendedActionSummary}</p>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Enqueue</span><strong>{shipyard.actionAvailability.enqueue.reasonLabel}</strong></div>
                  <div className="figma-data-row"><span>Completar vencidas</span><strong>{shipyard.actionAvailability.completeDue.reasonLabel}</strong></div>
                </div>
              </section>
            </div>
          </UiCard>

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
                <p className="eyebrow">Catalogo de produccion</p>
                <h3>Opciones orbitales y disponibilidad</h3>
                <p>El catalogo separa lo producible de los bloqueos visibles sin simular acciones de cola todavia.</p>
              </div>
              <UiBadge tone="warn">Solo lectura</UiBadge>
            </div>
            {shipyard.catalog.length === 0 ? (
              <p className="figma-panel-note">El backend no devolvio catalogo orbital util todavia. La cabina mantiene el shell y deja visible la frontera del modulo.</p>
            ) : (
              <>
                <div className="readiness-grid">
                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Disponibles</p>
                        <h4>Listas para entrar en cola</h4>
                      </div>
                      <UiBadge tone="good">{catalogBuckets.available.length}</UiBadge>
                    </div>
                    <p className="figma-panel-note">
                      {catalogBuckets.available.length > 0
                        ? "Estas opciones tienen una lectura de readiness util en el backend actual."
                        : "No hay opciones claramente producibles en la lectura actual."}
                    </p>
                  </section>
                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Bloqueadas</p>
                        <h4>Requieren resolver contexto</h4>
                      </div>
                      <UiBadge tone="warn">{catalogBuckets.blocked.length}</UiBadge>
                    </div>
                    <p className="figma-panel-note">
                      {catalogBuckets.blocked.length > 0
                        ? "Cada carta mantiene visible el motivo del bloqueo para evitar lecturas engañosas."
                        : "No hay bloqueos catalogados aparte de limitaciones del backend."}
                    </p>
                  </section>
                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">No soportadas</p>
                        <h4>Lectura parcial</h4>
                      </div>
                      <UiBadge tone="neutral">{catalogBuckets.unsupported.length}</UiBadge>
                    </div>
                    <p className="figma-panel-note">
                      {catalogBuckets.unsupported.length > 0
                        ? "El endpoint aun no describe bien estas opciones y la UI las muestra como limite real."
                        : "No hay opciones marcadas como no soportadas."}
                    </p>
                  </section>
                </div>

                {categoryGroups.map((group) => (
                  <section key={group.key} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Categoria orbital</p>
                        <h4>{group.label}</h4>
                        <p>{formatCountLabel(group.assets.length, "opcion visible", "opciones visibles")}</p>
                      </div>
                      <UiBadge>{group.assets.length} activos</UiBadge>
                    </div>
                    <div className="readiness-grid">
                      {group.assets.map((asset) => {
                        const isRecommended = recommendedAsset?.assetType === asset.assetType && asset.statusKey === "Available";
                        const bucket = getCatalogBucket(asset);
                        const badgeTone = bucket === "available" ? "good" : bucket === "blocked" ? "warn" : "neutral";

                        return (
                          <article key={asset.assetType} className="subpanel figma-subpanel">
                            <div className="figma-section-header">
                              <div>
                                <p className="eyebrow">{asset.roleLabel}</p>
                                <h4>{asset.label}</h4>
                              </div>
                              <div className="figma-badge-row">
                                {isRecommended ? <UiBadge tone="good">Recomendada</UiBadge> : null}
                                <UiBadge tone={badgeTone}>{asset.statusLabel}</UiBadge>
                              </div>
                            </div>
                            <div className="figma-data-list">
                              <div className="figma-data-row"><span>Coste</span><strong>{asset.estimatedCostLabel}</strong></div>
                              <div className="figma-data-row"><span>Duracion</span><strong>{asset.estimatedDurationLabel}</strong></div>
                              <div className="figma-data-row"><span>Stock local</span><strong>{asset.quantityLabel}</strong></div>
                              <div className="figma-data-row"><span>Requisitos</span><strong>{formatRequirementLabel(asset)}</strong></div>
                            </div>
                            <p>{bucket === "available" ? "Lista para preparar produccion cuando la tarea de mutacion quede habilitada." : asset.reasonLabel}</p>
                          </article>
                        );
                      })}
                    </div>
                  </section>
                ))}
              </>
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
