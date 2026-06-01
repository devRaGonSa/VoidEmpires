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
        <h3>Development endpoint surface</h3>
        <UiBadge tone="warn">Read-only prototype</UiBadge>
      </div>
      <p>
        Current frontend routes remain conservative and do not execute gameplay
        mutations or production authentication flows.
      </p>
      <dl className="dev-endpoint-meta">
        <div>
          <dt>Backend base URL</dt>
          <dd>{apiBaseUrl}</dd>
        </div>
        <div>
          <dt>Expected backend profile</dt>
          <dd>{backendProfile}</dd>
        </div>
      </dl>
    </UiCard>
  );
}
