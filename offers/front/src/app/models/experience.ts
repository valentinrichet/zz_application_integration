import { ApiExperience } from './api/api-experience';

export class Experience {
    public readonly id: bigint;
    public readonly title: string;
    public readonly description: string;
    public readonly start: string;
    public readonly end: string;

    constructor(json: ApiExperience) {
        this.id = json.id;
        this.title = json.title;
        this.description = json.description;
        this.start = json.start;
        this.end = json.end;
    }
}