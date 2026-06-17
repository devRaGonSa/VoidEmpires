import { PageStatePanel } from "./PageStatePanel";

export function RouteLoadingFallback() {
  return (
    <PageStatePanel
      kind="loading"
      eyebrow="Cambio de cabina"
      badgeLabel="Carga en progreso"
      title="Cargando cabina..."
      description="La superficie imperial sigue activa mientras se prepara el modulo solicitado."
    />
  );
}
