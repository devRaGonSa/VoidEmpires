import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import type { AccountApiError, AccountRegistrationResponse } from "../api/accountTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { getRegistrationWorldEntry } from "../utils/currentAccountSession";
import { savePlayableSession } from "../utils/playableSession";
import { buildGalaxyUrl, buildWorldEntryUrl } from "../utils/routeUrls";

function toRegisterError(error: AccountApiError) {
  switch (error.code) {
    case "EmailRequired":
      return "El correo es obligatorio.";
    case "EmailInvalid":
      return "El correo debe ser valido.";
    case "EmailAlreadyRegistered":
      return "Ese correo ya tiene una cuenta.";
    case "PasswordRequired":
      return "La contrasena es obligatoria.";
    case "PasswordTooWeak":
      return "La contrasena no cumple la politica minima.";
    case "ConfirmPasswordRequired":
      return "Confirma la contrasena.";
    case "PasswordMismatch":
      return "Las contrasenas no coinciden.";
    case "DisplayNameRequired":
      return "El nombre del comandante es obligatorio.";
    case "CivilizationNameRequired":
      return "El nombre de la civilizacion es obligatorio.";
    case "DisplayNameTooLong":
    case "CivilizationNameTooLong":
    case "HomePlanetNameTooLong":
      return "Uno de los nombres es demasiado largo.";
    case "DisplayNameTaken":
      return "Ese comandante ya existe.";
    case "CivilizationNameTaken":
      return "Esa civilizacion ya existe.";
    default:
      return "No se pudo completar el registro.";
  }
}

function resolveRegistrationRoute(registration: AccountRegistrationResponse) {
  const worldEntry = getRegistrationWorldEntry(registration);
  return worldEntry
    ? buildWorldEntryUrl(worldEntry.civilizationId, worldEntry.planetId, worldEntry.nextRoute)
    : "/";
}

export function RegisterPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [civilizationName, setCivilizationName] = useState("");
  const [homePlanetName, setHomePlanetName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setIsSubmitting(true);
    setErrors([]);

    try {
      const normalizedEmail = email.trim();
      const result = await voidEmpiresApi.account.register({
        email: normalizedEmail,
        password,
        confirmPassword,
        displayName: displayName.trim(),
        civilizationName: civilizationName.trim(),
        ...(homePlanetName.trim() ? { homePlanetName: homePlanetName.trim() } : {}),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        const nextErrors = result.response?.errors.map(toRegisterError) ?? ["No se pudo crear la cuenta."];
        setErrors(nextErrors.length > 0 ? nextErrors : ["No se pudo crear la cuenta."]);
        return;
      }

      const worldEntry = getRegistrationWorldEntry(result.response);
      if (!worldEntry) {
        setErrors(["La cuenta fue creada, pero no se pudo resolver el mundo inicial."]);
        return;
      }

      const loginResult = await voidEmpiresApi.account.login({ email: normalizedEmail, password });
      if (loginResult.httpStatus !== 200 || !loginResult.response?.succeeded) {
        setErrors(["La cuenta fue creada. Entra con tu correo para abrir el mundo inicial."]);
        return;
      }

      savePlayableSession({
        civilizationId: worldEntry.civilizationId,
        planetId: worldEntry.planetId,
        playerDisplayName: displayName.trim(),
        civilizationName: civilizationName.trim(),
        planetName: worldEntry.planetName ?? homePlanetName.trim(),
      });
      navigate(resolveRegistrationRoute(result.response), { replace: true });
    } catch {
      setErrors(["No se pudo contactar el servicio de cuentas."]);
    } finally {
      setPassword("");
      setConfirmPassword("");
      setIsSubmitting(false);
    }
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Cuenta"
        title="Crear cuenta"
        description="Registra tu comandante, funda una civilizacion y entra al primer mundo generado por el servidor."
        developmentNote="La sesion usa una cookie HTTP-only; el cliente no guarda tokens de autenticacion."
        badges={
          <>
            <UiBadge tone="good">Registro</UiBadge>
            <UiBadge>Cuenta segura</UiBadge>
            <UiBadge>Mundo inicial</UiBadge>
          </>
        }
      />

      <UiCard className="panel strategic-loader-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Registro</p>
            <h3>Datos de cuenta e imperio</h3>
          </div>
          <UiBadge tone="good">Acceso</UiBadge>
        </div>
        <p className="figma-panel-note">
          Usa un correo y una contrasena para crear la cuenta. El servidor prepara el comandante, la civilizacion y el planeta inicial.
        </p>

        <form className="query-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Correo</span>
            <input type="email" value={email} onChange={(event) => setEmail(event.target.value)} />
          </label>
          <label className="field">
            <span>Contrasena</span>
            <input type="password" value={password} onChange={(event) => setPassword(event.target.value)} />
          </label>
          <label className="field">
            <span>Confirmar contrasena</span>
            <input type="password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} />
          </label>
          <label className="field">
            <span>Comandante</span>
            <input type="text" value={displayName} onChange={(event) => setDisplayName(event.target.value)} maxLength={128} placeholder="Comandante Vega" />
          </label>
          <label className="field">
            <span>Civilizacion</span>
            <input type="text" value={civilizationName} onChange={(event) => setCivilizationName(event.target.value)} maxLength={128} placeholder="Dominio Solar" />
          </label>
          <label className="field">
            <span>Planeta inicial</span>
            <input type="text" value={homePlanetName} onChange={(event) => setHomePlanetName(event.target.value)} maxLength={128} placeholder="Nova Prime" />
          </label>
          <button type="submit" disabled={isSubmitting}>{isSubmitting ? "Creando cuenta..." : "Crear cuenta"}</button>
        </form>

        {errors.length > 0 ? (
          <ul className="stack-list compact-list">
            {errors.map((error) => <li className="error-text" key={error}>{error}</li>)}
          </ul>
        ) : null}
      </UiCard>

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Siguiente paso</p>
            <h3>Entrar al mundo inicial</h3>
          </div>
          <UiBadge>Servidor autoritativo</UiBadge>
        </div>
        <p className="figma-panel-note">
          Tras el registro, VoidEmpires abre el mundo inicial con el contexto seguro de la cuenta.
        </p>
        <div className="selection-chip-row">
          <Link className="selection-chip" to={buildGalaxyUrl()}>Ver galaxia</Link>
        </div>
      </UiCard>
    </section>
  );
}
