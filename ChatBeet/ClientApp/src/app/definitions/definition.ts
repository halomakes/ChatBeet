import { GuildId } from "../common/guild-id";
import { User } from "../common/user";
import { UserId } from "../common/user-id";

export class Definition {
    public guildId: GuildId;
    public key: string;
    public value: string;
    public createdBy: UserId;
    public createdAt: Date;
    public updatedAt: Date;
    public author: User;
    public get authorName(): string | undefined {
        return this.author.displayName;
    }

    constructor(data: any) {
        this.guildId = data.guildId;
        this.key = data.key;
        this.value = data.value;
        this.createdBy = data.createdBy;
        this.createdAt = new Date(data.createdAt);
            this.updatedAt = new Date(data.updatedAt);
        this.author = new User(data.author);
    }
}