import { GuildId } from "./guild-id";


export class Guild {
    id: GuildId;
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
