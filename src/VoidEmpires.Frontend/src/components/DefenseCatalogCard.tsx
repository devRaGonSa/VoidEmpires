import { UiBadge } from "./ui/UiBadge";
import { formatDefenseOptionEffect } from "../utils/defensePresentation";
import type { DefenseOption } from "../utils/defenseViewModel";

interface DefenseCatalogCardProps {
  option: DefenseOption;
  hasProductionAction: boolean;
  quantity: number;
  onQuantityChange: (buildingType: string, quantity: number) => void;
  onBuild: (option: DefenseOption, quantity: number) => void;
}

export function DefenseCatalogCard({
  option,
  hasProductionAction,
  quantity,
  onQuantityChange,
  onBuild,
}: DefenseCatalogCardProps) {
  const isAvailable = option.statusKey === "Available";
  const isUnitBased = option.productionModel === "unit";
  const canBuildUnits = hasProductionAction && isAvailable && isUnitBased && Boolean(option.assetType);
  const requirementLabel = isAvailable
    ? option.requirementLabel ?? "Lista para construir"
    : option.affordabilityLabel ?? option.requirementLabel ?? option.reasonLabel;
  const primaryLabel = isUnitBased
    ? `Cantidad actual ${option.currentLevel}`
    : `Nivel ${option.currentLevel} -> ${option.targetLevel}`;
  const primaryActionLabel = isUnitBased
    ? canBuildUnits ? "Construir" : "Bloqueada"
    : isAvailable ? "Mejorar" : "Bloqueada";

  return (
    <article className={`subpanel figma-subpanel defense-catalog-card${isAvailable ? "" : " defense-catalog-card-blocked"}`}>
      <div className="figma-section-header defense-catalog-card-header">
        <div>
          <p className="eyebrow">{option.categoryLabel}</p>
          <h4>{option.structureLabel}</h4>
        </div>
        <UiBadge tone={isAvailable ? "good" : "warn"}>{option.statusLabel}</UiBadge>
      </div>
      <div className="defense-catalog-card-primary">
        <span>{primaryLabel}</span>
        <strong>{primaryActionLabel}</strong>
      </div>
      <div className="figma-data-list defense-catalog-card-details">
        {isUnitBased ? (
          <div className="figma-data-row"><span>Cantidad actual</span><strong>{option.currentLevel}</strong></div>
        ) : null}
        <div className="figma-data-row"><span>Coste</span><strong>{option.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{option.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Requisito</span><strong>{requirementLabel}</strong></div>
      </div>
      <p className="figma-panel-note defense-catalog-card-effect">
        {formatDefenseOptionEffect(option.buildingType)}
      </p>
      <div className="transfer-confirmation-actions">
        {isUnitBased ? (
          <label className="field defense-quantity-field">
            <span>Unidades</span>
            <input
              type="number"
              min={1}
              step={1}
              value={quantity}
              disabled={!canBuildUnits}
              onChange={(event) => onQuantityChange(option.buildingType, Number(event.target.value))}
            />
          </label>
        ) : null}
        {isUnitBased ? (
          <button
            type="button"
            className={canBuildUnits ? "planet-action-button-secondary" : "planet-action-button-blocked"}
            disabled={!canBuildUnits}
            onClick={() => onBuild(option, quantity)}
          >
            Construir
          </button>
        ) : null}
      </div>
      {!canBuildUnits ? <p className="figma-panel-note">{requirementLabel}</p> : null}
    </article>
  );
}
