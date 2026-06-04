type DomainValue = string | number | null | undefined;

export interface EspionageErrorFeedback {
  primaryMessage: string;
  technicalDetail: string | null;
}

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

function formatCount(value: number, singular: string, plural: string) {
  return `${value} ${value === 1 ? singular : plural}`;
}

export function formatEspionageSignalCoverageSummary(rawSummary: string | null | undefined) {
  const normalized = rawSummary?.trim();
  if (!normalized) return "Sin lectura";

  if (/^No passive signal rows available\.$/i.test(normalized)) {
    return "Sin senales pasivas disponibles";
  }

  const passiveSignalMatch = normalized.match(/^(\d+) passive signal rows available\.$/i);
  if (passiveSignalMatch) {
    const count = Number.parseInt(passiveSignalMatch[1], 10);
    return `${formatCount(count, "senal pasiva", "senales pasivas")} disponible${count === 1 ? "" : "s"}`;
  }

  const sensorProfileMatch = normalized.match(/^(\d+) sensor profile rows\.$/i);
  if (sensorProfileMatch) {
    const count = Number.parseInt(sensorProfileMatch[1], 10);
    return formatCount(count, "lectura de perfil de sensores", "lecturas de perfil de sensores");
  }

  const localSensorProfileMatch = normalized.match(/^(\d+) local sensor profile rows\.$/i);
  if (localSensorProfileMatch) {
    const count = Number.parseInt(localSensorProfileMatch[1], 10);
    return formatCount(count, "lectura local de sensores", "lecturas locales de sensores");
  }

  const detectionCoverageMatch = normalized.match(/^(\d+) detection coverage rows\.$/i);
  if (detectionCoverageMatch) {
    const count = Number.parseInt(detectionCoverageMatch[1], 10);
    return formatCount(count, "lectura de cobertura de deteccion", "lecturas de cobertura de deteccion");
  }

  const localDetectionCoverageMatch = normalized.match(/^(\d+) local detection coverage rows\.$/i);
  if (localDetectionCoverageMatch) {
    const count = Number.parseInt(localDetectionCoverageMatch[1], 10);
    return formatCount(count, "lectura local de cobertura", "lecturas locales de cobertura");
  }

  const transferTrajectoryMatch = normalized.match(/^(\d+) visible transfer trajectories\.$/i);
  if (transferTrajectoryMatch) {
    const count = Number.parseInt(transferTrajectoryMatch[1], 10);
    return formatCount(count, "trayectoria de transferencia visible", "trayectorias de transferencia visibles");
  }

  return normalized;
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
    "espionage.reconnaissance.create": "Reconocimiento activo",
    "espionage.infiltration.create": "Infiltracion",
    "espionage.sabotage.create": "Sabotaje",
    "espionage.counterintelligence.create": "Contraespionaje",
    "espionage.technologyTheft.create": "Robo de tecnologia",
  };
  return actionKey ? labels[actionKey] ?? null : null;
}

export function getIntelConfidenceLabel(options: { visibilityLevel: DomainValue; sensorCount?: number; detectionCount?: number; }) {
  if (isOneOf(options.visibilityLevel, ["Owned", "2"])) return "Confirmada";
  if ((options.sensorCount ?? 0) > 0) return "Alta";
  return isOneOf(options.visibilityLevel, ["Visible", "1"]) || (options.detectionCount ?? 0) > 0 ? "Media" : "Sin confirmar";
}

export function getEspionageCueDescription(cue: "owned" | "observed" | "partial" | "signal" | "unconfirmed") {
  switch (cue) {
    case "owned":
      return "Control directo con contexto estable y sin ambiguedad operativa.";
    case "observed":
      return "Objetivo visible o conocido desde la lectura estrategica actual.";
    case "partial":
      return "Hay fragmentos utiles, pero la cabina todavia no confirma todos los campos.";
    case "signal":
      return "La pista existe, pero sigue siendo solo una senal pasiva.";
    default:
      return "Contacto presente sin evidencia suficiente para tratarlo como objetivo completo.";
  }
}

export function getEspionageMissingDataNote(cue: "partial" | "signal" | "unconfirmed") {
  switch (cue) {
    case "partial":
      return "Los datos incompletos no desbloquean acciones ofensivas.";
    case "signal":
      return "La lectura depende de la visibilidad estrategica actual.";
    default:
      return "La cabina oculta campos no confirmados en lugar de inventarlos.";
  }
}

export function formatEspionageRequestFailure(rawError: string | null | undefined): EspionageErrorFeedback {
  const technicalDetail = rawError?.trim() || null;

  switch (technicalDetail) {
    case "Civilization id is required.":
      return { primaryMessage: "Falta el id de civilizacion para abrir Espionaje.", technicalDetail };
    case "Civilization not found.":
      return { primaryMessage: "La civilizacion no existe en este escenario visible.", technicalDetail };
    case "Request failed with status 404.":
      return { primaryMessage: "La ruta de Espionaje no esta disponible fuera del entorno de desarrollo.", technicalDetail };
    case "Request failed with status 503.":
      return { primaryMessage: "La persistencia de desarrollo no esta disponible. Aplica cockpit-validation para cargar el escenario demo.", technicalDetail };
    case "Unsupported action.":
      return { primaryMessage: "Las misiones activas no estan disponibles en esta version.", technicalDetail };
    default:
      return {
        primaryMessage: "No se pudo cargar la lectura de inteligencia.",
        technicalDetail,
      };
  }
}

export function formatEspionageEmptyState(hasSignals: boolean) {
  return hasSignals
    ? "No hay objetivos visibles para esta civilizacion."
    : "No hay objetivos visibles ni lecturas pasivas para esta civilizacion.";
}

export function formatIntelCoverage(options: { sensorCount?: number; detectionCount?: number; transferCount?: number; }) {
  const values = [
    (options.sensorCount ?? 0) > 0 ? `${options.sensorCount} lectura${options.sensorCount === 1 ? "" : "s"} directa${options.sensorCount === 1 ? "" : "s"}` : null,
    (options.detectionCount ?? 0) > 0 ? `${options.detectionCount} cobertura${options.detectionCount === 1 ? "" : "s"} local${options.detectionCount === 1 ? "" : "es"}` : null,
    (options.transferCount ?? 0) > 0 ? `${options.transferCount} trayectoria${options.transferCount === 1 ? "" : "s"} observada${options.transferCount === 1 ? "" : "s"}` : null,
  ].filter((value): value is string => Boolean(value));
  return values.length > 0 ? values.join(" | ") : "Objetivo fuera de alcance";
}
