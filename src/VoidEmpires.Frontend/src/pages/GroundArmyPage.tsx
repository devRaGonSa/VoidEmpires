import { FormEvent, useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchGroundArmyUiState } from "../api/groundArmyApi";
import { CockpitHero } from "../components/CockpitHero";
import { PageContextStrip } from "../components/PageContextStrip";
import { PlanetDataRow } from "../components/PlanetModuleLayout";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatGroundArmyRequestFailure } from "../utils/groundArmyPresentation";
import { groupGroundOptionsByCategory, mapGroundArmyUiStateToViewModel, selectRecommendedGroundArmyAction } from "../utils/groundArmyViewModel";
import { cockpitNavigationLabels, cockpitStatusLabels } from "../utils/cockpitStatus";
import { buildConstructionUrl, buildDefensesUrl, buildFleetsUrl, buildGalaxyUrl, buildPlanetUrl, isSuspiciousCabinContext } from "../utils/routeUrls";
import { formatCompactGuid } from "../utils/domainPresentation";

function getGroundPosture(viewModel: ReturnType<typeof mapGroundArmyUiStateToViewModel>["groundArmy"]) {
  if (!viewModel) return "Lectura terrestre preparada";
  if (!viewModel.isOwnedByRequestingCivilization) return "Observacion externa";
  if (viewModel.readinessSummary.totalGarrisonQuantity > 0) return "Guarnicion terrestre disponible";
  if (viewModel.readinessSummary.availableOptionCount > 0) return "Entrenamiento listo";
  if (viewModel.readinessSummary.queueItemCount > 0) return "Preparacion en cola";
  return "Preparacion terrestre inicial";
}

function getRecommendedNextStep(viewModel: ReturnType<typeof mapGroundArmyUiStateToViewModel>["groundArmy"]) {
  if (!viewModel) return "Cargar contexto terrestre";
  if (!viewModel.isOwnedByRequestingCivilization) return "Volver a una colonia propia";
  if (viewModel.readinessSummary.queueItemCount > 0) return "Revisar cola terrestre";
  if (viewModel.actionAvailability.enqueueSupported) return "Preparar entrenamiento";
  if (viewModel.readinessSummary.blockedOptionCount > 0) return "Resolver bloqueo visible";
  return "Revisar estructuras y reservas";
}

function getResourcePressureSummary(viewModel: ReturnType<typeof mapGroundArmyUiStateToViewModel>["groundArmy"]) {
  if (!viewModel || viewModel.stockpile.length === 0) return "Sin reservas visibles";
  return [...viewModel.stockpile].sort((left, right) => right.quantity - left.quantity).slice(0, 3).map((entry) => `${entry.label} ${entry.quantity}`).join(" | ");
}

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed) ? "No disponible" : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

