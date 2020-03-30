import { ApiSkill } from './api/api-skill';

export class Skill {
    public readonly id: bigint;
    public readonly title: string;
    public readonly type: string;

    constructor(json: ApiSkill) {
        this.id = json.id;
        this.title = json.title;
        this.type = json.type;
    }
}