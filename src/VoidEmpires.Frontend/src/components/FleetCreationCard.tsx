import type { FleetOrbitalStock } from "../api/fleetTypes";
import { formatSpaceAssetType } from "../utils/domainPresentation";
import { UiBadge } from "./ui/UiBadge";

interface FleetCreationCardProps {
  stock: FleetOrbitalStock;
  quantity: number;
  onQuantityChange: (assetType: string, quantity: number) => void;
  onCreate: (stock: FleetOrbitalStock, quantity: number) => void;
}

export function FleetCreationCard({ stock, quantity, onQuantityChange, onCreate }: FleetCreationCardProps) {
  const canCreate = stock.quantity > 0;
  return (
    <article className="subpanel figma-subpanel">
      <div className="figma-section-header"><div><p className="eyebrow">Naves disponibles</p><h4>{formatSpaceAssetType(stock.assetType)}</h4></div><UiBadge tone={canCreate ? "good" : "neutral"}>Stock {stock.quantity}</UiBadge></div>
      <div className="transfer-confirmation-actions production-action-row">
        <label className="field production-quantity-field"><span>Unidades</span><input type="number" min={1} max={stock.quantity} step={1} value={quantity} disabled={!canCreate} onChange={(event) => onQuantityChange(stock.assetType, Number(event.target.value))} /></label>
        <button type="button" disabled={!canCreate} onClick={() => onCreate(stock, quantity)}>Crear flota</button>
      </div>
      {!canCreate ? <p className="figma-panel-note">No hay unidades disponibles.</p> : null}
    </article>
  );
}
