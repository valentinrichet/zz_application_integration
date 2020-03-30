import { ApiCompany } from './api-company';
import { ApiSkill } from './api-skill';

export interface ApiOffer {
    id: bigint;
    title: string;
    description: string;
    level: string;
    type: string;
    wage: number;
    company: ApiCompany;
    skills: ApiSkill[];
}

export interface ApiCreateOffer {
    idCompany: bigint;
    title: string;
    description: string;
    level: string;
    type: string;
    wage?: number;
    skills: bigint[];
}

export interface ApiUpdateOffer {
    title: string;
    description: string;
    level: string;
    type: string;
    wage: number;
    skills: bigint[];
}