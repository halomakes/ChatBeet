export class User {
    id: string;
    discord?: DiscordIdentity;
    irc?: IrcIdentity;
    createdAt: Date;
    updatedAt?: Date;

    get displayName(): string | undefined {
        return this.discord?.name
            ? `${this.discord?.name}#${this.discord?.discriminator}`
            : this?.irc?.nick;
    }

    constructor(data: any) {
        this.id = data.id;
        if (data.discord)
            this.discord = new DiscordIdentity(data.discord);
        if (data.irc)
            this.irc = new IrcIdentity(data.irc);
        this.createdAt = new Date(data.createdAt);
        this.updatedAt = data.updatedAt ? new Date(data.updatedAt) : undefined;
    }
}

export class DiscordIdentity {
    id?: number;
    name?: string;
    discriminator?: string;

    constructor(data: any) {
        this.id = data.id;
        this.name = data.name;
        this.discriminator = data.discriminator;
    }
}

export class IrcIdentity {
    nick?: string;

    constructor(data: any) {
        this.nick = data.nick;
    }
}

export class Guild {
    id: number;
    iconUrl?: string;
    splashUrl?: string;
    name: string;

    constructor(data: any) {
        this.id = data.id;
        this.iconUrl = data.iconUrl;
        this.splashUrl = data.splashUrl;
        this.name = data.name;
    }
}

export class CurrentUserModel {
    user: User;
    guilds: Array<Guild>;

    constructor(data: any) {
        this.user = new User(data.user);
        this.guilds = (data.guilds as any[]).map(g => new Guild(g));
    }
}