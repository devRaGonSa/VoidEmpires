import { UiBadge } from "./UiBadge";
import { UiCard } from "./UiCard";

interface DevEndpointNoticeProps {
  apiBaseUrl: string;
  backendProfile: string;
  compact?: boolean;
}

export function DevEndpointNotice({
  apiBaseUrl,
  backendProfile,
  compact = false,
}: DevEndpointNoticeProps) {
  return (
    <UiCard
      className={
        compact
          ? "dev-endpoint-notice dev-endpoint-notice-compact"
          : "dev-endpoint-notice"
      }
    >
      <div className="dev-endpoint-notice-head">
        <h3>Endpoints Development</h3>
        <UiBadge tone="warn">Acciones confirmadas</UiBadge>
      </div>
      <p>
        Las mutaciones disponibles requieren confirmacion explicita; el detalle
        tecnico queda como soporte secundario.
      </p>
      <dl className="dev-endpoint-meta">
        <div>
          <dt>URL base del backend</dt>
          <dd>{apiBaseUrl}</dd>
        </div>
        <div>
          <dt>Perfil esperado del backend</dt>
          <dd>{backendProfile}</dd>
        </div>
      </dl>
    </UiCard>
  );
}
