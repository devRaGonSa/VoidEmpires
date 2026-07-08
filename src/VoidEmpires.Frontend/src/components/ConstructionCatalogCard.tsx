import type { PlanetConstructionActionDto, PlanetResourceBalanceDto } from "../api/planetTypes";
import { UiBadge } from "./ui/UiBadge";
import {
  formatBuildingType,
  formatCompactResourceCost,
  formatConstructionActionButtonLabel,
  formatConstructionAvailability,
  formatConstructionBuildingEffect,
  formatMissingPlanetResources,
} from "../utils/planetPresentation";

interface ConstructionCatalogCardProps {
  action: PlanetConstructionActionDto;
  actionKey: string;
  categoryLabel: string;
  isPrepared: boolean;
  stockpile: PlanetResourceBalanceDto[];
  onPrepare: (actionKey: string) => void;
}

function formatDuration(value: string) {
  const match = /^(\d+)\.(\d{2}):(\d{2}):(\d{2})$/.exec(value) ?? /^(\d{2}):(\d{2}):(\d{2})$/.exec(value);
  if (!match) {
    return value;
  }

  if (match.length === 5) {
    const [, days, hours, minutes] = match;
    return `${Number(days)}d ${Number(hours)}h ${Number(minutes)}m`;
  }

  const [, hours, minutes] = match;
  return `${Number(hours)}h ${Number(minutes)}m`;
}

export function ConstructionCatalogCard({
  action,
  actionKey,
  categoryLabel,
  isPrepared,
  stockpile,
  onPrepare,
}: ConstructionCatalogCardProps) {
  const isAvailable = action.availabilityStatus === "Available";
  const buildingLabel = action.display?.buildingTypeLabel ?? formatBuildingType(action.buildingType);
  const availabilityLabel =
    action.display?.availabilityLabel ?? formatConstructionAvailability(action.availabilityStatus);
  const missingResources = formatMissingPlanetResources(stockpile, action.cost);
  const requirementLabel = action.availabilityStatus === "InsufficientResources" && missingResources
    ? missingResources
    : action.display?.availabilityReasonLabel ?? action.availabilityReason;
  const currentLevelLabel = action.currentLevel > 0
    ? `Nivel ${action.currentLevel}`
    : "Sin construir";
  const actionButtonLabel = isAvailable
    ? (isPrepared ? "En cola" : "Construir")
    : formatConstructionActionButtonLabel(action.availabilityStatus, isPrepared);

  return (
    <article
      className={`subpanel figma-subpanel planet-action-card construction-catalog-card${
        isAvailable ? "" : " planet-action-card-blocked"
      }`}
    >
      <div className="figma-section-header construction-catalog-card-header">
        <div>
          <p className="eyebrow">{categoryLabel}</p>
          <h4>{buildingLabel}</h4>
        </div>
        <UiBadge tone={isAvailable ? "good" : "warn"}>
          {availabilityLabel}
        </UiBadge>
      </div>
      <div className="construction-catalog-card-primary">
        <span>{currentLevelLabel}</span>
        <strong>{isAvailable ? "Lista para revisar" : "Bloqueada"}</strong>
      </div>
      <div className="figma-data-list construction-catalog-card-details">
        <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {action.targetLevel}</strong></div>
        <div className="figma-data-row"><span>Coste</span><strong>{formatCompactResourceCost(action.cost)}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{formatDuration(action.estimatedDuration)}</strong></div>
        <div className="figma-data-row"><span>Requisito</span><strong>{requirementLabel}</strong></div>
      </div>
      <p className="figma-panel-note construction-catalog-card-effect">
        {formatConstructionBuildingEffect(action.buildingType)}
      </p>
      <div className="transfer-confirmation-actions">
        <button
          type="button"
          className={isAvailable ? "" : "planet-action-button-blocked"}
          onClick={() => {
            if (isAvailable) {
              onPrepare(actionKey);
            }
          }}
          disabled={!isAvailable}
        >
          {actionButtonLabel}
        </button>
      </div>
    </article>
  );
}
