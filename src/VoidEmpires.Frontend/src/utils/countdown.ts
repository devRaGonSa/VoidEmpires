export function formatQueueCountdown(endsAtUtc: string, nowMs = Date.now()) {
  const endsAtMs = Date.parse(endsAtUtc);
  if (Number.isNaN(endsAtMs)) {
    return "tiempo no disponible";
  }

  const remainingSeconds = Math.ceil((endsAtMs - nowMs) / 1000);
  if (remainingSeconds <= 0) {
    return "finalizando...";
  }

  const days = Math.floor(remainingSeconds / 86400);
  const hours = Math.floor((remainingSeconds % 86400) / 3600);
  const minutes = Math.floor((remainingSeconds % 3600) / 60);
  const seconds = remainingSeconds % 60;
  const clock = `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;

  return days > 0 ? `${days}d ${clock}` : clock;
}

export function hasQueueCountdownExpired(endsAtUtc: string, nowMs = Date.now()) {
  const endsAtMs = Date.parse(endsAtUtc);
  return !Number.isNaN(endsAtMs) && endsAtMs <= nowMs;
}
