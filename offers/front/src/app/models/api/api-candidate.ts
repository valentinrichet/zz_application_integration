import { ApiEducation } from './api-education';
import { ApiExperience } from './api-experience';
import { ApiSkill } from './api-skill';

export interface ApiCandidate {
    id: bigint;
    mail: string;
    firstName: string;
    lastName: string;
    address: string;
    description: string;
    skills: ApiSkill[];
    educations: ApiEducation[];
    experiences: ApiExperience[];
}

export interface ApiSignInCandidate {
    mail: string;
    password: string;
}