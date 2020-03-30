export interface EducationForm {
    id: bigint;
    title: string;
    description: string;
    level: string;
}

export interface ExperienceForm {
    id: bigint;
    title: string;
    description: string;
    start: Date;
    end: Date;
}

export interface UpdateForm {
    mail: string;
    password?: string;
    firstName: string;
    lastName: string;
    address: string;
    description: string;
    skills: boolean[];
    educations: EducationForm[];
    experiences: ExperienceForm[];
}