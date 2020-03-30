export interface ApiExperience {
    id: bigint;
    title: string;
    description: string;
    start: string;
    end: string;
}

export interface ApiCreateExperience {
    title: string;
    description: string;
    start: string;
    end?: string;
}

export interface ApiUpdateExperience {
    title?: string;
    description?: string;
    start?: string;
    end?: string;
}