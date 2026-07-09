import type { PlanetCockpitDto } from "../api/planetTypes";
import { formatConstructionQueuePhase } from "../utils/planetPresentation";
import { UiCard } from "./ui/UiCard";

export interface QueueSummaryItem {
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

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
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

export function QueueSummaryPanels({
  moduleSummaries = [],
  planet,
}: {
  moduleSummaries?: readonly QueueSummaryItem[];
  planet: PlanetCockpitDto;
}) {
  const openConstruction = planet.constructionQueue.filter(isOpenConstructionQueueItem);
  const buildingQueue = openConstruction.filter((item) => String(item.category) !== "Defense");
  const defenseQueue = openConstruction.filter((item) => String(item.category) === "Defense");
  const movements = planet.orbitalContext.activeArrivals + planet.orbitalContext.activeDepartures;
  const summaries = [
    ...(buildingQueue.length > 0 ? [{
      label: "Construccion",
      value: buildingQueue.length === 1 ? "1 obra" : `${buildingQueue.length} obras`,
      detail: `${formatQueueItemLabel(buildingQueue[0])}: ${formatConstructionQueuePhase(buildingQueue[0].status, buildingQueue[0].isDue)}, cierre ${formatDateTime(buildingQueue[0].endsAtUtc)}`,
    }] : []),
    ...moduleSummaries,
    ...(defenseQueue.length > 0 ? [{
      label: "Defensas",
      value: defenseQueue.length === 1 ? "1 defensa" : `${defenseQueue.length} defensas`,
      detail: `${formatQueueItemLabel(defenseQueue[0])}: ${formatConstructionQueuePhase(defenseQueue[0].status, defenseQueue[0].isDue)}, cierre ${formatDateTime(defenseQueue[0].endsAtUtc)}`,
    }] : []),
    ...(movements > 0 ? [{
      label: "Flotas",
      value: movements === 1 ? "1 movimiento" : `${movements} movimientos`,
      detail: `${planet.orbitalContext.activeDepartures} salidas y ${planet.orbitalContext.activeArrivals} llegadas.`,
    }] : []),
  ] satisfies QueueSummaryItem[];

  if (summaries.length === 0) {
    return null;
  }

  return (
    <UiCard className="panel home-queue-panel">
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">Actividad</p>
          <h3>Colas y movimiento</h3>
        </div>
      </div>
      <div className="home-queue-grid">
        {summaries.map((summary, index) => (
          <QueueSummaryPanel key={`${summary.label}-${index}`} summary={summary} />
        ))}
      </div>
    </UiCard>
  );
}
