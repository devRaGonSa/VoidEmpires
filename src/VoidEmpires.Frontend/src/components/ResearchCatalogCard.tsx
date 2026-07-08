import { PlaceholderAsset } from "./PlaceholderAsset";
import { UiBadge } from "./ui/UiBadge";
import type { ResearchTechnology } from "../utils/researchPresentation";
import {
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

  return (
    <article className={cardClassName}>
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">{technology.categoryLabel}</p>
          <h4>{technology.label}</h4>
        </div>
        <UiBadge tone={visualState === "ready" ? "good" : visualState === "blocked" ? "warn" : "resource"}>
          {visualState === "blocked" ? blockedReasonLabel : technology.availability.label}
        </UiBadge>
      </div>
      <PlaceholderAsset
        kind="technology"
        label={technology.label}
        typeLabel={technology.categoryLabel}
        detail={`Impacto: ${technology.bonusLabel}. Nivel ${technology.currentLevel} a ${technology.nextLevel}.`}
      />
      <div className="figma-data-list">
        <div className="figma-data-row"><span>Categoria</span><strong>{technology.categoryLabel}</strong></div>
        <div className="figma-data-row"><span>Nivel actual</span><strong>{technology.currentLevel}</strong></div>
        <div className="figma-data-row"><span>Siguiente nivel</span><strong>{technology.nextLevel}</strong></div>
        <div className="figma-data-row"><span>Impacto</span><strong>{technology.bonusLabel}</strong></div>
        <div className="figma-data-row"><span>Coste</span><strong>{technology.estimatedCostLabel}</strong></div>
        <div className="figma-data-row"><span>Duracion</span><strong>{technology.estimatedDurationLabel}</strong></div>
        <div className="figma-data-row"><span>Accion</span><strong>{getResearchPrimaryAction(technology)}</strong></div>
      </div>
      <div className="research-requirements-block">
        <p className="research-card-caption">Requisitos para iniciar</p>
        <div className="selection-chip-row research-requirements-row">
          {technology.requirements.map((requirement) => (
            <span key={`${technology.researchType}-${requirement.key}`} className="selection-chip">
              {requirement.label}
            </span>
          ))}
        </div>
      </div>
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
            {isPrepared ? "Revision preparada" : technology.primaryActionLabel}
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
