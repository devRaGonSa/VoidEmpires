import { useEffect, useRef, useState } from "react";
import { formatQueueCountdown, hasQueueCountdownExpired } from "../utils/countdown";

interface LiveQueueCountdownProps {
  className?: string;
  endsAtUtc: string;
  expireKey?: string;
  onExpire?: () => void | Promise<void>;
}

export function LiveQueueCountdown({
  className,
  endsAtUtc,
  expireKey,
  onExpire,
}: LiveQueueCountdownProps) {
  const [nowMs, setNowMs] = useState(() => Date.now());
  const notifiedExpiryKeys = useRef<Set<string>>(new Set());
  const currentExpireKey = expireKey ?? endsAtUtc;

  useEffect(() => {
    setNowMs(Date.now());
    const intervalId = window.setInterval(() => setNowMs(Date.now()), 1000);
    return () => window.clearInterval(intervalId);
  }, [endsAtUtc]);

  useEffect(() => {
    if (!onExpire || notifiedExpiryKeys.current.has(currentExpireKey) || !hasQueueCountdownExpired(endsAtUtc, nowMs)) {
      return;
    }

    notifiedExpiryKeys.current.add(currentExpireKey);
    void onExpire();
  }, [currentExpireKey, endsAtUtc, nowMs, onExpire]);

  const countdownClassName = ["queue-countdown", className].filter(Boolean).join(" ");
  return <span className={countdownClassName}>{formatQueueCountdown(endsAtUtc, nowMs)}</span>;
}
