export interface ApiEducation {
    id: bigint;
    title: string;
    description: string;
    level: string;
}

export interface ApiCreateEducation {
    title: string;
    description: string;
    level: string;
}

export interface ApiUpdateEducation {
    title?: string;
    description?: string;
    level?: string;
}