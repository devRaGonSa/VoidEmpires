import type { FleetOrbitalStock } from "../api/fleetTypes";
import { FleetCreationCard } from "./FleetCreationCard";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

interface LocalOrbitalStockPanelProps {
  stock: FleetOrbitalStock[];
  quantities: Record<string, number>;
  onQuantityChange: (assetType: string, quantity: number) => void;
  onCreate: (stock: FleetOrbitalStock, quantity: number) => void;
}

export function LocalOrbitalStockPanel({ stock, quantities, onQuantityChange, onCreate }: LocalOrbitalStockPanelProps) {
  return (
    <UiCard className="panel">
      <div className="figma-section-header"><div><p className="eyebrow">Stock orbital local</p><h3>Crear flota</h3></div><UiBadge>{stock.length} tipos</UiBadge></div>
      {stock.length ? <div className="readiness-grid shipyard-catalog-grid">{stock.map((item) => <FleetCreationCard key={item.assetType} stock={item} quantity={quantities[item.assetType] ?? 1} onQuantityChange={onQuantityChange} onCreate={onCreate} />)}</div> : <p className="figma-panel-note">No hay stock orbital disponible en este planeta.</p>}
    </UiCard>
  );
}
