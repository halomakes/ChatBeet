export type KarmaLevels = { [key: string]: number };

export class KarmaLevel {
    key: string;
    level: number;

    constructor(key: string, level: number) {
        this.key = key;
        this.level = level;
    }
}