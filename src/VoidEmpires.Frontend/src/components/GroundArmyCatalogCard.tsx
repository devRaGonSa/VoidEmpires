import { UiBadge } from "./ui/UiBadge";
import { formatGroundArmyOptionEffect, formatGroundTrainingCost } from "../utils/groundArmyPresentation";
import type { GroundArmyOption } from "../utils/groundArmyViewModel";

interface GroundArmyCatalogCardProps {
  option: GroundArmyOption;
  quantity: number;
  onQuantityChange: (assetType: string, quantity: number) => void;
  onTrain: (option: GroundArmyOption, quantity: number) => void;
}

export function formatGroundTrainingTotalDuration(value: string, quantity: number) {
  const match = /^(?:(\d+)\.)?(\d{2}):(\d{2}):(\d{2})$/.exec(value);
  if (!match) return value;
  const totalMinutes = Math.ceil(((Number(match[1] ?? 0) * 86400) + (Number(match[2]) * 3600) + (Number(match[3]) * 60) + Number(match[4])) * quantity / 60);
  return totalMinutes < 60 ? `${totalMinutes} min` : `${Math.floor(totalMinutes / 60)} h ${totalMinutes % 60} min`;
}

export function GroundArmyCatalogCard({ option, quantity, onQuantityChange, onTrain }: GroundArmyCatalogCardProps) {
  const isAvailable = option.statusKey === "Available";
  const normalizedQuantity = Math.max(1, Math.floor(quantity));
  const requirementLabel = isAvailable
    ? option.requirementLabel
    : option.missingLabel ?? option.reasonLabel;
  const totalCost = option.cost.map((entry) => ({ ...entry, quantity: entry.quantity * normalizedQuantity }));

  return (
    <article className={`subpanel figma-subpanel ground-army-catalog-card${isAvailable ? "" : " ground-army-catalog-card-blocked"}`}>
      <div className="figma-section-header ground-army-catalog-card-header">
        <div>
          <p className="eyebrow">{option.categoryLabel}</p>
          <h4>{option.label}</h4>
        </div>
        <UiBadge tone={isAvailable ? "good" : "warn"}>{option.statusLabel}</UiBadge>
      </div>
      <div className="ground-army-catalog-card-primary">
        <span>Stock {option.currentStock}</span>
        <strong>{isAvailable ? "Lista para entrenar" : "Bloqueada"}</strong>
      </div>
      <div className="figma-data-list ground-army-catalog-card-details">
        <div className="figma-data-row"><span>Coste por unidad</span><strong>{option.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Coste total</span><strong>{formatGroundTrainingCost(totalCost)}</strong></div>
        <div className="figma-data-row"><span>Duracion por unidad</span><strong>{option.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion total</span><strong>{formatGroundTrainingTotalDuration(option.estimatedDuration, normalizedQuantity)}</strong></div>
        <div className="figma-data-row"><span>Requisito</span><strong>{requirementLabel}</strong></div>
      </div>
      <p className="figma-panel-note ground-army-catalog-card-effect">
        {formatGroundArmyOptionEffect(option.assetType)}
      </p>
      <div className="transfer-confirmation-actions production-action-row">
        <label className="field production-quantity-field ground-training-quantity-field">
          <span>Unidades</span>
          <input type="number" min={1} step={1} value={quantity} disabled={!isAvailable} onChange={(event) => onQuantityChange(option.assetType, Number(event.target.value))} />
        </label>
        <button type="button" className={isAvailable ? "planet-action-button-secondary" : "planet-action-button-blocked"} disabled={!isAvailable} onClick={() => onTrain(option, normalizedQuantity)}>
          Entrenar
        </button>
      </div>
      {!isAvailable ? <p className="figma-panel-note">{requirementLabel}</p> : null}
    </article>
  );
}
