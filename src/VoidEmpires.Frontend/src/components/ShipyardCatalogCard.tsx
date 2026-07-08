import { UiBadge } from "./ui/UiBadge";
import type { ShipyardAssetOption } from "../utils/shipyardViewModel";
import { formatShipyardAssetEffect } from "../utils/shipyardPresentation";

type ShipyardCatalogBucket = "available" | "blocked" | "unsupported";

interface ShipyardCatalogCardProps {
  asset: ShipyardAssetOption;
  bucket: ShipyardCatalogBucket;
  isRecommended: boolean;
  onReview: (asset: ShipyardAssetOption) => void;
}

function formatRequirementLabel(asset: ShipyardAssetOption) {
  return `Astillero ${asset.requirements.buildingLevel}+ | tripulacion ${asset.requirements.operatorCapacity}`;
}

export function ShipyardCatalogCard({
  asset,
  bucket,
  isRecommended,
  onReview,
}: ShipyardCatalogCardProps) {
  const badgeTone = bucket === "available" ? "good" : bucket === "blocked" ? "warn" : "neutral";
  const reviewLabel = bucket === "available"
    ? "Revisar orden"
    : bucket === "blocked"
      ? "Revisar bloqueo"
      : "Revisar limite";
  const primaryActionLabel = bucket === "available"
    ? "Lista para cola"
    : bucket === "blocked"
      ? "Bloqueada"
      : "No disponible";
  const requirementLabel = bucket === "available"
    ? formatRequirementLabel(asset)
    : asset.reasonLabel;

  return (
    <article className={`subpanel figma-subpanel shipyard-catalog-card shipyard-catalog-card-${bucket}`}>
      <div className="figma-section-header shipyard-catalog-card-header">
        <div>
          <p className="eyebrow">{asset.categoryLabel}</p>
          <h4>{asset.label}</h4>
        </div>
        <div className="figma-badge-row">
          {isRecommended ? <UiBadge tone="good">Recomendada</UiBadge> : null}
          <UiBadge tone={badgeTone}>{asset.statusLabel}</UiBadge>
        </div>
      </div>
      <div className="shipyard-catalog-card-primary">
        <span>{asset.quantityLabel}</span>
        <strong>{primaryActionLabel}</strong>
      </div>
      <div className="figma-data-list shipyard-catalog-card-details">
        <div className="figma-data-row"><span>Stock orbital actual</span><strong>{asset.quantityLabel}</strong></div>
        <div className="figma-data-row"><span>Coste</span><strong>{asset.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{asset.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Requisito</span><strong>{requirementLabel}</strong></div>
      </div>
      <p className="figma-panel-note shipyard-catalog-card-effect">
        {asset.roleLabel}. {formatShipyardAssetEffect(asset.assetType, asset.description)}
      </p>
      <div className="transfer-confirmation-actions">
        <button
          type="button"
          className={bucket === "available" ? "planet-action-button-secondary" : "planet-action-button-blocked"}
          onClick={() => onReview(asset)}
        >
          {reviewLabel}
        </button>
      </div>
    </article>
  );
}
