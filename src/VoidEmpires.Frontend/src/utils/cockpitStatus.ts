export const cockpitStatusLabels = {
  available: "Disponible",
  blocked: "Bloqueada",
  completed: "Completada",
  contextPreserved: "Contexto conservado",
  developmentOnly: "Preparado",
  diagnostics: "Diagnostico secundario",
  implementedReadOnly: "Disponible",
  inQueue: "En cola",
  preparation: "Preparacion",
  readOnly: "Disponible",
  reviewContext: "Revisar contexto",
  safePlaceholder: "Pendiente",
} as const;

export const cockpitNavigationLabels = {
  currentPlanet: "Abrir Planeta actual",
  openConstruction: "Abrir Construccion",
  openDefenses: "Abrir Defensas",
  openFleets: "Abrir Flotas",
  openResearch: "Abrir Investigacion",
  openShipyard: "Abrir Astillero",
  relatedCabin: "Cabina relacionada",
  relatedCabins: "Siguientes cabinas",
  returnToGalaxy: "Volver a Galaxia",
  returnToPlanet: "Volver a Planeta",
} as const;

export const productActionLabels = {
  build: "Construir",
  research: "Investigar",
  produce: "Producir",
  review: "Revisar",
  open: "Abrir",
  confirm: "Confirmar",
  cancel: "Cancelar",
  back: "Volver",
} as const;

export function formatProductActionLabel(
  action: keyof typeof productActionLabels,
  target?: string,
) {
  return target ? `${productActionLabels[action]} ${target}` : productActionLabels[action];
}
