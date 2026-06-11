import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { buildGalaxyUrl, buildPlanetUrl } from "../utils/routeUrls";

function toSpanishOnboardingError(message: string) {
  switch (message.trim()) {
    case "Display name is required.":
      return "El nombre del jugador es obligatorio.";
    case "Civilization name is required.":
      return "El nombre de la civilizacion es obligatorio.";
    case "Home planet name is too long.":
      return "El nombre del planeta inicial es demasiado largo.";
    case "Display name is already in use.":
      return "Ese nombre de jugador ya esta en uso.";
    case "Civilization name is already in use.":
      return "Ese nombre de civilizacion ya esta en uso.";
    default:
      return message;
  }
}

export function OnboardingPage() {
  const navigate = useNavigate();
  const [displayName, setDisplayName] = useState("");
  const [civilizationName, setCivilizationName] = useState("");
  const [homePlanetName, setHomePlanetName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [technicalDetail, setTechnicalDetail] = useState<string | null>(null);
  const [limitations, setLimitations] = useState<string[]>([
    "Flujo solo para desarrollo.",
    "No crea una sesion autenticada.",
    "Las cabinas seguiran navegando con `civilizationId` y `planetId` en la URL.",
  ]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    setTechnicalDetail(null);

    try {
      const result = await voidEmpiresApi.createPlayableStart({
        displayName: displayName.trim(),
        civilizationName: civilizationName.trim(),
        ...(homePlanetName.trim() ? { homePlanetName: homePlanetName.trim() } : {}),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        const backendError = result.response?.errors[0] ?? "El backend no pudo crear el inicio jugable.";
        setError(toSpanishOnboardingError(backendError));
        setTechnicalDetail(`HTTP ${result.httpStatus}${result.bodyParseFailed ? " sin cuerpo JSON legible." : "."}`);
        return;
      }

      setLimitations(
        result.response.limitations.length > 0
          ? result.response.limitations.map(toSpanishOnboardingError)
          : limitations,
      );

      if (!result.response.civilizationId || !result.response.homePlanetId) {
        setError("El backend creo el inicio, pero no devolvio el contexto minimo para abrir Planeta.");
        setTechnicalDetail("Faltan civilizationId o homePlanetId en la respuesta.");
        return;
      }

      navigate(buildPlanetUrl(result.response.civilizationId, result.response.homePlanetId));
    } catch (requestError) {
      setError("No se pudo contactar con la ruta de inicio jugable.");
      setTechnicalDetail(requestError instanceof Error ? requestError.message : "Error desconocido.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Onboarding v1"
        title="Crear inicio jugable"
        description="Esta ruta crea un punto de partida Development-only y te lleva directamente a Planeta con el contexto devuelto por el backend."
        developmentNote="No es un login, no guarda credenciales y no resuelve una sesion persistente de produccion."
        badges={
          <>
            <UiBadge tone="warn">Development-only</UiBadge>
            <UiBadge>Backend autoritativo</UiBadge>
            <UiBadge>Sin autenticacion real</UiBadge>
          </>
        }
      />

      <UiCard className="panel strategic-loader-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Nuevo inicio</p>
            <h3>Preparar comandante y colonia base</h3>
          </div>
          <UiBadge tone="warn">Sin login</UiBadge>
        </div>
        <p className="figma-panel-note">
          El backend crea un inicio aislado para desarrollo y devuelve los ids necesarios para abrir la cabina de Planeta sin tocar la civilizacion sembrada de validacion.
        </p>

        <form className="query-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Nombre del jugador</span>
            <input
              type="text"
              value={displayName}
              onChange={(event) => setDisplayName(event.target.value)}
              placeholder="Comandante Vega"
              maxLength={128}
            />
          </label>
          <label className="field">
            <span>Nombre de la civilizacion</span>
            <input
              type="text"
              value={civilizationName}
              onChange={(event) => setCivilizationName(event.target.value)}
              placeholder="Dominio Solar"
              maxLength={128}
            />
          </label>
          <label className="field">
            <span>Planeta inicial opcional</span>
            <input
              type="text"
              value={homePlanetName}
              onChange={(event) => setHomePlanetName(event.target.value)}
              placeholder="Nova Prime"
              maxLength={128}
            />
          </label>
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Creando inicio..." : "Crear inicio jugable"}
          </button>
        </form>

        {error ? <p className="error-text">{error}</p> : null}
        {technicalDetail ? <p className="figma-panel-note">{technicalDetail}</p> : null}

        <div className="selection-chip-row">
          <Link className="selection-chip" to={buildGalaxyUrl()}>
            Volver a Galaxia
          </Link>
        </div>
      </UiCard>

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Limites honestos</p>
            <h3>Lo que esta ruta si y no hace</h3>
          </div>
          <UiBadge>Lectura clara</UiBadge>
        </div>
        <ul className="stack-list compact-list">
          {limitations.map((item) => (
            <li key={item}>{item}</li>
          ))}
        </ul>
      </UiCard>
    </section>
  );
}
