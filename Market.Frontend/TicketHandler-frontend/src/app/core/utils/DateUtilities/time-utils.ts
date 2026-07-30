/**
 * Builds the selectable time-of-day slots ("00:00", "00:30", ... "23:30").
 */
export function buildTimeSlots(intervalMinutes = 30): string[] {
  const slots: string[] = [];

  for (let minutes = 0; minutes < 24 * 60; minutes += intervalMinutes) {
    const hours = Math.floor(minutes / 60).toString().padStart(2, '0');
    const remainder = (minutes % 60).toString().padStart(2, '0');
    slots.push(`${hours}:${remainder}`);
  }

  return slots;
}

/**
 * Normalizes a TimeOnly coming back from the API ("HH:mm:ss" or "HH:mm:ss.fffffff")
 * to the "HH:mm" value the time slot dropdowns bind to.
 */
export function toTimeSlot(value?: string | null): string {
  if (!value) {
    return '';
  }

  const [hours, minutes] = value.split(':');
  if (hours === undefined || minutes === undefined) {
    return '';
  }

  return `${hours.padStart(2, '0')}:${minutes.slice(0, 2).padStart(2, '0')}`;
}

/**
 * Expands a "HH:mm" slot back into the "HH:mm:ss" string the API binds to a TimeOnly.
 */
export function toTimeOnlyString(slot?: string | null): string {
  const normalized = toTimeSlot(slot);
  return normalized ? `${normalized}:00` : '';
}

/**
 * Merges already-saved times into the standard slots so a stored time that does not fall on
 * an interval boundary (e.g. "20:45") still shows up and round-trips instead of blanking out.
 */
export function withExtraTimeSlots(slots: string[], values: (string | null | undefined)[]): string[] {
  const merged = new Set(slots);

  for (const value of values) {
    const normalized = toTimeSlot(value);
    if (normalized) {
      merged.add(normalized);
    }
  }

  return [...merged].sort();
}
