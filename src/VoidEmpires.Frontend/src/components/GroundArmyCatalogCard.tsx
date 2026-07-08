import { UiBadge } from "./ui/UiBadge";
import { formatGroundArmyOptionEffect } from "../utils/groundArmyPresentation";
import type { GroundArmyOption } from "../utils/groundArmyViewModel";

interface GroundArmyCatalogCardProps {
  option: GroundArmyOption;
}

export function GroundArmyCatalogCard({ option }: GroundArmyCatalogCardProps) {
  const isAvailable = option.statusKey === "Available";
  const requirementLabel = isAvailable
    ? option.requirementLabel
    : option.missingLabel ?? option.reasonLabel;

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
        <strong>{isAvailable ? "Lista para revisar" : "Bloqueada"}</strong>
      </div>
      <div className="figma-data-list ground-army-catalog-card-details">
        <div className="figma-data-row"><span>Coste</span><strong>{option.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{option.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Requisito</span><strong>{requirementLabel}</strong></div>
      </div>
      <p className="figma-panel-note ground-army-catalog-card-effect">
        {formatGroundArmyOptionEffect(option.assetType)}
      </p>
      <div className="transfer-confirmation-actions">
        <span className="planet-action-handoff-message">
          Entrenamiento directo no disponible aqui
        </span>
      </div>
    </article>
  );
}
