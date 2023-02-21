import { ProgressSpan } from "./progress-span";
import { formatDistanceToNow } from "date-fns";

export class ProgressCompletion {
    public percentage: number;
    public message?: string;

    constructor(span: ProgressSpan, time?: Date) {
        time = time || new Date();
        const ratio = ProgressCompletion.getRatio(time, span.startDate, span.endDate);

        this.percentage = ratio;
        if (time.getTime() > span.endDate.getTime()) {
            this.message = span.afterRangeMessage;
        } else if (time.getTime() < span.startDate.getTime()) {
            this.message = span.beforeRangeMessage;
        } else {
            this.message = ProgressCompletion.formatTemplate(span.template, {
                'percentage': `${ratio.toFixed(2)}%`,
                'elapsed': formatDistanceToNow(span.startDate),
                'remaining': formatDistanceToNow(span.endDate)
            })
        }
    }

    private static formatTemplate = (template: string, values: { [key: string]: string }): string => {
        if (template && values) {
            Object.keys(values).forEach(key => {
                template = template.replace(`{${key}}`, values[key]);
            });
        }

        return template;
    }

    private static getRatio = (now: Date, start: Date, end: Date): number => ProgressCompletion.forceRange((now.getTime() - start.getTime()) * 100 / (end.getTime() - start.getTime()));

    private static forceRange = (value: number): number => value > 100
        ? 100
        : value < 0
            ? 0
            : value;
}
