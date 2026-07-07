import { FormEvent, useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchShipyardUiState } from "../api/shipyardApi";
import type { PlanetUiStateResult } from "../api/planetTypes";
import type { PlanetModuleRouteInfo } from "../utils/planetPresentation";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { PlanetDataRow } from "../components/PlanetModuleLayout";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatPlanetIdentity, formatPlanetOverviewLine, formatPlanetOwnerLabel, formatPlanetShortReference } from "../utils/planetPresentation";
import {
  formatAssetProductionDuration,
  formatAssetQuantity,
  getAssetCategoryLabel,
  getAssetRoleLabel,
  getAssetTypeLabel,
  getShipyardActionCatalog,
  getShipyardActionLabel,
  getShipyardAssetCatalog,
  getShipyardProductionStatusCatalog,
} from "../utils/shipyardPresentation";
import {
  getShipyardPrimaryAction,
  groupAssetOptionsByCategory,
  mapShipyardUiStateToViewModel,
  selectRecommendedAssetProduction,
  type ShipyardViewModel,
} from "../utils/shipyardViewModel";
import {
  getDefenseActionCatalog,
  getDefenseActionLabel,
  getDefenseCategoryLabel,
  getDefenseReadinessCatalog,
  getDefenseReadinessLabel,
  getDefenseStructureCatalog,
  getDefenseStructureLabel,
} from "../utils/defensePresentation";
import {
  buildConstructionUrl,
  buildDevelopmentHelperUrl,
  buildFleetsUrl,
  buildPlanetUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";

interface ModuleCabinPageProps {
  route: PlanetModuleRouteInfo;
}

export function ModuleCabinPage({ route }: ModuleCabinPageProps) {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(
    searchParams.get("civilizationId") ?? "",
  );
  const [planetIdInput, setPlanetIdInput] = useState(
    searchParams.get("planetId") ?? "",
  );
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<PlanetUiStateResult | null>(null);
  const [shipyardState, setShipyardState] = useState<ShipyardViewModel | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const planet = uiState?.planet ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const shipyardAssets = getShipyardAssetCatalog();
  const shipyardActions = getShipyardActionCatalog();
  const shipyardStatuses = getShipyardProductionStatusCatalog();
  const defenseStructures = getDefenseStructureCatalog();
  const defenseReadinessStates = getDefenseReadinessCatalog();
  const defenseActions = getDefenseActionCatalog();
  const shipyardCategoryGroups = groupAssetOptionsByCategory(shipyardState?.shipyard?.catalog ?? []);
  const recommendedShipyardAsset = selectRecommendedAssetProduction(shipyardState?.shipyard?.catalog ?? []);

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
        const [planetResponse, shipyardResponse] = await Promise.all([
          voidEmpiresApi.getPlanetUiState(
            queryCivilizationId,
            queryPlanetId || undefined,
          ),
          route.module === "Shipyard"
            ? fetchShipyardUiState(queryCivilizationId, queryPlanetId || undefined)
            : Promise.resolve(null),
        ]);

        if (!planetResponse.succeeded || !planetResponse.uiState) {
          setUiState(null);
          setError(planetResponse.errors[0] ?? "La cabina especializada no pudo cargarse.");
          return;
        }

        setUiState(planetResponse.uiState);

        if (shipyardResponse?.succeeded && shipyardResponse.uiState) {
          setShipyardState(mapShipyardUiStateToViewModel(shipyardResponse.uiState));
        } else if (route.module === "Shipyard") {
          setShipyardState(null);
        }
      } catch (requestError) {
        setUiState(null);
        setShipyardState(null);
        setError(
          requestError instanceof Error
            ? requestError.message
            : "La cabina especializada no pudo cargarse.",
        );
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, queryPlanetId]);

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
          <UiBadge tone="resource">{route.label} v1</UiBadge>
          <h2>{route.title}</h2>
          <p>{route.purpose}</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Cabina preparada para futura implementación.</UiBadge>
          <UiBadge tone="warn">Esta sección no ejecuta órdenes todavía.</UiBadge>
          <UiBadge tone="warn">Proximamente</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace de modulo</p>
              <h3>Cargar contexto especializado</h3>
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
              {isLoading ? "Cargando..." : "Abrir cabina"}
            </button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? (
            <p className="figma-panel-note">
              Introduce un `civilizationId` valido para cargar el contexto de la cabina especializada.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado actual</p>
              <h3>Contexto de planeta</h3>
            </div>
            <UiBadge>{planet ? formatPlanetShortReference(planet.planetId) : "Sin planeta"}</UiBadge>
          </div>
          {planet ? (
            <div className="figma-data-list">
              <PlanetDataRow label="Mundo" value={formatPlanetIdentity(planet)} />
              <PlanetDataRow label="Linea tactica" value={formatPlanetOverviewLine(planet)} />
              <PlanetDataRow label="Propiedad" value={formatPlanetOwnerLabel(planet)} />
              <PlanetDataRow label="Modulo" value={route.label} />
            </div>
          ) : (
            <p className="figma-panel-note">
              La cabina mostrara el planeta seleccionado cuando el contexto incluya `civilizationId` y, si aplica, `planetId`.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Que pertenece aqui</p>
              <h3>Limite de la cabina</h3>
            </div>
            <UiBadge tone="good">Sin ordenes activas</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            {route.belongsTo.map((item) => (
              <li key={item}>{item}</li>
            ))}
          </ul>
          <div className="figma-section-header module-boundary-spacer">
            <div>
              <p className="eyebrow">Que no pertenece aqui</p>
              <h4>Fuera de alcance</h4>
            </div>
          </div>
          <ul className="stack-list strategic-rules-list">
            {route.excludes.map((item) => (
              <li key={item}>{item}</li>
            ))}
          </ul>
        </UiCard>
      </div>

      {route.module === "Shipyard" ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Taxonomia orbital</p>
              <h3>Vocabulario jugable del astillero</h3>
              <p>Los nombres visibles priorizan lectura de cabina y dejan las claves tecnicas fuera del flujo principal.</p>
            </div>
            <UiBadge tone="resource">{shipyardAssets.length} clases</UiBadge>
          </div>
          <div className="readiness-grid">
            {shipyardAssets.map((asset) => (
              <section key={asset.key} className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{getAssetCategoryLabel(asset.key)}</p>
                    <h4>{getAssetTypeLabel(asset.key)}</h4>
                  </div>
                  <UiBadge tone="good">{formatAssetQuantity(1, asset.key)}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <PlanetDataRow label="Rol" value={getAssetRoleLabel(asset.key)} />
                  <PlanetDataRow label="Categoria" value={getAssetCategoryLabel(asset.key)} />
                  <PlanetDataRow label="Duracion base visible" value={formatAssetProductionDuration(3)} />
                </div>
              </section>
            ))}
          </div>
          <div className="figma-section-header module-boundary-spacer">
            <div>
              <p className="eyebrow">Estados y acciones</p>
              <h4>Terminos listos para reutilizar</h4>
            </div>
          </div>
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Cola orbital</p>
                  <h4>Estados visibles</h4>
                </div>
                <UiBadge>{shipyardStatuses.length} estados</UiBadge>
              </div>
              <ul className="stack-list compact-list">
                {shipyardStatuses.map((status) => (
                  <li key={status.key}>{status.label}</li>
                ))}
              </ul>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Flujo controlado</p>
                  <h4>Acciones del modulo</h4>
                </div>
                <UiBadge tone="warn">Dev-safe</UiBadge>
              </div>
              <ul className="stack-list compact-list">
                {shipyardActions.map((action) => (
                  <li key={action.key}>{getShipyardActionLabel(action.key)}</li>
                ))}
              </ul>
            </section>
          </div>
        </UiCard>
      ) : null}

      {route.module === "Defenses" ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Taxonomia defensiva</p>
              <h3>Vocabulario jugable de la cabina</h3>
              <p>Los nombres visibles priorizan proteccion y readiness sin insinuar combate activo.</p>
            </div>
            <UiBadge tone="resource">{defenseStructures.length} estructuras</UiBadge>
          </div>
          <div className="readiness-grid">
            {defenseStructures.map((structure) => (
              <section key={structure.key} className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{getDefenseCategoryLabel(structure.categoryKey)}</p>
                    <h4>{getDefenseStructureLabel(structure.key)}</h4>
                  </div>
                  <UiBadge tone="good">{getDefenseReadinessLabel(structure.readinessKey)}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <PlanetDataRow label="Categoria" value={getDefenseCategoryLabel(structure.categoryKey)} />
                  <PlanetDataRow label="Estado esperado" value={getDefenseReadinessLabel(structure.readinessKey)} />
                  <PlanetDataRow label="Accion primaria" value={getDefenseActionLabel("construction.enqueue")} />
                </div>
              </section>
            ))}
          </div>
          <div className="figma-section-header module-boundary-spacer">
            <div>
              <p className="eyebrow">Readiness y acciones</p>
              <h4>Etiquetas listas para cockpit</h4>
            </div>
          </div>
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Readiness</p>
                  <h4>Estados visibles</h4>
                </div>
                <UiBadge>{defenseReadinessStates.length} estados</UiBadge>
              </div>
              <ul className="stack-list compact-list">
                {defenseReadinessStates.map((state) => (
                  <li key={state.key}>{getDefenseReadinessLabel(state.key)}</li>
                ))}
              </ul>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Flujo seguro</p>
                  <h4>Acciones del modulo</h4>
                </div>
                <UiBadge tone="warn">Dev-safe</UiBadge>
              </div>
              <ul className="stack-list compact-list">
                {defenseActions.map((action) => (
                  <li key={action.key}>{getDefenseActionLabel(action.key)}</li>
                ))}
              </ul>
            </section>
          </div>
        </UiCard>
      ) : null}

      {route.module === "Shipyard" && shipyardState?.shipyard ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contrato tipado</p>
              <h3>Estado normalizado del astillero</h3>
              <p>La cabina ya consume un modelo de estado estable para presentar la informacion del astillero.</p>
            </div>
            <UiBadge tone={recommendedShipyardAsset?.statusKey === "Available" ? "good" : "warn"}>
              {recommendedShipyardAsset ? getShipyardPrimaryAction(recommendedShipyardAsset) : "Sin recomendacion"}
            </UiBadge>
          </div>
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Resumen orbital</p>
                  <h4>{shipyardState.shipyard.planetName}</h4>
                </div>
                <UiBadge tone={shipyardState.shipyard.isOwnedByRequestingCivilization ? "good" : "warn"}>
                  {shipyardState.shipyard.isOwnedByRequestingCivilization ? "Propio" : "Sin control"}
                </UiBadge>
              </div>
              <div className="figma-data-list">
                <PlanetDataRow label="Sistema" value={shipyardState.shipyard.solarSystemName} />
                <PlanetDataRow label="Cola visible" value={`${shipyardState.shipyard.queue.length} ordenes`} />
                <PlanetDataRow label="Stock orbital" value={`${shipyardState.shipyard.orbitalStock.length} filas`} />
                <PlanetDataRow label="Accion principal" value={recommendedShipyardAsset ? getShipyardPrimaryAction(recommendedShipyardAsset) : getShipyardActionLabel("catalog.read")} />
              </div>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Recomendacion</p>
                  <h4>{recommendedShipyardAsset?.label ?? "Sin activo recomendado"}</h4>
                </div>
                <UiBadge tone={recommendedShipyardAsset?.statusKey === "Available" ? "good" : "neutral"}>
                  {recommendedShipyardAsset?.statusLabel ?? "Pendiente"}
                </UiBadge>
              </div>
              <div className="figma-data-list">
                <PlanetDataRow label="Categoria" value={recommendedShipyardAsset?.categoryLabel ?? "Sin categoria"} />
                <PlanetDataRow label="Rol" value={recommendedShipyardAsset?.roleLabel ?? "Sin rol"} />
                <PlanetDataRow label="Coste" value={recommendedShipyardAsset?.estimatedCostLabel ?? "Sin coste visible"} />
                <PlanetDataRow label="Duracion" value={recommendedShipyardAsset?.estimatedDurationLabel ?? "Sin duracion visible"} />
              </div>
            </section>
          </div>
          <div className="figma-section-header module-boundary-spacer">
            <div>
              <p className="eyebrow">Grupos de catalogo</p>
              <h4>Colecciones listas para UI</h4>
            </div>
          </div>
          <div className="readiness-grid">
            {shipyardCategoryGroups.map((group) => (
              <section key={group.key} className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Categoria</p>
                    <h4>{group.label}</h4>
                  </div>
                  <UiBadge>{group.assets.length} activos</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {group.assets.map((asset) => (
                    <li key={asset.assetType}>
                      {asset.label}: {asset.reasonLabel}
                    </li>
                  ))}
                </ul>
              </section>
            ))}
          </div>
        </UiCard>
      ) : null}

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El identificador de civilizacion no parece valido para esta cabina.</h3>
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

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>Siguientes cabinas</h3>
          </div>
          <UiBadge tone="warn">Contexto conservado</UiBadge>
        </div>
        <div className="selection-chip-row">
          <Link
            className="selection-chip selection-chip-active"
            to={buildPlanetUrl(activeCivilizationId, planet ? planet.planetId : queryPlanetId)}
          >
            Volver a Planeta
          </Link>
          <Link
            className="selection-chip"
            to={buildConstructionUrl(activeCivilizationId, planet ? planet.planetId : queryPlanetId)}
          >
            Abrir Construccion
          </Link>
          {route.module === "Shipyard" ? (
            <Link
              className="selection-chip"
              to={buildFleetsUrl(activeCivilizationId, planet ? planet.planetId : queryPlanetId)}
            >
              Abrir Flotas
            </Link>
          ) : null}
        </div>
      </UiCard>
    </section>
  );
}
