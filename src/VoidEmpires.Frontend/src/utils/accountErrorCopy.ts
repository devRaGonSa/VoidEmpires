import type {
  AccountApiError,
  AccountApiResult,
  AccountRegistrationResponse,
  AccountSessionResponse,
} from "../api/accountTypes";

const registrationErrorCopy: Record<string, string> = {
  EmailRequired: "El correo es obligatorio.",
  EmailInvalid: "Escribe un correo valido.",
  EmailAlreadyRegistered: "Ese correo ya tiene una cuenta. Entra con ese correo o usa otro.",
  PasswordRequired: "La contrasena es obligatoria.",
  PasswordTooWeak: "Usa una contrasena de al menos 8 caracteres con mayuscula, minuscula, numero y simbolo.",
  ConfirmPasswordRequired: "Confirma la contrasena.",
  PasswordMismatch: "Las contrasenas no coinciden.",
  DisplayNameRequired: "El nombre del comandante es obligatorio.",
  CivilizationNameRequired: "El nombre de la civilizacion es obligatorio.",
  DisplayNameTooLong: "El nombre del comandante es demasiado largo.",
  CivilizationNameTooLong: "El nombre de la civilizacion es demasiado largo.",
  HomePlanetNameTooLong: "El nombre del planeta inicial es demasiado largo.",
  DisplayNameTaken: "Ese comandante ya existe. Elige otro nombre.",
  CivilizationNameTaken: "Esa civilizacion ya existe. Elige otro nombre.",
  PlayerProfileExists: "Esa cuenta ya tiene un comandante inicial.",
  BootstrapFailed: "No se pudo preparar el mundo inicial. Reintenta en unos segundos.",
  RegistrationFailed: "No se pudo completar el registro. Revisa los datos y vuelve a intentarlo.",
};

const loginErrorCopy: Record<string, string> = {
  EmailRequired: "El correo es obligatorio.",
  PasswordRequired: "La contrasena es obligatoria.",
  InvalidCredentials: "Correo o contrasena incorrectos.",
  Unauthenticated: "Inicia sesion para continuar.",
};

function formatAccountError(error: AccountApiError, copy: Record<string, string>, fallback: string) {
  return copy[error.code] ?? fallback;
}

function formatAccountResponseProblem<TResponse>(
  result: AccountApiResult<TResponse>,
  fallback: string,
) {
  if (result.httpStatus === 0 || result.httpStatus === 503) {
    return "El servicio de cuentas no esta disponible ahora. Espera unos segundos y vuelve a intentarlo.";
  }

  if (result.bodyParseFailed || !result.hasJsonBody) {
    return "No se pudo leer la respuesta del servicio de cuentas. Reintenta en unos segundos.";
  }

  return fallback;
}

function formatAccountException() {
  return "No se pudo contactar el servicio de cuentas. Revisa la conexion y vuelve a intentarlo.";
}

export function formatRegistrationErrors(
  result: AccountApiResult<AccountRegistrationResponse>,
) {
  const errors = result.response?.errors ?? [];
  if (errors.length > 0) {
    return errors.map((error) => formatAccountError(error, registrationErrorCopy, "No se pudo completar el registro."));
  }

  return [formatAccountResponseProblem(result, "No se pudo crear la cuenta. Revisa los datos y vuelve a intentarlo.")];
}

export function formatLoginErrors(
  result: AccountApiResult<AccountSessionResponse>,
) {
  const errors = result.response?.errors ?? [];
  if (errors.length > 0) {
    return errors.map((error) => formatAccountError(error, loginErrorCopy, "No se pudo iniciar sesion."));
  }

  return [formatAccountResponseProblem(result, "No se pudo iniciar sesion. Revisa los datos y vuelve a intentarlo.")];
}

export function formatAccountRequestException() {
  return [formatAccountException()];
}
