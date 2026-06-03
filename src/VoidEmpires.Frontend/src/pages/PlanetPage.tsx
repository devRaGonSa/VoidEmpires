import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import {
  planetBuildingTypeCatalog,
  planetConstructionActionCatalog,
} from "../api/planetTypes";
import type {
  PlanetCockpitDto,
  PlanetConstructionActionDto,
  PlanetUiStateResult,
} from "../api/planetTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import {
  formatColonizationStatus,
  formatCompactGuid,
  formatPlanetType,
  formatResourceType,
} from "../utils/domainPresentation";
import {
  formatBuildingCategory,
  formatBuildingType,
  formatConstructionAction,
  formatConstructionActionButtonLabel,
  formatConstructionAvailability,
  formatConstructionStatus,
  formatCompactResourceCost,
  formatMissingPlanetResources,
  formatPlanetControlStatus,
  formatPlanetIdentity,
  formatPlanetOverviewLine,
  formatPlanetOwnerLabel,
  formatPlanetShortReference,
  groupActionsByCategory,
  groupBuildingsByCategory,
  toPlanetCatalogId,
} from "../utils/planetPresentation";

interface PlanetDataRowProps {
  label: string;
  value: string;
}

function PlanetDataRow({ label, value }: PlanetDataRowProps) {
  return (
    <div className="figma-data-row">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  if (Number.isNaN(parsed)) {
    return "No disponible";
  }

  return new Intl.DateTimeFormat("es-ES", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(parsed);
}

function formatDuration(value: string) {
  const match = /^(\d+)\.(\d{2}):(\d{2}):(\d{2})$/.exec(value) ?? /^(\d{2}):(\d{2}):(\d{2})$/.exec(value);
  if (!match) {
    return value;
  }

  const hasDays = match.length === 5;
  const days = hasDays ? Number(match[1]) : 0;
  const hours = Number(match[hasDays ? 2 : 1]);
  const minutes = Number(match[hasDays ? 3 : 2]);
  const parts = [];

  if (days > 0) {
    parts.push(`${days}d`);
  }

  if (hours > 0) {
    parts.push(`${hours}h`);
  }

  if (minutes > 0) {
    parts.push(`${minutes}m`);
  }

  return parts.length > 0 ? parts.join(" ") : "Menos de 1 min";
}

function formatCost(cost: PlanetCockpitDto["stockpile"]) {
  return formatCompactResourceCost(cost);
}

function formatCompactCost(cost: PlanetCockpitDto["stockpile"]) {
  return cost.length
    ? cost
      .filter((item) => item.quantity > 0)
      .map((item) => `${formatResourceType(item.resourceType)} ${item.quantity}`)
      .join(" · ")
    : "Sin coste";
}

function formatQueueState(item: PlanetCockpitDto["constructionQueue"][number]) {
  if (item.isDue) {
    return "Lista para cierre";
  }

  return formatConstructionStatus(item.status);
}

function findPreparedAction(
  planet: PlanetCockpitDto | null,
  preparedActionKey: string,
) {
  if (!planet || !preparedActionKey) {
    return null;
  }

  return (
    planet.constructionActions.find(
      (action) => `${action.action}-${action.buildingType}` === preparedActionKey,
    ) ?? null
  );
}

export function PlanetPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(
    searchParams.get("civilizationId") ?? "",
  );
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<PlanetUiStateResult | null>(null);
  const [preparedActionKey, setPreparedActionKey] = useState("");
  const [hasConstructionAcknowledgement, setHasConstructionAcknowledgement] = useState(false);
  const [isSubmittingConstruction, setIsSubmittingConstruction] = useState(false);
  const [constructionFeedback, setConstructionFeedback] = useState<string | null>(null);
  const [constructionError, setConstructionError] = useState<string | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const planet = uiState?.planet ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;

  const preparedAction = useMemo(
    () => findPreparedAction(planet, preparedActionKey),
    [planet, preparedActionKey],
  );

  const buildingsByCategory = useMemo(
    () => groupBuildingsByCategory(planet?.buildings ?? []),
    [planet?.buildings],
  );

  const actionsByCategory = useMemo(
    () => groupActionsByCategory(planet?.constructionActions ?? []),
    [planet?.constructionActions],
  );

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);

    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        return;
      }

      setIsLoading(true);
      setError(null);
      setConstructionFeedback(null);
      setConstructionError(null);

      try {
        const response = await voidEmpiresApi.getPlanetUiState(
          queryCivilizationId,
          queryPlanetId,
        );

        if (!response.succeeded || !response.uiState) {
          setUiState(null);
          setError(response.errors[0] ?? "La cabina de planeta no pudo cargarse.");
          return;
        }

        setUiState(response.uiState);
        setPreparedActionKey("");
        setHasConstructionAcknowledgement(false);

        if (response.uiState.selectedPlanetId && response.uiState.selectedPlanetId !== queryPlanetId) {
          const nextParams = new URLSearchParams(searchParams);
          nextParams.set("civilizationId", queryCivilizationId);
          nextParams.set("planetId", response.uiState.selectedPlanetId);
          setSearchParams(nextParams, { replace: true });
        }
      } catch (requestError) {
        setUiState(null);
        setError(
          requestError instanceof Error
            ? requestError.message
            : "La cabina de planeta no pudo cargarse.",
        );
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, queryPlanetId, searchParams, setSearchParams]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationIdInput.trim();
    if (!trimmedCivilizationId) {
      setError("El id de civilizacion es obligatorio.");
      setUiState(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    if (queryPlanetId) {
      nextParams.set("planetId", queryPlanetId);
    }
    setSearchParams(nextParams);
  }

  async function handlePlanetSelection(planetId: string) {
    if (!queryCivilizationId) {
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", queryCivilizationId);
    nextParams.set("planetId", planetId);
    setSearchParams(nextParams);
  }

  async function handleConstructionSubmit() {
    if (
      !preparedAction ||
      preparedAction.availabilityStatus !== "Available" ||
      !planet ||
      !uiState?.civilizationId
    ) {
      return;
    }

    setIsSubmittingConstruction(true);
    setConstructionFeedback(null);
    setConstructionError(null);

    try {
      const result = await voidEmpiresApi.enqueuePlanetConstruction({
        planetId: planet.planetId,
        civilizationId: uiState.civilizationId,
        action: toPlanetCatalogId(preparedAction.action, planetConstructionActionCatalog),
        buildingType: toPlanetCatalogId(preparedAction.buildingType, planetBuildingTypeCatalog),
        requestedAtUtc: new Date().toISOString(),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        setConstructionError(
          result.response?.errors[0] ??
            "La cola de construccion rechazo la orden preparada.",
        );
        return;
      }

      setConstructionFeedback("Orden enviada. Estado actualizado desde la API.");
      setPreparedActionKey("");
      setHasConstructionAcknowledgement(false);

      const refreshed = await voidEmpiresApi.getPlanetUiState(
        uiState.civilizationId,
        planet.planetId,
      );

      if (refreshed.succeeded && refreshed.uiState) {
        setUiState(refreshed.uiState);
      }
    } catch (requestError) {
      setConstructionError(
        requestError instanceof Error
          ? requestError.message
          : "No se pudo enviar la orden de construccion.",
      );
    } finally {
      setIsSubmittingConstruction(false);
    }
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Cabina Planetaria v1</UiBadge>
          <h2>Gestion de colonia en 2D</h2>
          <p>
            La superficie de Planeta prioriza identidad colonial, recursos,
            edificios y cola de construccion antes que datos tecnicos.
          </p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Sin 3D</UiBadge>
          <UiBadge>Acciones seguras y confirmadas</UiBadge>
          <UiBadge tone="warn">Galaxia permanece en observacion</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace planetario</p>
              <h3>Cargar cabina de planeta</h3>
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
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Abrir cabina"}
            </button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? (
            <p className="figma-panel-note">
              Introduce un `civilizationId` valido o entra desde Galaxia para fijar
              el contexto automaticamente.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado de colonia</p>
              <h3>Resumen de gestion</h3>
            </div>
            <UiBadge>
              {planet ? formatPlanetShortReference(planet.planetId) : "Sin planeta"}
            </UiBadge>
          </div>
          {planet ? (
            <div className="figma-data-list">
              <PlanetDataRow label="Mundo" value={formatPlanetIdentity(planet)} />
              <PlanetDataRow label="Linea tactica" value={formatPlanetOverviewLine(planet)} />
              <PlanetDataRow label="Propiedad" value={formatPlanetOwnerLabel(planet)} />
              <PlanetDataRow
                label="Capacidad de accion"
                value={
                  planet.actionSummary.display?.queueActionStatusLabel
                  ?? formatConstructionAvailability(planet.actionSummary.queueActionStatus)
                }
              />
            </div>
          ) : (
            <p className="figma-panel-note">
              La cabina mostrara colonia, reservas, edificios y cola cuando exista
              un planeta seleccionado.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limites actuales</p>
              <h3>Seguridad operativa</h3>
            </div>
            <UiBadge tone="warn">Protecciones activas</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Solo puedes enviar una orden de construccion cuando la colonia esta lista y confirmas la accion.</li>
            <li>El cierre de obras vencidas sigue fuera de esta cabina y se gestiona por separado.</li>
            <li>Esta vista se centra en administracion colonial, no en combate ni maniobras espaciales.</li>
            <li>Los ids y notas tecnicas quedan guardados en el diagnostico secundario.</li>
          </ul>
        </UiCard>
      </div>

      {isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Cargando</p>
              <h3>Sincronizando planeta</h3>
            </div>
            <UiBadge>Cargando...</UiBadge>
          </div>
          <p>Consultando identidad planetaria, reservas, edificios y cola de construccion.</p>
        </UiCard>
      ) : null}

      {uiState && !planet && !error ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Sin colonia</p>
              <h3>No hay un planeta jugable en este contexto</h3>
            </div>
            <UiBadge tone="warn">Estado vacio</UiBadge>
          </div>
          <p>Esta civilizacion todavia no expone un planeta propio o el contexto no incluye un `planetId` valido.</p>
        </UiCard>
      ) : null}

      {planet ? (
        <>
          <UiCard className="panel planet-header-panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Cabina activa</p>
                <h3>{planet.planetName}</h3>
                <p>{formatPlanetOverviewLine(planet)}</p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={planet.isOwnedByRequestingCivilization ? "good" : "warn"}>
                  {formatPlanetOwnerLabel(planet)}
                </UiBadge>
                <UiBadge>
                  {planet.controlStatus
                    ? formatPlanetControlStatus(planet.controlStatus)
                    : "Sin control"}
                </UiBadge>
              </div>
            </div>

            <div className="selection-chip-row">
              {(uiState?.knownPlanets ?? []).map((item) => (
                <button
                  key={item.planetId}
                  type="button"
                  className={`selection-chip${
                    item.planetId === planet.planetId ? " selection-chip-active" : ""
                  }`}
                  onClick={() => void handlePlanetSelection(item.planetId)}
                >
                  {item.planetName} | {item.solarSystemName}
                </button>
              ))}
            </div>

            <div className="selection-chip-row">
              <Link
                className="selection-chip selection-chip-active"
                to={`/?civilizationId=${activeCivilizationId}&systemId=${planet.solarSystemId}&planetId=${planet.planetId}`}
              >
                Volver a Galaxia
              </Link>
              <Link
                className="selection-chip"
                to={`/fleets?civilizationId=${activeCivilizationId}&planetId=${planet.planetId}`}
              >
                Abrir Flotas
              </Link>
            </div>
          </UiCard>

          <div className="figma-two-column planet-overview-grid">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Panorama colonial</p>
                  <h3>Identidad y estado</h3>
                </div>
                <UiBadge>{formatPlanetShortReference(planet.planetId)}</UiBadge>
              </div>
              <div className="figma-data-list">
                <PlanetDataRow label="Sistema" value={planet.solarSystemName} />
                <PlanetDataRow label="Tipo" value={formatPlanetType(planet.planetType)} />
                <PlanetDataRow
                  label="Colonizacion"
                  value={formatColonizationStatus(planet.colonizationStatus)}
                />
                <PlanetDataRow label="Tamano" value={String(planet.size)} />
                <PlanetDataRow label="Orbita" value={`Slot ${planet.orbitalSlot}`} />
              </div>
            </UiCard>

            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Reservas y flujo</p>
                  <h3>Economia local</h3>
                </div>
                <UiBadge tone="resource">
                  {planet.stockpile.length > 0 ? `${planet.stockpile.length} reservas visibles` : "Sin reservas"}
                </UiBadge>
              </div>
              {planet.stockpile.length > 0 ? (
                <div className="figma-stat-grid">
                  {planet.stockpile.map((balance) => (
                    <div
                      key={`${planet.planetId}-${balance.resourceType}`}
                      className="figma-stat"
                    >
                      <strong>{balance.quantity}</strong>
                      <span>{formatResourceType(balance.resourceType)}</span>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="figma-panel-note">
                  No hay reservas visibles para este planeta dentro del contexto actual.
                </p>
              )}
              {planet.productionSummary ? (
                <div className="planet-inline-summary">
                  <PlanetDataRow
                    label="Produccion / h"
                    value={[
                      `Creditos ${planet.productionSummary.creditsPerHour}`,
                      `Metal ${planet.productionSummary.metalPerHour}`,
                      `Cristal ${planet.productionSummary.crystalPerHour}`,
                      `Gas ${planet.productionSummary.gasPerHour}`,
                    ].join(" | ")}
                  />
                  <PlanetDataRow
                    label="Multiplicador de investigacion"
                    value={`x${planet.productionSummary.researchMultiplier}`}
                  />
                </div>
              ) : (
                <p className="figma-panel-note">
                  Este planeta todavia no muestra un perfil de produccion disponible.
                </p>
              )}
            </UiCard>
          </div>

          <div className="figma-two-column planet-overview-grid">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Infraestructura</p>
                  <h3>Edificios actuales</h3>
                </div>
                <UiBadge>{planet.buildings.length} activos</UiBadge>
              </div>
              {planet.buildings.length > 0 ? (
                <div className="planet-building-groups">
                  {Object.entries(buildingsByCategory).map(([category, items]) => (
                    <section key={category} className="subpanel figma-subpanel">
                      <div className="figma-section-header">
                        <div>
                          <p className="eyebrow">Categoria</p>
                          <h4>{category}</h4>
                        </div>
                        <UiBadge>{items.length} edificios</UiBadge>
                      </div>
                      <div className="planet-building-grid">
                        {items.map((building) => (
                          <article
                            key={`${String(building.buildingType)}-${building.level}`}
                            className="subpanel figma-subpanel planet-building-card"
                          >
                            <strong>{building.display?.buildingTypeLabel ?? formatBuildingType(building.buildingType)}</strong>
                            <p className="figma-panel-note">
                              Nivel {building.level} | Huella {building.footprint}
                            </p>
                          </article>
                        ))}
                      </div>
                    </section>
                  ))}
                </div>
              ) : (
                <p className="figma-panel-note">
                  No hay edificios gestionables visibles para este planeta.
                </p>
              )}
            </UiCard>

            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Cola de construccion</p>
                  <h3>Progreso legible</h3>
                </div>
                <UiBadge tone={planet.constructionQueue.length > 0 ? "warn" : "good"}>
                  {planet.constructionQueue.length > 0
                    ? `${planet.constructionQueue.length} ordenes`
                    : "Sin cola"}
                </UiBadge>
              </div>
              {planet.constructionQueue.length > 0 ? (
                <div className="planet-queue-grid">
                  {planet.constructionQueue.map((item) => (
                    <section key={item.orderId} className="subpanel figma-subpanel">
                      <div className="figma-section-header">
                        <div>
                          <p className="eyebrow">Secuencia {item.sequence}</p>
                          <h4>{item.display?.buildingTypeLabel ?? formatBuildingType(item.buildingType)}</h4>
                        </div>
                        <UiBadge tone={item.isDue ? "warn" : "neutral"}>
                          {item.display?.statusLabel ?? formatQueueState(item)}
                        </UiBadge>
                      </div>
                      <div className="figma-data-list">
                        <PlanetDataRow
                          label="Accion"
                          value={`${item.display?.actionLabel ?? formatConstructionAction(item.action)} a nivel ${item.targetLevel}`}
                        />
                        <PlanetDataRow
                          label="Inicio"
                          value={formatDateTime(item.startsAtUtc)}
                        />
                        <PlanetDataRow
                          label="Fin"
                          value={formatDateTime(item.endsAtUtc)}
                        />
                        <PlanetDataRow label="Coste" value={formatCompactResourceCost(item.cost)} />
                      </div>
                      <p className="dev-meta">{formatCompactGuid(item.orderId)}</p>
                    </section>
                  ))}
                </div>
              ) : (
                <p className="figma-panel-note">No hay construcciones en cola.</p>
              )}
            </UiCard>
          </div>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Desarrollo disponible</p>
                <h3>Acciones de construccion protegidas</h3>
                <p>
                  Solo puedes preparar una orden cuando la colonia cumple las
                  condiciones actuales y la operacion sigue siendo segura.
                </p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={planet.actionSummary.queueActionStatus === "Available" ? "good" : "warn"}>
                  {planet.actionSummary.display?.queueActionStatusLabel
                    ?? formatConstructionAvailability(planet.actionSummary.queueActionStatus)}
                </UiBadge>
                <UiBadge tone="warn">
                  {planet.actionSummary.completeDueSupported
                    ? "Cierre disponible"
                    : planet.actionSummary.display?.completeDueActionStatusLabel ?? "No disponible en esta cabina"}
                </UiBadge>
              </div>
            </div>

            <div className="figma-data-list">
              <PlanetDataRow
                label="Estado de cola"
                value={planet.actionSummary.display?.queueActionReasonLabel ?? planet.actionSummary.queueActionReason}
              />
              <PlanetDataRow
                label="Construcciones vencidas"
                value={String(planet.actionSummary.dueConstructionCount)}
              />
              <PlanetDataRow
                label="Completar vencidas"
                value={planet.actionSummary.display?.completeDueActionReasonLabel ?? planet.actionSummary.completeDueActionReason}
              />
            </div>

            <div className="planet-action-groups">
              {Object.entries(actionsByCategory).map(([category, actions]) => (
                <section key={category} className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Categoria</p>
                      <h4>{category}</h4>
                    </div>
                    <UiBadge>{actions.length} opciones</UiBadge>
                  </div>
                  <div className="planet-action-grid">
                    {actions.map((action) => {
                      const actionKey = `${action.action}-${action.buildingType}`;
                      const isPrepared = preparedActionKey === actionKey;
                      const isAvailable = action.availabilityStatus === "Available";
                      const actionButtonLabel = formatConstructionActionButtonLabel(
                        action.availabilityStatus,
                        isPrepared,
                      );
                      const missingResources = formatMissingPlanetResources(planet.stockpile, action.cost);

                      return (
                        <article
                          key={actionKey}
                          className={`subpanel figma-subpanel planet-action-card${
                            isAvailable ? "" : " planet-action-card-blocked"
                          }`}
                        >
                          <div className="figma-section-header">
                            <div>
                              <p className="eyebrow">{action.display?.categoryLabel ?? formatBuildingCategory(action.category)}</p>
                              <h4>{action.display?.buildingTypeLabel ?? formatBuildingType(action.buildingType)}</h4>
                            </div>
                            <UiBadge tone={isAvailable ? "good" : "warn"}>
                              {action.display?.availabilityLabel ?? formatConstructionAvailability(action.availabilityStatus)}
                            </UiBadge>
                          </div>
                          <div className="planet-action-facts">
                            <div className="planet-action-stat">
                              <span>Orden</span>
                              <strong>{action.display?.actionLabel ?? formatConstructionAction(action.action)}</strong>
                            </div>
                            <div className="planet-action-stat">
                              <span>Nivel</span>
                              <strong>
                                {action.currentLevel > 0
                                  ? `${action.currentLevel} -> ${action.targetLevel}`
                                  : `Nuevo ${action.targetLevel}`}
                              </strong>
                            </div>
                            <div className="planet-action-stat">
                              <span>Duracion</span>
                              <strong>{formatDuration(action.estimatedDuration)}</strong>
                            </div>
                          </div>
                          <div className="planet-action-cost-block">
                            <span>Coste previsto</span>
                            <strong>{formatCompactResourceCost(action.cost)}</strong>
                          </div>
                          <p className="figma-panel-note planet-action-reason">
                            {action.availabilityStatus === "InsufficientResources" && missingResources
                              ? missingResources
                              : action.display?.availabilityReasonLabel ?? action.availabilityReason}
                          </p>
                          <div className="transfer-confirmation-actions">
                            <button
                              type="button"
                              className={isAvailable ? "" : "planet-action-button-blocked"}
                              onClick={() => {
                                if (!isAvailable) {
                                  return;
                                }

                                setPreparedActionKey(actionKey);
                                setHasConstructionAcknowledgement(false);
                                setConstructionFeedback(null);
                                setConstructionError(null);
                              }}
                              disabled={!isAvailable}
                            >
                              {actionButtonLabel}
                            </button>
                          </div>
                        </article>
                      );
                    })}
                  </div>
                </section>
              ))}
            </div>

            {preparedAction && preparedAction.availabilityStatus === "Available" ? (
              <section className="subpanel figma-subpanel transfer-confirmation-panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Confirmacion requerida</p>
                    <h4>{formatBuildingType(preparedAction.buildingType)}</h4>
                    <p>
                      Revisa coste, objetivo y duracion antes de enviar la orden a la
                      cola de construccion.
                    </p>
                  </div>
                  <UiBadge tone="warn">Confirmacion obligatoria</UiBadge>
                </div>
                <div className="figma-data-list">
                  <PlanetDataRow
                    label="Accion"
                    value={`${formatConstructionAction(preparedAction.action)} a nivel ${preparedAction.targetLevel}`}
                  />
                  <PlanetDataRow label="Coste" value={formatCompactResourceCost(preparedAction.cost)} />
                  <PlanetDataRow
                    label="Duracion"
                    value={formatDuration(preparedAction.estimatedDuration)}
                  />
                  <PlanetDataRow
                    label="Planeta"
                    value={planet.planetName}
                  />
                </div>
                <label className="confirmation-checkbox">
                  <input
                    type="checkbox"
                    checked={hasConstructionAcknowledgement}
                    onChange={(event) =>
                      setHasConstructionAcknowledgement(event.target.checked)
                    }
                  />
                  <span>Confirmo que quiero enviar esta orden de construccion</span>
                </label>
                <div className="transfer-confirmation-actions">
                  <button
                    type="button"
                    onClick={() => void handleConstructionSubmit()}
                    disabled={isSubmittingConstruction || !hasConstructionAcknowledgement}
                  >
                    {isSubmittingConstruction ? "Enviando..." : "Enviar orden"}
                  </button>
                </div>
              </section>
            ) : null}

            {constructionFeedback ? <p>{constructionFeedback}</p> : null}
            {constructionError ? <p className="error-text">{constructionError}</p> : null}
          </UiCard>

          <details className="technical-disclosure">
            <summary>
              <div>
                <p className="eyebrow">Diagnostico secundario</p>
                <strong>Ids, notas de soporte y lectura tecnica</strong>
              </div>
              <UiBadge tone="warn">Contraido por defecto</UiBadge>
            </summary>

            <div className="technical-disclosure-body">
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Metadatos tecnicos</p>
                    <h3>Lectura de soporte</h3>
                  </div>
                  <UiBadge>Soporte tecnico</UiBadge>
                </div>
                <div className="figma-detail-grid strategic-detail-grid">
                  <section className="subpanel figma-subpanel">
                    <div className="figma-data-list">
                      <PlanetDataRow label="Id planeta" value={formatCompactGuid(planet.planetId)} />
                      <PlanetDataRow
                        label="Id sistema"
                        value={formatCompactGuid(planet.diagnostics.solarSystemId)}
                      />
                      <PlanetDataRow
                        label="Id propietario"
                        value={formatCompactGuid(planet.diagnostics.ownerCivilizationId)}
                      />
                      <PlanetDataRow
                        label="Id planeta hogar"
                        value={formatCompactGuid(planet.diagnostics.homePlanetId)}
                      />
                    </div>
                  </section>
                  <section className="subpanel figma-subpanel">
                    <div className="figma-data-list">
                      <PlanetDataRow
                        label="Reserva persistida"
                        value={planet.diagnostics.hasResourceStockpile ? "Si" : "No"}
                      />
                      <PlanetDataRow
                        label="Perfil de produccion"
                        value={planet.diagnostics.hasProductionProfile ? "Si" : "No"}
                      />
                      <PlanetDataRow
                        label="Capacidad de edificios"
                        value={planet.diagnostics.hasBuildingCapacity ? "Si" : "No"}
                      />
                      <PlanetDataRow
                        label="Ordenes abiertas"
                        value={String(planet.diagnostics.openConstructionOrderCount)}
                      />
                    </div>
                    {planet.diagnostics.notes.length > 0 ? (
                      <ul className="stack-list compact-list">
                        {planet.diagnostics.notes.map((note) => (
                          <li key={note}>{note}</li>
                        ))}
                      </ul>
                    ) : null}
                  </section>
                  <section className="subpanel figma-subpanel">
                    <div className="figma-data-list">
                      <PlanetDataRow
                        label="Escuadras estacionadas"
                        value={String(planet.orbitalContext.stationedGroups)}
                      />
                      <PlanetDataRow
                        label="Salidas activas"
                        value={String(planet.orbitalContext.activeDepartures)}
                      />
                      <PlanetDataRow
                        label="Llegadas activas"
                        value={String(planet.orbitalContext.activeArrivals)}
                      />
                    </div>
                  </section>
                </div>
              </UiCard>
            </div>
          </details>
        </>
      ) : null}
    </section>
  );
}
