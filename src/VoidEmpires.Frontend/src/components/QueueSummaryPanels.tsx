import type { PlanetCockpitDto } from "../api/planetTypes";
import { formatConstructionQueuePhase } from "../utils/planetPresentation";
import { UiCard } from "./ui/UiCard";
export function QueueSummaryPanels({ planet }: { planet: PlanetCockpitDto }) {
  const openConstruction = planet.constructionQueue.filter((item) => item.status === "Pending" || item.status === "Active");
  const movements = planet.orbitalContext.activeArrivals + planet.orbitalContext.activeDepartures;
  const summaries = [
    {
      label: "Construccion",
      value: openConstruction.length > 0 ? `${openConstruction.length} activas` : "Cola libre",
      detail: openConstruction[0] ? formatConstructionQueuePhase(openConstruction[0].status, openConstruction[0].isDue) : "Sin orden abierta.",
    },
    { label: "Investigacion", value: "Sin cola visible", detail: "No hay proyectos activos en este resumen." },
    { label: "Astillero", value: "Sin cola visible", detail: "Produccion orbital pendiente de panel propio." },
    { label: "Defensas", value: "Sin cola visible", detail: "Preparacion defensiva sin cierre automatico." },
    {
      label: "Flotas",
      value: movements > 0 ? `${movements} movimientos` : "Sin movimiento",
      detail: planet.orbitalContext.stationedGroups > 0 ? `${planet.orbitalContext.stationedGroups} escuadras estacionadas.` : "Preparada para ordenes futuras.",
    },
  ];

  return (
    <UiCard className="panel home-queue-panel">
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">Actividad</p>
          <h3>Colas y movimiento</h3>
        </div>
      </div>
      <div className="home-queue-grid">
        {summaries.map((summary) => (
          <section className="subpanel home-queue-card" key={summary.label}>
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">{summary.label}</p>
                <h4>{summary.value}</h4>
              </div>
            </div>
            <p className="figma-panel-note">{summary.detail}</p>
          </section>
        ))}
      </div>
    </UiCard>
  );
}
