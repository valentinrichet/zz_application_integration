import { ApiCandidate } from './api/api-candidate';
import { Education } from './education';
import { Experience } from './experience';
import { Skill } from './skill';

export class Candidate {
    public readonly id: bigint;
    public readonly mail: string;
    public readonly firstName: string;
    public readonly lastName: string;
    public readonly address: string;
    public readonly description: string;
    public readonly skills: ReadonlyMap<bigint, Skill>;
    public readonly educations: ReadonlyMap<bigint, Education>;
    public readonly experiences: ReadonlyMap<bigint, Experience>;

    constructor(json: ApiCandidate) {
        this.id = json.id;
        this.mail = json.mail;
        this.firstName = json.firstName;
        this.lastName = json.lastName;
        this.address = json.address;
        this.description = json.description;
        this.skills = new Map(json.skills.map(skill => [skill.id, new Skill(skill)]));
        this.educations = new Map(json.educations.map(education => [education.id, new Education(education)]));
        this.experiences = new Map(json.experiences.map(experience => [experience.id, new Experience(experience)]));
    }
}