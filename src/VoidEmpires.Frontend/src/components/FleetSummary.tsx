import { UiCard } from "./ui/UiCard";

interface FleetSummaryProps {
  total: number;
  stationed: number;
  transit: number;
  ships: number;
}

export function FleetSummary({ total, stationed, transit, ships }: FleetSummaryProps) {
  const metrics = [["Flotas", total], ["Estacionadas", stationed], ["En transito", transit], ["Naves", ships]] as const;
  return (
    <UiCard className="panel">
      <div className="figma-section-header"><div><p className="eyebrow">Resumen</p><h3>Estado de flotas</h3></div></div>
      <div className="figma-stat-grid">
        {metrics.map(([label, value]) => <div className="figma-stat" key={label}><strong>{value}</strong><span>{label}</span></div>)}
      </div>
    </UiCard>
  );
}
