import { User } from "../common/user";

export class HighGround {
    public current?: User;
    public previous?: User;

    constructor(data: any) {
        if (data.current)
            this.current = new User(data.current);
        if (data.previous)
            this.previous = new User(data.previous);
    }
}