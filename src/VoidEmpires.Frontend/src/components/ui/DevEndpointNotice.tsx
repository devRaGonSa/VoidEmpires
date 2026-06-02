import { UiBadge } from "./UiBadge";
import { UiCard } from "./UiCard";

interface DevEndpointNoticeProps {
  apiBaseUrl: string;
  backendProfile: string;
}

export function DevEndpointNotice({
  apiBaseUrl,
  backendProfile,
}: DevEndpointNoticeProps) {
  return (
    <UiCard className="dev-endpoint-notice">
      <div className="dev-endpoint-notice-head">
        <h3>Superficie de endpoints de desarrollo</h3>
        <UiBadge tone="warn">Prototipo en solo lectura</UiBadge>
      </div>
      <p>
        Las rutas actuales del frontend siguen siendo conservadoras y no
        ejecutan mutaciones de juego ni flujos de autenticacion de produccion.
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
