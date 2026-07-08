import { PageStatePanel } from "./PageStatePanel";

export function RouteLoadingFallback() {
  return (
    <PageStatePanel
      kind="loading"
      eyebrow="Cambio de vista"
      badgeLabel="Carga en progreso"
      title="Cargando vista..."
      description="La superficie imperial sigue activa mientras se prepara el modulo solicitado."
    />
  );
}
