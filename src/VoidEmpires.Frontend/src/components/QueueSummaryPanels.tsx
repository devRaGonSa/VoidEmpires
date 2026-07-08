import type { PlanetCockpitDto } from "../api/planetTypes";
import { formatConstructionQueuePhase } from "../utils/planetPresentation";
import { UiCard } from "./ui/UiCard";

interface QueueSummaryItem {
  label: string;
  value: string;
  detail: string;
}

function isOpenConstructionQueueItem(item: PlanetCockpitDto["constructionQueue"][number]) {
  return item.status === "Pending" || item.status === "Active";
}

function formatQueueItemLabel(item: PlanetCockpitDto["constructionQueue"][number]) {
  const buildingLabel = item.display?.buildingTypeLabel ?? `${item.buildingType}`;
  return `${buildingLabel} nivel ${item.targetLevel}`;
}

function QueueSummaryPanel({ summary }: { summary: QueueSummaryItem }) {
  return (
    <section className="subpanel home-queue-card" key={summary.label}>
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">{summary.label}</p>
          <h4>{summary.value}</h4>
        </div>
      </div>
      <p className="figma-panel-note">{summary.detail}</p>
    </section>
  );
}

export function QueueSummaryPanels({ planet }: { planet: PlanetCockpitDto }) {
  const openConstruction = planet.constructionQueue.filter(isOpenConstructionQueueItem);
  const buildingQueue = openConstruction.filter((item) => String(item.category) !== "Defense");
  const defenseQueue = openConstruction.filter((item) => String(item.category) === "Defense");
  const movements = planet.orbitalContext.activeArrivals + planet.orbitalContext.activeDepartures;
  const summaries = [
    {
      label: "Construccion",
      value: buildingQueue.length > 0 ? `${buildingQueue.length} obras` : "Sin obras en cola.",
      detail: buildingQueue[0]
        ? `${formatQueueItemLabel(buildingQueue[0])}: ${formatConstructionQueuePhase(buildingQueue[0].status, buildingQueue[0].isDue)}`
        : "Sin obras en cola.",
    },
    {
      label: "Investigacion",
      value: "Sin investigacion activa.",
      detail: "Sin investigacion activa.",
    },
    {
      label: "Astillero",
      value: "Sin produccion orbital.",
      detail: "Sin produccion orbital.",
    },
    {
      label: "Defensas",
      value: defenseQueue.length > 0 ? `${defenseQueue.length} defensas` : "Sin defensas en cola.",
      detail: defenseQueue[0]
        ? `${formatQueueItemLabel(defenseQueue[0])}: ${formatConstructionQueuePhase(defenseQueue[0].status, defenseQueue[0].isDue)}`
        : "Sin defensas en cola.",
    },
    {
      label: "Flotas",
      value: movements > 0 ? `${movements} movimientos` : "Sin movimientos de flota.",
      detail: movements > 0
        ? `${planet.orbitalContext.activeDepartures} salidas y ${planet.orbitalContext.activeArrivals} llegadas.`
        : "Sin movimientos de flota.",
    },
  ] satisfies QueueSummaryItem[];

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
          <QueueSummaryPanel key={summary.label} summary={summary} />
        ))}
      </div>
    </UiCard>
  );
}
