import { GuildId } from "../common/guild-id";
import { User } from "../common/user";
import { UserId } from "../common/user-id";
import { QuoteMessage } from "./quote-message";

export class Quote {
    public guildId: GuildId;
    public slug: string;
    public savedById: UserId;
    public createdAt: Date;
    public channelName: string;

    public messages?: Array<QuoteMessage>;
    public savedBy: User;

    constructor(data: any) {
        this.guildId = data.guildId;
        this.slug = data.slug;
        this.savedById = data.savedById;
        this.createdAt = new Date(data.createdAt);
        this.channelName = data.channelName;
        if (data.messages?.length)
            this.messages = data.messages.map((m: any) => new QuoteMessage(m));
        this.savedBy = new User(data.savedBy);
    }
}