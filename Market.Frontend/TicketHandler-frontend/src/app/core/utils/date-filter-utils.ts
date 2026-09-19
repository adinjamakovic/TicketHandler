export function toDateOnlyParam(date: Date | null | undefined): Date | null {
  if (!date) {
    return null;
  }

  const value = date instanceof Date ? date : new Date(date);
  if (isNaN(value.getTime())) {
    return null;
  }

  return new Date(Date.UTC(value.getFullYear(), value.getMonth(), value.getDate()));
}