export function GroundArmyPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<ReturnType<typeof mapGroundArmyUiStateToViewModel> | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const groundArmy = uiState?.groundArmy ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const posture = getGroundPosture(groundArmy);
  const recommendedNextStep = getRecommendedNextStep(groundArmy);
  const resourcePressureSummary = getResourcePressureSummary(groundArmy);
  const optionGroups = groupGroundOptionsByCategory(groundArmy?.catalog ?? []);
  const recommendedOption = selectRecommendedGroundArmyAction(groundArmy?.catalog ?? []);

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
        const response = await fetchGroundArmyUiState(queryCivilizationId, queryPlanetId || undefined);
        if (!response.succeeded || !response.uiState) {
          const feedback = formatGroundArmyRequestFailure(response.errors[0] ?? null);
          setUiState(null);
          setError(feedback.primaryMessage);
          return;
        }

        setUiState(mapGroundArmyUiStateToViewModel(response.uiState));
      } catch (requestError) {
        const feedback = formatGroundArmyRequestFailure(requestError instanceof Error ? requestError.message : null);
        setUiState(null);
        setError(feedback.primaryMessage);
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
      return;
    }

    const nextParams = new URLSearchParams({ civilizationId: trimmedCivilizationId });
    const trimmedPlanetId = planetIdInput.trim();
    if (trimmedPlanetId) nextParams.set("planetId", trimmedPlanetId);
    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Ejercito de Tierra v1"
        title="Ejercito de Tierra"
        description="El mando terrestre prioriza guarnicion local, capacidad de entrenamiento y preparacion defensiva antes de activar operaciones de superficie."
        developmentNoteLabel="Activacion pendiente"
        developmentNote="Entrenamiento directo, invasion y combate terrestre siguen pendientes de activacion. Esta vista conserva la lectura de mando sin ejecutar ordenes nuevas."
        badges={
          <>
            <UiBadge>Guarnicion y entrenamiento</UiBadge>
            <UiBadge>{cockpitStatusLabels.preparation}</UiBadge>
            <UiBadge tone="warn">Combate pendiente</UiBadge>
          </>
        }
      />

      {queryCivilizationId ? (
        <PageContextStrip
          eyebrow="Cabina terrestre"
          title={groundArmy?.planetName ?? "Preparacion terrestre"}
          purpose="Guarnicion, poblacion, estructuras y opciones de preparacion visibles sin activar reclutamiento directo, combate ni invasion."
          statusLabel={posture}
          statusTone={groundArmy?.isOwnedByRequestingCivilization ? "good" : "warn"}
          contextItems={[
            { label: "Civilizacion", value: formatCompactGuid(activeCivilizationId) },
            {
              label: "Planeta",
              value: groundArmy?.planetName ?? formatCompactGuid(selectedPlanetId) ?? "Sin planeta enfocado",
              detail: groundArmy?.solarSystemName ?? undefined,
            },
            {
              label: "Control",
              value: groundArmy?.controlStatusLabel ?? "Sin lectura",
              detail: recommendedNextStep,
            },
            {
              label: "Guarnicion",
              value: groundArmy ? `${groundArmy.readinessSummary.totalGarrisonQuantity} unidades` : "Sin lectura",
              detail: groundArmy ? `${groundArmy.readinessSummary.garrisonUnitTypes} tipos` : "Carga pendiente",
            },
          ]}
          resourceItems={[
            { label: "Preparacion", value: cockpitStatusLabels.preparation, tone: "good" },
            { label: "Entrenamiento", value: "Pendiente de activacion", tone: "warn" },
            { label: "Combate", value: "Pendiente de activacion", tone: "neutral" },
          ]}
          primaryAction={
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>
                Abrir Planeta
              </Link>
              <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
                Abrir Construccion
              </Link>
              <Link className="selection-chip" to={buildDefensesUrl(activeCivilizationId, selectedPlanetId)}>
                Abrir Defensas
              </Link>
            </div>
          }
        />
      ) : null}

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header"><div><p className="eyebrow">Contexto</p><h3>Cargar mando terrestre</h3></div><UiBadge>Consulta de mando</UiBadge></div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field"><span>Id de civilizacion</span><input type="text" value={civilizationIdInput} onChange={(event) => setCivilizationIdInput(event.target.value)} placeholder="00000000-0000-0000-0000-000000000000" spellCheck={false} /></label>
            <label className="field"><span>Id de planeta opcional</span><input type="text" value={planetIdInput} onChange={(event) => setPlanetIdInput(event.target.value)} placeholder="40000000-0000-0000-0000-000000000000" spellCheck={false} /></label>
            <button type="submit" disabled={isLoading}>{isLoading ? "Cargando..." : "Abrir mando"}</button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header"><div><p className="eyebrow">Estado actual</p><h3>Resumen terrestre</h3></div><UiBadge>{posture}</UiBadge></div>
          {groundArmy ? (
            <div className="figma-data-list">
              <PlanetDataRow label="Planeta" value={groundArmy.planetName} />
              <PlanetDataRow label="Sistema" value={groundArmy.solarSystemName} />
              <PlanetDataRow label="Control" value={groundArmy.controlStatusLabel ?? "Sin control"} />
              <PlanetDataRow label="Siguiente paso" value={recommendedNextStep} />
            </div>
          ) : <p className="figma-panel-note">La cabina mostrara preparacion terrestre, estructuras y guarnicion cuando el contexto sea valido.</p>}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header"><div><p className="eyebrow">Limite de la cabina</p><h3>Preparacion terrestre</h3></div><UiBadge tone="warn">{cockpitStatusLabels.preparation}</UiBadge></div>
          <ul className="stack-list strategic-rules-list">
            <li>Resume fuerzas terrestres, guarnicion y preparacion local.</li>
            <li>Construccion mantiene edificios militares y Defensas mantiene proteccion planetaria.</li>
            <li>Flotas mantiene movimiento orbital y transporte.</li>
            <li>Invasion, combate y ocupacion siguen pendientes de activacion.</li>
          </ul>
        </UiCard>
      </div>

      <UiCard className="panel">
        <div className="figma-section-header"><div><p className="eyebrow">Estado de preparacion</p><h3>Dashboard terrestre</h3></div><UiBadge tone={groundArmy?.actionAvailability.enqueueSupported ? "good" : "warn"}>{groundArmy?.actionAvailability.enqueueStatusLabel ?? "Pendiente"}</UiBadge></div>
        {groundArmy ? (
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel"><div className="figma-data-list">
              <PlanetDataRow label="Poblacion total" value={groundArmy.population ? `${groundArmy.population.totalPopulation}` : "No disponible"} />
              <PlanetDataRow label="Reclutable base" value={groundArmy.population ? `${groundArmy.population.baseRecruitablePopulation}` : "No disponible"} />
              <PlanetDataRow label="Capacidad terrestre" value={groundArmy.population ? `${groundArmy.population.totalGroundCapacity}` : "No disponible"} />
              <PlanetDataRow label="Opciones visibles" value={`${groundArmy.catalog.length}`} />
            </div></section>
            <section className="subpanel figma-subpanel"><div className="figma-data-list">
              <PlanetDataRow label="Postura" value={posture} />
              <PlanetDataRow label="Estructuras" value={`${groundArmy.readinessSummary.structureCount}`} />
              <PlanetDataRow label="Tipos en guarnicion" value={`${groundArmy.readinessSummary.garrisonUnitTypes}`} />
              <PlanetDataRow label="Guarnicion" value={`${groundArmy.readinessSummary.totalGarrisonQuantity} unidades`} />
              <PlanetDataRow label="Bloqueadas" value={`${groundArmy.readinessSummary.blockedOptionCount}`} />
            </div></section>
            <section className="subpanel figma-subpanel"><div className="figma-data-list">
              <PlanetDataRow label="Disponibles" value={`${groundArmy.readinessSummary.availableOptionCount}`} />
              <PlanetDataRow label="Cola visible" value={`${groundArmy.readinessSummary.queueItemCount}`} />
              <PlanetDataRow label="Presion de recursos" value={resourcePressureSummary} />
              <PlanetDataRow label="Operacion terrestre" value="Combate e invasion pendientes de activacion." />
            </div></section>
          </div>
        ) : <p className="figma-panel-note">Todavia no hay datos terrestres visibles. El mando mantiene un estado honesto hasta cargar una colonia valida.</p>}
      </UiCard>

      {groundArmy ? (
        <UiCard className="panel">
          <div className="figma-section-header"><div><p className="eyebrow">Catalogo terrestre</p><h3>Unidades, estructuras y preparacion</h3><p>Las tarjetas muestran preparacion terrestre y continuidad de mando, no combate activo.</p></div><UiBadge tone={recommendedOption?.statusKey === "Available" ? "good" : "warn"}>{recommendedOption?.label ?? "Sin recomendacion"}</UiBadge></div>
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header"><div><p className="eyebrow">Guarnicion</p><h4>Unidades visibles</h4></div><UiBadge>{groundArmy.garrison.length} tipos</UiBadge></div>
              <ul className="stack-list compact-list">
                {groundArmy.garrison.length > 0 ? groundArmy.garrison.map((unit) => <li key={unit.assetType}>{unit.label}: {unit.quantity} | {unit.roleLabel}</li>) : <li>Sin unidades de guarnicion registradas para esta colonia.</li>}
              </ul>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header"><div><p className="eyebrow">Estructuras</p><h4>Soporte terrestre</h4></div><UiBadge>{groundArmy.structures.length} filas</UiBadge></div>
              <ul className="stack-list compact-list">
                {groundArmy.structures.length > 0 ? groundArmy.structures.map((structure) => <li key={`${structure.buildingType}-${structure.level}`}>{structure.label} | {structure.categoryLabel} | Nivel {structure.level}</li>) : <li>Gestionar desde Construccion si la colonia aun no tiene infraestructura terrestre.</li>}
              </ul>
            </section>
          </div>
          <div className="figma-section-header module-boundary-spacer"><div><p className="eyebrow">Opciones agrupadas</p><h4>Catalogo de preparacion</h4></div></div>
          <div className="readiness-grid">
            {optionGroups.map((group) => (
              <section key={group.key} className="subpanel figma-subpanel">
                <div className="figma-section-header"><div><p className="eyebrow">Categoria</p><h4>{group.label}</h4></div><UiBadge>{group.options.length} tarjetas</UiBadge></div>
                <ul className="stack-list compact-list">
                  {group.options.map((option) => (
                    <li key={option.assetType}>
                      {option.label} | {option.statusLabel} | {option.reasonLabel} | {option.requirementLabel} | {option.resourceScopeLabel} | Coste {option.estimatedCostLabel} | Duracion {option.estimatedDurationLabel}{option.missingLabel ? ` | ${option.missingLabel}` : ""}
                    </li>
                  ))}
                </ul>
              </section>
            ))}
          </div>
        </UiCard>
      ) : null}

      {groundArmy ? (
        <UiCard className="panel">
          <div className="figma-section-header"><div><p className="eyebrow">Cola terrestre</p><h3>Ordenes y cierre pendiente</h3><p>La cola muestra el estado de entrenamiento sin completar ordenes automaticamente.</p></div><UiBadge tone={groundArmy.queue.length > 0 ? "resource" : "neutral"}>{groundArmy.queue.length > 0 ? `${groundArmy.queue.length} visibles` : "Sin cola"}</UiBadge></div>
          {groundArmy.queue.length > 0 ? (
            <ul className="stack-list compact-list">
              {groundArmy.queue.map((item) => (
                <li key={item.orderId}>
                  {item.label} | {item.statusLabel} | Inicio {formatDateTime(item.startsAtUtc)} | Fin {formatDateTime(item.endsAtUtc)}{item.isDue ? " | Pendiente de cierre seguro" : ""}
                </li>
              ))}
            </ul>
          ) : (
            <p className="figma-panel-note">No hay ordenes terrestres en cola.</p>
          )}
          <div className="figma-section-header module-boundary-spacer"><div><p className="eyebrow">Completar vencidas</p><h4>{cockpitStatusLabels.safePlaceholder}</h4></div><UiBadge tone="warn">{groundArmy.actionAvailability.completeDueStatusLabel}</UiBadge></div>
          <p className="figma-panel-note">{groundArmy.actionAvailability.completeDueReason}. La cola terrestre sigue visible, pero el cierre por planeta permanece pendiente de activacion.</p>
        </UiCard>
      ) : null}

      {groundArmy && recommendedOption ? (
        <UiCard className="panel">
          <div className="figma-section-header"><div><p className="eyebrow">Preparacion recomendada</p><h3>Orden pendiente o bloqueo visible</h3><p>El mando muestra la mejor preparacion visible sin confirmar entrenamiento ni saltarse Construccion.</p></div><UiBadge tone={recommendedOption.statusKey === "Available" ? "good" : "warn"}>{recommendedOption.statusLabel}</UiBadge></div>
          <div className="figma-data-list">
            <PlanetDataRow label="Preparacion" value={recommendedOption.label} />
            <PlanetDataRow label="Requisito" value={recommendedOption.requirementLabel} />
            <PlanetDataRow label="Coste" value={recommendedOption.estimatedCostLabel} />
            <PlanetDataRow label="Duracion" value={recommendedOption.estimatedDurationLabel} />
          </div>
          <div className="selection-chip-row">
            <Link className="planet-action-button-secondary planet-action-handoff" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>Abrir Construccion</Link>
            <Link className="planet-action-button-secondary planet-action-handoff" to={buildDefensesUrl(activeCivilizationId, selectedPlanetId)}>Abrir Defensas</Link>
          </div>
          <p className="figma-panel-note">
            {recommendedOption.statusKey === "Available"
              ? "La opcion esta preparada para revision, pero la confirmacion directa sigue pendiente hasta que exista una via terrestre dedicada."
              : `La accion permanece bloqueada: ${recommendedOption.reasonLabel}.`}
          </p>
          <span className="planet-action-handoff-message">Entrenamiento directo pendiente de activacion.</span>
        </UiCard>
      ) : null}

      {isSuspiciousContext ? (
        <UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Contexto sospechoso</p><h3>El identificador de civilizacion no parece valido para esta cabina.</h3></div><UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge></div><p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p></UiCard>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header"><div><p className="eyebrow">Navegacion</p><h3>{cockpitNavigationLabels.relatedCabins}</h3></div><UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge></div>
        <div className="readiness-grid">
          <section className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">Planeta</p><h4>Volver al resumen</h4></div></div><p className="figma-panel-note">Usa Planeta para ver el contexto general de la colonia antes de entrar en una cabina especializada.</p><Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>{cockpitNavigationLabels.returnToPlanet}</Link></section>
          <section className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">Construccion</p><h4>Infraestructura militar</h4></div></div><p className="figma-panel-note">Barracones, academia y logistica terrestre siguen anclados a Construccion cuando pertenecen a obra e infraestructura.</p><Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>{cockpitNavigationLabels.openConstruction}</Link></section>
          <section className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">Defensas</p><h4>Proteccion planetaria</h4></div></div><p className="figma-panel-note">Defensas mantiene cobertura, fortificacion y proteccion. Ground Army no resuelve escudos ni defensa activa.</p><Link className="selection-chip" to={buildDefensesUrl(activeCivilizationId, selectedPlanetId)}>{cockpitNavigationLabels.openDefenses}</Link></section>
          <section className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">Flotas</p><h4>Movimiento orbital</h4></div></div><p className="figma-panel-note">Flotas mantiene movimiento, transferencias y contexto orbital. Ejercito de Tierra no lanza transporte ni invasion.</p><Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>{cockpitNavigationLabels.openFleets}</Link></section>
          <section className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">Galaxia</p><h4>Contexto estrategico</h4></div></div><p className="figma-panel-note">Galaxia sigue siendo lectura estrategica de alto nivel. Esta cabina solo prepara la disposicion terrestre local.</p><Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, undefined, selectedPlanetId ?? undefined)}>{cockpitNavigationLabels.returnToGalaxy}</Link></section>
        </div>
      </UiCard>
    </section>
  );
}
