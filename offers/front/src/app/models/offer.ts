import { ApiOffer } from './api/api-offer';
import { Skill } from './skill';
import { Company } from './company';

export class Offer {
    public readonly id: bigint;
    public readonly title: string;
    public readonly description: string;
    public readonly level: string;
    public readonly type: string;
    public readonly wage: number;
    public readonly company: Company;
    public readonly skills: ReadonlyArray<Skill>;

    constructor(json: ApiOffer) {
        this.id = json.id;
        this.title = json.title;
        this.description = json.description;
        this.level = json.level;
        this.type = json.type;
        this.wage = json.wage;
        this.company = new Company(json.company);
        this.skills = json.skills.map(skill => new Skill(skill));
    }
}