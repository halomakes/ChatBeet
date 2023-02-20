import { User } from "../common/user";

export class SuspicionLevel {
    public user: User;
    public level: number;
    public color: string;

    constructor(data: any) {
        this.user = new User(data.user);
        this.level = data.level;
        this.color = data.color;
    }
}