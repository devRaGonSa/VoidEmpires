import { UiBadge } from "./ui/UiBadge";
import type { ResearchTechnology } from "../utils/researchPresentation";
import {
  formatResearchTechnologyEffect,
  getResearchPrimaryAction,
  getResearchVisualState,
} from "../utils/researchPresentation";

interface ResearchCatalogCardProps {
  technology: ResearchTechnology;
  isPrepared: boolean;
  canPrepare: boolean;
  blockedReasonLabel: string;
  blockedReasonDetail: string;
  onPrepare: (technology: ResearchTechnology) => void;
}

export function ResearchCatalogCard({
  technology,
  isPrepared,
  canPrepare,
  blockedReasonLabel,
  blockedReasonDetail,
  onPrepare,
}: ResearchCatalogCardProps) {
  const visualState = getResearchVisualState(technology);
  const cardClassName = `subpanel figma-subpanel research-tech-card research-tech-card-${visualState}`;
  const buttonClassName = visualState === "ready"
    ? "research-action-button-ready"
    : "planet-action-button-secondary";
  const levelLabel = technology.currentLevel > 0
    ? `Nivel ${technology.currentLevel}`
    : "Sin investigar";
  const actionLabel = getResearchPrimaryAction(technology);
  const requirementLabel = technology.requirements.length > 0
    ? technology.requirements.map((requirement) => requirement.label).join(", ")
    : "Sin requisitos previos";

  return (
    <article className={cardClassName}>
      <div className="figma-section-header research-tech-card-header">
        <div>
          <p className="eyebrow">{technology.categoryLabel}</p>
          <h4>{technology.label}</h4>
        </div>
        <UiBadge tone={visualState === "ready" ? "good" : visualState === "blocked" ? "warn" : "resource"}>
          {visualState === "blocked" ? blockedReasonLabel : technology.availability.label}
        </UiBadge>
      </div>
      <div className="research-tech-card-primary">
        <span>{levelLabel}</span>
        <strong>{actionLabel}</strong>
      </div>
      <div className="figma-data-list research-tech-card-details">
        <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {technology.nextLevel}</strong></div>
        <div className="figma-data-row"><span>Coste</span><strong>{technology.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{technology.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Requisitos</span><strong>{visualState === "blocked" ? blockedReasonLabel : requirementLabel}</strong></div>
      </div>
      <div className="research-requirements-block">
        <p className="research-card-caption">Requisitos</p>
        <div className="selection-chip-row research-requirements-row">
          {technology.requirements.map((requirement) => (
            <span key={`${technology.researchType}-${requirement.key}`} className="selection-chip">
              {requirement.label}
            </span>
          ))}
        </div>
      </div>
      <p className="figma-panel-note research-tech-card-effect">
        {formatResearchTechnologyEffect(technology)}
      </p>
      {visualState === "blocked" ? (
        <div className="research-blocked-affordance" aria-disabled="true">
          <strong>Tecnologia bloqueada</strong>
          <span>{blockedReasonLabel}</span>
        </div>
      ) : (
        <div className="transfer-confirmation-actions">
          <button
            type="button"
            className={buttonClassName}
            onClick={() => onPrepare(technology)}
            disabled={!canPrepare}
          >
            {isPrepared ? "Investigar" : technology.primaryActionLabel}
          </button>
        </div>
      )}
      {visualState !== "ready" ? (
        <p className="figma-panel-note">
          {blockedReasonDetail}
        </p>
      ) : !technology.enqueueCommand ? (
        <p className="figma-panel-note">
          No se puede preparar esta investigacion en esta version.
        </p>
      ) : (
        <p className="figma-panel-note">
          La investigacion no se enviara hasta que confirmes la orden en el paso final.
        </p>
      )}
    </article>
  );
}
