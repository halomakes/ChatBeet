import { Guild } from "./Guild";
import { User } from "./user";


export class CurrentUserModel {
    user: User;
    guilds: Array<Guild>;
    avatarUrl?: string;

    constructor(data: any) {
        this.user = new User(data.user, data.avatarUrl);
        this.guilds = (data.guilds as any[]).map(g => new Guild(g));
        this.avatarUrl = data.avatarUrl;
    }
}
