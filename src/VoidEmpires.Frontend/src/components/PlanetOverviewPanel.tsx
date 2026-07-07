import type { PlanetCockpitDto } from "../api/planetTypes";
import { formatPlanetOverviewLine } from "../utils/planetPresentation";
import { formatResourceLabel } from "../utils/resourceDisplay";
import { PlaceholderAsset } from "./PlaceholderAsset";
import { UiCard } from "./ui/UiCard";
function formatNumber(value: number) {
  return new Intl.NumberFormat("es-ES", { maximumFractionDigits: 1 }).format(value);
}

export function PlanetOverviewPanel({ civilizationLabel, planet }: { civilizationLabel: string; planet: PlanetCockpitDto }) {
  const capacity = planet.buildingCapacity;

  return (
    <UiCard className="panel home-planet-panel">
      <PlaceholderAsset
        className="home-planet-visual"
        kind="civilization"
        label={planet.planetName}
        typeLabel="Planeta actual"
        detail={formatPlanetOverviewLine(planet)}
        imageKey={planet.planetType}
      />
      <div className="home-planet-facts">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">{civilizationLabel}</p>
            <h3>{planet.planetName}</h3>
            <p>{planet.solarSystemName} - Orbita {planet.orbitalSlot}</p>
          </div>
        </div>
        <dl className="figma-stat-grid">
          {capacity ? (
            <div className="figma-data-row">
              <dt>Campos</dt>
              <dd><strong>{capacity.usedCapacity}/{capacity.totalAvailableCapacity}</strong></dd>
            </div>
          ) : null}
          {planet.productionSummary ? (
            <div className="figma-data-row">
              <dt>Produccion</dt>
              <dd><strong>+{formatNumber(planet.productionSummary.metalPerHour + planet.productionSummary.crystalPerHour + planet.productionSummary.gasPerHour)}/h</strong></dd>
            </div>
          ) : null}
          {planet.stockpile.slice(0, 4).map((resource) => (
            <div className="figma-data-row" key={String(resource.resourceType)}>
              <dt>{formatResourceLabel(resource.resourceType)}</dt>
              <dd><strong>{formatNumber(resource.quantity)}</strong></dd>
            </div>
          ))}
        </dl>
      </div>
    </UiCard>
  );
}
