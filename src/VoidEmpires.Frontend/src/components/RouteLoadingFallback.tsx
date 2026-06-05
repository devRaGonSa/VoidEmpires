import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

export function RouteLoadingFallback() {
  return (
    <UiCard
      aria-busy="true"
      aria-live="polite"
      className="route-loading-card"
      as="section"
      role="status"
    >
      <UiBadge tone="neutral">Carga en progreso</UiBadge>
      <div className="route-loading-copy">
        <p className="eyebrow">Cambio de cabina</p>
        <h2>Cargando cabina...</h2>
        <p className="lede">
          La superficie imperial sigue activa mientras se prepara el modulo solicitado.
        </p>
      </div>
    </UiCard>
  );
}
