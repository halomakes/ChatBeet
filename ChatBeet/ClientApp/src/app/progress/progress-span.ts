import { GuildId } from "../common/guild-id";

export class ProgressSpan {
    public guildId: GuildId;
    public key: string;
    public template: string;
    public beforeRangeMessage?: string;
    public afterRangeMessage?: string;
    public startDate: Date;
    public endDate: Date;

    constructor(data: any) {
        this.guildId = data.guildId;
        this.key = data.key;
        this.template = data.template;
        this.beforeRangeMessage = data.beforeRangeMessage;
        this.afterRangeMessage = data.afterRangeMessage;
        this.startDate = new Date(data.startDate);
        this.endDate = new Date(data.endDate);
    }
}
