
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
