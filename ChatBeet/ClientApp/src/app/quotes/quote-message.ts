import { User } from "../common/user";
import { UserId } from "../common/user-id";

export class QuoteMessage {
    public content: string;
    public createdAt: Date;
    public authorId: UserId;
    public embeds: number;
    public attachments: number;

    public author: User;

    constructor(data: any) {
        this.content = data.content;
        this.createdAt = new Date(data.createdAt);
        this.authorId = data.authorId;
        this.embeds = data.embeds;
        this.author = new User(data.author);
        this.attachments = data.attachments;
    }

    public attachmentText = (): string => `This message originally had ${this.attachments} ${this.attachments === 1 ? 'attachment' : 'attachments'}.`;
    public embedText = (): string => `This message originally had ${this.embeds} ${this.embeds === 1 ? 'embed' : 'embeds'}.`;
}