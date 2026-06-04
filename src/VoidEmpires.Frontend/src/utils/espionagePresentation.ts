type DomainValue = string | number | null | undefined;

function read(value: DomainValue) {
  return typeof value === "number"
    ? String(value)
    : typeof value === "string" && value.trim().length > 0
      ? value.trim()
      : null;
}

function isOneOf(value: DomainValue, options: string[]) {
  const normalized = read(value);
  return normalized ? options.includes(normalized) : false;
}

export function getIntelligenceLevelLabel(visibilityLevel: DomainValue, visibilityReason?: DomainValue) {
  if (isOneOf(visibilityLevel, ["Owned", "2"]) || isOneOf(visibilityReason, ["OwnedPlanet", "SystemContainsOwnedPlanet", "1", "2"])) return "Inteligencia confirmada";
  if (isOneOf(visibilityLevel, ["Visible", "1"]) && isOneOf(visibilityReason, ["ExploredSystem", "ExploredPlanet", "3", "4"])) return "Objetivo conocido";
  return isOneOf(visibilityLevel, ["Visible", "1"]) ? "Objetivo observado" : "Lectura incompleta";
}

export function getTargetVisibilityLabel(visibilityLevel: DomainValue, isOwnedByRequestingCivilization = false) {
  if (isOwnedByRequestingCivilization || isOneOf(visibilityLevel, ["Owned", "2"])) return "Colonia propia";
  return isOneOf(visibilityLevel, ["Visible", "1"]) ? "Objetivo observado" : "Contacto parcial";
}

export function getObservationStatusLabel(options: { visibilityLevel: DomainValue; visibilityReason?: DomainValue; sensorCount?: number; detectionCount?: number; transferCount?: number; }) {
  if ((options.sensorCount ?? 0) > 0) return "Observacion directa";
  if ((options.detectionCount ?? 0) > 0 || (options.transferCount ?? 0) > 0) return "Senal orbital detectada";
  if (isOneOf(options.visibilityLevel, ["Visible", "1"]) && isOneOf(options.visibilityReason, ["ExploredSystem", "ExploredPlanet", "3", "4"])) return "Objetivo conocido";
  return isOneOf(options.visibilityLevel, ["Visible", "1", "Owned", "2"]) ? "Observacion estable" : "Sin confirmar";
}

export function getEspionageActionLabel(actionKey: string | null | undefined) {
  const labels: Record<string, string> = {
    "exploration.preview": "Reconocimiento futuro",
    "exploration.mission.create": "Mision no disponible",
    "sensor.profile.read": "Lecturas de sensores",
    "detection.coverage.read": "Cobertura local",
    "interception.opportunity.read": "Seguimiento de trayectorias",
  };
  return actionKey ? labels[actionKey] ?? null : null;
}

export function getIntelConfidenceLabel(options: { visibilityLevel: DomainValue; sensorCount?: number; detectionCount?: number; }) {
  if (isOneOf(options.visibilityLevel, ["Owned", "2"])) return "Confirmada";
  if ((options.sensorCount ?? 0) > 0) return "Alta";
  return isOneOf(options.visibilityLevel, ["Visible", "1"]) || (options.detectionCount ?? 0) > 0 ? "Media" : "Sin confirmar";
}

export function formatIntelCoverage(options: { sensorCount?: number; detectionCount?: number; transferCount?: number; }) {
  const values = [
    (options.sensorCount ?? 0) > 0 ? `${options.sensorCount} lectura${options.sensorCount === 1 ? "" : "s"} directa${options.sensorCount === 1 ? "" : "s"}` : null,
    (options.detectionCount ?? 0) > 0 ? `${options.detectionCount} cobertura${options.detectionCount === 1 ? "" : "s"} local${options.detectionCount === 1 ? "" : "es"}` : null,
    (options.transferCount ?? 0) > 0 ? `${options.transferCount} trayectoria${options.transferCount === 1 ? "" : "s"} observada${options.transferCount === 1 ? "" : "s"}` : null,
  ].filter((value): value is string => Boolean(value));
  return values.length > 0 ? values.join(" | ") : "Objetivo fuera de alcance";
}
