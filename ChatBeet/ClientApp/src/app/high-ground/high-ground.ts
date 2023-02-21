import { GuildId } from "../common/guild-id";
import { User } from "../common/user";
import { UserId } from "../common/user-id";

export class HighGround {
    public guildId: GuildId;
    public userId: UserId;
    public user: User;
    public updatedAt: Date;

    constructor(data: any) {
        this.guildId = data.guildId;
        this.userId = data.userId;
        this.updatedAt = new Date(data.updatedAt);
        this.user = new User(data.user);
    }
}