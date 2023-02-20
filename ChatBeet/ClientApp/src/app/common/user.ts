import { DiscordIdentity } from "./DiscordIdentity";
import { IrcIdentity } from "./IrcIdentity";
import { UserId } from "./user-id";

export class User {
    id: UserId;
    discord?: DiscordIdentity;
    irc?: IrcIdentity;
    createdAt: Date;
    updatedAt?: Date;
    avatarUrl?: string;

    get displayName(): string | undefined {
        return this.discord?.name
            ? `${this.discord?.name}#${this.discord?.discriminator}`
            : this?.irc?.nick;
    }

    constructor(data: any, avatarUrl?: string) {
        this.id = data.id;
        if (data.discord)
            this.discord = new DiscordIdentity(data.discord);
        if (data.irc)
            this.irc = new IrcIdentity(data.irc);
        this.createdAt = new Date(data.createdAt);
        this.updatedAt = data.updatedAt ? new Date(data.updatedAt) : undefined;
        this.avatarUrl = avatarUrl;
    }
}
