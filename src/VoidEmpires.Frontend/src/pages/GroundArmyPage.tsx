import { FormEvent, useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { fetchGroundArmyUiState } from "../api/groundArmyApi";
import { CockpitHero } from "../components/CockpitHero";
import { GroundArmyCatalogCard } from "../components/GroundArmyCatalogCard";
import { PlanetDataRow } from "../components/PlanetModuleLayout";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatGroundArmyRequestFailure } from "../utils/groundArmyPresentation";
import { groupGroundOptionsByCategory, mapGroundArmyUiStateToViewModel, selectRecommendedGroundArmyAction } from "../utils/groundArmyViewModel";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import { isSuspiciousCabinContext } from "../utils/routeUrls";
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
  if (!viewModel) return "Abrir vista terrestre";
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
        description="Guarnicion local, capacidad de entrenamiento y unidades terrestres disponibles para la colonia seleccionada."
        developmentNoteLabel="Activacion pendiente"
        developmentNote="Entrenamiento directo, invasion y combate terrestre siguen pendientes de activacion. Esta vista muestra catalogo y preparacion sin ejecutar ordenes nuevas."
        badges={
          <>
            <UiBadge>Guarnicion y entrenamiento</UiBadge>
            <UiBadge>{cockpitStatusLabels.preparation}</UiBadge>
            <UiBadge tone="warn">Combate pendiente</UiBadge>
          </>
        }
      />

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header"><div><p className="eyebrow">Ejercito terrestre</p><h3>Abrir entrenamiento terrestre</h3></div><UiBadge>Colonia activa</UiBadge></div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field"><span>Id de civilizacion</span><input type="text" value={civilizationIdInput} onChange={(event) => setCivilizationIdInput(event.target.value)} placeholder="00000000-0000-0000-0000-000000000000" spellCheck={false} /></label>
            <label className="field"><span>Id de planeta opcional</span><input type="text" value={planetIdInput} onChange={(event) => setPlanetIdInput(event.target.value)} placeholder="40000000-0000-0000-0000-000000000000" spellCheck={false} /></label>
            <button type="submit" disabled={isLoading}>{isLoading ? "Cargando..." : "Abrir ejercito"}</button>
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
          ) : <p className="figma-panel-note">La vista mostrara entrenamiento terrestre, estructuras y guarnicion cuando los datos sean validos.</p>}
        </UiCard>
      </div>

      <UiCard className="panel">
        <div className="figma-section-header"><div><p className="eyebrow">Estado de preparacion</p><h3>Dashboard terrestre</h3></div><UiBadge tone={groundArmy?.actionAvailability.enqueueSupported ? "good" : "warn"}>{groundArmy?.actionAvailability.enqueueStatusLabel ?? "Pendiente"}</UiBadge></div>
        {groundArmy ? (
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel"><div className="figma-data-list">
              <PlanetDataRow label="Población total" value={groundArmy.population ? `${groundArmy.population.totalPopulation}` : "No disponible"} />
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
        ) : <p className="figma-panel-note">Todavia no hay datos terrestres visibles. La vista mantiene un estado honesto hasta cargar una colonia valida.</p>}
      </UiCard>

      {groundArmy ? (
        <UiCard className="panel">
          <div className="figma-section-header"><div><p className="eyebrow">Catalogo terrestre</p><h3>Unidades, estructuras y preparacion</h3><p>Las tarjetas muestran entrenamiento terrestre y requisitos, no combate activo.</p></div><UiBadge tone={recommendedOption?.statusKey === "Available" ? "good" : "warn"}>{recommendedOption?.label ?? "Sin recomendacion"}</UiBadge></div>
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
          <div className="ground-army-catalog-groups">
            {optionGroups.map((group) => (
              <section key={group.key} className="subpanel figma-subpanel">
                <div className="figma-section-header"><div><p className="eyebrow">Categoria</p><h4>{group.label}</h4></div><UiBadge>{group.options.length} tarjetas</UiBadge></div>
                <div className="readiness-grid ground-army-catalog-grid">
                  {group.options.map((option) => (
                    <GroundArmyCatalogCard key={option.assetType} option={option} />
                  ))}
                </div>
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

      {isSuspiciousContext ? (
        <UiCard className="panel"><div className="figma-section-header"><div><p className="eyebrow">Contexto sospechoso</p><h3>El identificador de civilizacion no parece valido para esta vista.</h3></div><UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge></div><p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p></UiCard>
      ) : null}

    </section>
  );
}
