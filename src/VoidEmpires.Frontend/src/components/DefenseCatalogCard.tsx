import { UiBadge } from "./ui/UiBadge";
import { formatDefenseOptionEffect } from "../utils/defensePresentation";
import type { DefenseOption } from "../utils/defenseViewModel";

interface DefenseCatalogCardProps {
  option: DefenseOption;
  hasProductionAction: boolean;
}

export function DefenseCatalogCard({
  option,
  hasProductionAction,
}: DefenseCatalogCardProps) {
  const isAvailable = option.statusKey === "Available";
  const requirementLabel = isAvailable
    ? option.requirementLabel ?? "Lista para revisar desde Construccion"
    : option.affordabilityLabel ?? option.requirementLabel ?? option.reasonLabel;

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
        <span>Nivel {option.currentLevel} {"->"} {option.targetLevel}</span>
        <strong>{hasProductionAction && isAvailable ? option.actionLabel : "Sin accion local"}</strong>
      </div>
      <div className="figma-data-list defense-catalog-card-details">
        <div className="figma-data-row"><span>Coste</span><strong>{option.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{option.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Requisito</span><strong>{requirementLabel}</strong></div>
      </div>
      <p className="figma-panel-note defense-catalog-card-effect">
        {formatDefenseOptionEffect(option.buildingType)}
      </p>
      <div className="transfer-confirmation-actions">
        <span className="planet-action-handoff-message">
          {hasProductionAction && isAvailable ? "Accion disponible" : "Produccion defensiva no disponible aqui"}
        </span>
      </div>
    </article>
  );
}
