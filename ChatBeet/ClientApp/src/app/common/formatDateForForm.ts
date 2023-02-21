export function formatDateForForm(date: Date): string {
    const offset = date.getTimezoneOffset();
    const adjusted = new Date(date.getTime() - (offset * 60000));
    return adjusted.toISOString().substring(0, 16);
}