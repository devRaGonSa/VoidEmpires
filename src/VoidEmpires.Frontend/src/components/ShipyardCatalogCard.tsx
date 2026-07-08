import { UiBadge } from "./ui/UiBadge";
import type { ShipyardAssetOption } from "../utils/shipyardViewModel";
import { formatShipyardAssetEffect } from "../utils/shipyardPresentation";

type ShipyardCatalogBucket = "available" | "blocked" | "unsupported";

interface ShipyardCatalogCardProps {
  asset: ShipyardAssetOption;
  bucket: ShipyardCatalogBucket;
  isRecommended: boolean;
  quantity: number;
  onQuantityChange: (assetType: string, quantity: number) => void;
  onProduce: (asset: ShipyardAssetOption, quantity: number) => void;
}

function formatRequirementLabel(asset: ShipyardAssetOption) {
  return `Astillero ${asset.requirements.buildingLevel}+ | tripulacion ${asset.requirements.operatorCapacity}`;
}

export function ShipyardCatalogCard({
  asset,
  bucket,
  isRecommended,
  quantity,
  onQuantityChange,
  onProduce,
}: ShipyardCatalogCardProps) {
  const isAvailable = bucket === "available";
  const badgeTone = bucket === "available" ? "good" : bucket === "blocked" ? "warn" : "neutral";
  const primaryActionLabel = bucket === "available"
    ? "Producir"
    : bucket === "blocked"
      ? "Bloqueada"
      : "No disponible";
  const requirementLabel = isAvailable ? formatRequirementLabel(asset) : asset.reasonLabel;

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
        <label className="field shipyard-quantity-field">
          <span>Unidades</span>
          <input
            type="number"
            min={1}
            step={1}
            value={quantity}
            disabled={!isAvailable}
            onChange={(event) => onQuantityChange(asset.assetType, Number(event.target.value))}
          />
        </label>
        <button
          type="button"
          className={isAvailable ? "planet-action-button-secondary" : "planet-action-button-blocked"}
          onClick={() => onProduce(asset, quantity)}
          disabled={!isAvailable}
        >
          Producir
        </button>
      </div>
      {!isAvailable ? (
        <p className="figma-panel-note">{requirementLabel}</p>
      ) : null}
    </article>
  );
}
