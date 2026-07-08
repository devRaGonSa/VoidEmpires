import { PlaceholderAsset } from "./PlaceholderAsset";
import { UiBadge } from "./ui/UiBadge";
import type { ShipyardAssetOption } from "../utils/shipyardViewModel";

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

  return (
    <article className={`subpanel figma-subpanel shipyard-catalog-card shipyard-catalog-card-${bucket}`}>
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">{asset.roleLabel}</p>
          <h4>{asset.label}</h4>
        </div>
        <div className="figma-badge-row">
          {isRecommended ? <UiBadge tone="good">Recomendada</UiBadge> : null}
          <UiBadge tone={badgeTone}>{asset.statusLabel}</UiBadge>
        </div>
      </div>
      <PlaceholderAsset
        kind="ship"
        label={asset.label}
        typeLabel={asset.roleLabel}
        detail={asset.description}
        imageKey={asset.imageKey}
      />
      <div className="figma-data-list">
        <div className="figma-data-row"><span>Rol</span><strong>{asset.roleLabel}</strong></div>
        <div className="figma-data-row"><span>Clase</span><strong>{asset.categoryLabel}</strong></div>
        <div className="figma-data-row"><span>Stock orbital actual</span><strong>{asset.quantityLabel}</strong></div>
        <div className="figma-data-row"><span>Coste</span><strong>{asset.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{asset.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Requisitos</span><strong>{formatRequirementLabel(asset)}</strong></div>
      </div>
      <p>{bucket === "available" ? "Lista para revisar y confirmar produccion orbital." : asset.reasonLabel}</p>
      <div className="selection-chip-row">
        <button
          type="button"
          className="planet-action-button-secondary"
          onClick={() => onReview(asset)}
        >
          {reviewLabel}
        </button>
      </div>
    </article>
  );
}
