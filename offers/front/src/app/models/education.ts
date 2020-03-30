import { ApiEducation } from './api/api-education';

export class Education {
    public readonly id: bigint;
    public readonly title: string;
    public readonly description: string;
    public readonly level: string;

    constructor(json: ApiEducation) {
        this.id = json.id;
        this.title = json.title;
        this.description = json.description;
        this.level = json.level;
    }
}