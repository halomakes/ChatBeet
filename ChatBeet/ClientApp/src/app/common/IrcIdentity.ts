
export class IrcIdentity {
    nick?: string;

    constructor(data: any) {
        this.nick = data.nick;
    }
}
