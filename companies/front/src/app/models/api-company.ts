export interface ApiCompany {
    id: bigint;
    mail: string;
    name: string;
    address: string;
    description: string;
}

export interface ApiSignInCompany {
    mail: string;
    password: string;
}

export interface ApiCreateCompany {
    mail: string;
    password: string;
    name: string;
    address: string;
    description: string;
}

export interface ApiUpdateCompany {
    mail?: string;
    password?: string;
    name?: string;
    address?: string;
    description?: string;
}