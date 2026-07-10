import type { PlanetCockpitDto } from "../api/planetTypes";
import { LiveQueueCountdown } from "./LiveQueueCountdown";
import {
  isOpenQueueStatus,
  isSpecialDefenseBuildingType,
  normalizeBuildingCategory,
  normalizeQueueStatus,
} from "../utils/enumNormalization";
import { formatBuildingType, formatConstructionQueuePhase } from "../utils/planetPresentation";
import { UiCard } from "./ui/UiCard";

export interface QueueSummaryItem {
  label: string;
  value: string;
  detail: string;
  endsAtUtc?: string;
  expireKey?: string;
  onExpire?: () => void | Promise<void>;
}

function isOpenConstructionQueueItem(item: PlanetCockpitDto["constructionQueue"][number]) {
  return isOpenQueueStatus(item.status);
}

function formatQueueItemLabel(item: PlanetCockpitDto["constructionQueue"][number]) {
  const buildingLabel = item.display?.buildingTypeLabel ?? formatBuildingType(item.buildingType);
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
      <p className="figma-panel-note">
        {summary.detail}
        {summary.endsAtUtc ? (
          <>
            {", "}
            <LiveQueueCountdown
              endsAtUtc={summary.endsAtUtc}
              expireKey={summary.expireKey}
              onExpire={summary.onExpire}
            />
          </>
        ) : null}
      </p>
    </section>
  );
}

export function QueueSummaryPanels({
  moduleSummaries = [],
  onQueueExpired,
  planet,
}: {
  moduleSummaries?: readonly QueueSummaryItem[];
  onQueueExpired?: () => void | Promise<void>;
  planet: PlanetCockpitDto;
}) {
  const openConstruction = planet.constructionQueue.filter(isOpenConstructionQueueItem);
  const buildingQueue = openConstruction.filter((item) => normalizeBuildingCategory(item.category) !== "Defense" && !isSpecialDefenseBuildingType(item.buildingType));
  const defenseQueue = openConstruction.filter((item) => normalizeBuildingCategory(item.category) === "Defense" || isSpecialDefenseBuildingType(item.buildingType));
  const movements = planet.orbitalContext.activeArrivals + planet.orbitalContext.activeDepartures;
  const summaries = [
    ...(buildingQueue.length > 0 ? [{
      label: "Construccion",
      value: buildingQueue.length === 1 ? "1 obra" : `${buildingQueue.length} obras`,
      detail: `${formatQueueItemLabel(buildingQueue[0])}: ${formatConstructionQueuePhase(normalizeQueueStatus(buildingQueue[0].status), buildingQueue[0].isDue)}`,
      endsAtUtc: buildingQueue[0].endsAtUtc,
      expireKey: buildingQueue[0].orderId,
      onExpire: onQueueExpired,
    }] : []),
    ...moduleSummaries,
    ...(defenseQueue.length > 0 ? [{
      label: "Defensas",
      value: defenseQueue.length === 1 ? "1 defensa" : `${defenseQueue.length} defensas`,
      detail: `${formatQueueItemLabel(defenseQueue[0])}: ${formatConstructionQueuePhase(normalizeQueueStatus(defenseQueue[0].status), defenseQueue[0].isDue)}`,
      endsAtUtc: defenseQueue[0].endsAtUtc,
      expireKey: defenseQueue[0].orderId,
      onExpire: onQueueExpired,
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
