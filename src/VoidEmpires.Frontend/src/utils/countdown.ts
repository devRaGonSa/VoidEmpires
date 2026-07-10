export function formatQueueCountdown(endsAtUtc: string, nowMs = Date.now()) {
  const endsAtMs = Date.parse(endsAtUtc);
  if (Number.isNaN(endsAtMs)) {
    return "tiempo no disponible";
  }

  const remainingSeconds = Math.ceil((endsAtMs - nowMs) / 1000);
  if (remainingSeconds <= 0) {
    return "finalizando...";
  }

  const hours = Math.floor(remainingSeconds / 3600);
  const minutes = Math.floor((remainingSeconds % 3600) / 60);
  const seconds = remainingSeconds % 60;

  if (hours > 0) {
    return `${hours}h ${String(minutes).padStart(2, "0")}m`;
  }

  if (minutes > 0) {
    return seconds > 0 ? `${minutes}m ${String(seconds).padStart(2, "0")}s` : `${minutes}m`;
  }

  return `${seconds}s`;
}
